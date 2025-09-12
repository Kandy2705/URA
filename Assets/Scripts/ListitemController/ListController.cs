using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class ListController : MonoBehaviour
{
    [SerializeField] private GameObject listItemPrefab;
    [SerializeField] private List<GameObject> availablePrefabs;
    private bool isVisible = true;
    [SerializeField] private Animator anim;
    private int spawnCount = 0;
    private float timer = 0f;  

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int i = 0; i < 5; i++)
        {
            SpawnItemInList();
        }
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= 10f && isVisible)
        {
            ToggleList();
            timer = 0f; // Reset the timer
        }
    }

    public void ToggleList()
    {
        isVisible = !isVisible;
        if (isVisible)
        {
            anim.Play("listOpen");
        }
        else anim.Play("listClose");
        //listItemPrefab.gameObject.SetActive(isVisible);
    }

    public void SpawnItemInList()
    {
        if (availablePrefabs.Count == 0)
        {
            Debug.Log("Đã hết item để chọn!");
            return;
        }

        int randomIndex = Random.Range(0, availablePrefabs.Count);
        int randomQuantity = Random.Range(1, 10);

        GameObject randomPrefab = availablePrefabs[randomIndex];

        GameObject spawnedItem = Instantiate(randomPrefab, listItemPrefab.transform);

        spawnedItem.transform.localPosition = new Vector3(0f, 20f + spawnCount * -25f, 0f);
        spawnedItem.transform.localRotation = Quaternion.identity;

        TextMeshProUGUI quantityText = spawnedItem.transform.Find("Quantity").GetComponent<TextMeshProUGUI>();
        if (quantityText != null)
        {
            quantityText.text = randomQuantity.ToString();
        }
        spawnCount++;
        availablePrefabs.RemoveAt(randomIndex);
    }
}
