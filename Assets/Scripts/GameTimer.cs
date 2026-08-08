using TMPro;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameTimer : MonoBehaviour
{
    [Header("Game Timer UI")]
    [SerializeField]
    private Timer[] timers;
    [Header("Timer Settings")]
    public float limitSeconds = 240f; 

    public GameObject paymentUI;

    [Header("UI")]
    public TMP_Text timerText; 

    [Header("Events")]
    public UnityEvent onTimeUp; 

    [Header("Checkout Flow On Time Up")]
    [Tooltip("Khi hết giờ sẽ chạy luồng thanh toán giống nút thanh toán giỏ hàng (teleport tới PoiChargeMoney + khoá di chuyển + cashier intro).")]
    [SerializeField] private VRCheckoutTeleport checkoutTeleport;

    private float timeLeft;
    public bool isRunning = false;
    private int seconds;
    private int minutes;
    private int hours;
    private bool checkoutTriggered = false;

    // public static event Action OnTimeUpPaymentTriggered;

    void Awake()
    {
        (hours, minutes, seconds) = TimeUtils.SecondsToHMS(limitSeconds);
        foreach (Timer timer in timers) timer.startAtRuntime = false;
        foreach (Timer timer in timers) UIStartTime(timer);
        if (paymentUI != null)
            paymentUI.SetActive(false);
    }

    void Start()
    {
        StartTimer();
        foreach (Timer timer in timers) {if(timer) timer.StartTimer();}
    }


    void Update()
    {
        if (!isRunning) return;

        timeLeft -= Time.deltaTime;

        if (timeLeft > 0)
        {
            UpdateUI();
        }
        else
        {
            TimeIsUp();
        }
    }

    public void UIStartTime(Timer timerObject)
    {
        timerObject.seconds = seconds;
        timerObject.minutes = minutes;
        timerObject.hours = hours;
    }
        
    public void StartTimer()
    {
        timeLeft = limitSeconds;
        isRunning = true;
        UpdateUI();
    }

    public void StopTimer()
    {
        isRunning = false;
        //DataManager.Instance.Report();

        Scene level2Scene = SceneManager.GetSceneByName("Scene-level-2");
        Scene dressScene = SceneManager.GetSceneByName("Dress_Game");

        bool hasLevel2 = level2Scene.IsValid() && level2Scene.isLoaded;
        bool hasDress = dressScene.IsValid() && dressScene.isLoaded;

        if (hasLevel2)
        {
            Debug.Log("StopTimer: Đang ở Level 2 — flow thanh toán do MoveToCheckout xử lý riêng.");
            return;
        }

        if (!hasDress)
        {
            // Level 1: bấm thanh toán lần 1 → tính tiền + hiện Dress_Game.
            if (checkoutTriggered)
            {
                Debug.Log("StopTimer: Đã trigger thanh toán rồi, bỏ qua.");
                return;
            }

            checkoutTriggered = true;

            if (CartManager.Instance != null)
            {
                CartManager.Instance.ProcessCheckout();
            }

            const string dressName = "Dress_Game";

            if (!Application.CanStreamedLevelBeLoaded(dressName))
            {
                Debug.LogError($"Scene '{dressName}' không có trong Build Settings.");
                return;
            }

            SceneManager.LoadSceneAsync(dressName, LoadSceneMode.Additive);
            Debug.Log("StopTimer: Đã tính tiền và load thêm Dress_Game (Additive).");
            return;
        }

        // Đã có Dress_Game → bấm tiếp để chuyển sang Level 2.
        const string level2Name = "Scene-level-2";

        if (!Application.CanStreamedLevelBeLoaded(level2Name))
        {
            Debug.LogError($"Scene '{level2Name}' không có trong Build Settings.");
            return;
        }

        SceneManager.LoadScene(level2Name, LoadSceneMode.Single);
        Debug.Log("StopTimer: Đã chuyển sang Scene-level-2 (Single).");
    }

    void UpdateUI()
    {
        if (timerText == null) return;

        int minutes = Mathf.FloorToInt(timeLeft / 60f);
        int seconds = Mathf.FloorToInt(timeLeft % 60f);
        timerText.text = $"{minutes:00}:{seconds:00}";
    }

    void TimeIsUp()
    {
        isRunning = false;
        timeLeft = 0;
        UpdateUI();

        Debug.Log("Hết giờ");

        if (paymentUI != null)
            paymentUI.SetActive(true);

        if (CartManager.Instance != null)
        {
            CartManager.Instance.ProcessCheckout();
        }

        if (PaymentManager.Instance != null)
        {
            PaymentManager.Instance.UpdatePaymentUI();
        }

        if (checkoutTeleport == null)
            checkoutTeleport = FindObjectOfType<VRCheckoutTeleport>();

        if (checkoutTeleport != null)
        {
            checkoutTeleport.MoveToCheckout();
        }
        else
        {
            Debug.LogWarning("GameTimer: Không tìm thấy VRCheckoutTeleport — chạy flow thanh toán đơn giản (Level 1).");
            LoadDressSceneAfterCheckout();
        }

        onTimeUp?.Invoke();
    }

    private void LoadDressSceneAfterCheckout()
    {
        Scene dressScene = SceneManager.GetSceneByName("Dress_Game");
        bool hasDress = dressScene.IsValid() && dressScene.isLoaded;

        if (hasDress)
            return;

        const string dressName = "Dress_Game";

        if (!Application.CanStreamedLevelBeLoaded(dressName))
        {
            Debug.LogError($"Scene '{dressName}' không có trong Build Settings.");
            return;
        }

        SceneManager.LoadSceneAsync(dressName, LoadSceneMode.Additive);
        Debug.Log("GameTimer: Đã load thêm Dress_Game (Additive) khi hết giờ.");
    }
}
