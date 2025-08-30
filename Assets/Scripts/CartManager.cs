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

    [SerializeField] private GameObject itemsContainer;

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
        ItemQuantityUpdate(item.itemName);
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

    public void ItemQuantityUpdate(string itemName)
    {
        foreach (Transform item in itemsContainer.transform)
        {
            
            TMP_Text nameText = item.Find("Name")?.GetComponent<TMP_Text>();
            TMP_Text quantityText = item.Find("Quantity")?.GetComponent<TMP_Text>();

            if (nameText == null || quantityText == null)
                continue;
                
            Debug.Log($"nameText: {nameText.text}, itemName: {itemName}");
            if (nameText.text == itemName)
            {
                Debug.Log("đúng Item name rồi hehehe");

                int currentQty = int.Parse(quantityText.text);

                if (currentQty > 0)
                {
                    currentQty--;
                    quantityText.text = currentQty.ToString();

                    Debug.Log($"Đã lấy {itemName}, còn lại: {currentQty}");
                }
                else
                {
                    Debug.Log($"{itemName} đã đủ số lượng!");
                }

                return;
            }
        }

        Debug.Log($"{itemName} không có trong danh sách cần mua.");
    }
}
