using System;
using UnityEngine;

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
