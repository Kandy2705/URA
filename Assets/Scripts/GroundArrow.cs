using UnityEngine;

/// <summary>
/// Mũi tên chỉ hướng trên mặt sàn và bám theo người chơi.
/// Player Transform và target có thể gán thủ công trong Inspector.
/// </summary>
public class GroundArrow : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform targetItem;

    [Header("Ground Placement")]
    [SerializeField] private float floorYOffset = 0.02f;
    [SerializeField] private float distanceFromPlayer;

    [Header("Optional Attention Effect")]
    [SerializeField] private bool enableBobbing;
    [SerializeField] private float bobbingHeight = 0.03f;
    [SerializeField] private float bobbingSpeed = 3f;

    private Vector3 baseScale;

    private void Awake()
    {
        baseScale = transform.localScale;
    }

    private void Update()
    {
        if (playerTransform == null || targetItem == null)
            return;

        Vector3 arrowPosition = playerTransform.position;
        Vector3 forward = playerTransform.forward;
        forward.y = 0f;

        if (forward.sqrMagnitude > Mathf.Epsilon)
        {
            forward.Normalize();
            arrowPosition += forward * distanceFromPlayer;
        }

        // Khóa tuyệt đối cao độ để mũi tên luôn song song với mặt sàn.
        arrowPosition.y = floorYOffset;
        transform.position = arrowPosition;

        Vector3 direction = targetItem.position - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude > Mathf.Epsilon)
            transform.rotation = Quaternion.LookRotation(direction.normalized, Vector3.up);

        if (enableBobbing)
        {
            float bob = Mathf.Sin(Time.time * bobbingSpeed) * bobbingHeight;
            transform.localScale = baseScale * (1f + bob);
        }
    }

    /// <summary>Đổi vật phẩm mục tiêu khi tutorial chuyển bước.</summary>
    public void SetTarget(Transform newTarget)
    {
        targetItem = newTarget;
    }

    /// <summary>Bật hoặc tắt toàn bộ object mũi tên.</summary>
    public void Show(bool isVisible)
    {
        gameObject.SetActive(isVisible);
    }

    /// <summary>Gán player lúc runtime nếu cần.</summary>
    public void SetPlayer(Transform player)
    {
        playerTransform = player;
    }

    /// <summary>Tiện ích gán cả player và target cùng lúc.</summary>
    public void Setup(Transform player, Transform item)
    {
        playerTransform = player;
        targetItem = item;
    }
}
