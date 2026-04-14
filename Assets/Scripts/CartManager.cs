using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CartManager : MonoBehaviour
{
    public static CartManager Instance { get; private set; }
    public Dictionary<string, BillEntry> bill = new Dictionary<string, BillEntry>();
    public delegate void BillChangedHandler(BillEntry entry, bool isNew);
    public event BillChangedHandler OnBillChanged;

    private int totalPaid = 0;
    public int TotalPaid { get { return totalPaid; } }
    public bool isNew; 

    [Header("UI Thanh toán")]
    public TMP_Text billText;
    public TMP_Text paidTotalText;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        UpdateBillUI();
    }
    public void ProcessCheckout()
    {
        if (PokeManager.Instance == null)
        {
            Debug.LogError("Không tìm thấy PokeManager!");
            return;
        }

        var shoppingCart = PokeManager.Instance.inventory;

        if (shoppingCart.Count == 0)
        {
            Debug.Log("Giỏ hàng rỗng, không có gì để thanh toán.");
            return;
        }

        Debug.Log($"Đang chuyển {shoppingCart.Count} loại item từ giỏ hàng sang quầy thanh toán...");

        foreach (var itemEntry in shoppingCart.Values)
        {
            if (bill.ContainsKey(itemEntry.itemName))
            {
                bill[itemEntry.itemName].quantity += itemEntry.quantity;
            }
            else
            {
                bill.Add(itemEntry.itemName, new BillEntry(itemEntry.itemName, itemEntry.price, itemEntry.quantity));
            }
        }

        UpdateBillUI();
        PokeManager.Instance.ClearCart();
    }

    private void UpdateBillUI()
    {
        totalPaid = 0;

        Debug.Log($"[CartManager] Cập nhật hóa đơn... Có {bill.Count} loại mặt hàng.");

        if (billText != null)
        {
            billText.text = "Hóa đơn:\n";
            foreach (var entry in bill.Values)
            {
                int subTotal = entry.price * entry.quantity;
                billText.text += $"{entry.itemName} x{entry.quantity} = {subTotal}₫\n";

                totalPaid += subTotal;
                Debug.Log($"[CartManager] - {entry.itemName}: SL={entry.quantity}, Giá={entry.price}");
            }
        }

        if (paidTotalText != null)
        {
            paidTotalText.text = $"Tổng: {totalPaid}₫";
        }

        Debug.Log($"[CartManager] Tổng tiền hóa đơn: {totalPaid}₫");
    }
}
