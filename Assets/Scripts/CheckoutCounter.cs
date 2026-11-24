using UnityEngine;

public class CheckoutCounter : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Level 3 uses the cart button/timer as its sole checkout entry.
        if (gameObject.scene.name == "Scene-level-3") return;
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player đã vào quầy, kích hoạt thanh toán!");

            CartManager.Instance.ProcessCheckout();
        }
    }
}