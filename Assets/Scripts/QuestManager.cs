using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Drives the toolbar-only tutorial in Scene-level-1-tourtorial.
/// Progress is based on the live toolbar controls and list lifecycle, never simulated input.
/// </summary>
public class QuestManager : MonoBehaviour
{
    private enum QuestStep
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

    [SerializeField] private QuestStep currentStep = QuestStep.IntroToolbar;

    [Header("Core References")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Pointer uiPointer;
    [SerializeField] private WorldPointer worldPointer;

    [Header("Quest UI Notification")]
    [SerializeField] private GameObject questCanvasObject;
    [SerializeField] private CanvasGroup notificationCanvasGroup;
    [SerializeField] private TextMeshProUGUI notificationText;
    [SerializeField] private float displayDistance = 1.2f;
    [SerializeField] private float displayYOffset = -0.1f;

    [Header("Toolbar Targets")]
    [SerializeField] private Transform listButtonTarget;
    [SerializeField] private Transform cartButtonTarget;
    [SerializeField] private Transform cancelButtonTarget;
    [SerializeField] private Transform timeButtonTarget;
    [SerializeField] private Transform listBoardTarget;
    [SerializeField] private Transform cartPanelTarget;
    [SerializeField] private Transform realTimerTarget;

    [Header("Real UI Actions")]
    [SerializeField] private ListController listController;
    [SerializeField] private Button cartButton;
    [SerializeField] private Button cancelButton;
    [SerializeField] private Button timeButton;

    [Header("Step Timing")]
    [SerializeField, Min(0f)] private float introDuration = 2.5f;
    [SerializeField, Min(0f)] private float cartExplanationDuration = 2.5f;
    [SerializeField, Min(0f)] private float timeExplanationDuration = 2.5f;

    private Coroutine progressionRoutine;
    private bool isSubscribed;
    private bool isInitialized;

    private void OnEnable()
    {
        SubscribeToRealActions();
    }

    private void Start()
    {
        if (!ValidateReferences())
            return;

        isInitialized = true;
        EnterStep(QuestStep.IntroToolbar);
    }

    private void OnDisable()
    {
        if (progressionRoutine != null)
        {
            StopCoroutine(progressionRoutine);
            progressionRoutine = null;
        }

        UnsubscribeFromRealActions();
    }

    private void SubscribeToRealActions()
    {
        if (isSubscribed)
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

        isSubscribed = true;
    }

    private void UnsubscribeFromRealActions()
    {
        if (!isSubscribed)
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

        isSubscribed = false;
    }

    private bool ValidateReferences()
    {
        if (playerCamera == null)
            playerCamera = Camera.main;

        bool valid = true;
        valid &= WarnIfMissing(playerCamera, nameof(playerCamera));
        valid &= WarnIfMissing(uiPointer, nameof(uiPointer));
        valid &= WarnIfMissing(worldPointer, nameof(worldPointer));
        valid &= WarnIfMissing(questCanvasObject, nameof(questCanvasObject));
        valid &= WarnIfMissing(notificationCanvasGroup, nameof(notificationCanvasGroup));
        valid &= WarnIfMissing(notificationText, nameof(notificationText));
        valid &= WarnIfMissing(listButtonTarget, nameof(listButtonTarget));
        valid &= WarnIfMissing(cartButtonTarget, nameof(cartButtonTarget));
        WarnIfMissing(cancelButtonTarget, nameof(cancelButtonTarget));
        valid &= WarnIfMissing(timeButtonTarget, nameof(timeButtonTarget));
        valid &= WarnIfMissing(listBoardTarget, nameof(listBoardTarget));
        valid &= WarnIfMissing(cartPanelTarget, nameof(cartPanelTarget));
        valid &= WarnIfMissing(realTimerTarget, nameof(realTimerTarget));
        valid &= WarnIfMissing(listController, nameof(listController));
        valid &= WarnIfMissing(cartButton, nameof(cartButton));
        WarnIfMissing(cancelButton, nameof(cancelButton));
        valid &= WarnIfMissing(timeButton, nameof(timeButton));
        return valid;
    }

    private bool WarnIfMissing(Object reference, string fieldName)
    {
        if (reference != null)
            return true;

        Debug.LogWarning($"[QuestManager] Missing required reference: {fieldName}.", this);
        return false;
    }

    private void EnterStep(QuestStep step)
    {
        if (progressionRoutine != null)
        {
            StopCoroutine(progressionRoutine);
            progressionRoutine = null;
        }

        currentStep = step;

        switch (step)
        {
            case QuestStep.IntroToolbar:
                HideAllPointers();
                ShowQuestNotification(IntroToolbarMessage);
                progressionRoutine = StartCoroutine(AdvanceAfterDelay(introDuration, QuestStep.SelectListButton));
                break;

            case QuestStep.SelectListButton:
                ShowUIPointer(listButtonTarget);
                ShowQuestNotification(SelectListButtonMessage);
                break;

            case QuestStep.ExplainListBoard:
                ShowWorldPointer(listBoardTarget);
                ShowQuestNotification(ExplainListBoardMessage);
                break;

            case QuestStep.SelectCartButton:
                ShowUIPointer(cartButtonTarget);
                ShowQuestNotification(SelectCartButtonMessage);
                break;

            case QuestStep.ExplainCart:
                ShowUIPointer(cartPanelTarget);
                ShowQuestNotification(ExplainCartMessage);
                progressionRoutine = StartCoroutine(AdvanceAfterDelay(cartExplanationDuration, QuestStep.CloseCart));
                break;

            case QuestStep.CloseCart:
                ShowUIPointer(cancelButtonTarget);
                ShowQuestNotification(CloseCartMessage);
                break;

            case QuestStep.SelectTimeButton:
                ShowUIPointer(timeButtonTarget);
                ShowQuestNotification(SelectTimeButtonMessage);
                break;

            case QuestStep.ExplainRealTimer:
                ShowWorldPointer(realTimerTarget);
                ShowQuestNotification(ExplainRealTimerMessage);
                progressionRoutine = StartCoroutine(AdvanceAfterDelay(timeExplanationDuration, QuestStep.CloseTimePanel));
                break;

            case QuestStep.CloseTimePanel:
                ShowUIPointer(cancelButtonTarget);
                ShowQuestNotification(CloseTimePanelMessage);
                break;

            case QuestStep.CompletedToolbarTutorial:
                HideAllPointers();
                break;
        }
    }

    private IEnumerator AdvanceAfterDelay(float delay, QuestStep nextStep)
    {
        yield return new WaitForSecondsRealtime(delay);
        progressionRoutine = null;
        EnterStep(nextStep);
    }

    private void ShowUIPointer(Transform target)
    {
        worldPointer.SetTarget(null);
        uiPointer.SetTarget(target);
    }

    private void ShowWorldPointer(Transform target)
    {
        uiPointer.SetTarget(null);
        worldPointer.SetTarget(target);
    }

    private void HideAllPointers()
    {
        uiPointer.SetTarget(null);
        worldPointer.SetTarget(null);
    }

    private void HandleListShown(int _)
    {
        if (isInitialized && currentStep == QuestStep.SelectListButton)
            EnterStep(QuestStep.ExplainListBoard);
    }

    private void HandleListHidden()
    {
        if (isInitialized && currentStep == QuestStep.ExplainListBoard)
            EnterStep(QuestStep.SelectCartButton);
    }

    private void HandleCartButtonClicked()
    {
        if (isInitialized && currentStep == QuestStep.SelectCartButton)
        {
            Debug.Log($"[Tutorial] Cart button clicked: {GetHierarchyPath(cartButton.transform)}", this);
            Debug.Log("[Tutorial] Step -> ExplainCart", this);
            EnterStep(QuestStep.ExplainCart);
        }
    }

    private void HandleCancelButtonClicked()
    {
        if (!isInitialized)
            return;

        if (currentStep == QuestStep.CloseTimePanel)
        {
            Debug.Log($"[Tutorial] Cancel button clicked: {GetHierarchyPath(cancelButton.transform)}", this);
            Debug.Log("[Tutorial] Step -> CompletedToolbarTutorial", this);
            EnterStep(QuestStep.CompletedToolbarTutorial);
        }
        else if (currentStep == QuestStep.ExplainCart || currentStep == QuestStep.CloseCart)
        {
            Debug.Log($"[Tutorial] Cancel button clicked: {GetHierarchyPath(cancelButton.transform)}", this);
            Debug.Log("[Tutorial] Step -> SelectTimeButton", this);
            EnterStep(QuestStep.SelectTimeButton);
        }
    }

    private void HandleTimeButtonClicked()
    {
        if (isInitialized && currentStep == QuestStep.SelectTimeButton)
        {
            Debug.Log($"[Tutorial] Time button clicked: {GetHierarchyPath(timeButton.transform)}", this);
            Debug.Log("[Tutorial] Step -> ExplainRealTimer", this);
            EnterStep(QuestStep.ExplainRealTimer);
        }
    }

    public void ShowQuestNotification(string message)
    {
        if (playerCamera == null || questCanvasObject == null)
            return;

        notificationText.text = message;

        Transform cameraTransform = playerCamera.transform;
        Vector3 targetPosition = cameraTransform.position
            + cameraTransform.forward * displayDistance
            + cameraTransform.up * displayYOffset;

        questCanvasObject.transform.position = targetPosition;
        questCanvasObject.transform.LookAt(cameraTransform.position);
        questCanvasObject.transform.Rotate(0f, 180f, 0f);
        questCanvasObject.SetActive(true);
        notificationCanvasGroup.alpha = 1f;
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
}
