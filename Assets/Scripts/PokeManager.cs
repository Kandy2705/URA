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
    public static PokeManager Instance { get; private set; }
    private const float DISPLAY_DURATION = 3f;
    private const float ANIMATION_TIME = 0.2f;
    public Dictionary<string, BillEntry> inventory = new Dictionary<string, BillEntry>();

    [Header("Debug View (Read-only)")]
    [SerializeField] private List<BillEntry> inventoryView = new List<BillEntry>();
    public delegate void InventoryChangedHandler(BillEntry entry, bool isNew);
    public event InventoryChangedHandler OnInventoryChanged;
    private int totalValue = 0;
    public int TotalValue {get{return totalValue;}}
    public bool isNew;

    [Header("UI Inventory")]
    public TMP_Text inventoryText;
    public TMP_Text totalValueText;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        UpdateTotalsAndUI();
    }

    public void PokingItem(SelectableItem item)
    {
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

        itemNameText.text = item.itemName;
        DisplayNotificationPanel();
        UpdateInventoryView();
        UpdateTotalsAndUI();

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

    private void DisplayNotificationPanel()
    {
        if (NotificationPanel == null || UserCameraTransform == null)
        {
            Debug.LogError("Chưa gán NotificationPanel hoặc UserCameraTransform!");
            return;
        }
        Debug.Log("Hiển thị ra bản Panel thông báo");
        StopAllCoroutines(); 
        NotificationPanel.SetActive(true);
        Vector3 newPosition = UserCameraTransform.position + UserCameraTransform.forward * DisplayDistance;
        NotificationPanel.transform.position = newPosition;
        NotificationPanel.transform.rotation = Quaternion.LookRotation(
            NotificationPanel.transform.position - UserCameraTransform.position
        );

        StartCoroutine(ShowAndHidePanelSequence());
    }
    
    private IEnumerator ShowAndHidePanelSequence()
    {
        yield return StartCoroutine(FadePanel(1f, ANIMATION_TIME)); // Fade đến Alpha = 1

        yield return new WaitForSeconds(DISPLAY_DURATION);

        yield return StartCoroutine(FadePanel(0f, ANIMATION_TIME)); // Fade đến Alpha = 0
        NotificationPanel.SetActive(false);
    }

    private IEnumerator FadePanel(float targetAlpha, float duration)
    {
        float startAlpha = notificationCanvasGroup.alpha;
        float startTime = Time.time;
        
        while (Time.time < startTime + duration)
        {
            float t = (Time.time - startTime) / duration;
            
            
            notificationCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, t);
            
            yield return null;
        }
        
        notificationCanvasGroup.alpha = targetAlpha;
    }
}


