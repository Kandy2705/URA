using UnityEngine;
using System.Collections;
using TMPro;
public class QuestManager : MonoBehaviour
{
    private enum QuestStep { LookAtObject, WalkToLocation, CommingSoon }
    [SerializeField] private QuestStep currentStep = QuestStep.LookAtObject;

    [Header("Core References")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Pointer pointer;

    [Header("Step 1: Look at Object")]
    [SerializeField] private Transform targetObject;
    [SerializeField] private float lookDistanceMax = 5f;
    [SerializeField] private float viewThresholdAngle = 15f;
    [SerializeField] private float requiredLookDuration = 10f;
    [SerializeField] private string QuestNotification1 = "Hãy nhìn bảng 10s và ghi nhớ danh sách!";
    private float lookTimer = 0f;

    [Header("Step 2: Walk to Destination")]
    [SerializeField] private Transform destinationPoint; // Điểm cần tới
    [SerializeField] private float arrivalDistance = 5f; // Khoảng cách được tính là "đã tới gần"
    [SerializeField] private float requiredStayDuration = 10f;
    [SerializeField] private string QuestNotification2 = "Hãy dùng cần analog di chuyển đến điểm mũi tên đang chỉ và đợi 10s!";

    private float stayTimer = 0f;

    void Start()
    {
        ShowQuestNotification(QuestNotification1);

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
                Log("Không tìm ra bảng");
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
                Log("Không tìm ra CheckPoint");
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

            case QuestStep.CommingSoon:
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
            Log($"Đang nhìn mục tiêu: {lookTimer:F1}/{requiredLookDuration}s");

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
        Log("Hoàn thành Bước 1! Đang đổi hướng mũi tên sang điểm đến...");
        currentStep = QuestStep.WalkToLocation;

        // Đổi mục tiêu của mũi tên sang điểm đến mới
        if (pointer != null && destinationPoint != null)
        {
            pointer.SetTarget(destinationPoint);
        }

        ShowQuestNotification(QuestNotification2);
    }

    // Xử lý Bước 2: Đi đến vị trí
    private void HandleWalkStep()
    {
        if (destinationPoint == null) return;

        // Tính khoảng cách giữa vị trí chân Player (Camera chiếu xuống mặt phẳng) và điểm đến

        Vector2 playerPosition2D = new Vector2(playerCamera.transform.position.x, playerCamera.transform.position.z);
        Vector2 destinationPosition2D = new Vector2(destinationPoint.position.x, destinationPoint.position.z);
        float distance = Vector2.Distance(playerPosition2D, destinationPosition2D);
        Log($"Khoảng cách là {distance}");
        if (distance <= arrivalDistance)
        {
            stayTimer += Time.deltaTime;
            Log($"Đang đứng trong vùng đích: {stayTimer:F1}/{requiredStayDuration}s");

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
        currentStep = QuestStep.CommingSoon;
        Log("Coming Soon!");

        // Ẩn mũi tên khi xong hết
        if (pointer != null)
        {
            pointer.SetTarget(null);
            pointer.gameObject.SetActive(false);
        }

        ShowQuestNotification("Coming Soon");
    }

    private void Log(string message)
    {
        Debug.Log($"[QuestManager] {message}");
    }

    [Header("Quest UI Notification")]
    [SerializeField] private GameObject questCanvasObject; // Kéo thẳng GameObject Canvas vào đây
    [SerializeField] private CanvasGroup notificationCanvasGroup;
    [SerializeField] private TextMeshProUGUI notificationText;
    [SerializeField] private float displayDistance = 1.2f;
    [SerializeField] private float displayYOffset = -1.2f;
    [SerializeField] private float notificationDuration = 5f;

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