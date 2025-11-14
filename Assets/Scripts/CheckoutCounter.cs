using UnityEngine;

public class CheckoutCounter : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player đã vào quầy, kích hoạt thanh toán!");

            CartManager.Instance.ProcessCheckout();
        }
    }
}