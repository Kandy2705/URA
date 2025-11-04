using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PokeManager : MonoBehaviour
{
    public static PokeManager Instance { get; private set; }
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

        UpdateInventoryView();

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
}
