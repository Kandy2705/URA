using UnityEngine;

public class Pointer : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Transform target;

    [Header("Settings")]
    [SerializeField] private float distanceFromCamera = 1.2f;
    [SerializeField] private float verticalOffset = -0.3f;
    [SerializeField] private float smoothSpeed = 10f;
    void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;
        if (target == null){
            GameObject gameObject = GameObject.Find("Notice_Board");
            if(gameObject != null){
                target = gameObject.transform;
            }else{
                Debug.Log("[ArrowObject] Không tìm ra bảng");
            }
        }
    }

    void Update(){
   
    }

    void LateUpdate()
    {
        if (mainCamera == null || target == null) return;

        Vector3 targetPosition = mainCamera.transform.position 
                               + (mainCamera.transform.forward * distanceFromCamera) 
                               + (mainCamera.transform.up * verticalOffset);

        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);

        Vector3 directionToTarget = target.position - transform.position;
        if (directionToTarget != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(directionToTarget);
        }
    }

    // Hàm cập nhật target mới khi đổi nhiệm vụ/mục tiêu
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}