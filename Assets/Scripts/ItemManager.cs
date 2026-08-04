using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class ItemsManager : MonoBehaviour {
    private static List<SelectableItem> itemList = new List<SelectableItem>();
    private static HashSet<string> itemNameList = new HashSet<string>();
    private static int minDiscount = 10;
    private static int maxDiscount = 60;

    private static string discountText;

    public static void RegisterSelectableItem(SelectableItem item)
    {
        itemList.Add(item);
        itemNameList.Add(item.itemName);
    }

    public static void UnregisterSelectableItem(SelectableItem item)
    {
        itemList.Remove(item);
        itemNameList.Remove(item.itemName);
    }

    public static void Testing()
    {
        foreach (var name in itemNameList)
        {
            Debug.Log(name);
        }
    }

    public static string DiscountRandomItem()
    {
        int randomIndex = UnityEngine.Random.Range(0, itemNameList.Count);
        string randomName = itemNameList.ElementAt(randomIndex);

        int randomDiscount = UnityEngine.Random.Range(minDiscount, maxDiscount); // Discount 10 - 60%
        foreach (SelectableItem item in itemList)
        {
            if (item.itemName == randomName)
            {
                item.price = (int)(item.price * (1 - randomDiscount / 100.0));
                Debug.Log($"Đã giảm giá {item.itemName}, còn {item.price}");
            }
        }

        discountText = $"Đã giảm giá {randomName} {randomDiscount} phần trăm";
        Debug.Log(discountText);
        return discountText;
    }
}