using UnityEngine;

public class MllmDialogueTestHarness : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private BackendApiConfig config;
    [SerializeField] private MllmDialogueOrchestrator orchestrator;
    [SerializeField] private MllmApiClient apiClient;
    [SerializeField] private MllmAuthClient authClient;
    [SerializeField] private GameSessionContext sessionContext;

    [Header("Test payload")]
    [SerializeField] private string testCitizenId = "";
    [Range(1, 3)]
    [SerializeField] private int testLevel = 1;
    [SerializeField] private string testGamePhase = MllmGamePhases.PreGame;
    [SerializeField] private string testEventCode = MllmEventCodes.Lvl1MapIntro;
    [TextArea(2, 4)]
    [SerializeField] private string testEventDetails = "Bệnh nhân vừa bước vào siêu thị";
    [TextArea(3, 8)]
    [SerializeField] private string testContextJson =
        "{\n  \"map_layout\": [\n    { \"zone_name\": \"Quầy rau củ\", \"relative_position\": \"Bên tay trái\" }\n  ]\n}";

    [Header("Auth test (optional, không lưu vào code)")]
    [SerializeField] private string testUsername = "";
    [SerializeField] private string testPassword = "";

    [Header("Map intro zones (dùng factory)")]
    [SerializeField] private MapZoneInfo[] testZones =
    {
        new MapZoneInfo { zoneName = "Quầy rau củ", relativePosition = "Bên tay trái" },
        new MapZoneInfo { zoneName = "Quầy nước uống", relativePosition = "Phía trước" }
    };

    private void Awake()
    {
        EnsureReferences();
    }

    [ContextMenu("Test Ping Hello")]
    public void TestPingHello()
    {
        EnsureReferences();
        apiClient.SetConfig(config);
        apiClient.PingHello((ok, message) =>
        {
            Debug.Log(ok ? $"[TestHarness] Ping OK: {message}" : $"[TestHarness] Ping FAIL: {message}");
        });
    }

    [ContextMenu("Test Generate Dialogue (raw payload)")]
    public void TestGenerateDialogueRaw()
    {
        EnsureReferences();
        ApplyTestSession();

        MllmGenerateDialogueRequest request = MllmDialogueRequestFactory.Build(
            testCitizenId,
            testLevel,
            testGamePhase,
            testEventCode,
            testEventDetails,
            MllmDialogueRequestFactory.ParseContextJson(testContextJson));

        orchestrator.RequestDialogue(request, LogResult, bypassGate: true);
    }

    [ContextMenu("Test Generate Dialogue (map intro factory)")]
    public void TestGenerateMapIntroFactory()
    {
        EnsureReferences();
        ApplyTestSession();

        MllmGenerateDialogueRequest request = MllmDialogueRequestFactory.BuildMapIntro(sessionContext, testZones);
        orchestrator.RequestDialogue(request, LogResult, bypassGate: true);
    }

    [ContextMenu("Test Login And Set Bearer Token")]
    public void TestLoginAndSetToken()
    {
        EnsureReferences();

        if (authClient == null)
            authClient = gameObject.AddComponent<MllmAuthClient>();

        StartCoroutine(authClient.Login(
            config,
            testUsername,
            testPassword,
            token =>
            {
                apiClient.SetBearerToken(token.access_token);
                if (config != null)
                {
                    config.sendBearerToken = true;
                    config.bearerToken = token.access_token;
                }

                Debug.Log("[TestHarness] Đã set Bearer token từ login. Chạy lại Test Generate Dialogue.");
            },
            (status, message) => Debug.LogError($"[TestHarness] Login failed ({status}): {message}")));
    }

    private void ApplyTestSession()
    {
        if (sessionContext == null)
            return;

        if (!string.IsNullOrWhiteSpace(testCitizenId))
            sessionContext.citizenId = testCitizenId;

        sessionContext.level = testLevel;
        sessionContext.gamePhase = testGamePhase;
    }

    private void EnsureReferences()
    {
        if (apiClient == null)
            apiClient = MllmApiClient.Instance ?? FindObjectOfType<MllmApiClient>();

        if (orchestrator == null)
            orchestrator = FindObjectOfType<MllmDialogueOrchestrator>();

        if (sessionContext == null)
            sessionContext = GameSessionContext.Instance ?? FindObjectOfType<GameSessionContext>();

        if (apiClient != null && config != null)
            apiClient.SetConfig(config);
    }

    private static void LogResult(MllmApiCallResult result)
    {
        if (result.success)
        {
            Debug.Log(
                $"[TestHarness] SUCCESS | appointment={result.response?.appointment_uid} | " +
                $"action={result.response?.result?.action} | dialogue={result.response?.result?.dialogue}");
        }
        else if (result.wasSkipped)
        {
            Debug.LogWarning($"[TestHarness] SKIPPED | {result.skipReason}");
        }
        else
        {
            Debug.LogWarning(
                $"[TestHarness] FAIL ({result.statusCode}) fallback={result.usedFallback} | {result.errorMessage}");
        }
    }
}