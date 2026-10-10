using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit;

public class QuestManager : MonoBehaviour
{
    // Bốn bước người chơi phải thực hiện, sau đó mới được chuyển vào level chính.
    public enum QuestStep { LookAtMap, MoveToPoint, PickItem, OpenUI, Complete }
    [SerializeField] private QuestStep currentStep = QuestStep.LookAtMap;
    [Tooltip("Bước bắt đầu khi scene chạy (dùng để test nhanh). Không làm mất luồng tuần tự của các bước.")]
    [SerializeField] private QuestStep startStep = QuestStep.LookAtMap;
    private bool hasInitializedStartStep;

    public enum UIPlacementMode
    {
        CanvasUITopRight,    // Nằm cùng CanvasUI với Scroll UI Sample, neo ở góc trên bên phải
        FollowCameraCorner,  // Bám theo góc nhìn của Camera nhưng lệch ở 1 góc tầm nhìn
        StaticInScene        // Giữ nguyên vị trí thiết lập trong Scene
    }

    [Header("Scene Flow")]
    [SerializeField] private string nextSceneName = "Scene-level-1";
    [SerializeField] private float completionDelay = 2.5f;

    [Header("Core References")]
    [SerializeField] private Transform xrOriginTransform;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Pointer pointer;

    [Header("VR HUD Placement")]
    [SerializeField] private UIPlacementMode placementMode = UIPlacementMode.CanvasUITopRight;
    [Tooltip("CanvasUI đang chứa Scroll UI Sample. QuestCanvas sẽ trở thành con trực tiếp của Canvas này.")]
    [SerializeField] private RectTransform canvasUITransform;
    [Tooltip("Khoảng cách tính từ góc trên bên phải của CanvasUI (số âm dịch vào trong màn hình).")]
    [SerializeField] private Vector2 hudAnchoredPosition = new Vector2(-40f, -40f);
    [Tooltip("Tỉ lệ của QuestCanvas trong hệ tọa độ CanvasUI.")]
    [SerializeField, Range(0.5f, 1.25f)] private float hudScale = 1f;
    [SerializeField] private float smoothFollowSpeed = 6f;

    [Header("Step 1: Look at Map")]
    [SerializeField] private Transform targetObject;
    [SerializeField] private float lookDistanceMax = 5f;
    [SerializeField] private float viewThresholdAngle = 15f;
    [SerializeField] private float requiredLookDuration = 10f;
    [SerializeField] private string questNotification1 = "Hãy xoay theo mũi tên và nhìn Bảng thông báo trong 10 giây.";
    private float lookTimer = 0f;

    [Header("Step 2: Move to Point")]
    [SerializeField] private Transform destinationPoint;
    [SerializeField] private float arrivalDistance = 5f;
    [SerializeField] private float checkpointCompletionDelay = 2.5f;
    [SerializeField] private string questNotification2 = "Dùng cần analog di chuyển theo mũi tên đến điểm CheckPoint.";
    private float checkpointTimer = 0f;

    [Header("Step 3: Pick Item")]
    [SerializeField] private SelectableItem targetItem;
    [Tooltip("Để trống sẽ tự tìm item mẫu có tên chocolate/Kẹo socola trong scene.")]
    [SerializeField] private string targetItemName = "chocolate";
    [SerializeField] private string questNotification3 = "Hướng tay cầm vào item mẫu và bấm nút bên hông để lấy vào giỏ.";
    private int targetItemQuantityBeforePick;
    private int inventoryQuantityBeforePick;
    private bool wrongItemPicked;
    [Tooltip("Hiệu ứng đánh dấu (▼ + highlight) trên item mẫu. Để trống sẽ tự thêm vào item.")]
    [SerializeField] private TutorialTargetVisual tutorialTargetVisual;
    private UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInteractor[] tutorialInteractors;

    [Header("Step 4: Toolbar Tutorial")]
    [SerializeField] private ListController listController;
    [Tooltip("Mũi tên nhỏ chỉ vào nút/bảng UI trên thanh công cụ.")]
    [SerializeField] private UIPointer uiPointer;
    [Tooltip("Mũi tên 3D trước mặt người chơi, chỉ vào vật trong scene (bảng danh sách, đồng hồ).")]
    [SerializeField] private WorldPointer worldPointer;
    [SerializeField] private Transform listButtonTarget;
    [SerializeField] private Transform cartButtonTarget;
    [SerializeField] private Transform cancelButtonTarget;
    [SerializeField] private Transform timeButtonTarget;
    [SerializeField] private Transform listBoardTarget;
    [SerializeField] private Transform cartPanelTarget;
    [SerializeField] private Transform realTimerTarget;
    [SerializeField] private Button cartButton;
    [SerializeField] private Button cancelButton;
    [SerializeField] private Button timeButton;
    [SerializeField, Min(0f)] private float introDuration = 2.5f;
    [SerializeField, Min(0f)] private float cartExplanationDuration = 2.5f;
    [SerializeField, Min(0f)] private float timeExplanationDuration = 2.5f;

    // Các thao tác con của Bước 4 (từ nhánh feat/tutorial-interactions-with-toolbar).
    private enum ToolbarStep
    {
        IntroToolbar,
        SelectListButton,
        ExplainListBoard,
        SelectCartButton,
        ExplainCart,
        CloseCart,
        SelectTimeButton,
        ExplainRealTimer,
        CloseTimePanel,
        CompletedToolbarTutorial
    }

    private const string IntroToolbarMessage = "Giờ chúng ta sẽ thao tác với thanh công cụ.";
    private const string SelectListButtonMessage = "Bấm vào Danh sách để xem các sản phẩm cần mua.";
    private const string ExplainListBoardMessage = "Đây là danh sách sản phẩm cần mua. Hãy ghi nhớ các món trong danh sách nhé!";
    private const string SelectCartButtonMessage = "Bấm vào Giỏ hàng để xem các sản phẩm đã chọn.";
    private const string ExplainCartMessage = "Đây là giỏ hàng. Bạn có thể xem các sản phẩm đã chọn tại đây.";
    private const string CloseCartMessage = "Bấm HỦY để đóng giỏ hàng.";
    private const string SelectTimeButtonMessage = "Bấm vào Đồng hồ để xem thời gian còn lại.";
    private const string ExplainRealTimerMessage = "Đây là thời gian còn lại. Hãy hoàn thành mua sắm trước khi hết giờ nhé!";
    private const string CloseTimePanelMessage = "Bấm HỦY để đóng bảng thời gian và tiếp tục.";

    private ToolbarStep toolbarStep = ToolbarStep.IntroToolbar;
    private Coroutine toolbarRoutine;
    private bool toolbarSubscribed;

    [Header("Tutorial UI Elements")]
    [SerializeField] private GameObject questCanvasObject;
    [SerializeField] private CanvasGroup notificationCanvasGroup;
    [SerializeField] private TextMeshProUGUI txtStepTitle;
    [SerializeField] private TextMeshProUGUI txtStepInstruction;
    [SerializeField] private TextMeshProUGUI txtStepStatus;
    [SerializeField] private Image imgProgressBar;
    [SerializeField] private TextMeshProUGUI txtProgressPercent;
    [SerializeField] private Button btnSkip;
    [SerializeField] private Button btnRetry;
    [SerializeField] private TextMeshProUGUI notificationText; // Tương thích ngược

    private bool isTransitioning = false;

    void Awake()
    {
        EnsureParenting();
    }

    void Start()
    {
        EnsureParenting();

        // 1. Tự động tìm kiếm Camera nếu chưa gán
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
            if (playerCamera == null)
            {
                Camera cam = Object.FindAnyObjectByType<Camera>();
                if (cam != null) playerCamera = cam;
            }
        }

        // 2. Tìm Notice_Board
        if (targetObject == null)
        {
            GameObject noticeBoard = GameObject.Find("Notice_Board");
            if (noticeBoard != null)
                targetObject = noticeBoard.transform;
            else
                Log("Không tìm ra Notice_Board");
        }

        // 3. Tìm Pointer (kể cả khi ArrowWrapper đang tắt)
        if (pointer == null)
            pointer = Object.FindFirstObjectByType<Pointer>(FindObjectsInactive.Include);
        if (pointer != null)
            pointer.InitializeCamera(playerCamera);

        // 4. Tìm CheckPoint
        if (destinationPoint == null)
        {
            GameObject checkPoint = GameObject.Find("CheckPoint");
            if (checkPoint != null)
                destinationPoint = checkPoint.transform;
            else
                Log("Không tìm ra CheckPoint");
        }

        ResolveTutorialItem();
        if (listController == null)
            listController = Object.FindFirstObjectByType<ListController>();
        SubscribeToolbarActions();

        EnsureTutorialUI();
        InitializeButtons();

        // Khởi động bước bắt đầu (mặc định là bước 1)
        hasInitializedStartStep = true;
        SetStep(startStep);
    }

    private void OnEnable()
    {
        if (!hasInitializedStartStep)
            return;

        if (currentStep == QuestStep.PickItem)
            EnterPickItemGuidance();
        SubscribeToolbarActions();
    }

    private void OnDisable()
    {
        ExitPickItemGuidance();
        StopToolbarRoutine();
        UnsubscribeToolbarActions();
    }

    public void EnsureParenting()
    {
        if (placementMode != UIPlacementMode.CanvasUITopRight || questCanvasObject == null)
            return;

        if (canvasUITransform == null)
        {
            GameObject canvasUI = GameObject.Find("CanvasUI");
            if (canvasUI != null)
                canvasUITransform = canvasUI.GetComponent<RectTransform>();
        }

        RectTransform questRect = questCanvasObject.GetComponent<RectTransform>();
        if (canvasUITransform == null || questRect == null)
            return;

        if (questRect.parent != canvasUITransform)
            questRect.SetParent(canvasUITransform, false);

        questRect.anchorMin = Vector2.one;
        questRect.anchorMax = Vector2.one;
        questRect.pivot = Vector2.one;
        questRect.anchoredPosition = hudAnchoredPosition;
        questRect.localPosition = new Vector3(questRect.localPosition.x, questRect.localPosition.y, 0f);
        questRect.localRotation = Quaternion.identity;
        questRect.localScale = Vector3.one * hudScale;

        RectTransform panelRect = questRect.Find("Background") as RectTransform;
        if (panelRect != null)
        {
            panelRect.anchoredPosition = Vector2.zero;
            panelRect.localRotation = Quaternion.identity;
            panelRect.localScale = Vector3.one;
        }
    }

    public void SnapUIToCorner()
    {
        EnsureParenting();

        if (placementMode == UIPlacementMode.FollowCameraCorner && playerCamera != null && questCanvasObject != null)
        {
            Transform cam = playerCamera.transform;
            Vector3 targetPos = cam.position + (cam.forward * 1.8f) + (cam.right * 0.75f) + (cam.up * -0.15f);
            questCanvasObject.transform.position = targetPos;
            questCanvasObject.transform.LookAt(cam.position);
            questCanvasObject.transform.Rotate(0, 180, 0);
        }
    }

    private void InitializeButtons()
    {
        if (btnSkip != null)
        {
            btnSkip.onClick.RemoveListener(SkipTutorial);
            btnSkip.onClick.AddListener(SkipTutorial);
        }

        if (btnRetry != null)
        {
            btnRetry.onClick.RemoveListener(RetryTutorial);
            btnRetry.onClick.AddListener(RetryTutorial);
        }
    }

    void Update()
    {
        switch (currentStep)
        {
            case QuestStep.LookAtMap:
                HandleLookStep();
                break;

            case QuestStep.MoveToPoint:
                HandleWalkStep();
                break;

            case QuestStep.PickItem:
                HandlePickItemStep();
                break;

            case QuestStep.OpenUI:
                // Bước 4 chạy theo sự kiện thật của thanh công cụ (xem EnterToolbarStep).
                break;

            case QuestStep.Complete:
                break;
        }

        UpdateUI();

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.K))
        {
            Log("Phím tắt [K] - Bỏ qua hướng dẫn");
            SkipTutorial();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            Log("Phím tắt [R] - Thử lại hướng dẫn");
            RetryTutorial();
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            Log("Phím tắt [C] - Đưa UI về lại góc trên bên phải của CanvasUI");
            SnapUIToCorner();
        }
#endif
    }

    void LateUpdate()
    {
        if (questCanvasObject == null) return;

        if (placementMode == UIPlacementMode.FollowCameraCorner && playerCamera != null)
        {
            UpdateFollowCameraCornerPosition();
        }
        else if (placementMode == UIPlacementMode.CanvasUITopRight)
        {
            // Giữ HUD trong cùng hệ tọa độ với Scroll UI Sample kể cả khi scene được nạp lại.
            if (canvasUITransform == null || questCanvasObject.transform.parent != canvasUITransform)
            {
                EnsureParenting();
            }
        }
    }

    private void UpdateFollowCameraCornerPosition()
    {
        if (playerCamera == null) return;

        Transform camTransform = playerCamera.transform;
        Vector3 targetPos = camTransform.position
                          + (camTransform.forward * 1.8f)
                          + (camTransform.right * 0.75f)
                          + (camTransform.up * -0.15f);

        questCanvasObject.transform.position = Vector3.Lerp(
            questCanvasObject.transform.position,
            targetPos,
            Time.deltaTime * smoothFollowSpeed
        );

        Vector3 lookDir = questCanvasObject.transform.position - camTransform.position;
        if (lookDir != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(lookDir);
            questCanvasObject.transform.rotation = Quaternion.Slerp(
                questCanvasObject.transform.rotation,
                targetRot,
                Time.deltaTime * smoothFollowSpeed
            );
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

            if (lookTimer >= requiredLookDuration)
            {
                CompleteStep1();
            }
        }
        else
        {
            lookTimer = Mathf.Max(0f, lookTimer - Time.deltaTime * 0.5f);
        }
    }

    private bool CheckIfLookingAtTarget()
    {
        if (targetObject == null || playerCamera == null) return false;

        Vector3 cameraPos = playerCamera.transform.position;
        Vector3 toTarget = targetObject.position - cameraPos;

        float angle = Vector3.Angle(playerCamera.transform.forward, toTarget);
        return angle <= viewThresholdAngle;
    }

    private void CompleteStep1()
    {
        Log("Hoàn thành Bước 1! Chuyển sang Bước 2: Di chuyển đến CheckPoint.");
        SetStep(QuestStep.MoveToPoint);
    }

    // Xử lý Bước 2: Đi đến vị trí
    private void HandleWalkStep()
    {
        if (destinationPoint == null || playerCamera == null) return;

        Vector2 playerPosition2D = new Vector2(playerCamera.transform.position.x, playerCamera.transform.position.z);
        Vector2 destinationPosition2D = new Vector2(destinationPoint.position.x, destinationPoint.position.z);
        float distance = Vector2.Distance(playerPosition2D, destinationPosition2D);

        if (distance > arrivalDistance)
        {
            checkpointTimer = 0f;
            return;
        }

        // Vào vùng checkpoint là đủ điều kiện. Chỉ dừng vài giây để người chơi
        // thấy thông báo hoàn thành rồi mới chuyển sang bước lấy item.
        checkpointTimer += Time.deltaTime;
        if (checkpointTimer >= checkpointCompletionDelay)
            SetStep(QuestStep.PickItem);
    }

    private void HandlePickItemStep()
    {
        if (PokeManager.Instance == null || targetItem == null)
            return;

        bool targetWasAdded = PokeManager.Instance.inventory.TryGetValue(
            targetItem.itemName, out BillEntry entry) &&
            entry != null && entry.quantity > targetItemQuantityBeforePick;

        if (targetWasAdded)
        {
            Log($"Đã lấy đúng item mẫu: {targetItem.itemName}.");
            SetStep(QuestStep.OpenUI);
        }
        else if (GetInventoryQuantity() > inventoryQuantityBeforePick)
        {
            wrongItemPicked = true;
        }
    }

    private void CompleteAllQuests()
    {
        if (isTransitioning) return;
        isTransitioning = true;

        SetStep(QuestStep.Complete);
        Log("Hoàn thành đủ 4 bước hướng dẫn!");

        if (pointer != null)
        {
            pointer.SetTarget(null);
            pointer.gameObject.SetActive(false);
        }

        StartCoroutine(TransitionToNextSceneRoutine());
    }

    private IEnumerator TransitionToNextSceneRoutine()
    {
        yield return new WaitForSeconds(completionDelay);
        LoadNextScene();
    }

    public void SkipTutorial()
    {
        Log("Người chơi bấm Bỏ qua hướng dẫn.");
        LoadNextScene();
    }

    public void RetryTutorial()
    {
        Log("Người chơi bấm Thử lại hướng dẫn từ đầu.");
        StopAllCoroutines();
        isTransitioning = false;
        lookTimer = 0f;
        checkpointTimer = 0f;
        toolbarRoutine = null;
        toolbarStep = ToolbarStep.IntroToolbar;
        wrongItemPicked = false;

        SetStep(QuestStep.LookAtMap);
        SnapUIToCorner();
    }

    private void LoadNextScene()
    {
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            Log($"Đang chuyển sang scene: {nextSceneName}");
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Log("Chưa thiết lập nextSceneName!");
        }
    }

    private void SetStep(QuestStep step)
    {
        if (step != QuestStep.PickItem)
            ExitPickItemGuidance();

        if (step != QuestStep.OpenUI)
        {
            StopToolbarRoutine();
            HideToolbarPointers();
        }

        currentStep = step;

        switch (currentStep)
        {
            case QuestStep.LookAtMap:
                lookTimer = 0f;
                if (pointer != null && targetObject != null)
                {
                    pointer.gameObject.SetActive(true);
                    pointer.SetTarget(targetObject);
                }
                break;

            case QuestStep.MoveToPoint:
                checkpointTimer = 0f;
                if (pointer != null && destinationPoint != null)
                {
                    pointer.gameObject.SetActive(true);
                    pointer.SetTarget(destinationPoint);
                }
                break;

            case QuestStep.PickItem:
                checkpointTimer = 0f;
                wrongItemPicked = false;
                ResolveTutorialItem();
                CaptureInventoryBeforePick();
                if (pointer != null && targetItem != null)
                {
                    pointer.gameObject.SetActive(true);
                    pointer.SetTarget(targetItem.transform);
                }
                EnterPickItemGuidance();
                break;

            case QuestStep.OpenUI:
                if (pointer != null)
                    pointer.gameObject.SetActive(false);
                if (listController == null)
                    listController = Object.FindFirstObjectByType<ListController>();
                // Đăng ký lại để chắc chắn bắt được ListController vừa tìm thấy.
                UnsubscribeToolbarActions();
                SubscribeToolbarActions();
                EnterToolbarStep(ToolbarStep.IntroToolbar);
                break;

            case QuestStep.Complete:
                if (pointer != null)
                {
                    pointer.gameObject.SetActive(false);
                }
                break;
        }

        UpdateUI();
    }

    private void UpdateUI()
    {
        float overallProgress;
        string title;
        string instruction;
        string status;

        switch (currentStep)
        {
            case QuestStep.LookAtMap:
                overallProgress = Mathf.Lerp(0f, 0.25f, Mathf.Clamp01(lookTimer / requiredLookDuration));
                title = "BƯỚC 1/4: QUAN SÁT BẢN ĐỒ";
                instruction = questNotification1;
                status = CheckIfLookingAtTarget()
                    ? $"<color=#00E5FF>Đang đọc bảng: {lookTimer:F1}s / {requiredLookDuration:F0}s</color>"
                    : "<color=#FBBF24>Hãy quay đầu nhìn theo mũi tên chỉ dẫn.</color>";
                break;

            case QuestStep.MoveToPoint:
                float moveProgress = Mathf.Clamp01(checkpointTimer / Mathf.Max(0.01f, checkpointCompletionDelay));
                overallProgress = 0.25f + moveProgress * 0.25f;
                title = "BƯỚC 2/4: DI CHUYỂN ĐẾN ĐIỂM";
                instruction = questNotification2;
                status = BuildMoveStatus();
                break;

            case QuestStep.PickItem:
                overallProgress = 0.5f;
                title = "BƯỚC 3/4: LẤY ITEM MẪU";
                instruction = questNotification3;
                status = wrongItemPicked
                    ? $"<color=#F87171>Bạn vừa lấy nhầm item. Hãy lấy đúng: {GetTargetItemLabel()}.</color>"
                    : $"<color=#FBBF24>Chưa ghi nhận item {GetTargetItemLabel()} trong giỏ.</color>";
                break;

            case QuestStep.OpenUI:
                int toolbarIndex = (int)toolbarStep;
                int toolbarCount = (int)ToolbarStep.CompletedToolbarTutorial;
                overallProgress = 0.75f + 0.25f * Mathf.Clamp01((float)toolbarIndex / toolbarCount);
                title = "BƯỚC 4/4: THANH CÔNG CỤ";
                instruction = GetToolbarMessage(toolbarStep);
                status = $"<color=#00E5FF>Thao tác {Mathf.Min(toolbarIndex + 1, toolbarCount)}/{toolbarCount}</color>";
                break;

            case QuestStep.Complete:
            default:
                overallProgress = 1f;
                title = "<color=#10B981>HOÀN TẤT 4/4 BƯỚC!</color>";
                instruction = "Bạn đã hoàn thành tutorial thực hành.";
                status = "<color=#00E5FF>Đang chuyển vào Scene-level-1...</color>";
                break;
        }

        if (txtStepTitle != null) txtStepTitle.text = title;
        if (txtStepInstruction != null) txtStepInstruction.text = instruction;
        if (txtStepStatus != null) txtStepStatus.text = status;

        if (imgProgressBar != null)
        {
            imgProgressBar.fillAmount = overallProgress;
        }

        if (txtProgressPercent != null)
        {
            txtProgressPercent.text = $"{Mathf.RoundToInt(overallProgress * 100f)}%";
        }

        if (notificationText != null && txtStepInstruction != null)
        {
            notificationText.text = txtStepInstruction.text;
        }
    }

    // ===== Bước 3: đánh dấu item mẫu + lắng nghe sự kiện lấy item (từ nhánh grabbing-item-tutorial) =====
    private void EnterPickItemGuidance()
    {
        if (targetItem == null)
        {
            Debug.LogWarning("[QuestManager] Chưa có item mẫu cho bước lấy item.");
            return;
        }

        if (tutorialTargetVisual == null)
            tutorialTargetVisual = targetItem.GetComponent<TutorialTargetVisual>();
        if (tutorialTargetVisual == null)
            tutorialTargetVisual = targetItem.gameObject.AddComponent<TutorialTargetVisual>();

        tutorialTargetVisual.SetPlayerCamera(playerCamera);
        tutorialTargetVisual.SetTarget(targetItem);
        tutorialTargetVisual.SetVisualsActive(true);

        SubscribePickItemEvents();
    }

    private void ExitPickItemGuidance()
    {
        UnsubscribePickItemEvents();

        if (tutorialTargetVisual != null)
            tutorialTargetVisual.SetVisualsActive(false);
    }

    private void SubscribePickItemEvents()
    {
        UnsubscribePickItemEvents();

        if (PokeManager.Instance != null)
            PokeManager.Instance.OnItemSuccessfullyAdded += HandleItemSuccessfullyAdded;
        else
            Debug.LogWarning("[QuestManager] PokeManager is missing; chỉ dùng cách kiểm tra giỏ hàng dự phòng.");

        UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInteractor[] activeInteractors =
            Object.FindObjectsByType<UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInteractor>(
                FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        tutorialInteractors = System.Array.FindAll(activeInteractors, interactor =>
            interactor != null &&
            (interactor.gameObject.name == "Left_NearFarInteractor" || interactor.gameObject.name == "Right_NearFarInteractor"));

        if (tutorialInteractors.Length == 0)
            Debug.LogWarning("[QuestManager] Left_NearFarInteractor and Right_NearFarInteractor could not be resolved.");

        foreach (UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInteractor interactor in tutorialInteractors)
            interactor.selectEntered.AddListener(HandleInteractorSelectEntered);
    }

    private void UnsubscribePickItemEvents()
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
        if (currentStep != QuestStep.PickItem)
            return;

        // Item sản phẩm được PokeManager xử lý sau khi thực sự vào giỏ.
        // Listener này chỉ bắt trường hợp người chơi cầm nhầm vật không phải sản phẩm.
        if (args.interactableObject == null ||
            args.interactableObject.transform.GetComponentInParent<SelectableItem>() == null)
            wrongItemPicked = true;
    }

    private void HandleItemSuccessfullyAdded(SelectableItem sourceItem)
    {
        if (currentStep != QuestStep.PickItem)
            return;

        if (sourceItem != targetItem)
        {
            wrongItemPicked = true;
            return;
        }

        Log($"Đã lấy đúng item mẫu: {targetItem.itemName}.");
        SetStep(QuestStep.OpenUI);
    }

    private void ResolveTutorialItem()
    {
        if (targetItem != null)
            return;

        SelectableItem[] items = Object.FindObjectsByType<SelectableItem>(
            FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        foreach (SelectableItem item in items)
        {
            if (item == null)
                continue;

            string itemLabel = $"{item.itemName} {item.gameObject.name}";
            if (!string.IsNullOrWhiteSpace(targetItemName) &&
                itemLabel.IndexOf(targetItemName, System.StringComparison.OrdinalIgnoreCase) >= 0)
            {
                targetItem = item;
                return;
            }
        }

        if (items.Length > 0)
            targetItem = items[0];
    }

    private void CaptureInventoryBeforePick()
    {
        inventoryQuantityBeforePick = GetInventoryQuantity();
        targetItemQuantityBeforePick = 0;

        if (PokeManager.Instance != null && targetItem != null &&
            PokeManager.Instance.inventory.TryGetValue(targetItem.itemName, out BillEntry entry))
        {
            targetItemQuantityBeforePick = entry.quantity;
        }
    }

    private int GetInventoryQuantity()
    {
        if (PokeManager.Instance == null)
            return 0;

        int quantity = 0;
        foreach (BillEntry entry in PokeManager.Instance.inventory.Values)
        {
            if (entry != null)
                quantity += entry.quantity;
        }
        return quantity;
    }

    private string GetTargetItemLabel()
    {
        if (targetItem != null && !string.IsNullOrWhiteSpace(targetItem.itemName))
            return targetItem.itemName;
        return string.IsNullOrWhiteSpace(targetItemName) ? "item mẫu" : targetItemName;
    }

    private string BuildMoveStatus()
    {
        if (playerCamera == null || destinationPoint == null)
            return "<color=#F87171>Chưa gán CheckPoint hoặc Camera.</color>";

        Vector2 playerPosition = new Vector2(playerCamera.transform.position.x, playerCamera.transform.position.z);
        Vector2 destinationPosition = new Vector2(destinationPoint.position.x, destinationPoint.position.z);
        float distance = Vector2.Distance(playerPosition, destinationPosition);

        if (distance <= arrivalDistance)
            return $"<color=#10B981>Đã hoàn thành! Chuyển bước sau {Mathf.Max(0f, checkpointCompletionDelay - checkpointTimer):F1}s.</color>";

        return $"<color=#FBBF24>Còn cách CheckPoint: {distance:F1}m. Tiếp tục di chuyển.</color>";
    }

    // ===== Bước 4: hướng dẫn thanh công cụ (từ nhánh feat/tutorial-interactions-with-toolbar) =====
    private void SubscribeToolbarActions()
    {
        if (toolbarSubscribed)
            return;

        if (listController != null)
        {
            listController.OnListShown += HandleListShown;
            listController.OnListHidden += HandleListHidden;
        }

        if (cartButton != null)
            cartButton.onClick.AddListener(HandleCartButtonClicked);
        if (cancelButton != null)
            cancelButton.onClick.AddListener(HandleCancelButtonClicked);
        if (timeButton != null)
            timeButton.onClick.AddListener(HandleTimeButtonClicked);

        toolbarSubscribed = true;
    }

    private void UnsubscribeToolbarActions()
    {
        if (!toolbarSubscribed)
            return;

        if (listController != null)
        {
            listController.OnListShown -= HandleListShown;
            listController.OnListHidden -= HandleListHidden;
        }

        if (cartButton != null)
            cartButton.onClick.RemoveListener(HandleCartButtonClicked);
        if (cancelButton != null)
            cancelButton.onClick.RemoveListener(HandleCancelButtonClicked);
        if (timeButton != null)
            timeButton.onClick.RemoveListener(HandleTimeButtonClicked);

        toolbarSubscribed = false;
    }

    private bool ValidateToolbarReferences()
    {
        bool valid = true;
        valid &= WarnIfMissing(listController, nameof(listController));
        valid &= WarnIfMissing(cartButton, nameof(cartButton));
        valid &= WarnIfMissing(timeButton, nameof(timeButton));
        WarnIfMissing(cancelButton, nameof(cancelButton));
        WarnIfMissing(uiPointer, nameof(uiPointer));
        WarnIfMissing(worldPointer, nameof(worldPointer));
        return valid;
    }

    private bool WarnIfMissing(Object reference, string fieldName)
    {
        if (reference != null)
            return true;

        Debug.LogWarning($"[QuestManager] Missing toolbar tutorial reference: {fieldName}.", this);
        return false;
    }

    private void StopToolbarRoutine()
    {
        if (toolbarRoutine == null)
            return;

        StopCoroutine(toolbarRoutine);
        toolbarRoutine = null;
    }

    private void EnterToolbarStep(ToolbarStep step)
    {
        StopToolbarRoutine();
        toolbarStep = step;
        Log($"Thanh công cụ -> {step}");

        switch (step)
        {
            case ToolbarStep.IntroToolbar:
                HideToolbarPointers();
                if (!ValidateToolbarReferences())
                {
                    Log("Thiếu tham chiếu cho hướng dẫn thanh công cụ, bỏ qua bước 4.");
                    CompleteAllQuests();
                    return;
                }
                toolbarRoutine = StartCoroutine(AdvanceToolbarAfterDelay(introDuration, ToolbarStep.SelectListButton));
                break;

            case ToolbarStep.SelectListButton:
                ShowUIPointer(listButtonTarget);
                break;

            case ToolbarStep.ExplainListBoard:
                ShowWorldPointer(listBoardTarget);
                break;

            case ToolbarStep.SelectCartButton:
                ShowUIPointer(cartButtonTarget);
                break;

            case ToolbarStep.ExplainCart:
                ShowUIPointer(cartPanelTarget);
                toolbarRoutine = StartCoroutine(AdvanceToolbarAfterDelay(cartExplanationDuration, ToolbarStep.CloseCart));
                break;

            case ToolbarStep.CloseCart:
                ShowUIPointer(cancelButtonTarget);
                break;

            case ToolbarStep.SelectTimeButton:
                ShowUIPointer(timeButtonTarget);
                break;

            case ToolbarStep.ExplainRealTimer:
                ShowWorldPointer(realTimerTarget);
                toolbarRoutine = StartCoroutine(AdvanceToolbarAfterDelay(timeExplanationDuration, ToolbarStep.CloseTimePanel));
                break;

            case ToolbarStep.CloseTimePanel:
                ShowUIPointer(cancelButtonTarget);
                break;

            case ToolbarStep.CompletedToolbarTutorial:
                HideToolbarPointers();
                CompleteAllQuests();
                return;
        }

        UpdateUI();
    }

    private IEnumerator AdvanceToolbarAfterDelay(float delay, ToolbarStep nextStep)
    {
        yield return new WaitForSecondsRealtime(delay);
        toolbarRoutine = null;
        if (currentStep == QuestStep.OpenUI)
            EnterToolbarStep(nextStep);
    }

    private void ShowUIPointer(Transform target)
    {
        if (worldPointer != null) worldPointer.SetTarget(null);
        if (uiPointer != null) uiPointer.SetTarget(target);
    }

    private void ShowWorldPointer(Transform target)
    {
        if (uiPointer != null) uiPointer.SetTarget(null);
        if (worldPointer != null) worldPointer.SetTarget(target);
    }

    private void HideToolbarPointers()
    {
        if (uiPointer != null) uiPointer.SetTarget(null);
        if (worldPointer != null) worldPointer.SetTarget(null);
    }

    private bool IsInToolbarStep(ToolbarStep step) =>
        hasInitializedStartStep && currentStep == QuestStep.OpenUI && toolbarStep == step;

    private void HandleListShown(int _)
    {
        if (IsInToolbarStep(ToolbarStep.SelectListButton))
            EnterToolbarStep(ToolbarStep.ExplainListBoard);
    }

    private void HandleListHidden()
    {
        if (IsInToolbarStep(ToolbarStep.ExplainListBoard))
            EnterToolbarStep(ToolbarStep.SelectCartButton);
    }

    private void HandleCartButtonClicked()
    {
        if (IsInToolbarStep(ToolbarStep.SelectCartButton))
        {
            Log($"Cart button clicked: {GetHierarchyPath(cartButton.transform)}");
            EnterToolbarStep(ToolbarStep.ExplainCart);
        }
    }

    private void HandleCancelButtonClicked()
    {
        if (IsInToolbarStep(ToolbarStep.CloseTimePanel))
        {
            Log($"Cancel button clicked: {GetHierarchyPath(cancelButton.transform)}");
            EnterToolbarStep(ToolbarStep.CompletedToolbarTutorial);
        }
        else if (IsInToolbarStep(ToolbarStep.ExplainCart) || IsInToolbarStep(ToolbarStep.CloseCart))
        {
            Log($"Cancel button clicked: {GetHierarchyPath(cancelButton.transform)}");
            EnterToolbarStep(ToolbarStep.SelectTimeButton);
        }
    }

    private void HandleTimeButtonClicked()
    {
        if (IsInToolbarStep(ToolbarStep.SelectTimeButton))
        {
            Log($"Time button clicked: {GetHierarchyPath(timeButton.transform)}");
            EnterToolbarStep(ToolbarStep.ExplainRealTimer);
        }
    }

    private static string GetToolbarMessage(ToolbarStep step)
    {
        switch (step)
        {
            case ToolbarStep.IntroToolbar: return IntroToolbarMessage;
            case ToolbarStep.SelectListButton: return SelectListButtonMessage;
            case ToolbarStep.ExplainListBoard: return ExplainListBoardMessage;
            case ToolbarStep.SelectCartButton: return SelectCartButtonMessage;
            case ToolbarStep.ExplainCart: return ExplainCartMessage;
            case ToolbarStep.CloseCart: return CloseCartMessage;
            case ToolbarStep.SelectTimeButton: return SelectTimeButtonMessage;
            case ToolbarStep.ExplainRealTimer: return ExplainRealTimerMessage;
            case ToolbarStep.CloseTimePanel: return CloseTimePanelMessage;
            default: return "Hoàn tất hướng dẫn thanh công cụ.";
        }
    }

    private static string GetHierarchyPath(Transform item)
    {
        if (item == null)
            return "<missing>";

        string path = item.name;
        while (item.parent != null)
        {
            item = item.parent;
            path = $"{item.name}/{path}";
        }

        return path;
    }

    public void EnsureTutorialUI()
    {
        if (questCanvasObject == null)
        {
            Canvas canvas = Object.FindAnyObjectByType<Canvas>();
            if (canvas != null)
                questCanvasObject = canvas.gameObject;
        }

        if (questCanvasObject != null)
        {
            questCanvasObject.SetActive(true);

            if (notificationCanvasGroup == null)
                notificationCanvasGroup = questCanvasObject.GetComponent<CanvasGroup>();

            if (notificationCanvasGroup != null)
            {
                notificationCanvasGroup.alpha = 1f;
                notificationCanvasGroup.interactable = true;
                notificationCanvasGroup.blocksRaycasts = true;
            }

            if (txtStepTitle == null)
            {
                Transform t = questCanvasObject.transform.Find("Background/TxtStepTitle");
                if (t == null) t = questCanvasObject.transform.Find("TxtStepTitle");
                if (t != null) txtStepTitle = t.GetComponent<TextMeshProUGUI>();
            }

            if (txtStepInstruction == null)
            {
                Transform t = questCanvasObject.transform.Find("Background/TxtStepInstruction");
                if (t == null) t = questCanvasObject.transform.Find("TxtStepInstruction");
                if (t == null) t = questCanvasObject.transform.Find("Background/Notification_Text");
                if (t != null) txtStepInstruction = t.GetComponent<TextMeshProUGUI>();
            }

            if (txtStepStatus == null)
            {
                Transform t = questCanvasObject.transform.Find("Background/TxtStepStatus");
                if (t == null) t = questCanvasObject.transform.Find("TxtStepStatus");
                if (t != null) txtStepStatus = t.GetComponent<TextMeshProUGUI>();
            }

            if (imgProgressBar == null)
            {
                Transform t = questCanvasObject.transform.Find("Background/ProgressBar/ImgBarFill");
                if (t == null) t = questCanvasObject.transform.Find("ProgressBar/ImgBarFill");
                if (t != null) imgProgressBar = t.GetComponent<Image>();
            }

            if (txtProgressPercent == null)
            {
                Transform t = questCanvasObject.transform.Find("Background/ProgressBar/TxtProgressPercent");
                if (t == null) t = questCanvasObject.transform.Find("ProgressBar/TxtProgressPercent");
                if (t != null) txtProgressPercent = t.GetComponent<TextMeshProUGUI>();
            }

            if (btnSkip == null)
            {
                Transform t = questCanvasObject.transform.Find("Background/ButtonGroup/BtnSkip");
                if (t == null) t = questCanvasObject.transform.Find("BtnSkip");
                if (t != null) btnSkip = t.GetComponent<Button>();
            }

            if (btnRetry == null)
            {
                Transform t = questCanvasObject.transform.Find("Background/ButtonGroup/BtnRetry");
                if (t == null) t = questCanvasObject.transform.Find("BtnRetry");
                if (t != null) btnRetry = t.GetComponent<Button>();
            }
        }
    }

    public void ShowQuestNotification(string message)
    {
        if (txtStepInstruction != null)
            txtStepInstruction.text = message;
        else if (notificationText != null)
            notificationText.text = message;
    }

    private void Log(string message)
    {
        Debug.Log($"[QuestManager] {message}");
    }
}
