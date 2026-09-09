using System;
using UnityEngine;

[Serializable]
public class ShoppingTaskItem
{
    [Tooltip("Tên phải khớp SelectableItem.itemName.")]
    public string itemName;
    [Min(1)] public int requiredQuantity = 1;
    [Tooltip("Prefab dòng UI hiện có. Để trống sẽ tìm theo tên trong ListController.")]
    public GameObject viewPrefab;

    public ShoppingTaskItem() { }

    public ShoppingTaskItem(string name, int quantity)
    {
        itemName = name;
        requiredQuantity = Mathf.Max(1, quantity);
    }
}
