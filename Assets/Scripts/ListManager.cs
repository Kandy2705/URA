using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShoppingListManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject shoppingListPanel;   // Panel (the pop-up window)
    public GameObject listItemPrefab;      // Prefab for each item (TMP text)
    public Transform contentArea;          // Vertical Layout container
    public Button shoppingListButton;      // assign in inspector

    // Product list
    List<string> products = new List<string>() { "Táo", "Mì", "Cá", "Thịt", "Nước" };

    // Shopping list
    private List<string> shoppingList = new List<string>();

    void Start()
    {
        // Fix: call ToggleShoppingList instead of missing ToBuyList
        shoppingListButton.onClick.AddListener(ToggleShoppingList);

        // Generate random items for the shopping list
        int quantity = Random.Range(1, products.Count);
        for (int i = 0; i < quantity; i++)
        {
            AddRandomProduct(i);
        }

        // Debug print
        foreach (string item in shoppingList)
        {
            Debug.Log(item);
        }

        UpdateListUI();

        // Start hidden
        shoppingListPanel.SetActive(false);
    }

    void AddRandomProduct(int i)
    {
        // Pick random quantity (between 1 and 5, can adjust)
        int quantity = Random.Range(1, 5);

        // Add to shopping list
        string product = products[i];
        string entry = $"{quantity} x {product}";
        shoppingList.Add(entry);
    }

    void ToggleShoppingList()
    {
        shoppingListPanel.SetActive(!shoppingListPanel.activeSelf);
    }

    public void AddItem(string itemName)
    {
        shoppingList.Add(itemName);
        UpdateListUI();
    }

    public void RemoveItem(string itemName)
    {
        shoppingList.Remove(itemName);
        UpdateListUI();
    }

    void UpdateListUI()
    {
        // Clear old items
        foreach (Transform child in contentArea)
        {
            Destroy(child.gameObject);
        }

        // Create new items
        foreach (string item in shoppingList)
        {
            GameObject newItem = Instantiate(listItemPrefab, contentArea);
            newItem.GetComponent<TextMeshProUGUI>().text = "• " + item;
        }
    }
}
