using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public class BillEntry
{
    public string itemName;
    public int price;
    public int quantity;

    public BillEntry(string itemName, int price, int quantity = 1)
    {
        this.itemName = itemName;
        this.price = price;
        this.quantity = quantity;
    }
}

public class CartManager : MonoBehaviour
{
    public static CartManager Instance { get; private set; }

    private Dictionary<string, BillEntry> bill = new Dictionary<string, BillEntry>();
    private int totalPaid = 0;

    [Header("UI Thanh toán")]
    public TMP_Text billText;
    public TMP_Text paidTotalText;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void CheckoutItem(SelectableItem item)
    {
        if (bill.ContainsKey(item.itemName))
        {
            bill[item.itemName].quantity++;
        }
        else
        {
            bill[item.itemName] = new BillEntry(item.itemName, item.price, 1);
        }

        totalPaid += item.price;

        UpdateBillUI();
        //Debug.Log($"Thanh toán {item.itemName} với giá {item.price}₫ (SL={bill[item.itemName].quantity})");
    }

    private void UpdateBillUI()
    {
        billText.text = "Giỏ hàng:\n";
        foreach (var entry in bill.Values)
        {
            int subTotal = entry.price * entry.quantity;
            billText.text += $"{entry.itemName} x{entry.quantity} = {subTotal}₫\n";
        }

        paidTotalText.text = $"Tổng: {totalPaid}₫";
    }
    public TMP_Text cartText;
    public TMP_Text totalText;
    public TMP_Text correctCountText; // Assign in inspector

    [Header("Choiced Items")]
    // Removed local choicedItems list. Will use ListController's choicedItems.
    [Header("References")]
    [SerializeField] private ListController listController; // Assign in Inspector

    public void AddItem(SelectableItem item)
    {
        string key = item.itemName;

        if (bill.ContainsKey(key))
        {
            bill[key].quantity++;
        }
        else
        {
            bill[key] = new BillEntry(item.itemName, item.price, 1);
        }

        UpdateBillUI();
        Debug.Log($"Đã thêm {item.itemName} (SL={bill[key].quantity}) vào giỏ!");
    }

    private void UpdateUI()
    {
        cartText.text = "Giỏ hàng:\n";
        int total = 0;

        foreach (var entry in bill.Values)
        {
            int sub = entry.price * entry.quantity;
            cartText.text += $"{entry.itemName} x{entry.quantity} = {sub}₫\n";
            total += sub;
        }

        totalText.text = $"Tổng: {total}₫";

        // Count correct items compared to choicedItems in ListController
        int correctCount = 0;
        var choicedItems = listController != null ? listController.choicedItems : new List<GameObject>();
        foreach (var item in choicedItems)
        {
            if (bill.ContainsKey(item.name) && bill[item.name].quantity > 0)
            {
                correctCount++;
            }
        }
        if (correctCountText != null)
            correctCountText.text = $"Correctly selected: {correctCount}/{choicedItems.Count}";
    }
}
