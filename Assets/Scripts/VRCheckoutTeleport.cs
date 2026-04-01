using UnityEngine;

public class VRCheckoutTeleport : MonoBehaviour
{
    [Header("References")]
    public Transform xrOrigin;
    public Transform mainCamera;
    public Transform checkoutAnchor;

    [Header("Optional disable scripts")]
    public Behaviour[] movementScripts;
    public Behaviour[] turnScripts;
    public Behaviour[] otherControlScripts;

    [Header("Optional")]
    public CharacterController characterController;
    public Behaviour xrDeviceSimulator;
    public bool lockAfterTeleport = true;

    [Header("Desired LOCAL rotation of Main Camera at checkout")]
    public Vector3 checkoutCameraEuler = new Vector3(30f, 260f, 0f);

    private Vector3 savedPosition;
    private Quaternion savedRigRotation;
    private Quaternion savedCameraLocalRotation;
    private bool isLocked = false;

    public void MoveToCheckout()
    {
        if (xrOrigin == null || mainCamera == null || checkoutAnchor == null)
        {
            Debug.LogWarning("Thiếu reference");
            return;
        }

        savedPosition = xrOrigin.position;
        savedRigRotation = xrOrigin.rotation;
        savedCameraLocalRotation = mainCamera.localRotation;

        if (characterController != null)
            characterController.enabled = false;

        // Tính offset từ rig tới camera
        Vector3 rigToCameraOffset = mainCamera.position - xrOrigin.position;

        // Di chuyển rig sao cho camera world position trùng anchor
        xrOrigin.position = checkoutAnchor.position - rigToCameraOffset;

        if (lockAfterTeleport)
            LockPlayer();

        // Nếu bạn vẫn muốn ép local rotation của camera
        mainCamera.localRotation = Quaternion.Euler(checkoutCameraEuler);
    }

    public void LockPlayer()
    {
        isLocked = true;

        SetBehaviours(movementScripts, false);
        SetBehaviours(turnScripts, false);
        SetBehaviours(otherControlScripts, false);

        if (characterController != null)
            characterController.enabled = false;

        if (xrDeviceSimulator != null)
            xrDeviceSimulator.enabled = false;
    }

    public void UnlockPlayer()
    {
        isLocked = false;

        if (xrDeviceSimulator != null)
            xrDeviceSimulator.enabled = true;

        if (characterController != null)
            characterController.enabled = true;

        SetBehaviours(movementScripts, true);
        SetBehaviours(turnScripts, true);
        SetBehaviours(otherControlScripts, true);
    }

    public void ReturnFromCheckout()
    {
        if (characterController != null)
            characterController.enabled = false;

        xrOrigin.SetPositionAndRotation(savedPosition, savedRigRotation);
        mainCamera.localRotation = savedCameraLocalRotation;

        UnlockPlayer();
    }

    private void SetBehaviours(Behaviour[] arr, bool state)
    {
        if (arr == null) return;

        foreach (var b in arr)
        {
            if (b != null)
                b.enabled = state;
        }
    }
}