using UnityEngine;
using System.Collections;
using TMPro;
public class QuestManager : MonoBehaviour
{
    private enum QuestStep { LookAtObject, WalkToLocation, Completed }
    [SerializeField] private QuestStep currentStep = QuestStep.LookAtObject;

    [Header("Core References")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Pointer pointer;

    [Header("Step 1: Look at Object")]
    [SerializeField] private Transform targetObject;
    [SerializeField] private float lookDistanceMax = 5f;
    [SerializeField] private float viewThresholdAngle = 15f;
    [SerializeField] private float requiredLookDuration = 10f;
    private float lookTimer = 0f;

    [Header("Step 2: Walk to Destination")]
    [SerializeField] private Transform destinationPoint; // Điểm cần tới
    [SerializeField] private float arrivalDistance = 5f; // Khoảng cách được tính là "đã tới gần"
    [SerializeField] private float requiredStayDuration = 10f;
    private float stayTimer = 0f;

    void Start()
    {
        ShowQuestNotification("Nhiệm vụ: Hãy nhìn bảng 10s và ghi nhớ danh sách!");

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
                Debug.Log("[QuestManager] Không tìm ra bảng");
            }
        }

        if (pointer == null)
        {
            pointer = Object.FindAnyObjectByType<Pointer>();
            pointer.SetTarget(targetObject);
        }

        if (destinationPoint == null)
        {
            GameObject gameObject = GameObject.Find("CheckPoint");
            if (gameObject != null)
            {
                destinationPoint = gameObject.transform;
            }
            else
            {
                Debug.Log("[QuestManager] Không tìm ra CheckPoint");
            }
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

            case QuestStep.Completed:
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
            Debug.Log($"[QuestManager] Đang nhìn mục tiêu: {lookTimer:F1}/{requiredLookDuration}s");

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
        Debug.Log("[QuestManager] Hoàn thành Bước 1! Đang đổi hướng mũi tên sang điểm đến...");
        currentStep = QuestStep.WalkToLocation;

        // Đổi mục tiêu của mũi tên sang điểm đến mới
        if (pointer != null && destinationPoint != null)
        {
            pointer.SetTarget(destinationPoint);
        }
    }

    // Xử lý Bước 2: Đi đến vị trí
    private void HandleWalkStep()
    {
        if (destinationPoint == null) return;

        // Tính khoảng cách giữa vị trí chân Player (Camera chiếu xuống mặt phẳng) và điểm đến

        Vector2 playerPosition2D = new Vector2(playerCamera.transform.position.x, playerCamera.transform.position.z);
        Vector2 destinationPosition2D = new Vector2(destinationPoint.position.x, destinationPoint.position.z);
        float distance = Vector2.Distance(playerPosition2D, destinationPosition2D);
        Debug.Log($"[QuestManager] Khoảng cách là {distance}");
        if (distance <= arrivalDistance)
        {
            stayTimer += Time.deltaTime;
            Debug.Log($"[QuestManager] Đang đứng trong vùng đích: {stayTimer:F1}/{requiredStayDuration}s");

            if (stayTimer >= requiredStayDuration)
            {
                CompleteAllQuests();
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
        currentStep = QuestStep.Completed;
        Debug.Log("[QuestManager] Toàn bộ chuỗi nhiệm vụ hoàn thành!");

        // Ẩn mũi tên khi xong hết
        if (pointer != null)
        {
            pointer.SetTarget(null);
        }
    }

    [Header("Quest UI Notification")]
    [SerializeField] private GameObject questCanvasObject; // Kéo thẳng GameObject Canvas vào đây
    [SerializeField] private CanvasGroup notificationCanvasGroup;
    [SerializeField] private TextMeshProUGUI notificationText;
    [SerializeField] private float displayDistance = 1.2f;
    [SerializeField] private float displayYOffset = -0.1f;
    [SerializeField] private float notificationDuration = 5.0f;

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