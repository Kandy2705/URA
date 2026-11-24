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
    [SerializeField] private SupplementalShoppingTaskController supplementalTaskController;
    private bool mainListRegistered;
    private float itemHeight = 5f;
    private float startY = 10f;
    public List<CompareResult> compareResults = new List<CompareResult>();
    public bool loadStart = true;
    public static bool compareResultListUpdated = false;
    public Action LoadCSV;
    private bool hasFinalizedReport;

    private void Start()
    {
        BindInventory(); // All scene Awake calls have completed.
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
            if (listController != null)
            {
                listController.OnMainListGenerated += HandleMainListGenerated;
                listController.OnListChanged += HandleLegacyListChanged;
                if (listController.MainListGenerated)
                    HandleMainListGenerated();
            }
        }

        if (supplementalTaskController != null)
        {
            supplementalTaskController.OnSupplementalTaskAdded += HandleSupplementalTaskAdded;
            foreach (ShoppingTaskItem task in supplementalTaskController.SupplementalTasks)
                HandleSupplementalTaskAdded(task);
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
        {
            listController.OnMainListGenerated -= HandleMainListGenerated;
            listController.OnListChanged -= HandleLegacyListChanged;
        }
        if (supplementalTaskController != null)
            supplementalTaskController.OnSupplementalTaskAdded -= HandleSupplementalTaskAdded;
    }

    private PokeManager inventoryPublisher;
    private void BindInventory()
    {
        if (inventoryPublisher == PokeManager.Instance) return;
        UnbindInventory();
        inventoryPublisher = PokeManager.Instance;
        if (inventoryPublisher != null)
            inventoryPublisher.OnInventoryChanged += HandleBillChanged;
    }
    private void UnbindInventory()
    {
        if (inventoryPublisher != null)
            inventoryPublisher.OnInventoryChanged -= HandleBillChanged;
        inventoryPublisher = null;
    }
    private void OnEnable() => BindInventory();
    private void OnDisable() => UnbindInventory();
    private void Update()
    {
        if (gameObject.scene.name == "Scene-level-3") return;
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

    private void HandleMainListGenerated()
    {
        if (mainListRegistered)
            return;
        mainListRegistered = true;
        RegisterRenderedLegacyItems();
    }

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
        if (listController == null || listController.choicedItems == null)
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
        if (gameObject.scene.name == "Scene-level-3" && CartManager.Instance != null &&
            CartManager.Instance.bill.TryGetValue(itemName, out BillEntry billed)) return billed;
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
            quantityText = $"{currentQuantity} (Thiáº¿u {shortage})";
            return $"Thiáº¿u {shortage}";
        }
        if (currentQuantity == indexQuantity)
        {
            quantityText = $"{currentQuantity} (Äá»§)";
            return "Äá»§";
        }
        int surplus = currentQuantity - indexQuantity;
        quantityText = $"{currentQuantity} (DÆ° {surplus})";
        return $"DÆ° {surplus}";
    }

    private void CreateNewInstantData(BillEntry entry, Transform parentContainer, string quantityText)
    {
        if (entry == null || dataContainer == null || parentContainer == null)
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
        if (hasFinalizedReport || DataManager.Instance == null)
            return;

        List<CompareResult> filteredResults = new List<CompareResult>();
        foreach (CompareResult result in compareResults)
        {
            if (!result.required && result.currentQuantity == 0)
                continue;
            filteredResults.Add(result);
        }

        DataManager.Instance.SetCompareResults(filteredResults);
        DataManager.Instance.Report();
        hasFinalizedReport = true;
        LoadCSV -= LoadCompareResult;
    }

    public void FinalizeResultsForReport()
    {
        if (gameObject.scene.name == "Scene-level-3")
        {
            if (!mainListRegistered) HandleMainListGenerated();
            foreach (string name in new List<string>(requiredQuantities.Keys)) UpsertRequiredResult(name);
            if (CartManager.Instance != null)
                foreach (BillEntry entry in CartManager.Instance.bill.Values)
                    if (!requiredQuantities.ContainsKey(entry.itemName)) UpsertUnexpectedResult(entry);
        }
        LoadCompareResult();
    }
}
