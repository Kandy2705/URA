using UnityEngine;
using UnityEngine.UI;

public class CartItemUIEntry : MonoBehaviour, ICartItem
{
    private ShoppingCart shoppingCart;
    private CartItemInstance currentItemInstance;

    public Image itemIcon;
    public Text itemName;

    public void SetItem(CartItemInstance itemInstance, ShoppingCart cartRef)
    {
        currentItemInstance = itemInstance;
        shoppingCart = cartRef;

        if (itemIcon != null)
        {
            itemIcon.sprite = currentItemInstance.itemData.itemIcon;
            itemIcon.type = Image.Type.Simple;
            itemIcon.preserveAspect = true;
        }

        if (itemName != null)
        {
            itemName.text = currentItemInstance.itemData.itemName;
        }
    }

    public void OnClickRemoveItem()
    {
        if (currentItemInstance != null && PokeManager.Instance != null)
        {
            string itemName = currentItemInstance.itemData.itemName;

            PokeManager.Instance.RemoveItem(itemName);
        }
    }

    public CollectibleItem GetItemData() { 
        return currentItemInstance?.itemData;
    }
}
