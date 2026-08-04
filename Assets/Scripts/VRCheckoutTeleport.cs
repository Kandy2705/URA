using UnityEngine;

public class VRCheckoutTeleport : MonoBehaviour
{
    [Header("References")]
    public Transform xrOrigin;
    public Transform mainCamera;
    public Transform checkoutAnchor;
    [Tooltip("Điểm POI tại quầy thanh toán — XR Origin sẽ được đặt trùng toàn bộ transform của POI này. Để trống sẽ tự tìm object tên 'PoiChargeMoney'.")]
    public Transform poiChargeMoney;
    [Tooltip("Camera Offset (con của XR Origin) sẽ được đặt trùng toàn bộ transform của CameraPoi. Để trống sẽ tự tìm child tên 'Camera Offset'.")]
    public Transform cameraOffset;
    [Tooltip("Điểm CameraPoi — Camera Offset sẽ trùng transform với nó. Để trống sẽ tự tìm object tên 'CameraPoi'.")]
    public Transform cameraPoi;
    [Tooltip("Điểm MainCameraPoi — Main Camera sẽ trùng transform với nó. Để trống sẽ tự tìm object tên 'MainCameraPoi'. Nếu không có thì Main Camera reset local về 0.")]
    public Transform mainCameraPoi;
    [Tooltip("Điểm nhìn vào mặt NPC khi bấm thanh toán. Để trống sẽ tự tìm Head của object 'NPCs charge money'.")]
    public Transform cashierFaceTarget;
    public GameObject paymentUiToHide;
    [Tooltip("UI 'Scroll UI Sample' sẽ bị ẩn khi thanh toán. Để trống sẽ tự tìm object tên 'Scroll UI Sample'.")]
    public GameObject scrollUiSampleToHide;

    [Header("Optional disable scripts")]
    public Behaviour[] movementScripts;
    public Behaviour[] turnScripts;
    public Behaviour[] otherControlScripts;

    [Header("Optional")]
    public CharacterController characterController;
    public Behaviour xrDeviceSimulator;
    public bool lockAfterTeleport = true;
    [Tooltip("Giữ XR Device Simulator hoạt động để tay cầm vẫn chọn được tiền khi đã khóa vị trí player.")]
    public bool keepControllerInteractionActive = true;

    [Header("Desired LOCAL rotation of Main Camera at checkout")]
    public Vector3 checkoutCameraEuler = new Vector3(30f, 260f, 0f);

    private Vector3 savedPosition;
    private Quaternion savedRigRotation;
    private Quaternion savedCameraLocalRotation;
    private bool isLocked = false;

    private void Awake()
    {
        ResolveTargets();
    }

    private void ResolveTargets()
    {
        if (xrOrigin == null)
            xrOrigin = Camera.main != null ? Camera.main.transform.parent : null;

        if (mainCamera == null && Camera.main != null)
            mainCamera = Camera.main.transform;

        if (poiChargeMoney == null)
        {
            GameObject poi = GameObject.Find("PoiChargeMoney");
            if (poi != null)
                poiChargeMoney = poi.transform;
            else
                Debug.LogWarning("VRCheckoutTeleport: Không tìm thấy object 'PoiChargeMoney'. Kiểm tra tên trong scene.");
        }

        if (cameraOffset == null && xrOrigin != null)
        {
            Transform offset = xrOrigin.Find("Camera Offset");
            if (offset != null)
                cameraOffset = offset;
            else
                Debug.LogWarning("VRCheckoutTeleport: Không tìm thấy child 'Camera Offset' trong XR Origin.");
        }

        if (cameraPoi == null)
        {
            GameObject poi = GameObject.Find("CameraPoi");
            if (poi != null)
                cameraPoi = poi.transform;
            else
                Debug.LogWarning("VRCheckoutTeleport: Không tìm thấy object 'CameraPoi'. Kiểm tra tên trong scene.");
        }

        if (mainCameraPoi == null)
        {
            GameObject poi = GameObject.Find("MainCameraPoi");
            if (poi != null)
                mainCameraPoi = poi.transform;
            else
                Debug.LogWarning("VRCheckoutTeleport: Không tìm thấy object 'MainCameraPoi'. Main Camera sẽ reset local về 0.");
        }

        if (checkoutAnchor == null)
        {
            GameObject anchor = GameObject.Find("CheckoutAnchor");
            if (anchor != null)
                checkoutAnchor = anchor.transform;
        }

        if (cashierFaceTarget == null)
            cashierFaceTarget = ResolveCashierFaceTarget();

        if (scrollUiSampleToHide == null)
        {
            GameObject scrollUi = GameObject.Find("Scroll UI Sample");
            if (scrollUi != null)
                scrollUiSampleToHide = scrollUi;
            else
                Debug.LogWarning("VRCheckoutTeleport: Không tìm thấy object 'Scroll UI Sample'. Kiểm tra tên trong scene.");
        }
    }

    public void MoveToCheckout()
    {
        ResolveTargets();

        Transform checkoutTarget = GetCheckoutTarget();
        if (xrOrigin == null || mainCamera == null || checkoutTarget == null)
        {
            Debug.LogWarning("Thiếu reference (xrOrigin / mainCamera / checkoutTarget)");
            return;
        }

        int checkoutAmount = PrepareCheckoutAmount();

        if (paymentUiToHide != null)
            paymentUiToHide.SetActive(false);

        if (scrollUiSampleToHide != null)
            scrollUiSampleToHide.SetActive(false);

        savedPosition = xrOrigin.position;
        savedRigRotation = xrOrigin.rotation;
        savedCameraLocalRotation = mainCamera.localRotation;

        if (characterController != null)
            characterController.enabled = false;

        // Dịch chuyển rig sao cho camera hiện tại rơi đúng vào POI checkout, không động vào hierarchy con.
        Vector3 rigToCameraOffset = mainCamera.position - xrOrigin.position;
        xrOrigin.position = checkoutTarget.position - rigToCameraOffset;

        RotateRigTowardCashierFace();

        if (lockAfterTeleport)
            LockPlayer();

        if (PaymentManager.Instance != null)
        {
            PaymentManager.Instance.SetRequiredAmount(checkoutAmount);
            PaymentManager.Instance.PlayCashierIntro();
        }
    }

    private int PrepareCheckoutAmount()
    {
        if (CartManager.Instance == null)
        {
            return 0;
        }

        CartManager.Instance.ProcessCheckout();
        return CartManager.Instance.TotalPaid;
    }

    public void LockPlayer()
    {
        isLocked = true;

        SetBehaviours(movementScripts, false);
        SetBehaviours(turnScripts, false);
        SetBehaviours(otherControlScripts, false);

        if (characterController != null)
            characterController.enabled = false;

        if (xrDeviceSimulator != null && !keepControllerInteractionActive)
            xrDeviceSimulator.enabled = false;
    }

    public void UnlockPlayer()
    {
        isLocked = false;

        if (xrDeviceSimulator != null && !keepControllerInteractionActive)
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

    private Transform GetCheckoutTarget()
    {
        if (poiChargeMoney != null)
            return poiChargeMoney;

        if (checkoutAnchor != null)
            return checkoutAnchor;

        return cameraPoi;
    }

    private void RotateRigTowardCashierFace()
    {
        if (xrOrigin == null || mainCamera == null)
            return;

        if (cashierFaceTarget == null)
            cashierFaceTarget = ResolveCashierFaceTarget();

        if (cashierFaceTarget == null)
            return;

        Vector3 lookDirection = cashierFaceTarget.position - mainCamera.position;
        lookDirection.y = 0f;
        if (lookDirection.sqrMagnitude < 0.0001f)
            return;

        Vector3 currentForward = mainCamera.forward;
        currentForward.y = 0f;
        if (currentForward.sqrMagnitude < 0.0001f)
            currentForward = xrOrigin.forward;

        float yawDelta = Vector3.SignedAngle(currentForward.normalized, lookDirection.normalized, Vector3.up);

        xrOrigin.RotateAround(mainCamera.position, Vector3.up, yawDelta);
    }

    private Transform ResolveCashierFaceTarget()
    {
        GameObject cashier = GameObject.Find("NPCs charge money");
        if (cashier == null)
            return null;

        Transform head = FindChildRecursive(cashier.transform, "mixamorig6:Head");
        if (head != null)
            return head;

        return cashier.transform;
    }

    private Transform FindChildRecursive(Transform parent, string childName)
    {
        if (parent == null)
            return null;

        if (parent.name == childName)
            return parent;

        for (int i = 0; i < parent.childCount; i++)
        {
            Transform result = FindChildRecursive(parent.GetChild(i), childName);
            if (result != null)
                return result;
        }

        return null;
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
