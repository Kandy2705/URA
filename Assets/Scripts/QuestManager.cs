using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class QuestManager : MonoBehaviour
{
    // Bốn bước người chơi phải thực hiện, sau đó mới được chuyển vào level chính.
    public enum QuestStep { LookAtMap, MoveToPoint, PickItem, OpenUI, Complete }
    [SerializeField] private QuestStep currentStep = QuestStep.LookAtMap;

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

    [Header("Step 4: Open UI")]
    [SerializeField] private ListController listController;
    [SerializeField] private float requiredUIReadDuration = 10f;
    [SerializeField] private string questNotification4 = "Bấm cò mở thanh công cụ, chọn Danh sách/Giỏ hàng và đọc trong 10 giây.";
    private float uiReadTimer = 0f;
    private bool uiWasOpened = false;
    private int listViewsBeforeOpen;

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

        // 3. Tìm Pointer
        if (pointer == null)
        {
            pointer = Object.FindAnyObjectByType<Pointer>();
        }

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

        EnsureTutorialUI();
        InitializeButtons();

        // Khởi động bước 1
        SetStep(QuestStep.LookAtMap);
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
                HandleOpenUIStep();
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

    private void HandleOpenUIStep()
    {
        if (!uiWasOpened)
        {
            if (listController == null)
                listController = Object.FindFirstObjectByType<ListController>();

            if (listController != null && listController.GetClickNumber() > listViewsBeforeOpen)
                uiWasOpened = true;

            if (!uiWasOpened && IsToolbarVisible())
                uiWasOpened = true;
        }

        if (!uiWasOpened)
            return;

        uiReadTimer += Time.deltaTime;
        if (uiReadTimer >= requiredUIReadDuration)
            CompleteAllQuests();
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
        uiReadTimer = 0f;
        uiWasOpened = false;
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
                break;

            case QuestStep.OpenUI:
                uiReadTimer = 0f;
                uiWasOpened = false;
                listViewsBeforeOpen = listController != null ? listController.GetClickNumber() : 0;
                if (pointer != null)
                    pointer.gameObject.SetActive(false);
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
                float uiProgress = Mathf.Clamp01(uiReadTimer / Mathf.Max(0.01f, requiredUIReadDuration));
                overallProgress = 0.75f + uiProgress * 0.25f;
                title = "BƯỚC 4/4: MỞ THANH CÔNG CỤ";
                instruction = questNotification4;
                status = !uiWasOpened
                    ? "<color=#FBBF24>Bấm cò và chọn Danh sách/Giỏ hàng để bắt đầu.</color>"
                    : $"<color=#00E5FF>Đang đọc danh sách: {uiReadTimer:F1}s / {requiredUIReadDuration:F0}s</color>";
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

    private bool IsToolbarVisible()
    {
        UIManager[] managers = Object.FindObjectsByType<UIManager>(
            FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (UIManager manager in managers)
        {
            if (manager == null)
                continue;

            CanvasGroup[] groups = manager.GetComponentsInChildren<CanvasGroup>(true);
            foreach (CanvasGroup group in groups)
            {
                if (group != null && group.alpha > 0.9f && group.interactable)
                    return true;
            }
        }

        return false;
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
