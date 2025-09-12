using UnityEngine;

public class CheckoutCounter : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        SelectableItem item = other.GetComponent<SelectableItem>();

        if (item != null)
        {
            CartManager.Instance.CheckoutItem(item);

            Destroy(other.gameObject);

            Debug.Log($"[{item.itemName}] đã được thanh toán!");
        }
    }
}
