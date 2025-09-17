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
<<<<<<< HEAD
        string key = item.itemName;

        Debug.Log("Here");
        DataManager.Instance.updateTime(key);

        if (cart.ContainsKey(key))
=======
        if (bill.ContainsKey(item.itemName))
>>>>>>> f26885d9aa89b9f8502d4db60d05dc7dbe414eae
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
}
