using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public class CartEntry
{
    public string itemName;
    public int price;
    public int quantity;

    public CartEntry(string itemName, int price)
    {
        this.itemName = itemName;
        this.price = price;
        this.quantity = 1;
    }
}

public class CartManager : MonoBehaviour
{
    public static CartManager Instance { get; private set; }

    private Dictionary<string, CartEntry> cart = new Dictionary<string, CartEntry>();

    [Header("UI References")]
    public TMP_Text cartText;
    public TMP_Text totalText;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    public void AddItem(SelectableItem item)
    {
        string key = item.itemName;

        if (cart.ContainsKey(key))
        {
            cart[key].quantity++;
        }
        else
        {
            cart[key] = new CartEntry(item.itemName, item.price);
        }

        UpdateUI();
        Debug.Log($"Đã thêm {item.itemName} (SL={cart[key].quantity}) vào giỏ!");
    }

    private void UpdateUI()
    {
        cartText.text = "Giỏ hàng:\n";
        int total = 0;

        foreach (var entry in cart.Values)
        {
            int sub = entry.price * entry.quantity;
            cartText.text += $"{entry.itemName} x{entry.quantity} = {sub}₫\n";
            total += sub;
        }

        totalText.text = $"Tổng: {total}₫";
    }
}
