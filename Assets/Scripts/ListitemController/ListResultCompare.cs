using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>Builds result/scoring data for both initial and supplemental tasks.</summary>
public class ListResultCompare : MonoBehaviour
{
    public ListController listController;
    [SerializeField] private GameObject dataContainer;
    [SerializeField] private Transform obtainProductContainer;
    [SerializeField] private Transform wrongProductContainer;
    [SerializeField] private GameTimer gameTimer;

    private readonly Dictionary<string, int> requiredQuantities = new Dictionary<string, int>(StringComparer.Ordinal);
    private ShoppingMissionController taskController;
    private float itemHeight = 5f;
    private float startY = 10f;
    public List<CompareResult> compareResults = new List<CompareResult>();
    public bool loadStart = true;
    public static bool compareResultListUpdated = false;
    public Action LoadCSV;

    private void Start()
    {
        if (listController == null)
            listController = FindListController();
        if (gameTimer == null)
            gameTimer = FindFirstObjectByType<GameTimer>();

        taskController = FindFirstObjectByType<ShoppingMissionController>();
        if (taskController != null)
        {
            taskController.OnInitialTasksRendered += HandleInitialTasksRendered;
            taskController.OnSupplementalTaskAdded += HandleSupplementalTaskAdded;
            RegisterTasks(taskController.InitialTasks);
        }
        else
        {
            RegisterRenderedLegacyItems();
            if (listController != null)
                listController.OnListChanged += HandleLegacyListChanged;
        }

        LoadCSV += LoadCompareResult;
        loadStart = false;
    }

    private void OnDestroy()
    {
        if (taskController != null)
        {
            taskController.OnInitialTasksRendered -= HandleInitialTasksRendered;
            taskController.OnSupplementalTaskAdded -= HandleSupplementalTaskAdded;
        }
        if (listController != null)
            listController.OnListChanged -= HandleLegacyListChanged;
    }

    private void OnEnable()
    {
        if (PokeManager.Instance != null)
            PokeManager.Instance.OnInventoryChanged += HandleBillChanged;
    }

    private void OnDisable()
    {
        if (PokeManager.Instance != null)
            PokeManager.Instance.OnInventoryChanged -= HandleBillChanged;
    }

    private void Update()
    {
        if (gameTimer != null && !gameTimer.isRunning)
            LoadCSV?.Invoke();
    }

    private void HandleInitialTasksRendered(IReadOnlyList<ShoppingTaskItem> tasks)
    {
        // Initial tasks are rendered only once per session; this protects the immutable list
        // from accidental re-render/reset while a supplemental notification is active.
        if (requiredQuantities.Count == 0)
            RegisterTasks(tasks);
    }

    private void HandleSupplementalTaskAdded(ShoppingTaskItem task) => RegisterTask(task);

    private void RegisterTasks(IReadOnlyList<ShoppingTaskItem> tasks)
    {
        if (tasks == null)
            return;
        foreach (ShoppingTaskItem task in tasks)
            RegisterTask(task);
    }

    private void RegisterTask(ShoppingTaskItem task)
    {
        if (task == null || string.IsNullOrWhiteSpace(task.itemName))
            return;

        int requiredQuantity = Mathf.Max(1, task.requiredQuantity);
        requiredQuantities.TryGetValue(task.itemName, out int currentRequired);
        requiredQuantities[task.itemName] = currentRequired + requiredQuantity;
        UpsertRequiredResult(task.itemName);
    }

    private void RegisterRenderedLegacyItems()
    {
        if (listController == null)
            return;
        foreach (GameObject item in listController.choicedItems)
        {
            TMP_Text name = item != null ? item.transform.Find("Name")?.GetComponent<TMP_Text>() : null;
            TMP_Text quantity = item != null ? item.transform.Find("Quantity")?.GetComponent<TMP_Text>() : null;
            if (name == null || quantity == null || !int.TryParse(quantity.text, out int count))
                continue;
            RegisterTask(new ShoppingTaskItem(name.text, count));
        }
    }

    private void HandleLegacyListChanged(string oldName, GameObject newData, int newQuantity)
    {
        if (!string.IsNullOrWhiteSpace(oldName))
            requiredQuantities.Remove(oldName);
        if (newData != null)
            RegisterTask(new ShoppingTaskItem(newData.name, newQuantity));
    }

    private void HandleBillChanged(BillEntry entry, bool _)
    {
        if (entry == null || string.IsNullOrWhiteSpace(entry.itemName))
            return;

        if (requiredQuantities.ContainsKey(entry.itemName))
        {
            UpsertRequiredResult(entry.itemName);
            UpdateDisplay(obtainProductContainer, entry.itemName, FormatQuantity(entry.itemName));
            return;
        }

        UpsertUnexpectedResult(entry);
        if (!HasDisplay(wrongProductContainer, entry.itemName))
            CreateNewInstantData(entry, wrongProductContainer, entry.quantity.ToString());
        else
            UpdateDisplay(wrongProductContainer, entry.itemName, entry.quantity.ToString());
    }

    private void UpsertRequiredResult(string itemName)
    {
        int required = requiredQuantities[itemName];
        BillEntry entry = GetInventoryEntry(itemName);
        int current = entry?.quantity ?? 0;
        string display = FormatQuantity(current, required);
        UpsertCompareResult(itemName, current, required, entry?.price ?? 0, true);
        if (!HasDisplay(obtainProductContainer, itemName))
            CreateNewInstantData(new BillEntry(itemName, entry?.price ?? 0, current), obtainProductContainer, display);
        else
            UpdateDisplay(obtainProductContainer, itemName, display);
    }

    private void UpsertUnexpectedResult(BillEntry entry) =>
        UpsertCompareResult(entry.itemName, entry.quantity, 0, entry.price, false);

    private void UpsertCompareResult(string itemName, int current, int required, int price, bool isRequired)
    {
        string ignored = string.Empty;
        string status = StatusChange(current, required, ref ignored);
        int index = compareResults.FindIndex(result => result.itemName == itemName);
        CompareResult result = new CompareResult(itemName, current, required, status, price, isRequired);
        if (index >= 0)
            compareResults[index] = result;
        else
            compareResults.Add(result);
    }

    private BillEntry GetInventoryEntry(string itemName)
    {
        return PokeManager.Instance != null && PokeManager.Instance.inventory.TryGetValue(itemName, out BillEntry entry)
            ? entry : null;
    }

    private string FormatQuantity(string itemName) =>
        FormatQuantity(GetInventoryEntry(itemName)?.quantity ?? 0, requiredQuantities[itemName]);

    private string FormatQuantity(int current, int required)
    {
        string text = string.Empty;
        StatusChange(current, required, ref text);
        return text;
    }

    public string StatusChange(int currentQuantity, int indexQuantity, ref string quantityText)
    {
        if (currentQuantity < indexQuantity)
        {
            int shortage = indexQuantity - currentQuantity;
            quantityText = $"{currentQuantity} (Thiếu {shortage})";
            return $"Thiếu {shortage}";
        }
        if (currentQuantity == indexQuantity)
        {
            quantityText = $"{currentQuantity} (Đủ)";
            return "Đủ";
        }
        int surplus = currentQuantity - indexQuantity;
        quantityText = $"{currentQuantity} (Dư {surplus})";
        return $"Dư {surplus}";
    }

    private void CreateNewInstantData(BillEntry entry, Transform parentContainer, string quantityText)
    {
        if (dataContainer == null || parentContainer == null)
            return;
        GameObject data = Instantiate(dataContainer, parentContainer, false);
        data.name = entry.itemName;
        TMP_Text name = data.transform.Find("Name")?.GetComponent<TMP_Text>();
        TMP_Text quantity = data.transform.Find("Quantity")?.GetComponent<TMP_Text>();
        if (name != null) name.text = entry.itemName;
        if (quantity != null) quantity.text = quantityText;
        Vector3 newPos = new Vector3(0f, startY, 0f);
        if (parentContainer.childCount > 1)
            newPos.y = parentContainer.GetChild(parentContainer.childCount - 2).localPosition.y - itemHeight;
        data.transform.localPosition = newPos;
    }

    private static bool HasDisplay(Transform parent, string itemName) =>
        parent != null && parent.Find(itemName) != null;

    private static void UpdateDisplay(Transform parent, string itemName, string quantityText)
    {
        Transform item = parent != null ? parent.Find(itemName) : null;
        TMP_Text quantity = item != null ? item.Find("Quantity")?.GetComponent<TMP_Text>() : null;
        if (quantity != null) quantity.text = quantityText;
    }

    private static ListController FindListController()
    {
        foreach (ListController candidate in FindObjectsByType<ListController>(FindObjectsInactive.Exclude, FindObjectsSortMode.None))
            if (candidate != null && candidate.HasListContainer)
                return candidate;
        return FindFirstObjectByType<ListController>();
    }

    private void LoadCompareResult()
    {
        if (DataManager.Instance == null)
            return;
        foreach (CompareResult result in compareResults)
            DataManager.Instance.AddCompareResult(result);
        DataManager.Instance.Report();
        LoadCSV -= LoadCompareResult;
    }
}
