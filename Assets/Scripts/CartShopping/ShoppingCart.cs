using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[System.Serializable]

public class ShoppingCart : MonoBehaviour
{
    public List<CartItemInstance> collectedItems = new List<CartItemInstance>();

    [Header("UI References")]
    public Text cartContentsText;
    public Transform cartItemsContentParent;
    public GameObject cartItemUIEntryPrefab;

    [Header("TESTING DATA")]
    public List<CollectibleItem> testItemAssets;

    public void Start()
    {
        if (testItemAssets != null)
        {
            foreach (CollectibleItem itemAsset in testItemAssets)
            {
                CartItemInstance newInstance = new CartItemInstance(itemAsset, null);
                collectedItems.Add(newInstance);
            }
        }

        UpdateCartUI(); 
    }

    public void RemoveSpecificItemInstance(CartItemInstance itemToRemoveInstance)
    {
        if (collectedItems.Remove(itemToRemoveInstance))
        {
            Debug.Log("Removing item instance: " + itemToRemoveInstance.itemData.itemName);
            if (itemToRemoveInstance.sceneObjectReference != null)
            {
                itemToRemoveInstance.sceneObjectReference.SetActive(true);
                Debug.Log("Reactivated scene object for item: " + itemToRemoveInstance.itemData.itemName);
            }
            else
            {
                Debug.LogWarning("Scene object reference is null for item: " + itemToRemoveInstance.itemData.itemName);
            }
            UpdateCartUI();
        }
        else
        {
            Debug.LogWarning("Item instance not found in cart: " + itemToRemoveInstance.itemData.itemName);
        }
    }

    public void UpdateCartUI()
    {
        if (cartItemsContentParent == null || cartItemUIEntryPrefab == null)
        {
            Debug.LogWarning("UI references are not set properly in ShoppingCart.");
            return;
        }

        foreach (Transform child in cartItemsContentParent)
        {
            Destroy(child.gameObject);
        }

        if (collectedItems.Count == 0)
        {
            if(cartContentsText != null)
                cartContentsText.text = "Cart is Empty";
        }
        else
        {
            if (cartContentsText != null)
            {
                cartContentsText.text = $"{collectedItems.Count} Item{(collectedItems.Count > 1 ? "s" : "")}";
            }

            foreach (CartItemInstance itemInstance in collectedItems)
            {
                GameObject newEntryObj = Instantiate(cartItemUIEntryPrefab, cartItemsContentParent);
                CartItemUIEntry entryComponent = newEntryObj.GetComponent<CartItemUIEntry>();
                if (entryComponent != null)
                {
                    entryComponent.SetItem(itemInstance, this);
                }
            }
        }
    }
}
