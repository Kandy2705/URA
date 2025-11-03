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
    public Dictionary<string, BillEntry> bill = new Dictionary<string, BillEntry>();
    public delegate void BillChangedHandler(BillEntry entry, bool isNew);
    public event BillChangedHandler OnBillChanged;
    // public List<BillEntry> billList = new List<BillEntry>();
    private int totalPaid = 0;
    public int TotalPaid {get{return totalPaid;}}
    public bool isNew;

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
        isNew = false;
        BillEntry entry;
        if (bill.ContainsKey(item.itemName))
        {
            bill[item.itemName].quantity++;
            OnBillChanged?.Invoke(bill[item.itemName], isNew);
        }
        else
        {
            entry = new BillEntry(item.itemName, item.price, 1);
            bill[item.itemName] = entry;
            isNew = true;
            OnBillChanged?.Invoke(entry, isNew);
        }

        Debug.Log($"Checked out: {item.itemName}, Price: {item.price}, Quantity: {bill[item.itemName].quantity}");

        if (DataManager.Instance != null)
        {
            DataManager.Instance.updateTime(item.itemName);
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
}