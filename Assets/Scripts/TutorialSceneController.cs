using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.XR;
using TMPro;

/// <summary>
/// Controller cho scene hướng dẫn sử dụng tay cầm VR.
/// Hiển thị các trang hướng dẫn dạng carousel và cho phép dùng cò tay cầm
/// để chuyển bước mà không cần biết cách dùng tia UI từ trước.
/// Sau khi xem xong hoặc nhấn Skip → tự động chuyển sang Scene-level-1-tourtorial.
/// 
/// Cách setup trong Unity:
///  1. Tạo Scene mới tên "Scene-Tutorial" → vào Build Settings → thêm vào TRƯỚC Scene-level-1-tourtorial.
///  2. Tạo Canvas → tạo GameObject "TutorialController" → gắn script này lên.
///  3. Tạo các child GameObject cho từng trang hướng dẫn (Page_01, Page_02, ...).
///  4. Kéo các reference vào Inspector theo mô tả Tooltip bên dưới.
/// </summary>
public class TutorialSceneController : MonoBehaviour
{
    public enum PracticeAction
    {
        Thumbstick,
        Trigger,
        Grip,
        QuickMenu
    }
    // ─────────────────────────────────────────────
    // INSPECTOR FIELDS
    // ─────────────────────────────────────────────

    [Header("=== Scene Chuyển Tiếp ===")]
    [Tooltip("Tên scene sẽ load sau khi hướng dẫn kết thúc (phải có trong Build Settings).")]
    [SerializeField] private string nextSceneName = "Scene-level-1-tourtorial";

    [Header("=== Các Trang Hướng Dẫn ===")]
    [Tooltip("Kéo các GameObject của từng trang hướng dẫn vào đây theo thứ tự. " +
             "Mỗi trang là một GameObject chứa Image minh họa + Text mô tả nút đó.")]
    [SerializeField] private GameObject[] tutorialPages;

    [Header("=== UI Buttons ===")]
    [Tooltip("Nút chuyển sang trang tiếp theo.")]
    [SerializeField] private Button btnNext;

    [Tooltip("Nút quay lại trang trước.")]
    [SerializeField] private Button btnPrev;

    [Tooltip("Nút bỏ qua toàn bộ hướng dẫn, vào game ngay.")]
    [SerializeField] private Button btnSkip;

    [Tooltip("Nút BẮT ĐẦU – chỉ hiện ở trang cuối cùng.")]
    [SerializeField] private Button btnStart;

    [Tooltip("Mỗi trang có một nút THỬ NGAY tương ứng.")]
    [SerializeField] private Button[] practiceButtons;

    [Header("=== Khung Thực Hành ===")]
    [Tooltip("Khung nhỏ hiện ở góc màn hình khi người dùng bấm THỬ NGAY.")]
    [SerializeField] private GameObject practiceOverlay;
    [SerializeField] private TMP_Text txtPracticeAction;
    [SerializeField] private TMP_Text txtPracticeStatus;
    [SerializeField] private TMP_Text txtPracticeCountdown;
    [SerializeField] private Button btnPracticeClose;
    [SerializeField] private PracticeAction[] practiceActionPerPage;
    [Tooltip("Tự mở bài tập sau khi người dùng đọc trang trong vài giây.")]
    [SerializeField] private bool autoOpenPractice = true;
    [SerializeField][Min(0.5f)] private float autoPracticeDelay = 1.75f;

    [Header("=== UI Text & Progress ===")]
    [Tooltip("Text hiển thị số trang hiện tại, ví dụ: '1 / 5'.")]
    [SerializeField] private TMP_Text txtPageIndicator;

    [Tooltip("Thanh tiến trình – dùng Image với Image Type = Filled, Fill Method = Horizontal.")]
    [SerializeField] private Image imgProgressBar;

    [Header("=== Animation ===")]
    [Tooltip("Thời gian fade giữa các trang (giây). Đặt 0 để tắt fade.")]
    [SerializeField][Min(0f)] private float fadeDuration = 0.25f;

    [Header("=== Điều Khiển VR ===")]
    [Tooltip("Cho phép bóp cò trên một trong hai tay cầm để sang bước tiếp theo.")]
    [SerializeField] private bool triggerToAdvance = true;

    [Tooltip("Ngưỡng nhận thao tác bóp cò (0-1).")]
    [SerializeField][Range(0.1f, 1f)] private float triggerPressThreshold = 0.65f;

    // ─────────────────────────────────────────────
    // PRIVATE STATE
    // ─────────────────────────────────────────────

    private int _currentPage = 0;
    private int _totalPages = 0;
    private bool _isTransitioning = false;
    private CanvasGroup[] _pageCanvasGroups;
    private readonly List<InputDevice> _vrControllers = new List<InputDevice>();
    private bool _vrAdvanceWasPressed;
    private bool _isPracticeOpen;
    private bool _practiceInputWasPressed;
    private Coroutine _practiceRoutine;
    private Coroutine _autoPracticeRoutine;

    // ─────────────────────────────────────────────
    // UNITY LIFECYCLE
    // ─────────────────────────────────────────────

    private void Awake()
    {
        _totalPages = (tutorialPages != null) ? tutorialPages.Length : 0;

        // Tạo CanvasGroup cho mỗi trang để có thể fade alpha
        _pageCanvasGroups = new CanvasGroup[_totalPages];
        for (int i = 0; i < _totalPages; i++)
        {
            if (tutorialPages[i] == null) continue;
            _pageCanvasGroups[i] = tutorialPages[i].GetComponent<CanvasGroup>();
            if (_pageCanvasGroups[i] == null)
                _pageCanvasGroups[i] = tutorialPages[i].AddComponent<CanvasGroup>();
        }
    }

    private void Start()
    {
        EnsurePracticeUi();

        // Gắn sự kiện nút
        if (btnNext != null) btnNext.onClick.AddListener(GoToNextPage);
        if (btnPrev != null) btnPrev.onClick.AddListener(GoToPrevPage);
        if (btnSkip != null) btnSkip.onClick.AddListener(SkipTutorial);
        if (btnStart != null) btnStart.onClick.AddListener(StartGame);
        if (practiceButtons != null)
        {
            foreach (Button practiceButton in practiceButtons)
                if (practiceButton != null) practiceButton.onClick.AddListener(OpenPractice);
        }
        if (btnPracticeClose != null) btnPracticeClose.onClick.AddListener(ClosePractice);

        if (_totalPages == 0)
        {
            Debug.LogError("[TutorialSceneController] Chưa có trang hướng dẫn nào được cấu hình.");
            return;
        }

        // Hiện trang đầu tiên ngay lập tức (không fade)
        ShowPageImmediate(_currentPage);
        RefreshUI();
        ScheduleAutoPractice();
    }

    /// <summary>
    /// Tương thích với các bản scene cũ chưa có các reference THỬ NGAY.
    /// Khi chạy, controller tự tạo các nút và khung nhỏ bằng component chuẩn.
    /// </summary>
    private void EnsurePracticeUi()
    {
        if (_totalPages == 0)
            return;

        if (practiceButtons == null || practiceButtons.Length != _totalPages)
            practiceButtons = new Button[_totalPages];

        TMP_FontAsset font = TMP_Settings.defaultFontAsset;
        for (int i = 0; i < _totalPages; i++)
        {
            if (tutorialPages[i] == null)
                continue;

            if (practiceButtons[i] == null)
            {
                Transform existing = tutorialPages[i].transform.Find("BtnPractice");
                practiceButtons[i] = existing != null ? existing.GetComponent<Button>() : null;
            }

            if (practiceButtons[i] == null)
                practiceButtons[i] = CreateRuntimePracticeButton(tutorialPages[i].transform, font);
        }

        if (practiceOverlay == null)
        {
            Transform canvas = transform.parent != null ? transform.parent : transform;
            practiceOverlay = CreateRuntimePracticeOverlay(canvas, font, out txtPracticeAction,
                out txtPracticeStatus, out txtPracticeCountdown, out btnPracticeClose);
        }
    }

    private static Button CreateRuntimePracticeButton(Transform parent, TMP_FontAsset font)
    {
        GameObject buttonObject = new GameObject("BtnPractice", typeof(RectTransform), typeof(CanvasRenderer),
            typeof(Image), typeof(Button));
        buttonObject.layer = 5;
        RectTransform rect = buttonObject.GetComponent<RectTransform>();
        rect.SetParent(parent, false);
        rect.anchorMin = new Vector2(0.78f, 0.035f);
        rect.anchorMax = new Vector2(0.95f, 0.105f);
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        Image image = buttonObject.GetComponent<Image>();
        image.color = new Color32(23, 132, 151, 255);
        image.raycastTarget = true;
        Button button = buttonObject.GetComponent<Button>();
        button.targetGraphic = image;
        CreateRuntimeText(buttonObject.transform, "THỬ NGAY", font, 25f, FontStyles.Bold,
            Color.white, TextAlignmentOptions.Center, new Vector2(0.04f, 0.06f), new Vector2(0.96f, 0.94f));
        return button;
    }

    private static GameObject CreateRuntimePracticeOverlay(Transform parent, TMP_FontAsset font,
        out TMP_Text action, out TMP_Text status, out TMP_Text countdown, out Button close)
    {
        GameObject overlay = new GameObject("PracticeOverlay", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        overlay.layer = 5;
        RectTransform overlayRect = overlay.GetComponent<RectTransform>();
        overlayRect.SetParent(parent, false);
        overlayRect.anchorMin = Vector2.zero;
        overlayRect.anchorMax = Vector2.one;
        overlayRect.offsetMin = Vector2.zero;
        overlayRect.offsetMax = Vector2.zero;
        Image overlayImage = overlay.GetComponent<Image>();
        overlayImage.color = new Color(0.01f, 0.03f, 0.06f, 0.78f);
        overlayImage.raycastTarget = true;

        GameObject card = new GameObject("PracticeCard", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        card.layer = 5;
        RectTransform cardRect = card.GetComponent<RectTransform>();
        cardRect.SetParent(overlay.transform, false);
        cardRect.anchorMin = new Vector2(0.61f, 0.14f);
        cardRect.anchorMax = new Vector2(0.95f, 0.86f);
        cardRect.offsetMin = Vector2.zero;
        cardRect.offsetMax = Vector2.zero;
        card.GetComponent<Image>().color = new Color32(25, 51, 67, 255);

        CreateRuntimeText(card.transform, "THỬ NGAY", font, 46f, FontStyles.Bold,
            new Color32(255, 198, 62, 255), TextAlignmentOptions.Center,
            new Vector2(0.06f, 0.79f), new Vector2(0.94f, 0.95f));
        action = CreateRuntimeText(card.transform, "", font, 34f, FontStyles.Bold,
            Color.white, TextAlignmentOptions.Center, new Vector2(0.08f, 0.45f), new Vector2(0.92f, 0.77f));
        status = CreateRuntimeText(card.transform, "", font, 27f, FontStyles.Normal,
            new Color32(189, 222, 231, 255), TextAlignmentOptions.Center,
            new Vector2(0.08f, 0.29f), new Vector2(0.92f, 0.44f));
        countdown = CreateRuntimeText(card.transform, "", font, 28f, FontStyles.Bold,
            new Color32(255, 198, 62, 255), TextAlignmentOptions.Center,
            new Vector2(0.08f, 0.18f), new Vector2(0.92f, 0.3f));

        GameObject closeObject = new GameObject("BtnPracticeClose", typeof(RectTransform), typeof(CanvasRenderer),
            typeof(Image), typeof(Button));
        closeObject.layer = 5;
        RectTransform closeRect = closeObject.GetComponent<RectTransform>();
        closeRect.SetParent(card.transform, false);
        closeRect.anchorMin = new Vector2(0.33f, 0.035f);
        closeRect.anchorMax = new Vector2(0.67f, 0.16f);
        closeRect.offsetMin = Vector2.zero;
        closeRect.offsetMax = Vector2.zero;
        Image closeImage = closeObject.GetComponent<Image>();
        closeImage.color = new Color32(52, 70, 89, 255);
        closeImage.raycastTarget = true;
        close = closeObject.GetComponent<Button>();
        close.targetGraphic = closeImage;
        CreateRuntimeText(close.transform, "ĐÓNG", font, 25f, FontStyles.Bold,
            Color.white, TextAlignmentOptions.Center, new Vector2(0.04f, 0.06f), new Vector2(0.96f, 0.94f));

        overlay.SetActive(false);
        return overlay;
    }

    private static TMP_Text CreateRuntimeText(Transform parent, string value, TMP_FontAsset font, float size,
        FontStyles style, Color color, TextAlignmentOptions alignment, Vector2 anchorMin, Vector2 anchorMax)
    {
        GameObject textObject = new GameObject("Text", typeof(RectTransform), typeof(CanvasRenderer),
            typeof(TextMeshProUGUI));
        textObject.layer = 5;
        RectTransform rect = textObject.GetComponent<RectTransform>();
        rect.SetParent(parent, false);
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        TextMeshProUGUI text = textObject.GetComponent<TextMeshProUGUI>();
        text.font = font;
        text.text = value;
        text.fontSize = size;
        text.fontStyle = style;
        text.color = color;
        text.alignment = alignment;
        text.textWrappingMode = TextWrappingModes.Normal;
        text.overflowMode = TextOverflowModes.Overflow;
        text.raycastTarget = false;
        return text;
    }

    private void OnDestroy()
    {
        if (btnNext != null) btnNext.onClick.RemoveListener(GoToNextPage);
        if (btnPrev != null) btnPrev.onClick.RemoveListener(GoToPrevPage);
        if (btnSkip != null) btnSkip.onClick.RemoveListener(SkipTutorial);
        if (btnStart != null) btnStart.onClick.RemoveListener(StartGame);
        if (practiceButtons != null)
        {
            foreach (Button practiceButton in practiceButtons)
                if (practiceButton != null) practiceButton.onClick.RemoveListener(OpenPractice);
        }
        if (btnPracticeClose != null) btnPracticeClose.onClick.RemoveListener(ClosePractice);
        if (_practiceRoutine != null) StopCoroutine(_practiceRoutine);
        if (_autoPracticeRoutine != null) StopCoroutine(_autoPracticeRoutine);
    }

    private void Update()
    {
        if (_isPracticeOpen)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                ClosePractice();
            UpdatePracticeInput();
            return;
        }

        // Hỗ trợ phím bàn phím để test trong Editor
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            GoToNextPage();

        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            GoToPrevPage();

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            if (_currentPage >= _totalPages - 1) StartGame();
            else GoToNextPage();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
            SkipTutorial();

        UpdateVrControllerNavigation();
    }

    /// <summary>Mở bài tập ngắn của trang hiện tại trong khung nhỏ ở góc phải.</summary>
    public void OpenPractice()
    {
        if (_isPracticeOpen || _isTransitioning || _totalPages == 0)
            return;

        _isPracticeOpen = true;
        if (_autoPracticeRoutine != null)
        {
            StopCoroutine(_autoPracticeRoutine);
            _autoPracticeRoutine = null;
        }
        _practiceInputWasPressed = false;
        _vrAdvanceWasPressed = false;
        if (_practiceRoutine != null) StopCoroutine(_practiceRoutine);

        if (practiceOverlay != null) practiceOverlay.SetActive(true);
        if (btnNext != null) btnNext.gameObject.SetActive(false);
        if (btnPrev != null) btnPrev.gameObject.SetActive(false);
        if (btnSkip != null) btnSkip.gameObject.SetActive(false);
        if (btnStart != null) btnStart.gameObject.SetActive(false);
        SetPageAlpha(_currentPage, 0.2f);

        PracticeAction action = GetPracticeAction(_currentPage);
        if (txtPracticeAction != null) txtPracticeAction.text = GetPracticeInstruction(action);
        if (txtPracticeStatus != null) txtPracticeStatus.text = "Hãy thử thao tác ngay bây giờ.";
        if (txtPracticeCountdown != null) txtPracticeCountdown.text = "Còn 10 giây";
        _practiceRoutine = StartCoroutine(PracticeTimeoutRoutine());
    }

    private void ScheduleAutoPractice()
    {
        if (!autoOpenPractice || _totalPages == 0 || _isPracticeOpen)
            return;
        if (_autoPracticeRoutine != null) StopCoroutine(_autoPracticeRoutine);
        _autoPracticeRoutine = StartCoroutine(AutoPracticeRoutine(_currentPage));
    }

    private IEnumerator AutoPracticeRoutine(int page)
    {
        yield return new WaitForSecondsRealtime(Mathf.Max(0.5f, autoPracticeDelay));
        _autoPracticeRoutine = null;
        if (!_isPracticeOpen && !_isTransitioning && page == _currentPage)
            OpenPractice();
    }

    /// <summary>Đóng bài tập và quay lại trang hướng dẫn.</summary>
    public void ClosePractice()
    {
        if (!_isPracticeOpen)
            return;

        if (_practiceRoutine != null)
        {
            StopCoroutine(_practiceRoutine);
            _practiceRoutine = null;
        }

        _isPracticeOpen = false;
        _practiceInputWasPressed = false;
        _vrAdvanceWasPressed = false;
        if (practiceOverlay != null) practiceOverlay.SetActive(false);
        SetPageAlpha(_currentPage, 1f);
        RefreshUI();
    }

    private IEnumerator PracticeTimeoutRoutine()
    {
        const float duration = 10f;
        float remaining = duration;
        while (remaining > 0f && _isPracticeOpen)
        {
            remaining -= Time.unscaledDeltaTime;
            if (txtPracticeCountdown != null)
                txtPracticeCountdown.text = $"Còn {Mathf.CeilToInt(Mathf.Max(0f, remaining))} giây";
            yield return null;
        }

        if (!_isPracticeOpen)
            yield break;

        if (txtPracticeStatus != null)
            txtPracticeStatus.text = "Hết thời gian. Hãy bấm ĐÓNG rồi chọn THỬ NGAY để thử lại.";
        if (txtPracticeCountdown != null)
            txtPracticeCountdown.text = string.Empty;
        yield return new WaitForSecondsRealtime(1.5f);
        if (_isPracticeOpen) ClosePractice();
    }

    private void UpdatePracticeInput()
    {
        if (!_isPracticeOpen)
            return;

        bool pressed = IsPracticeActionPressed(GetPracticeAction(_currentPage));
        if (pressed && !_practiceInputWasPressed)
        {
            if (txtPracticeStatus != null)
                txtPracticeStatus.text = "Đã nhận thao tác! Làm rất tốt.";
            if (txtPracticeCountdown != null)
                txtPracticeCountdown.text = "";
            if (_practiceRoutine != null) StopCoroutine(_practiceRoutine);
            _practiceRoutine = StartCoroutine(ClosePracticeAfterSuccess());
        }

        _practiceInputWasPressed = pressed;
    }

    private IEnumerator ClosePracticeAfterSuccess()
    {
        yield return new WaitForSecondsRealtime(1.2f);
        if (_isPracticeOpen) ClosePractice();
    }

    private PracticeAction GetPracticeAction(int page)
    {
        if (practiceActionPerPage != null && page >= 0 && page < practiceActionPerPage.Length)
            return practiceActionPerPage[page];
        return page switch
        {
            0 => PracticeAction.Thumbstick,
            1 => PracticeAction.Trigger,
            2 => PracticeAction.Grip,
            3 => PracticeAction.QuickMenu,
            _ => PracticeAction.Trigger
        };
    }

    private static string GetPracticeInstruction(PracticeAction action)
    {
        return action switch
        {
            PracticeAction.Thumbstick => "Đặt ngón cái lên cần tay trái và đẩy nhẹ theo một hướng.",
            PracticeAction.Trigger => "Hướng tay cầm vào một điểm rồi bóp nhẹ CÒ TRƯỚC.",
            PracticeAction.Grip => "Dùng ngón giữa BÓP GIỮ NÚT BÊN HÔNG tay cầm.",
            PracticeAction.QuickMenu => "BÓP GIỮ NÚT BÊN HÔNG để thử mở thanh công cụ nhanh.",
            _ => "Thử thao tác được hướng dẫn trên trang này."
        };
    }

    private bool IsPracticeActionPressed(PracticeAction action)
    {
#if UNITY_EDITOR
        if (action == PracticeAction.Thumbstick &&
            (Input.GetAxisRaw("Horizontal") != 0f || Input.GetAxisRaw("Vertical") != 0f ||
             Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D)))
            return true;

        if (action == PracticeAction.Trigger &&
            (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.T) || Input.GetMouseButton(0)))
            return true;

        if ((action == PracticeAction.Grip || action == PracticeAction.QuickMenu) &&
            (Input.GetKey(KeyCode.G) || Input.GetMouseButton(1) || Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.M)))
            return true;
#endif

        _vrControllers.Clear();
        InputDevices.GetDevicesWithCharacteristics(
            InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.HeldInHand,
            _vrControllers);

        foreach (InputDevice controller in _vrControllers)
        {
            if (action == PracticeAction.Thumbstick &&
                controller.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 axis) && axis.magnitude >= 0.45f)
                return true;

            if (action == PracticeAction.Trigger && IsTriggerPressed(controller))
                return true;

            if ((action == PracticeAction.Grip || action == PracticeAction.QuickMenu) &&
                controller.TryGetFeatureValue(CommonUsages.gripButton, out bool gripButton) && gripButton)
                return true;

            if ((action == PracticeAction.Grip || action == PracticeAction.QuickMenu) &&
                controller.TryGetFeatureValue(CommonUsages.grip, out float gripValue) && gripValue >= triggerPressThreshold)
                return true;
        }

        return false;
    }

    private bool IsTriggerPressed(InputDevice controller)
    {
        if (controller.TryGetFeatureValue(CommonUsages.triggerButton, out bool triggerButton) && triggerButton)
            return true;
        return controller.TryGetFeatureValue(CommonUsages.trigger, out float triggerValue) &&
               triggerValue >= triggerPressThreshold;
    }

    /// <summary>
    /// Bóp cò là thao tác đầu tiên người dùng được học, vì vậy tutorial cũng dùng
    /// chính thao tác này để chuyển bước. Chỉ nhận cạnh nhấn để tránh lướt qua
    /// nhiều trang khi người dùng giữ cò lâu.
    /// </summary>
    private void UpdateVrControllerNavigation()
    {
        if (!triggerToAdvance || _isTransitioning || _totalPages == 0)
            return;

        _vrControllers.Clear();
        InputDevices.GetDevicesWithCharacteristics(
            InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.HeldInHand,
            _vrControllers);

        bool isPressed = false;
        foreach (InputDevice controller in _vrControllers)
        {
            if (IsTriggerPressed(controller))
            {
                isPressed = true;
                break;
            }
        }

        if (isPressed && !_vrAdvanceWasPressed)
        {
            if (_currentPage >= _totalPages - 1)
                StartGame();
            else
                GoToNextPage();
        }

        _vrAdvanceWasPressed = isPressed;
    }

    // ─────────────────────────────────────────────
    // PUBLIC NAVIGATION (gọi từ Button OnClick hoặc code khác)
    // ─────────────────────────────────────────────

    /// <summary>Chuyển sang trang tiếp theo.</summary>
    public void GoToNextPage()
    {
        if (_isTransitioning || _totalPages == 0 || _currentPage >= _totalPages - 1) return;
        StartCoroutine(ChangePage(_currentPage, _currentPage + 1));
    }

    /// <summary>Quay lại trang trước.</summary>
    public void GoToPrevPage()
    {
        if (_isTransitioning || _totalPages == 0 || _currentPage <= 0) return;
        StartCoroutine(ChangePage(_currentPage, _currentPage - 1));
    }

    /// <summary>Bỏ qua hướng dẫn, vào game ngay.</summary>
    public void SkipTutorial()
    {
        if (_isTransitioning) return;
        LoadNextScene();
    }

    /// <summary>Nút Bắt Đầu ở trang cuối.</summary>
    public void StartGame()
    {
        if (_isTransitioning) return;
        LoadNextScene();
    }

    // ─────────────────────────────────────────────
    // INTERNAL: PAGE DISPLAY
    // ─────────────────────────────────────────────

    /// <summary>Chuyển trang có fade animation.</summary>
    private IEnumerator ChangePage(int from, int to)
    {
        _isTransitioning = true;

        if (fadeDuration > 0f)
        {
            // Fade out trang cũ
            yield return StartCoroutine(FadePage(from, 1f, 0f));
        }

        // Ẩn trang cũ, hiện trang mới
        SetPageActive(from, false);
        _currentPage = to;
        SetPageActive(to, true);

        if (fadeDuration > 0f)
        {
            // Đặt alpha = 0 trước rồi fade in trang mới
            SetPageAlpha(to, 0f);
            yield return StartCoroutine(FadePage(to, 0f, 1f));
        }

        RefreshUI();
        ScheduleAutoPractice();
        _isTransitioning = false;
    }

    /// <summary>Hiện một trang ngay lập tức, tắt tất cả trang còn lại.</summary>
    private void ShowPageImmediate(int index)
    {
        for (int i = 0; i < _totalPages; i++)
        {
            bool isActive = (i == index);
            SetPageActive(i, isActive);
            SetPageAlpha(i, isActive ? 1f : 0f);
        }
    }

    private void SetPageActive(int index, bool active)
    {
        if (index < 0 || index >= _totalPages || tutorialPages[index] == null) return;
        tutorialPages[index].SetActive(active);
    }

    private void SetPageAlpha(int index, float alpha)
    {
        if (index < 0 || index >= _totalPages || _pageCanvasGroups[index] == null) return;
        CanvasGroup cg = _pageCanvasGroups[index];
        cg.alpha = alpha;
        cg.interactable = alpha >= 0.999f;
        cg.blocksRaycasts = alpha >= 0.999f;
    }

    private IEnumerator FadePage(int index, float fromAlpha, float toAlpha)
    {
        if (index < 0 || index >= _totalPages || _pageCanvasGroups[index] == null)
            yield break;

        float elapsed = 0f;
        CanvasGroup cg = _pageCanvasGroups[index];

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            cg.alpha = Mathf.Lerp(fromAlpha, toAlpha, Mathf.Clamp01(elapsed / fadeDuration));
            yield return null;
        }
        cg.alpha = toAlpha;
    }

    // ─────────────────────────────────────────────
    // INTERNAL: UI REFRESH
    // ─────────────────────────────────────────────

    private void RefreshUI()
    {
        if (_totalPages == 0)
        {
            if (btnPrev != null) btnPrev.gameObject.SetActive(false);
            if (btnNext != null) btnNext.gameObject.SetActive(false);
            if (btnStart != null) btnStart.gameObject.SetActive(false);
            if (txtPageIndicator != null) txtPageIndicator.text = "0 / 0";
            if (imgProgressBar != null) imgProgressBar.fillAmount = 0f;
            return;
        }

        bool isFirst = (_currentPage <= 0);
        bool isLast = (_currentPage >= _totalPages - 1);

        // Nút Prev: ẩn ở trang đầu
        if (btnPrev != null) btnPrev.gameObject.SetActive(!isFirst);

        // Nút Next: ẩn ở trang cuối
        if (btnNext != null) btnNext.gameObject.SetActive(!isLast);

        // Nút Start: chỉ hiện ở trang cuối
        if (btnStart != null) btnStart.gameObject.SetActive(isLast);

        // Text chỉ số trang: "1 / 5"
        if (txtPageIndicator != null && _totalPages > 0)
            txtPageIndicator.text = $"{_currentPage + 1} / {_totalPages}";

        // Thanh tiến trình
        if (imgProgressBar != null && _totalPages > 1)
            imgProgressBar.fillAmount = (float)_currentPage / (_totalPages - 1);
    }

    // ─────────────────────────────────────────────
    // INTERNAL: SCENE TRANSITION
    // ─────────────────────────────────────────────

    private void LoadNextScene()
    {
        _isTransitioning = true;

        if (string.IsNullOrEmpty(nextSceneName))
        {
            Debug.LogError("[TutorialSceneController] Chưa đặt tên scene đích (nextSceneName)!");
            _isTransitioning = false;
            return;
        }

        if (!Application.CanStreamedLevelBeLoaded(nextSceneName))
        {
            Debug.LogError($"[TutorialSceneController] Scene '{nextSceneName}' không có trong Build Settings!\n" +
                           "Vào File > Build Settings > Add Open Scenes để thêm vào.");
            _isTransitioning = false;
            return;
        }

        Debug.Log($"[TutorialSceneController] Chuyển sang scene '{nextSceneName}'...");
        SceneManager.LoadScene(nextSceneName, LoadSceneMode.Single);
    }
}
