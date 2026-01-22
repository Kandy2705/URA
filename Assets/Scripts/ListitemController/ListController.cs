using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using TMPro;

public class ListController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject listContainer; // Content hoặc Panel

    [Header("Data")]
    [SerializeField] private List<GameObject> availablePrefabs;

    [Header("Limit")]
    [SerializeField] private int limit = 2;
    private int currentLimit;

    [SerializeField] public List<GameObject> choicedItems;

    [Header("Config")]
    [SerializeField] private int spawnOnStart = 5;
    [SerializeField] private float showDuration = 10f;

    [Header("Notify UI")]
    [SerializeField] private GameObject notificationCanvas;
    [SerializeField] private TextMeshProUGUI notificationText;

    [SerializeField] private bool hasTriggeredRandomChange;

    private string currentScene;

    [SerializeField] private float minDelay = 30f;
    [SerializeField] private float maxDelay = 90f;

    private int spawnCount = 0;
    private Coroutine currentRoutine;

    private void Start()
    {
        currentLimit = -1;
        currentScene = SceneManager.GetActiveScene().name;
        for (int i = 0; i < spawnOnStart; i++) SpawnItemInList();
        ShowList();

        StartCoroutine(RandomChangeCoroutine());
    }

    // private void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.L))
    //     {
    //          ReplaceRandomItemWithUniquePrefab();
    //     }
    // }

    public void ShowList()
    {

        if (currentRoutine != null || (currentLimit == limit && currentScene == "Scene-level-2"))
        {
            return;
        }
        currentLimit++;
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

    public string ReplaceRandomItemWithUniquePrefab()
    {
        if (choicedItems.Count == 0)
        {
            return "Không có item nào để thay đổi!";
            
        }

        if (availablePrefabs.Count == 0)
        {
            return "Không có prefab nào trong availablePrefabs!";
        }

        HashSet<string> currentNames = new HashSet<string>();

        foreach (var item in choicedItems)
        {
            currentNames.Add(item.name);
        }

        List<GameObject> validPrefabs = new List<GameObject>();

        foreach (var prefab in availablePrefabs)
        {
            if (!currentNames.Contains(prefab.name))
            {
                validPrefabs.Add(prefab);
            }
        }
       
        //Chọn 1 item ngẫu nhiên trong 5 item
        int randomItemIndex = Random.Range(0, choicedItems.Count);
        GameObject targetItem = choicedItems[randomItemIndex];

        var oldNameText = targetItem.transform.Find("Name")?.GetComponent<TextMeshProUGUI>();
        string oldName = oldNameText != null ? oldNameText.text : targetItem.name;

        GameObject newPrefab = validPrefabs[Random.Range(0, validPrefabs.Count)];
        targetItem.name = newPrefab.name;

        var nameText = targetItem.transform.Find("Name")?.GetComponent<TextMeshProUGUI>();
        if (nameText != null)
            nameText.text = newPrefab.name;

        
        int newQuantity = Random.Range(1, 10);
        var quantityText = targetItem.transform.Find("Quantity")?.GetComponent<TextMeshProUGUI>();
        if (quantityText != null)
            quantityText.text = newQuantity.ToString();

        Debug.Log($"Đã đổi {oldName} → {newPrefab.name} (x{newQuantity})");
        return $"Đã đổi {oldName} → {newPrefab.name} (x{newQuantity})";
    }

    private IEnumerator RandomChangeCoroutine()
    {
        float waitTime = Random.Range(minDelay, maxDelay);
        Debug.Log($"Sự kiện đổi item sẽ xảy ra trong {waitTime} giây...");

        yield return new WaitForSeconds(waitTime);

        if (hasTriggeredRandomChange)
        {
        
            string msg = ReplaceRandomItemWithUniquePrefab();
            hasTriggeredRandomChange = false;

            Debug.Log(msg);

            // 🌟 Gọi thông báo UI
            ShowNotification(msg, 3f);
        }
    }


    private void ShowNotification(string message, float duration = 3f)
    {
        notificationText.text = message;
        notificationCanvas.SetActive(true);
        StartCoroutine(HideNotificationAfter(duration));
    }

    private IEnumerator HideNotificationAfter(float duration)
    {
        yield return new WaitForSeconds(duration);
        notificationCanvas.SetActive(false);
    }

    public int GetClickNumber()
    {
        return currentLimit;
    }

}
