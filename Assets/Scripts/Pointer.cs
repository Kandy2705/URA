using TMPro;
using UnityEngine;

/// <summary>
/// Tracks a tutorial target. The root follows the target exactly, while the
/// pre-authored child visual faces the player and provides gentle attention animation.
/// </summary>
public class Pointer : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Transform target;
    [SerializeField] private Transform visual;
    [SerializeField] private TextMeshPro arrowGraphic;

    [Header("Target Follow")]
    [SerializeField] private Vector3 targetLocalOffset = new Vector3(0f, 0.12f, 0f);
    [SerializeField, Min(0f)] private float rectTargetPadding = 0.04f;

    [Header("Billboard")]
    [SerializeField] private float billboardYawOffset;

    [Header("Attention Animation")]
    [SerializeField, Min(0f)] private float pulseSpeed = 3f;
    [SerializeField, Min(0f)] private float pulseAmount = 0.10f;
    [SerializeField, Range(0f, 1f)] private float minAlpha = 0.65f;
    [SerializeField, Range(0f, 1f)] private float maxAlpha = 1.00f;
    [SerializeField, Min(0f)] private float bobAmount = 0.01f;

    private readonly Vector3[] worldCorners = new Vector3[4];
    private Vector3 baseVisualScale;
    private Vector3 baseVisualLocalPosition;
    private Color arrowBaseColor;
    private bool referencesChecked;

    private void Awake()
    {
        ValidateReferences();

        if (visual != null)
        {
            baseVisualScale = visual.localScale;
            baseVisualLocalPosition = visual.localPosition;
        }

        if (arrowGraphic != null)
            arrowBaseColor = arrowGraphic.color;

        SetVisualVisible(target != null);
    }

    private void LateUpdate()
    {
        if (target == null || !HasRequiredReferences())
            return;

        UpdateTargetPosition();
        UpdateVisual();
    }

    /// <summary>Changes the tutorial target immediately. A null target hides only the visual.</summary>
    public void SetTarget(Transform newTarget)
    {
        ValidateReferences();
        target = newTarget;
        SetVisualVisible(target != null);

        if (target == null)
            return;

        Debug.Log($"[Pointer] Target -> {target.name}", this);

        if (!HasRequiredReferences())
            return;

        UpdateTargetPosition();
        UpdateVisual();
    }

    private void ValidateReferences()
    {
        if (referencesChecked)
            return;

        referencesChecked = true;

        if (mainCamera == null)
            mainCamera = Camera.main;

        if (mainCamera == null)
            Debug.LogWarning("[Pointer] Missing Main Camera", this);

        if (visual == null)
            Debug.LogWarning("[Pointer] Missing visual reference", this);

        if (arrowGraphic == null)
            Debug.LogWarning("[Pointer] Missing ArrowGraphic reference", this);
    }

    private bool HasRequiredReferences()
    {
        return mainCamera != null && visual != null && arrowGraphic != null;
    }

    private void UpdateTargetPosition()
    {
        RectTransform rectTarget = target as RectTransform;
        if (rectTarget != null)
        {
            rectTarget.GetWorldCorners(worldCorners);
            Vector3 topCenter = (worldCorners[1] + worldCorners[2]) * 0.5f;
            Vector3 aboveTarget = rectTarget.TransformDirection(Vector3.up).normalized;
            transform.position = topCenter + aboveTarget * rectTargetPadding;
        }
        else
        {
            transform.position = target.TransformPoint(targetLocalOffset);
        }
    }

    private void UpdateVisual()
    {
        Vector3 direction = mainCamera.transform.position - visual.position;
        if (direction.sqrMagnitude > 0.0001f)
        {
            Quaternion billboard = Quaternion.LookRotation(direction, mainCamera.transform.up);
            visual.rotation = billboard * Quaternion.Euler(0f, billboardYawOffset, 0f);
        }

        float wave = (Mathf.Sin(Time.unscaledTime * pulseSpeed) + 1f) * 0.5f;
        float pulse = 1f + ((wave * 2f - 1f) * pulseAmount);
        visual.localScale = baseVisualScale * pulse;
        visual.localPosition = baseVisualLocalPosition + Vector3.up * ((wave * 2f - 1f) * bobAmount);
        arrowGraphic.color = new Color(
            arrowBaseColor.r,
            arrowBaseColor.g,
            arrowBaseColor.b,
            Mathf.Lerp(minAlpha, maxAlpha, wave));
    }

    private void SetVisualVisible(bool isVisible)
    {
        if (visual != null && visual.gameObject.activeSelf != isVisible)
            visual.gameObject.SetActive(isVisible);
    }
}
