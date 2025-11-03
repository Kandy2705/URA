using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class ListController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject listContainer; // Content hoặc Panel

    [Header("Data")]
    [SerializeField] private List<GameObject> availablePrefabs;

    [SerializeField] public List<GameObject> choicedItems;

    [Header("Config")]
    [SerializeField] private int spawnOnStart = 5;
    [SerializeField] private float showDuration = 10f;

    private int spawnCount = 0;
    private Coroutine currentRoutine;

    private void Start()
    {
        for (int i = 0; i < spawnOnStart; i++) SpawnItemInList();
        ShowList();
    }

    public void ShowList()
    {
        if (currentRoutine != null)
        {
            return;
        }
        currentRoutine = StartCoroutine(ShowThenHide());
    }


    private IEnumerator ShowThenHide()
    {
        listContainer.SetActive(true);
        yield return new WaitForSeconds(showDuration);
        listContainer.SetActive(false);
        currentRoutine = null;
    }

    private void SpawnItemInList()
    {
        if (availablePrefabs.Count == 0)
        {
            Debug.Log("Đã hết item để chọn!");
            return;
        }

        int randomIndex    = Random.Range(0, availablePrefabs.Count);
        int randomQuantity = Random.Range(1, 10);

        GameObject randomPrefab = availablePrefabs[randomIndex];
        GameObject spawnedItem  = Instantiate(randomPrefab, listContainer.transform);

        choicedItems.Add(spawnedItem);

        spawnedItem.transform.localPosition = new Vector3(0f, 20f + spawnCount * -25f, 0f);
        spawnedItem.transform.localRotation = Quaternion.identity;

        var quantityText = spawnedItem.transform.Find("Quantity")?.GetComponent<TextMeshProUGUI>();
        if (quantityText != null) quantityText.text = randomQuantity.ToString();

        spawnCount++;
        availablePrefabs.RemoveAt(randomIndex);
    }
}
