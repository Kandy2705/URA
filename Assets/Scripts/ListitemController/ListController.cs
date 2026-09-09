using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Shopping-list view. Legacy random-list APIs remain for scenes that do not use ShoppingMissionController.
/// </summary>
public class ListController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject listContainer;

    [Header("Legacy data / mission view prefabs")]
    [SerializeField] private List<GameObject> availablePrefabs = new List<GameObject>();
    [SerializeField] public List<GameObject> choicedItems = new List<GameObject>();

    [Header("Manual view limit")]
    [SerializeField] private int limit = 2;
    [SerializeField] private int currentLimit;
    [SerializeField] private bool enforceManualViewLimitForLegacyList;

    [Header("Legacy random list")]
    [SerializeField] private int spawnOnStart = 5;
    [SerializeField] private float showDuration = 10f;
    [SerializeField] private bool enableRandomTaskChange = false;
    [SerializeField] private float minDelay = 30f;
    [SerializeField] private float maxDelay = 90f;

    [Header("Notify UI")]
    [SerializeField] private GameObject notificationCanvas;
    [SerializeField] private TextMeshProUGUI notificationText;

    public GameObject NotificationCanvas => notificationCanvas;
    public TextMeshProUGUI NotificationText => notificationText;
    public bool HasListContainer => listContainer != null;
    public bool hasTriggeredRandomChange = true;
    public bool HasPendingRandomChange => hasTriggeredRandomChange;
    public event Action<string, GameObject, int> OnListChanged;
    public event Action<int> OnListShown;
    public event Action<string, string, int, string> OnRandomListChange;
    public event Action<IReadOnlyList<ShoppingTaskItem>> OnInitialTasksRendered;

    private readonly List<GameObject> renderedItems = new List<GameObject>();
    private Coroutine currentRoutine;
    private int spawnCount;
    private string lastReplacedOldName;
    private string lastReplacedNewName;
    private int lastReplacedQuantity;

    private void Start()
    {
        if (FindFirstObjectByType<ShoppingMissionController>() != null)
            return;

        for (int i = 0; i < spawnOnStart; i++)
            SpawnItemInList();
        ShowListAutomatically();
        ResetViewLimit();
        if (enableRandomTaskChange)
            StartCoroutine(RandomChangeCoroutine());
    }

    /// <summary>Manual request; retained for NoticeBoardButtonFunction and NpcActionDispatcher.</summary>
    public void ShowList()
    {
        if (currentRoutine != null || (ShouldLimitManualViews() && currentLimit >= limit))
            return;

        currentLimit++;
        OnListShown?.Invoke(currentLimit);
        currentRoutine = StartCoroutine(ShowThenHide());
    }

    /// <summary>Used for the initial list. It deliberately does not consume a manual view.</summary>
    public void ShowListAutomatically()
    {
        if (listContainer == null)
            return;
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);
        currentRoutine = StartCoroutine(ShowThenHide());
    }

    // A stale scene persistent call references this method; keep it as a compatibility shim.
    public void ToggleList()
    {
        if (listContainer != null && listContainer.activeSelf)
        {
            if (currentRoutine != null)
                StopCoroutine(currentRoutine);
            listContainer.SetActive(false);
            currentRoutine = null;
            return;
        }
        ShowList();
    }

    public void ResetViewLimit() => currentLimit = 0;
    public int GetClickNumber() => currentLimit;

    public void RenderTasks(IReadOnlyList<ShoppingTaskItem> tasks)
    {
        ClearRenderedItems();
        choicedItems.Clear();
        spawnCount = 0;
        if (tasks != null)
        {
            foreach (ShoppingTaskItem task in tasks)
                SpawnMissionItem(task);
        }
        OnInitialTasksRendered?.Invoke(tasks);
    }

    public void ShowNotification(string message, float duration = 3f)
    {
        if (notificationText != null)
            notificationText.text = message;
        if (notificationCanvas == null)
            return;
        notificationCanvas.SetActive(true);
        StartCoroutine(HideNotificationAfter(duration));
    }

    private bool ShouldLimitManualViews() =>
        enforceManualViewLimitForLegacyList || FindFirstObjectByType<ShoppingMissionController>() != null;

    private IEnumerator ShowThenHide()
    {
        if (listContainer == null)
        {
            currentRoutine = null;
            yield break;
        }
        listContainer.SetActive(true);
        yield return new WaitForSeconds(showDuration);
        listContainer.SetActive(false);
        currentRoutine = null;
    }

    private void SpawnMissionItem(ShoppingTaskItem task)
    {
        if (task == null)
            return;
        GameObject prefab = task.viewPrefab != null ? task.viewPrefab : FindPrefab(task.itemName);
        if (prefab == null || listContainer == null)
        {
            Debug.LogWarning($"[ListController] Missing view prefab for mission item '{task.itemName}'.");
            return;
        }
        GameObject item = Instantiate(prefab, listContainer.transform);
        ConfigureItemView(item, task.itemName, task.requiredQuantity);
    }

    private GameObject FindPrefab(string itemName)
    {
        foreach (GameObject prefab in availablePrefabs)
        {
            if (prefab != null && string.Equals(prefab.name, itemName, StringComparison.Ordinal))
                return prefab;
        }
        return null;
    }

    private void SpawnItemInList()
    {
        if (availablePrefabs.Count == 0 || listContainer == null)
            return;
        int randomIndex = UnityEngine.Random.Range(0, availablePrefabs.Count);
        GameObject prefab = availablePrefabs[randomIndex];
        ConfigureItemView(Instantiate(prefab, listContainer.transform), prefab.name, UnityEngine.Random.Range(1, 10));
        availablePrefabs.RemoveAt(randomIndex);
    }

    private void ConfigureItemView(GameObject item, string itemName, int quantity)
    {
        if (item == null)
            return;
        renderedItems.Add(item);
        choicedItems.Add(item);
        item.transform.localPosition = new Vector3(0f, 20f + spawnCount * -25f, 0f);
        item.transform.localRotation = Quaternion.identity;
        TMP_Text nameText = item.transform.Find("Name")?.GetComponent<TMP_Text>();
        TMP_Text quantityText = item.transform.Find("Quantity")?.GetComponent<TMP_Text>();
        if (nameText != null) nameText.text = itemName;
        if (quantityText != null) quantityText.text = Mathf.Max(1, quantity).ToString();
        spawnCount++;
    }

    private void ClearRenderedItems()
    {
        foreach (GameObject item in renderedItems)
        {
            if (item != null)
                Destroy(item);
        }
        renderedItems.Clear();
    }

    public string ReplaceRandomItemWithUniquePrefab()
    {
        if (choicedItems.Count == 0 || availablePrefabs.Count == 0)
            return "Không có item hoặc prefab để thay đổi.";
        GameObject target = choicedItems[UnityEngine.Random.Range(0, choicedItems.Count)];
        GameObject prefab = availablePrefabs[UnityEngine.Random.Range(0, availablePrefabs.Count)];
        TMP_Text oldText = target.transform.Find("Name")?.GetComponent<TMP_Text>();
        lastReplacedOldName = oldText != null ? oldText.text : target.name;
        lastReplacedNewName = prefab.name;
        lastReplacedQuantity = UnityEngine.Random.Range(1, 10);
        ConfigureExistingItem(target, lastReplacedNewName, lastReplacedQuantity);
        OnListChanged?.Invoke(lastReplacedOldName, prefab, lastReplacedQuantity);
        return $"Đã đổi {lastReplacedOldName} → {lastReplacedNewName} (x{lastReplacedQuantity})";
    }

    private static void ConfigureExistingItem(GameObject item, string name, int quantity)
    {
        TMP_Text nameText = item.transform.Find("Name")?.GetComponent<TMP_Text>();
        TMP_Text quantityText = item.transform.Find("Quantity")?.GetComponent<TMP_Text>();
        if (nameText != null) nameText.text = name;
        if (quantityText != null) quantityText.text = quantity.ToString();
    }

    private IEnumerator RandomChangeCoroutine()
    {
        yield return new WaitForSeconds(UnityEngine.Random.Range(minDelay, maxDelay));
        if (!hasTriggeredRandomChange)
            yield break;
        string discountInfo = ItemsManager.DiscountRandomItem();
        string message = ReplaceRandomItemWithUniquePrefab();
        hasTriggeredRandomChange = false;
        OnRandomListChange?.Invoke(lastReplacedOldName, lastReplacedNewName, lastReplacedQuantity, discountInfo);
        ShowNotification(message);
    }

    private IEnumerator HideNotificationAfter(float duration)
    {
        yield return new WaitForSeconds(duration);
        if (notificationCanvas != null)
            notificationCanvas.SetActive(false);
    }
}
