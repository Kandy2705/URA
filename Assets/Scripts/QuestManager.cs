using System.Collections;
using TMPro;
using UnityEngine;

/// <summary>
/// Điều phối chuỗi nhiệm vụ: nhìn bảng -> đi tới CheckPoint -> nhặt đúng item và bỏ vào giỏ.
/// Tất cả reference gameplay phải được kéo thủ công vào Inspector.
/// </summary>
public class QuestManager : MonoBehaviour
{
    private enum QuestStep { LookAtObject, WalkToLocation, PickUpItem, Completed }

    [Header("Quest State")]
    [SerializeField] private QuestStep currentStep = QuestStep.LookAtObject;

    [Header("Core References - assign in Inspector")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Pointer pointer;

    [Header("Step 1: Look at Notice Board")]
    [SerializeField] private Transform targetObject;
    [SerializeField] private float viewThresholdAngle = 15f;
    [SerializeField] private float requiredLookDuration = 10f;
    [SerializeField] private string lookStepMessage = "Hãy nhìn bảng thông báo trong 10 giây và ghi nhớ danh sách!";
    private float lookTimer;

    [Header("Step 2: Walk to CheckPoint")]
    [SerializeField] private Transform destinationPoint;
    [SerializeField] private float arrivalDistance = 5f;
    [SerializeField] private float requiredStayDuration = 10f;
    [SerializeField] private string walkStepMessage = "Hãy dùng cần analog đi tới điểm được chỉ và đứng đó trong 10 giây!";
    private float stayTimer;
    private bool isTransitioningToPickUpStep;

    [Header("Step 3: Pick Up Item")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform sampleItem;
    [SerializeField] private GameObject defaultItemCylinderObject;
    [SerializeField] private GameObject groundArrowObject;
    [SerializeField] private GroundArrow groundArrow;
    [SerializeField] private GameObject controllerTutorialCanvas;
    [SerializeField] private float delayBeforeStep3 = 2f;
    [SerializeField] private string pickUpStepMessage = "Hãy hướng tay vào item mẫu và bấm nút Grip bên hông để nhặt!";
    [SerializeField] private string wrongItemMessage = "Bạn đã chọn nhầm vật phẩm. Hãy làm theo mũi tên và nhặt đúng item mẫu!";
    [SerializeField] private string itemGrabbedMessage = "Đã nhặt đúng item. Hãy đưa item vào giỏ hàng!";
    [SerializeField] private string completedMessage = "Chúc mừng! Bạn đã hoàn thành toàn bộ chuỗi nhiệm vụ.";

    [Header("Quest UI Notification")]
    [SerializeField] private GameObject questCanvasObject;
    [SerializeField] private CanvasGroup notificationCanvasGroup;
    [SerializeField] private TextMeshProUGUI notificationText;
    [SerializeField] private float displayDistance = 1.2f;
    [SerializeField] private float displayYOffset = -1.2f;
    [SerializeField] private float notificationDuration = 5f;
    private Coroutine hideNotificationCoroutine;

    private void OnEnable()
    {
        SampleItem.OnSampleItemAddedToCart += HandleSampleItemAddedToCart;
        SampleItem.OnWrongItemTouched += HandleWrongItemTouched;
    }

    private void OnDisable()
    {
        SampleItem.OnSampleItemAddedToCart -= HandleSampleItemAddedToCart;
        SampleItem.OnWrongItemTouched -= HandleWrongItemTouched;
    }

    private void Start()
    {
        // Không dùng GameObject.Find. Nếu chưa gán, các object sẽ không được tự tìm.
        if (groundArrow == null && groundArrowObject != null)
            groundArrow = groundArrowObject.GetComponent<GroundArrow>();

        HidePickUpGuidance();
        ShowQuestNotification(lookStepMessage);

        if (pointer != null && targetObject != null)
        {
            pointer.gameObject.SetActive(true);
            pointer.SetTarget(targetObject);
        }
    }

    private void Update()
    {
        switch (currentStep)
        {
            case QuestStep.LookAtObject:
                HandleLookStep();
                break;
            case QuestStep.WalkToLocation:
                HandleWalkStep();
                break;
            case QuestStep.PickUpItem:
            case QuestStep.Completed:
                break;
        }
    }

    // ------------------------- Step 1 -------------------------

    private void HandleLookStep()
    {
        if (targetObject == null || playerCamera == null)
            return;

        if (CheckIfLookingAtTarget())
        {
            lookTimer += Time.deltaTime;
            if (lookTimer >= requiredLookDuration)
                CompleteStep1();
        }
        else
        {
            lookTimer = 0f;
        }
    }

    private bool CheckIfLookingAtTarget()
    {
        Vector3 toTarget = targetObject.position - playerCamera.transform.position;
        if (toTarget.sqrMagnitude <= Mathf.Epsilon)
            return true;

        return Vector3.Angle(playerCamera.transform.forward, toTarget) <= viewThresholdAngle;
    }

    private void CompleteStep1()
    {
        currentStep = QuestStep.WalkToLocation;
        stayTimer = 0f;

        if (pointer != null && destinationPoint != null)
            pointer.SetTarget(destinationPoint);

        ShowQuestNotification(walkStepMessage);
    }

    // ------------------------- Step 2 -------------------------

    private void HandleWalkStep()
    {
        if (destinationPoint == null || playerTransform == null || isTransitioningToPickUpStep)
            return;

        Vector2 playerPosition = new Vector2(playerTransform.position.x, playerTransform.position.z);
        Vector2 destinationPosition = new Vector2(destinationPoint.position.x, destinationPoint.position.z);
        float distance = Vector2.Distance(playerPosition, destinationPosition);

        if (distance <= arrivalDistance)
        {
            stayTimer += Time.deltaTime;
            if (stayTimer >= requiredStayDuration)
                CompleteStep2();
        }
        else
        {
            stayTimer = 0f;
        }
    }

    private void CompleteStep2()
    {
        if (isTransitioningToPickUpStep)
            return;

        isTransitioningToPickUpStep = true;
        StartCoroutine(BeginPickUpStepAfterDelay());
    }

    private IEnumerator BeginPickUpStepAfterDelay()
    {
        yield return new WaitForSeconds(delayBeforeStep3);

        if (pointer != null)
        {
            pointer.SetTarget(null);
            pointer.gameObject.SetActive(false);
        }

        BeginPickUpStep();
    }

    // ------------------------- Step 3 -------------------------

    private void BeginPickUpStep()
    {
        currentStep = QuestStep.PickUpItem;

        // Tắt hình trụ 3D mặc định quanh item nếu scene có object này.
        if (defaultItemCylinderObject != null)
            defaultItemCylinderObject.SetActive(false);

        if (groundArrow != null)
        {
            groundArrow.SetTarget(sampleItem);
            groundArrow.Show(true);
        }
        else if (groundArrowObject != null)
        {
            groundArrowObject.SetActive(true);
        }

        if (controllerTutorialCanvas != null)
            controllerTutorialCanvas.SetActive(true);

        ShowQuestNotification(pickUpStepMessage);
    }

    /// <summary>Gọi khi người chơi chạm hoặc nhặt nhầm vật phẩm khác.</summary>
    public void OnWrongItemInteracted()
    {
        if (currentStep != QuestStep.PickUpItem)
            return;

        if (controllerTutorialCanvas != null)
            controllerTutorialCanvas.SetActive(true);

        ShowQuestNotification(wrongItemMessage);
    }

    /// <summary>Alias tương thích với UnityEvent hoặc script cũ trong scene.</summary>
    public void OnWrongItemTouched()
    {
        if (currentStep != QuestStep.PickUpItem)
            return;

        if (controllerTutorialCanvas != null)
            controllerTutorialCanvas.SetActive(true);

        ShowQuestNotification("Bạn chọn nhầm vật thể khác! Hướng tay cầm vào item và bấm nút bên hông.");
    }

    /// <summary>Đã nhặt đúng item nhưng chưa bỏ vào giỏ, nên chưa hoàn thành quest.</summary>
    public void OnSampleItemGrabbed()
    {
        if (currentStep != QuestStep.PickUpItem)
            return;

        ShowQuestNotification(itemGrabbedMessage);
    }

    /// <summary>Gọi từ trigger giỏ hàng. Chỉ sampleItem được tính là hoàn thành.</summary>
    public void OnTargetItemAddedToCart(GameObject itemAdded)
    {
        if (currentStep != QuestStep.PickUpItem || itemAdded == null || sampleItem == null)
            return;

        bool isTargetItem = itemAdded == sampleItem.gameObject
            || itemAdded.transform.IsChildOf(sampleItem)
            || sampleItem.IsChildOf(itemAdded.transform);
        if (!isTargetItem)
        {
            OnWrongItemInteracted();
            return;
        }

        CompleteQuestFromCart();
    }

    /// <summary>Alias parameterless cho event SampleItem hiện có.</summary>
    public void OnSampleItemAddedToCart()
    {
        if (currentStep != QuestStep.PickUpItem)
            return;

        CompleteQuestFromCart();
    }

    private void CompleteQuestFromCart()
    {
        currentStep = QuestStep.Completed;
        isTransitioningToPickUpStep = false;
        HidePickUpGuidance();
        StartCoroutine(FinishQuestAfterDelay());
    }

    private void HandleSampleItemAddedToCart()
    {
        OnSampleItemAddedToCart();
    }

    private void HandleWrongItemTouched()
    {
        OnWrongItemInteracted();
    }

    private IEnumerator FinishQuestAfterDelay()
    {
        ShowQuestNotification("Đã hoàn thành! Item đã được thêm vào giỏ hàng.");

        yield return new WaitForSeconds(3f);

        if (pointer != null)
        {
            pointer.SetTarget(null);
            pointer.gameObject.SetActive(false);
        }

        if (questCanvasObject != null)
            questCanvasObject.SetActive(false);

        if (notificationCanvasGroup != null)
            notificationCanvasGroup.alpha = 0f;
    }

    private void HidePickUpGuidance()
    {
        if (groundArrow != null)
            groundArrow.Show(false);

        if (groundArrowObject != null)
            groundArrowObject.SetActive(false);

        if (controllerTutorialCanvas != null)
            controllerTutorialCanvas.SetActive(false);
    }

    // ------------------------- UI -------------------------

    public void ShowQuestNotification(string message)
    {
        if (notificationText != null)
            notificationText.text = message;

        if (questCanvasObject == null)
            return;

        if (playerCamera != null)
        {
            Transform cameraTransform = playerCamera.transform;
            questCanvasObject.transform.position = cameraTransform.position
                + cameraTransform.forward * displayDistance
                + cameraTransform.up * displayYOffset;
            questCanvasObject.transform.LookAt(cameraTransform.position);
            questCanvasObject.transform.Rotate(0f, 180f, 0f);
        }

        questCanvasObject.SetActive(true);

        if (notificationCanvasGroup != null)
            notificationCanvasGroup.alpha = 1f;

        if (hideNotificationCoroutine != null)
            StopCoroutine(hideNotificationCoroutine);

        hideNotificationCoroutine = StartCoroutine(HideNotificationAfter(notificationDuration));
    }

    private IEnumerator HideNotificationAfter(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (notificationCanvasGroup != null)
            notificationCanvasGroup.alpha = 0f;

        if (questCanvasObject != null)
            questCanvasObject.SetActive(false);

        hideNotificationCoroutine = null;
    }
}
