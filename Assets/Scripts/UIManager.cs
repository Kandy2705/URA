using UnityEngine;
using UnityEngine.InputSystem;
// Nếu bạn dùng XR Interaction Toolkit 3.x+, namespace Interactors như bên dưới.
// Với bản cũ hơn, có thể là UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

[DisallowMultipleComponent]
public class UIManager : MonoBehaviour
{
    [Header("Targets")]
    [Tooltip("Gốc UI (panel/canvas) cần ẩn/hiện; vẫn Active để các script/timer bên trong chạy.")]
    [SerializeField] private GameObject menuRoot;

    [Tooltip("Nếu để trống, script sẽ tự thêm CanvasGroup lên menuRoot.")]
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Input (XRI)")]
    [SerializeField] private InputActionReference showMenuAction;   // map vào Trigger/Secondary/Menu...
    [SerializeField] private XRBaseInteractor rightHandInteractor;  // để chặn mở khi đang grab (tùy chọn)

    [Header("Behaviour")]
    [Tooltip("Bật UI ngay khi vào scene.")]
    [SerializeField] private bool startVisible = false;

    [Tooltip("Bật/tắt mỗi lần nhấn (true) hay Nhấn-giữ để hiện, thả để ẩn (false).")]
    [SerializeField] private bool toggleMode = true;

    [Tooltip("Thời gian fade (0 = hiện/ẩn ngay). Dùng unscaled time để không lệ thuộc Time.timeScale.")]
    [Min(0f)][SerializeField] private float fadeDuration = 0f;

    private bool _visible;
    private Coroutine _fadeCo;

    void Reset()
    {
        // Thử đoán target mặc định
        if (!menuRoot) menuRoot = gameObject;
    }

    void Awake()
    {
        if (!menuRoot) menuRoot = gameObject;

        // Đảm bảo menuRoot luôn active để logic bên trong vẫn chạy
        if (!menuRoot.activeSelf) menuRoot.SetActive(true);

        if (!canvasGroup)
        {
            canvasGroup = menuRoot.GetComponent<CanvasGroup>();
            if (!canvasGroup) canvasGroup = menuRoot.AddComponent<CanvasGroup>();
        }

        SetVisibleImmediate(startVisible);
    }

    void OnEnable()
    {
        if (showMenuAction != null)
        {
            var a = showMenuAction.action;

            if (toggleMode)
            {
                a.performed += OnTogglePerformed;
            }
            else
            {
                a.performed += OnHoldPerformed;
                a.canceled  += OnHoldCanceled;
            }
            a.Enable();
        }
    }

    void OnDisable()
    {
        if (showMenuAction != null)
        {
            var a = showMenuAction.action;
            a.performed -= OnTogglePerformed;
            a.performed -= OnHoldPerformed;
            a.canceled  -= OnHoldCanceled;
            a.Disable();
        }
        if (_fadeCo != null) { StopCoroutine(_fadeCo); _fadeCo = null; }
    }

    // --- Input handlers ---
    private void OnTogglePerformed(InputAction.CallbackContext _)
    {
        if (rightHandInteractor && rightHandInteractor.hasSelection) return; // đang grab thì bỏ qua
        SetVisible(!_visible);
    }

    private void OnHoldPerformed(InputAction.CallbackContext _)
    {
        if (rightHandInteractor && rightHandInteractor.hasSelection) return;
        SetVisible(true);
    }

    private void OnHoldCanceled(InputAction.CallbackContext _)
    {
        SetVisible(false);
    }

    // --- Public API (gọi từ chỗ khác nếu cần) ---
    public void Show() => SetVisible(true);
    public void Hide() => SetVisible(false);

    public void SetVisible(bool v)
    {
        if (_visible == v) return;
        _visible = v;

        if (fadeDuration <= 0f)
        {
            ApplyVisibleImmediate(v);
        }
        else
        {
            if (_fadeCo != null) StopCoroutine(_fadeCo);
            _fadeCo = StartCoroutine(FadeRoutine(v ? 1f : 0f));
        }
    }

    public void SetVisibleImmediate(bool v)
    {
        _visible = v;
        ApplyVisibleImmediate(v);
    }

    // --- Internals ---
    private void ApplyVisibleImmediate(bool v)
    {
        if (!canvasGroup) return;
        canvasGroup.alpha = v ? 1f : 0f;
        canvasGroup.interactable = v;
        canvasGroup.blocksRaycasts = v;
    }

    private System.Collections.IEnumerator FadeRoutine(float targetAlpha)
    {
        if (!canvasGroup) yield break;

        float start = canvasGroup.alpha;
        float t = 0f;

        // Vô hiệu tương tác trong lúc fade để tránh click nhầm
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;            // không phụ thuộc timeScale (timer/paused game)
            float p = Mathf.Clamp01(t / fadeDuration);
            canvasGroup.alpha = Mathf.Lerp(start, targetAlpha, p);
            yield return null;
        }

        canvasGroup.alpha = targetAlpha;
        bool on = targetAlpha >= 0.999f;
        canvasGroup.interactable = on;
        canvasGroup.blocksRaycasts = on;
        _fadeCo = null;
    }
}
