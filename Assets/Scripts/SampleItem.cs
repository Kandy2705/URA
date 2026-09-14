using System;
using UnityEngine;

public class SampleItem : MonoBehaviour
{
    public static event Action OnSampleItemAddedToCart;
    public static event Action OnWrongItemTouched;

    [SerializeField] private string cartTag = "Cart";
    private bool isInCart = false;

    // Gọi hàm này khi có bất kỳ tay/raycast nào chạm nhầm vào vật khác
    public static void TriggerWrongItem()
    {
        OnWrongItemTouched?.Invoke();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isInCart) return;

        // Kiểm tra vật phẩm rơi vào Giỏ hàng
        if (other.CompareTag(cartTag))
        {
            isInCart = true;
            Debug.Log("[SampleItem] Đã bỏ item vào giỏ hàng thành công!");
            OnSampleItemAddedToCart?.Invoke();
        }
    }
}