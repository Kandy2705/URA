using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit;
public class QuestManager : MonoBehaviour
{
    private enum QuestStep { LookAtObject, WalkToLocation, AcquireTutorialItem, CommingSoon }
    [SerializeField] private QuestStep currentStep = QuestStep.LookAtObject;
    [Tooltip("The quest state entered when this scene starts. This does not remove the sequential quest flow.")]
    [SerializeField] private QuestStep startStep = QuestStep.LookAtObject;
    private bool hasInitializedStartStep;

    [Header("Core References")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Pointer pointer;

    [Header("Step 1: Look at Object")]
    [SerializeField] private Transform targetObject;
    [SerializeField] private float lookDistanceMax = 5f;
    [SerializeField] private float viewThresholdAngle = 15f;
    [SerializeField] private float requiredLookDuration = 10f;
    [SerializeField] private string QuestNotification1 = "Hãy nhìn bảng 10s và ghi nhớ danh sách!";
    private float lookTimer = 0f;

    [Header("Step 2: Walk to Destination")]
    [SerializeField] private Transform destinationPoint; // Điểm cần tới
    [SerializeField] private float arrivalDistance = 5f; // Khoảng cách được tính là "đã tới gần"
    [SerializeField] private float requiredStayDuration = 10f;
    [SerializeField] private string QuestNotification2 = "Hãy dùng cần analog di chuyển đến điểm mũi tên đang chỉ và đợi 10s!";

    private float stayTimer = 0f;

    [Header("Step 3: Acquire Tutorial Item")]
    [SerializeField] private SelectableItem tutorialItem;
    [SerializeField] private string tutorialItemInstruction = "Hãy lấy hộp sữa đang được đánh dấu.\nHướng tay cầm vào hộp sữa và bấm nút bên hông để lấy.";
    private const string WrongTutorialItemInstruction = "Bạn chọn sai món rồi, hãy chọn lại nhé.";
    [SerializeField] private TutorialTargetVisual tutorialTargetVisual;
    private UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInteractor[] tutorialInteractors;
    private bool tutorialItemStepCompleted;

    void Start()
    {
        if (playerCamera == null) playerCamera = Camera.main;

        if (targetObject == null)
        {
            GameObject gameObject = GameObject.Find("Notice_Board");
            if (gameObject != null)
            {
                targetObject = gameObject.transform;
            }
            else
            {
                Log("Không tìm ra bảng");
            }
        }

        ResolvePointer();

        if (destinationPoint == null)
        {
            GameObject gameObject = GameObject.Find("CheckPoint");
            if (gameObject != null)
            {
                destinationPoint = gameObject.transform;
            }
            else
            {
                Log("Không tìm ra CheckPoint");
            }
        }

        currentStep = startStep;
        hasInitializedStartStep = true;
        InitializeConfiguredStartStep();
    }

    private void OnEnable()
    {
        if (hasInitializedStartStep && currentStep == QuestStep.AcquireTutorialItem && !tutorialItemStepCompleted)
            EnterAcquireTutorialItem();
    }

    private void InitializeConfiguredStartStep()
    {
        lookTimer = 0f;
        stayTimer = 0f;

        switch (currentStep)
        {
            case QuestStep.LookAtObject:
                SetPointerTarget(targetObject);
                ShowQuestNotification(QuestNotification1);
                break;

            case QuestStep.WalkToLocation:
                SetPointerTarget(destinationPoint);
                ShowQuestNotification(QuestNotification2);
                break;

            case QuestStep.AcquireTutorialItem:
                StartTutorialItemStep();
                break;

            case QuestStep.CommingSoon:
                CompleteAllQuests();
                break;
        }
    }

    void Update()
    {
        switch (currentStep)
        {
            case QuestStep.LookAtObject:
                HandleLookStep();
                break;

            case QuestStep.WalkToLocation:
                HandleWalkStep();
                break;

            case QuestStep.AcquireTutorialItem:
                break;

            case QuestStep.CommingSoon:
                break;
        }
    }

    // Xử lý Bước 1: Nhìn vào vật
    private void HandleLookStep()
    {
        if (targetObject == null) return;

        bool isLooking = CheckIfLookingAtTarget();

        if (isLooking)
        {
            lookTimer += Time.deltaTime;
            Log($"Đang nhìn mục tiêu: {lookTimer:F1}/{requiredLookDuration}s");

            if (lookTimer >= requiredLookDuration)
            {
                CompleteStep1();
            }
        }
        else
        {
            // Rời mắt thì reset bộ đếm
            lookTimer = 0;
        }
    }

    private bool CheckIfLookingAtTarget()
    {
        if (targetObject == null || playerCamera == null) return false;

        Vector3 cameraPos = playerCamera.transform.position;
        Vector3 toTarget = targetObject.position - cameraPos;

        // 1. Kiểm tra góc nhìn trong phạm vi cho phép
        float angle = Vector3.Angle(playerCamera.transform.forward, toTarget);
        if (angle > viewThresholdAngle) return false;

        // 2. Bắn tia kiểm tra xem có vật cản chắn tầm mắt không
        // RaycastHit hit;
        // if (Physics.Raycast(cameraPos, toTarget.normalized, out hit, lookDistanceMax)) return false;

        return true;
    }

    private void CompleteStep1()
    {
        Log("Hoàn thành Bước 1! Đang đổi hướng mũi tên sang điểm đến...");
        currentStep = QuestStep.WalkToLocation;

        // Đổi mục tiêu của mũi tên sang điểm đến mới
        SetPointerTarget(destinationPoint);

        ShowQuestNotification(QuestNotification2);
    }

    // Xử lý Bước 2: Đi đến vị trí
    private void HandleWalkStep()
    {
        if (destinationPoint == null) return;

        // Tính khoảng cách giữa vị trí chân Player (Camera chiếu xuống mặt phẳng) và điểm đến

        Vector2 playerPosition2D = new Vector2(playerCamera.transform.position.x, playerCamera.transform.position.z);
        Vector2 destinationPosition2D = new Vector2(destinationPoint.position.x, destinationPoint.position.z);
        float distance = Vector2.Distance(playerPosition2D, destinationPosition2D);
        Log($"Khoảng cách là {distance}");
        if (distance <= arrivalDistance)
        {
            stayTimer += Time.deltaTime;
            Log($"Đang đứng trong vùng đích: {stayTimer:F1}/{requiredStayDuration}s");

            if (stayTimer >= requiredStayDuration)
            {
                StartTutorialItemStep();
            }
        }
        else
        {
            // Đi ra khỏi vùng thì reset timer
            stayTimer = 0f;
        }
    }

    private void CompleteAllQuests()
    {
        currentStep = QuestStep.CommingSoon;
        Log("Coming Soon!");

        // Ẩn mũi tên khi xong hết
        ClearPointerTarget(nameof(CompleteAllQuests));
        SetArrowWrapperActive(false, nameof(CompleteAllQuests));

        if (tutorialTargetVisual != null)
            tutorialTargetVisual.SetVisualsActive(false);

        ShowQuestNotification("Coming Soon");
    }

    private void StartTutorialItemStep()
    {
        currentStep = QuestStep.AcquireTutorialItem;
        stayTimer = 0f;
        tutorialItemStepCompleted = false;
        ClearPreviousStepPresentation();
        EnterAcquireTutorialItem();
    }

    private void EnterAcquireTutorialItem()
    {
        Debug.Log("[QuestManager] EnterAcquireTutorialItem BEGIN");

        if (tutorialItem == null)
        {
            Debug.LogError("[QuestManager] Tutorial item is not configured.");
            return;
        }

        tutorialTargetVisual = tutorialTargetVisual != null
            ? tutorialTargetVisual
            : tutorialItem.GetComponent<TutorialTargetVisual>();
        if (tutorialTargetVisual == null)
            tutorialTargetVisual = tutorialItem.gameObject.AddComponent<TutorialTargetVisual>();
        tutorialTargetVisual.SetPlayerCamera(playerCamera);
        tutorialTargetVisual.SetTarget(tutorialItem);
        tutorialTargetVisual.SetVisualsActive(true);
        SetPointerTarget(tutorialItem.transform);

        Debug.Log($"[QuestManager] EnterAcquireTutorialItem END | arrowActive={pointer != null && pointer.gameObject.activeSelf} | camera={(pointer != null && pointer.MainCamera != null ? pointer.MainCamera.name : "None")} | target={(pointer != null && pointer.Target != null ? pointer.Target.name : "None")}");

        SubscribeTutorialItemEvents();
        ShowQuestNotification(tutorialItemInstruction);
    }

    private void SubscribeTutorialItemEvents()
    {
        UnsubscribeTutorialItemEvents();
        if (PokeManager.Instance != null)
            PokeManager.Instance.OnItemSuccessfullyAdded += HandleItemSuccessfullyAdded;
        else
            Debug.LogWarning("[QuestManager] PokeManager is missing; the tutorial item cannot complete this step.");

        UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInteractor[] activeInteractors =
            FindObjectsByType<UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInteractor>(FindObjectsInactive.Exclude,
                FindObjectsSortMode.None);
        tutorialInteractors = System.Array.FindAll(activeInteractors, interactor =>
            interactor != null &&
            (interactor.gameObject.name == "Left_NearFarInteractor" || interactor.gameObject.name == "Right_NearFarInteractor"));

        if (tutorialInteractors.Length == 0)
            Debug.LogWarning("[QuestManager] Left_NearFarInteractor and Right_NearFarInteractor could not be resolved.");

        foreach (UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInteractor interactor in tutorialInteractors)
            interactor.selectEntered.AddListener(HandleInteractorSelectEntered);
    }

    private void UnsubscribeTutorialItemEvents()
    {
        if (PokeManager.Instance != null)
            PokeManager.Instance.OnItemSuccessfullyAdded -= HandleItemSuccessfullyAdded;

        if (tutorialInteractors == null)
            return;

        foreach (UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInteractor interactor in tutorialInteractors)
            if (interactor != null)
                interactor.selectEntered.RemoveListener(HandleInteractorSelectEntered);
        tutorialInteractors = null;
    }

    private void HandleInteractorSelectEntered(SelectEnterEventArgs args)
    {
        if (currentStep != QuestStep.AcquireTutorialItem || tutorialItemStepCompleted)
            return;

        if (args.interactableObject == null)
        {
            ShowQuestNotification(WrongTutorialItemInstruction);
            return;
        }

        SelectableItem selectedItem = args.interactableObject.transform.GetComponentInParent<SelectableItem>();
        // Product selections are handled after a real cart mutation by PokeManager.
        // This listener remains responsible only for non-product interactables.
        if (selectedItem == null)
            ShowQuestNotification(WrongTutorialItemInstruction);
    }

    private void HandleItemSuccessfullyAdded(SelectableItem sourceItem)
    {
        if (currentStep != QuestStep.AcquireTutorialItem || tutorialItemStepCompleted)
            return;

        if (sourceItem != tutorialItem)
        {
            // A different product reached the real cart successfully. Keep every
            // guidance visual active and remind the player of the exact target.
            ShowQuestNotification(WrongTutorialItemInstruction);
            return;
        }

        tutorialItemStepCompleted = true;
        UnsubscribeTutorialItemEvents();
        CompleteAllQuests();
    }

    private void OnDisable()
    {
        Debug.Log($"[QuestManager] OnDisable while state={currentStep}");
        UnsubscribeTutorialItemEvents();
        HideTutorialItemGuidance();
    }

    private void HideTutorialItemGuidance()
    {
        ClearPointerTarget(nameof(HideTutorialItemGuidance));
        SetArrowWrapperActive(false, nameof(HideTutorialItemGuidance));

        if (tutorialTargetVisual != null)
            tutorialTargetVisual.SetVisualsActive(false);
    }

    private void ClearPreviousStepPresentation()
    {
        lookTimer = 0f;
        stayTimer = 0f;

        if (hideNotificationCoroutine != null)
        {
            StopCoroutine(hideNotificationCoroutine);
            hideNotificationCoroutine = null;
        }

        if (notificationCanvasGroup != null)
            notificationCanvasGroup.alpha = 0f;
        if (questCanvasObject != null)
            questCanvasObject.SetActive(false);

        ClearPointerTarget(nameof(ClearPreviousStepPresentation));
        SetArrowWrapperActive(false, nameof(ClearPreviousStepPresentation));

        if (tutorialTargetVisual != null)
            tutorialTargetVisual.SetVisualsActive(false);
    }

    private void SetPointerTarget(Transform target)
    {
        ResolvePointer();
        if (pointer == null)
            return;

        if (target != null)
        {
            pointer.InitializeCamera(playerCamera);
            Debug.Log($"[QuestManager] ArrowWrapper before activate: {pointer.gameObject.activeSelf}");
            SetArrowWrapperActive(true, nameof(SetPointerTarget));
            ClearPointerTarget(nameof(SetPointerTarget));
        }
        else
        {
            ClearPointerTarget(nameof(SetPointerTarget));
            SetArrowWrapperActive(false, nameof(SetPointerTarget));
            return;
        }

        pointer.SetTarget(target);

        if (target != null)
            Debug.Log($"[QuestManager] Pointer target -> {target.name}");
    }

    private void ResolvePointer()
    {
        if (pointer == null)
            pointer = Object.FindFirstObjectByType<Pointer>(FindObjectsInactive.Include);

        if (pointer == null)
        {
            Debug.LogWarning("[QuestManager] Pointer is missing; direction guidance is unavailable.");
            return;
        }

        pointer.InitializeCamera(playerCamera);
    }

    private void ClearPointerTarget(string source)
    {
        if (pointer == null)
            return;

        Debug.Log($"[QuestManager] Pointer target -> None from {source}");
        pointer.SetTarget(null);
    }

    private void SetArrowWrapperActive(bool active, string source)
    {
        ResolvePointer();
        if (pointer == null)
            return;

        if (!active)
            Debug.Log($"[QuestManager] ArrowWrapper -> INACTIVE from {source}");

        pointer.gameObject.SetActive(active);

        if (active)
            Debug.Log("[QuestManager] ArrowWrapper -> ACTIVE");
    }

    private void Log(string message)
    {
        Debug.Log($"[QuestManager] {message}");
    }

    [Header("Quest UI Notification")]
    [SerializeField] private GameObject questCanvasObject; // Kéo thẳng GameObject Canvas vào đây
    [SerializeField] private CanvasGroup notificationCanvasGroup;
    [SerializeField] private TextMeshProUGUI notificationText;
    [SerializeField] private float displayDistance = 1.2f;
    [SerializeField] private float displayYOffset = -1.2f;
    [SerializeField] private float notificationDuration = 5f;

    private Coroutine hideNotificationCoroutine;

    public void ShowQuestNotification(string message)
    {
        if (playerCamera == null) playerCamera = Camera.main;

        if (playerCamera == null || questCanvasObject == null)
        {
            Debug.LogWarning("Chưa gán đủ Camera hoặc QuestCanvasObject!");
            return;
        }

        if (notificationText != null)
        {
            notificationText.text = message;
        }

        Transform camTransform = playerCamera.transform;

        // Đặt vị trí trước mắt Camera
        Vector3 targetPos = camTransform.position 
                        + (camTransform.forward * displayDistance) 
                        + (camTransform.up * displayYOffset);

        questCanvasObject.transform.position = targetPos;

        // Quay mặt phẳng về phía mắt nhìn
        questCanvasObject.transform.LookAt(camTransform.position);
        questCanvasObject.transform.Rotate(0, 180, 0); // Khắc phục ngược gương nếu có

        questCanvasObject.SetActive(true);

        if (notificationCanvasGroup != null)
        {
            notificationCanvasGroup.alpha = 1f;
        }

        if (hideNotificationCoroutine != null)
        {
            StopCoroutine(hideNotificationCoroutine);
            hideNotificationCoroutine = null;
        }
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
