using UnityEngine;
using UnityEngine.UI;

public class CheckoutButtonBinder : MonoBehaviour
{
    public Button checkoutButton;
    public VRCheckoutTeleport checkoutSystem;

    void Start()
    {
        checkoutButton.onClick.AddListener(checkoutSystem.MoveToCheckout);
    }
}