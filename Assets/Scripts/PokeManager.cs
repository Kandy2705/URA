using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using TMPro;

public class PokeManager : MonoBehaviour
{
    [SerializeField] private TMP_Text itemNameText;
    [SerializeField] private GameObject NotificationPanel;
    [SerializeField] private Transform UserCameraTransform;
    [SerializeField] private CanvasGroup notificationCanvasGroup;
    public float DisplayDistance = 0.5f;
    [Header("Proximity item hint")]
    [SerializeField] private float proximityHintDistance = 2.5f;
    [SerializeField] private float proximityScanInterval = 0.15f;
    public static PokeManager Instance { get; private set; }
    public Dictionary<string, BillEntry> inventory = new Dictionary<string, BillEntry>();
    private readonly Dictionary<string, int> initialProductStock = new Dictionary<string, int>();
    private readonly Dictionary<string, int> remainingProductStock = new Dictionary<string, int>();

    [Header("Debug View (Read-only)")]
    [SerializeField] private List<BillEntry> inventoryView = new List<BillEntry>();
    public delegate void InventoryChangedHandler(BillEntry entry, bool isNew);
    public event InventoryChangedHandler OnInventoryChanged;
    private int totalValue = 0;
    private SelectableItem[] selectableItems;
    private float nextProximityScanTime;
    private SelectableItem currentNearbyItem;
    public int TotalValue {get{return totalValue;}}
    public bool isNew;

    [Header("UI Inventory")]
    public TMP_Text inventoryText;
    public TMP_Text totalValueText;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (Camera.main != null)
            UserCameraTransform = Camera.main.transform;

        if (notificationCanvasGroup != null)
        {
            notificationCanvasGroup.interactable = false;
            notificationCanvasGroup.blocksRaycasts = false;
        }

        if (NotificationPanel != null)
            NotificationPanel.SetActive(false);

        UpdateTotalsAndUI();
    }

    private void Start()
    {
        selectableItems = FindObjectsByType<SelectableItem>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        InitializeProductStock();
    }

    private void Update()
    {
        if (Time.unscaledTime < nextProximityScanTime)
            return;

        nextProximityScanTime = Time.unscaledTime + Mathf.Max(0.05f, proximityScanInterval);
        UpdateProximityHint();
    }

    public void PokingItem(SelectableItem item)
    {
        EnsureProductStockInitialized();
        int remainingStock = GetRemainingStock(item.itemName);
        if (remainingStock <= 0)
        {
            ShowItemHint(item);
            Debug.Log($"PokeManager: {item.itemName} đã hết hàng.");
            return;
        }

        remainingProductStock[item.itemName] = remainingStock - 1;
        isNew = false;
        BillEntry entry;
        if (inventory.ContainsKey(item.itemName))
        {
            inventory[item.itemName].quantity++;
            OnInventoryChanged?.Invoke(inventory[item.itemName], isNew);
        }
        else
        {
            entry = new BillEntry(item.itemName, item.price, 1);
            inventory[item.itemName] = entry;
            isNew = true;
            OnInventoryChanged?.Invoke(entry, isNew);
        }

        if (DataManager.Instance != null)
        {
            DataManager.Instance.updateTime(item.itemName);
        }

        UpdateInventoryView();
        UpdateTotalsAndUI();
        ShowItemHint(item);

        Debug.Log($"Poked: {item.itemName}, Price: {item.price}, Quantity: {inventory[item.itemName].quantity}");
    }


    private void UpdateInventoryView()
    {
        inventoryView.Clear();
        foreach (var entry in inventory.Values)
        {
            inventoryView.Add(entry);
        }
    }
    public void RemoveItem(string itemName)
    {
        if (inventory.ContainsKey(itemName))
        {
            BillEntry entry = inventory[itemName];
            entry.quantity--;

            if (entry.quantity <= 0)
            {
                inventory.Remove(itemName);
            }

            OnInventoryChanged?.Invoke(entry, false);

            if (initialProductStock.TryGetValue(itemName, out int initialStock))
            {
                int currentStock = GetRemainingStock(itemName);
                remainingProductStock[itemName] = Mathf.Min(initialStock, currentStock + 1);
            }

            UpdateInventoryView();
            UpdateTotalsAndUI();
        }
    }
    public void ClearCart()
    {
        inventory.Clear();

        UpdateInventoryView();
        UpdateTotalsAndUI();

        OnInventoryChanged?.Invoke(null, false);
    }
    public void UpdateTotalsAndUI()
    {
        int newTotalValue = 0;
        int totalItemCount = 0;

        foreach (BillEntry entry in inventory.Values)
        {
            newTotalValue += entry.price * entry.quantity;
            totalItemCount += entry.quantity;
        }

        totalValue = newTotalValue;

        if (totalValueText != null)
        {
            totalValueText.text = $"Tổng: {totalValue} VND";
        }

        if (inventoryText != null)
        {
            inventoryText.text = $"Số lượng: {totalItemCount}";
        }
    }

    private void UpdateProximityHint()
    {
        if (UserCameraTransform == null && Camera.main != null)
            UserCameraTransform = Camera.main.transform;

        if (UserCameraTransform == null)
            return;

        if (selectableItems == null || selectableItems.Length == 0)
            selectableItems = FindObjectsByType<SelectableItem>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);

        SelectableItem closestItem = null;
        float closestDistance = proximityHintDistance;
        foreach (SelectableItem item in selectableItems)
        {
            if (item == null || !item.isActiveAndEnabled)
                continue;

            float distance = Vector3.Distance(UserCameraTransform.position, item.transform.position);
            if (distance > closestDistance)
                continue;

            closestDistance = distance;
            closestItem = item;
        }

        if (closestItem == null)
        {
            currentNearbyItem = null;
            HideItemHint();
            return;
        }

        currentNearbyItem = closestItem;
        ShowItemHint(currentNearbyItem);
    }

    private void EnsureProductStockInitialized()
    {
        if (remainingProductStock.Count == 0)
            InitializeProductStock();
    }

    private void InitializeProductStock()
    {
        initialProductStock.Clear();
        remainingProductStock.Clear();

        if (selectableItems == null || selectableItems.Length == 0)
            selectableItems = FindObjectsByType<SelectableItem>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);

        foreach (SelectableItem item in selectableItems)
        {
            if (item == null || string.IsNullOrWhiteSpace(item.itemName))
                continue;

            int stockToAdd = Mathf.Max(1, item.stockQuantity);
            initialProductStock[item.itemName] = GetStockValue(initialProductStock, item.itemName) + stockToAdd;
        }

        foreach (KeyValuePair<string, int> stockEntry in initialProductStock)
            remainingProductStock[stockEntry.Key] = stockEntry.Value;
    }

    private int GetRemainingStock(string itemName)
    {
        return GetStockValue(remainingProductStock, itemName);
    }

    private static int GetStockValue(Dictionary<string, int> stock, string itemName)
    {
        return stock.TryGetValue(itemName, out int value) ? value : 0;
    }

    private void ShowItemHint(SelectableItem item)
    {
        if (item == null || NotificationPanel == null || UserCameraTransform == null)
            return;

        EnsureProductStockInitialized();
        int remainingStock = GetRemainingStock(item.itemName);
        string stockText = remainingStock > 0 ? $"Còn lại: {remainingStock}" : "Hết hàng";

        if (itemNameText != null)
        {
            itemNameText.enableAutoSizing = true;
            itemNameText.fontSizeMin = 18f;
            itemNameText.fontSizeMax = 34f;
            itemNameText.text =
                $"{item.itemName}\n{item.price:N0}đ  |  {stockText}\n" +
                (remainingStock > 0 ? "Chạm để lấy" : "Vui lòng chọn món khác");
        }

        NotificationPanel.SetActive(true);
        if (notificationCanvasGroup != null)
            notificationCanvasGroup.alpha = 0.92f;

        Vector3 newPosition = UserCameraTransform.position + UserCameraTransform.forward * DisplayDistance;
        NotificationPanel.transform.position = newPosition;
        NotificationPanel.transform.rotation = Quaternion.LookRotation(
            NotificationPanel.transform.position - UserCameraTransform.position
        );
    }

    private void HideItemHint()
    {
        if (notificationCanvasGroup != null)
            notificationCanvasGroup.alpha = 0f;

        if (NotificationPanel != null)
            NotificationPanel.SetActive(false);
    }
}
