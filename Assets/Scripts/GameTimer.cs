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

    private float timeLeft;
    public bool isRunning = false;
    private int seconds;
    private int minutes;
    private int hours;

    // public static event Action OnTimeUpPaymentTriggered;

    void Awake()
    {
        (hours, minutes, seconds) = TimeUtils.SecondsToHMS(limitSeconds);
        foreach (Timer timer in timers) timer.startAtRuntime = false;
        foreach (Timer timer in timers) UIStartTime(timer);
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

        Scene demoScene = SceneManager.GetSceneByName("Demo_18_11");
        Scene level2Scene = SceneManager.GetSceneByName("Scene-level-2");
        Scene dressScene = SceneManager.GetSceneByName("Dress_Game");

        bool hasDemo = demoScene.IsValid() && demoScene.isLoaded;
        bool hasLevel2 = level2Scene.IsValid() && level2Scene.isLoaded;
        bool hasDress = dressScene.IsValid() && dressScene.isLoaded;

        if ((hasDemo) && !hasDress)
        {
            const string dressName = "Dress_Game";

            if (!Application.CanStreamedLevelBeLoaded(dressName))
            {
                Debug.LogError($"Scene '{dressName}' không có trong Build Settings.");
                return;
            }

            SceneManager.LoadSceneAsync(dressName, LoadSceneMode.Additive);
            Debug.Log("StopTimer: Đã load thêm Dress_Game (Additive).");
            return;
        }

        if (hasDemo && hasDress)
        {
            const string level2Name = "Scene-level-2";

            if (!Application.CanStreamedLevelBeLoaded(level2Name))
            {
                Debug.LogError($"Scene '{level2Name}' không có trong Build Settings.");
                return;
            }

            SceneManager.LoadScene(level2Name, LoadSceneMode.Single);
            Debug.Log("StopTimer: Đã chuyển sang Scene-level-2 (Single), tắt luôn Demo_18_11 và Dress_Game.");
            return;
        }

        Debug.Log("StopTimer: Trạng thái scene hiện tại không khớp rule nào, không làm gì.");
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
        paymentUI.SetActive(true);

        if (CartManager.Instance != null)
        {
            CartManager.Instance.ProcessCheckout();
        }

        if (PaymentManager.Instance != null)
        {
            PaymentManager.Instance.UpdatePaymentUI();
        }
        onTimeUp?.Invoke();
    }
}
