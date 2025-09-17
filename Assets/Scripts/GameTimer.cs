using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class GameTimer : MonoBehaviour
{
    [Header("Timer Settings")]
    public float limitSeconds = 240f; 

    [Header("UI")]
    public TMP_Text timerText; 

    [Header("Events")]
    public UnityEvent onTimeUp; 

    private float timeLeft;
    private bool isRunning = false;

    void Start()
    {
        StartTimer();
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

    public void StartTimer()
    {
        timeLeft = limitSeconds;
        isRunning = true;
        UpdateUI();
    }

    public void StopTimer()
    {
        isRunning = false;
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
        DataManager.Instance.Report();
        UpdateUI();

        Debug.Log("Hết giờ");

        onTimeUp?.Invoke();
    }
}
