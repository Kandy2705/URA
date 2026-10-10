using UnityEngine;

/// <summary>
/// World-only extraction of the original teammate Pointer behaviour.
/// It stays in front of the player camera and points at a physical target.
/// </summary>
public class WorldPointer : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Transform target;
    [SerializeField] private GameObject visualRoot;

    [Header("Settings")]
    [SerializeField] private float distanceFromCamera = 1.2f;
    [SerializeField] private float verticalOffset = -0.3f;
    [SerializeField] private float smoothSpeed = 10f;

    private void Awake()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        if (mainCamera == null)
            Debug.LogWarning("[WorldPointer] Missing Main Camera", this);

        if (visualRoot == null)
            Debug.LogWarning("[WorldPointer] Missing visual root", this);

        SetVisualVisible(target != null);
    }

    private void LateUpdate()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        if (mainCamera == null || target == null)
            return;

        Vector3 desiredPosition = mainCamera.transform.position
            + mainCamera.transform.forward * distanceFromCamera
            + mainCamera.transform.up * verticalOffset;

        transform.position = Vector3.Lerp(
            transform.position,
            desiredPosition,
            smoothSpeed * Time.deltaTime);

        Vector3 directionToTarget = target.position - transform.position;
        if (directionToTarget != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(directionToTarget);
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        SetVisualVisible(target != null);

        if (target != null)
            Debug.Log($"[WorldPointer] Target -> {target.name}", this);
    }

    private void SetVisualVisible(bool isVisible)
    {
        if (visualRoot != null && visualRoot.activeSelf != isVisible)
            visualRoot.SetActive(isVisible);
    }
}
