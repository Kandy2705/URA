using UnityEngine;

public class GameSessionContext : MonoBehaviour
{
    public static GameSessionContext Instance { get; private set; }

    [Header("Patient / Session")]
    [Tooltip("CCCD hoặc mã định danh bệnh nhân — lấy từ hệ thống đăng nhập/QR, không hardcode.")]
    public string citizenId = "";

    [Range(1, 3)]
    public int level = 1;

    public string gamePhase = MllmGamePhases.PreGame;

    [Header("Runtime (read-only trong Play Mode)")]
    [SerializeField] private string lastAppointmentUid;

    public string LastAppointmentUid => lastAppointmentUid;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void SetAppointmentUid(string appointmentUid)
    {
        lastAppointmentUid = appointmentUid;
    }

    public bool HasValidCitizenId()
    {
        return !string.IsNullOrWhiteSpace(citizenId);
    }
}