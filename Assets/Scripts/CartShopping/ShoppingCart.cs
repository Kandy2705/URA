using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]

public class ShoppingCart : MonoBehaviour
{

    [Header("UI References")]
    public Text cartContentsText;
    public Transform cartItemsContentParent;
    public GameObject cartItemUIEntryPrefab;

    [Header("Item Database")]
    [Tooltip("Kéo TẤT CẢ các ScriptableObject 'CollectibleItem' của bạn vào đây")]
    public List<CollectibleItem> allItemDefinitions;

    private CollectibleItem GetItemDefinition(string name)
    {
        // Tên hiển thị trong scene có thể khác chữ hoa/thường hoặc có khoảng trắng
        // thừa so với tên trong database (ví dụ: "Kẹo socola" / "Kẹo Socola").
        if (allItemDefinitions == null || string.IsNullOrWhiteSpace(name))
            return null;

        string normalizedName = name.Trim();
        return allItemDefinitions.FirstOrDefault(item =>
            item != null &&
            !string.IsNullOrWhiteSpace(item.itemName) &&
            string.Equals(item.itemName.Trim(), normalizedName, System.StringComparison.OrdinalIgnoreCase));
    }

    public void Start()
    {
        if (PokeManager.Instance != null)
        {
            UpdateCartUI();

            PokeManager.Instance.OnInventoryChanged += (entry, isNew) => UpdateCartUI();
        }
        else
        {
            Debug.LogError("Không tìm thấy PokeManager.Instance trong Scene!");
        }
    }

    //public void RemoveSpecificItemInstance(CartItemInstance itemToRemoveInstance)
    //{
    //    if (collectedItems.Remove(itemToRemoveInstance))
    //    {
    //        Debug.Log("Removing item instance: " + itemToRemoveInstance.itemData.itemName);
    //        if (itemToRemoveInstance.sceneObjectReference != null)
    //        {
    //            itemToRemoveInstance.sceneObjectReference.SetActive(true);
    //            Debug.Log("Reactivated scene object for item: " + itemToRemoveInstance.itemData.itemName);
    //        }
    //        else
    //        {
    //            Debug.LogWarning("Scene object reference is null for item: " + itemToRemoveInstance.itemData.itemName);
    //        }
    //        UpdateCartUI();
    //    }
    //    else
    //    {
    //        Debug.LogWarning("Item instance not found in cart: " + itemToRemoveInstance.itemData.itemName);
    //    }
    //}

    public void UpdateCartUI()
    {
        if (cartItemsContentParent == null || cartItemUIEntryPrefab == null || PokeManager.Instance == null)
        {
            Debug.LogWarning("UI references hoặc PokeManager không được set!");
            return;
        }

        foreach (Transform child in cartItemsContentParent)
        {
            Destroy(child.gameObject);
        }

        Dictionary<string, BillEntry> realInventory = PokeManager.Instance.inventory;

        int totalQuantity = 0;
        foreach (BillEntry entry in realInventory.Values)
        {
            totalQuantity += entry.quantity;
        }

        if (totalQuantity == 0)
        {
            if (cartContentsText != null)
                cartContentsText.text = "Cart is Empty";
        }
        else
        {
            if (cartContentsText != null)
            {
                cartContentsText.text = $"{totalQuantity} Item{(totalQuantity > 1 ? "s" : "")}";
            }

            foreach (BillEntry billEntry in realInventory.Values)
            {
                CollectibleItem itemDef = GetItemDefinition(billEntry.itemName);

                if (itemDef == null)
                {
                    Debug.LogWarning($"Không tìm thấy CollectibleItem cho '{billEntry.itemName}' trong 'allItemDefinitions'");
                    continue;
                }

                CartItemInstance tempInstance = new CartItemInstance(itemDef, null);

                for (int i = 0; i < billEntry.quantity; i++)
                {
                    GameObject newEntryObj = Instantiate(cartItemUIEntryPrefab, cartItemsContentParent);
                    CartItemUIEntry entryComponent = newEntryObj.GetComponent<CartItemUIEntry>();

                    if (entryComponent != null)
                    {
                        entryComponent.SetItem(tempInstance, this);
                    }
                }
            }
        }
    }
}
