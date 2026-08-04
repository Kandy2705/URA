using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Chỉ chạy trong Scene-level-2. Tự bootstrap Backend API services và gọi dialogue theo event gameplay.
/// </summary>
[DefaultExecutionOrder(-200)]
public class Level2MllmDialogueBridge : MonoBehaviour
{
    private const string Level2SceneName = "Scene-level-2";

    [Header("Config")]
    [SerializeField] private BackendApiConfig apiConfig;
    [SerializeField] private string citizenIdOverride = "";

    [Header("Level 2 intro sequence")]
    [SerializeField] private bool runIntroSequenceOnStart = true;
    [SerializeField] private float delayBeforeMapIntro = 1f;
    [Tooltip("Nên >= minimumSendIntervalSeconds trong BackendApiConfig để tránh skip do cooldown.")]
    [SerializeField] private float delayBetweenIntroSteps = 5f;
    [SerializeField] private bool triggerPrioritySetup = true;
    [SerializeField] private bool triggerHiddenTaskSetup = true;

    [Header("Map zones (gửi backend nếu DataManager chưa có dữ liệu)")]
    [SerializeField]
    private MapZoneInfo[] defaultMapZones =
    {
        new MapZoneInfo { zoneName = "Quầy trái cây", relativePosition = "Khu A" },
        new MapZoneInfo { zoneName = "Quầy nước uống", relativePosition = "Khu B" },
        new MapZoneInfo { zoneName = "Quầy bánh kẹo", relativePosition = "Khu C" }
    };

    [Header("NPC")]
    [SerializeField] private string npcObjectName = NpcSceneResolver.DefaultNpcObjectName;
    [SerializeField] private Transform npcTransformOverride;
    [SerializeField] private bool ensureNpcActiveOnStart = true;

    [Header("Runtime refs (auto-wire nếu để trống)")]
    [SerializeField] private MllmDialogueOrchestrator orchestrator;
    [SerializeField] private GameSessionContext sessionContext;
    [SerializeField] private ListController listController;
    [SerializeField] private GameTimer gameTimer;
    [SerializeField] private DataManager dataManager;
    [SerializeField] private CartManager cartManager;

    private bool _introRunning;
    private bool _hasAnnouncedReadList;
    private bool _hasRequestedCheckout;

    private void Awake()
    {
        if (SceneManager.GetActiveScene().name != Level2SceneName)
        {
            Debug.Log($"[Level2MllmDialogueBridge] Bỏ qua — scene hiện tại không phải {Level2SceneName}.");
            enabled = false;
            return;
        }

        LoadConfigIfNeeded();
        BootstrapServices();
        WireReferences();
        ConfigureSession();
    }

    private void Start()
    {
        SubscribeGameplayEvents();

        if (runIntroSequenceOnStart)
            StartCoroutine(RunIntroSequence());
    }

    private void OnDestroy()
    {
        UnsubscribeGameplayEvents();
    }

    private void LoadConfigIfNeeded()
    {
        if (apiConfig != null)
            return;

        apiConfig = Resources.Load<BackendApiConfig>("BackendApiConfig");
        if (apiConfig == null)
            Debug.LogWarning("[Level2MllmDialogueBridge] Chưa gán BackendApiConfig — tạo asset trong Resources/BackendApiConfig.");
    }

    private void BootstrapServices()
    {
        Transform servicesRoot = transform;

        MllmApiClient apiClient = servicesRoot.GetComponent<MllmApiClient>();
        if (apiClient == null)
            apiClient = servicesRoot.gameObject.AddComponent<MllmApiClient>();

        if (sessionContext == null)
            sessionContext = servicesRoot.GetComponent<GameSessionContext>();
        if (sessionContext == null)
            sessionContext = servicesRoot.gameObject.AddComponent<GameSessionContext>();

        if (orchestrator == null)
            orchestrator = servicesRoot.GetComponent<MllmDialogueOrchestrator>();
        if (orchestrator == null)
            orchestrator = servicesRoot.gameObject.AddComponent<MllmDialogueOrchestrator>();

        NpcActionDispatcher actionDispatcher = servicesRoot.GetComponent<NpcActionDispatcher>();
        if (actionDispatcher == null)
            actionDispatcher = servicesRoot.gameObject.AddComponent<NpcActionDispatcher>();

        NpcDialoguePresenter dialoguePresenter = servicesRoot.GetComponent<NpcDialoguePresenter>();
        if (dialoguePresenter == null)
            dialoguePresenter = servicesRoot.gameObject.AddComponent<NpcDialoguePresenter>();

        apiClient.SetConfig(apiConfig);
        orchestrator.Configure(apiConfig, apiClient, sessionContext, actionDispatcher, dialoguePresenter);
    }

    private void WireReferences()
    {
        if (listController == null)
            listController = FindScrollUiListController();

        if (gameTimer == null)
            gameTimer = FindObjectOfType<GameTimer>();

        if (dataManager == null)
            dataManager = FindObjectOfType<DataManager>();

        if (cartManager == null)
            cartManager = CartManager.Instance ?? FindObjectOfType<CartManager>();

        Transform guideNpc = ResolveGuideNpcTransform();

        NpcActionDispatcher dispatcher = GetComponent<NpcActionDispatcher>();
        if (dispatcher != null)
            dispatcher.Configure(NpcSceneResolver.FindNpcAnimator(npcObjectName, npcTransformOverride), listController);

        NpcDialoguePresenter presenter = GetComponent<NpcDialoguePresenter>();
        if (presenter != null)
        {
            if (guideNpc != null)
                presenter.ConfigureHeadAnchor(guideNpc, 2.1f);
            else
                Debug.LogWarning($"[Level2MllmDialogueBridge] Không tìm thấy NPC '{npcObjectName}' — bubble dialogue sẽ không hiện trên đầu NPC.");
        }

        if (orchestrator != null)
        {
            orchestrator.Configure(
                apiConfig,
                GetComponent<MllmApiClient>(),
                sessionContext,
                dispatcher,
                presenter);
        }
    }

    private Transform ResolveGuideNpcTransform()
    {
        Transform npc = NpcSceneResolver.FindNpcTransform(npcObjectName, npcTransformOverride);
        if (npc == null)
            return null;

        if (ensureNpcActiveOnStart && !npc.gameObject.activeSelf)
        {
            npc.gameObject.SetActive(true);
            Debug.Log($"[Level2MllmDialogueBridge] Đã bật GameObject NPC '{npc.name}'.");
        }

        return npc;
    }

    private static ListController FindScrollUiListController()
    {
        ListController[] controllers = FindObjectsOfType<ListController>(true);
        foreach (ListController controller in controllers)
        {
            if (controller == null)
                continue;

            if (controller.gameObject.name.Contains("Scroll UI Sample") ||
                controller.NotificationCanvas != null)
                return controller;
        }

        return controllers.Length > 0 ? controllers[0] : null;
    }

    private void ConfigureSession()
    {
        if (sessionContext == null)
            return;

        sessionContext.level = 2;
        sessionContext.gamePhase = MllmGamePhases.PreGame;

        if (!string.IsNullOrWhiteSpace(citizenIdOverride))
            sessionContext.citizenId = citizenIdOverride;
    }

    private void SubscribeGameplayEvents()
    {
        if (listController != null)
        {
            listController.OnListShown += HandleListShown;
            listController.OnListChanged += HandleListChanged;
            listController.OnRandomListChange += HandleRandomListChange;
        }

        if (gameTimer != null)
            gameTimer.onTimeUp.AddListener(HandleTimeUp);
    }

    private void UnsubscribeGameplayEvents()
    {
        if (listController != null)
        {
            listController.OnListShown -= HandleListShown;
            listController.OnListChanged -= HandleListChanged;
            listController.OnRandomListChange -= HandleRandomListChange;
        }

        if (gameTimer != null)
            gameTimer.onTimeUp.RemoveListener(HandleTimeUp);
    }

    private IEnumerator RunIntroSequence()
    {
        if (_introRunning || orchestrator == null)
            yield break;

        _introRunning = true;
        yield return new WaitForSeconds(delayBeforeMapIntro);

        if (sessionContext != null)
            sessionContext.gamePhase = MllmGamePhases.PreGame;

        yield return RunIntroDialogueStep(RequestMapIntro, "map_intro");
        yield return RunIntroDialogueStep(RequestRulesExplanation, "rules_explanation");
        yield return RunIntroDialogueStep(RequestOutOfStockWarning, "out_of_stock_warning");

        if (triggerPrioritySetup)
            yield return RunIntroDialogueStep(RequestPrioritySetup, "priority_setup");

        if (triggerHiddenTaskSetup)
            yield return RunIntroDialogueStep(RequestHiddenTaskSetup, "hidden_task_setup");

        if (sessionContext != null)
            sessionContext.gamePhase = MllmGamePhases.InGame;

        _introRunning = false;

        if (!_hasAnnouncedReadList)
        {
            _hasAnnouncedReadList = true;
            yield return RunDialogueWithRetry(
                cb => orchestrator.RequestDialogue(
                    MllmDialogueRequestFactory.BuildReadShoppingList(sessionContext, listController),
                    cb),
                "read_shopping_list");
        }
    }

    private IEnumerator RunIntroDialogueStep(Action<Action<MllmApiCallResult>> sendStep, string stepName)
    {
        yield return RunDialogueWithRetry(sendStep, stepName);

        if (delayBetweenIntroSteps > 0f)
            yield return new WaitForSeconds(delayBetweenIntroSteps);
    }

    private IEnumerator RunDialogueWithRetry(Action<Action<MllmApiCallResult>> sendStep, string stepName)
    {
        float cooldown = GetMinimumSendIntervalSeconds();
        const int maxAttempts = 8;

        for (int attempt = 1; attempt <= maxAttempts; attempt++)
        {
            MllmApiCallResult result = null;
            bool completed = false;
            sendStep(r =>
            {
                result = r;
                completed = true;
            });

            yield return WaitUntilOrTimeout(() => completed, 45f);

            if (result != null && result.success)
                yield break;

            if (result != null && result.wasSkipped)
            {
                float retryDelay = cooldown + 0.25f;
                Debug.Log(
                    $"[Level2MllmDialogueBridge] {stepName} bị skip ({result.skipReason}) — " +
                    $"thử lại sau {retryDelay:F1}s (lần {attempt}/{maxAttempts})");
                yield return new WaitForSeconds(retryDelay);
                continue;
            }

            if (result != null)
            {
                Debug.LogWarning(
                    $"[Level2MllmDialogueBridge] {stepName} lỗi/timeout — bỏ qua bước này: {result.errorMessage}");
            }

            yield break;
        }
    }

    private float GetMinimumSendIntervalSeconds()
    {
        if (apiConfig != null)
            return Mathf.Max(0f, apiConfig.minimumSendIntervalSeconds);

        MllmApiClient client = GetComponent<MllmApiClient>();
        if (client?.SendGate != null)
            return Mathf.Max(0f, client.SendGate.MinimumSendIntervalSeconds);

        return 4f;
    }

    private static IEnumerator WaitUntilOrTimeout(Func<bool> condition, float timeoutSeconds)
    {
        float elapsed = 0f;
        while (!condition() && elapsed < timeoutSeconds)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    private void HandleListShown(int viewCount)
    {
        if (_introRunning || _hasAnnouncedReadList || orchestrator == null)
            return;

        RequestReadShoppingList(null);
    }

    private void RequestReadShoppingList(string eventDetails = null)
    {
        if (_hasAnnouncedReadList || orchestrator == null)
            return;

        _hasAnnouncedReadList = true;
        orchestrator.RequestDialogue(
            MllmDialogueRequestFactory.BuildReadShoppingList(
                sessionContext,
                listController,
                eventDetails));
    }

    private void HandleListChanged(string oldName, GameObject newPrefab, int newQuantity)
    {
        if (orchestrator == null)
            return;

        if (listController != null && listController.HasPendingRandomChange)
            return;

        string newItemName = newPrefab != null ? newPrefab.name : string.Empty;
        // Postman: caller_info + shifting_task{old_item, new_item, reason}
        orchestrator.RequestDialogue(
            MllmDialogueRequestFactory.BuildShiftingOrder(
                sessionContext,
                oldName,
                newItemName,
                $"Danh sách đổi từ {oldName} sang {newItemName} (x{newQuantity})"));
    }

    private void HandleRandomListChange(string oldName, string newName, int newQuantity, string discountInfo)
    {
        if (orchestrator == null)
            return;

        // Postman: discount_items[{item_name, offers[{discount_percentage, condition_instruction}]}]
        orchestrator.RequestDialogue(
            MllmDialogueRequestFactory.BuildFlashSaleDistraction(
                sessionContext,
                newName,
                discountInfo));
    }

    private void HandleTimeUp()
    {
        if (orchestrator == null)
            return;

        if (sessionContext != null)
            sessionContext.gamePhase = MllmGamePhases.PostGame;

        StartCoroutine(HandleTimeUpSequence());
    }

    private IEnumerator HandleTimeUpSequence()
    {
        yield return RunDialogueWithRetry(
            cb => orchestrator.RequestDialogue(
                MllmDialogueRequestFactory.BuildTimeUpAnnouncement(sessionContext, gameTimer),
                cb),
            "time_up");

        if (_hasRequestedCheckout)
            yield break;

        _hasRequestedCheckout = true;
        yield return RunDialogueWithRetry(
            cb => orchestrator.RequestDialogue(
                MllmDialogueRequestFactory.BuildCheckoutCheck(sessionContext, cartManager),
                cb),
            "checkout");
    }

    private void RequestCheckoutCheck()
    {
        if (_hasRequestedCheckout || orchestrator == null)
            return;

        _hasRequestedCheckout = true;
        StartCoroutine(RunDialogueWithRetry(
            cb => orchestrator.RequestDialogue(
                MllmDialogueRequestFactory.BuildCheckoutCheck(sessionContext, cartManager),
                cb),
            "checkout"));
    }

    public void RequestMapIntro(Action<MllmApiCallResult> onComplete = null)
    {
        if (orchestrator == null)
            return;

        IEnumerable<MapZoneInfo> zones = BuildMapZonesFromDataManager();
        MllmGenerateDialogueRequest request = MllmDialogueRequestFactory.BuildMapIntro(sessionContext, zones);
        orchestrator.RequestDialogue(request, onComplete);
    }

    public void RequestRulesExplanation(Action<MllmApiCallResult> onComplete = null)
    {
        if (orchestrator == null)
            return;

        orchestrator.RequestDialogue(
            MllmDialogueRequestFactory.BuildRulesExplanation(sessionContext, gameTimer),
            onComplete);
    }

    public void RequestPrioritySetup(Action<MllmApiCallResult> onComplete = null)
    {
        if (orchestrator == null)
            return;

        orchestrator.RequestDialogue(
            MllmDialogueRequestFactory.BuildLvl2PrioritySetup(sessionContext, dataManager),
            onComplete);
    }

    public void RequestHiddenTaskSetup(Action<MllmApiCallResult> onComplete = null)
    {
        if (orchestrator == null)
            return;

        orchestrator.RequestDialogue(
            MllmDialogueRequestFactory.BuildLvl2HiddenTaskSetup(sessionContext, listController),
            onComplete);
    }

    public void RequestOutOfStockWarning(Action<MllmApiCallResult> onComplete = null)
    {
        if (orchestrator == null)
            return;

        orchestrator.RequestDialogue(
            MllmDialogueRequestFactory.BuildOutOfStockWarning(sessionContext),
            onComplete);
    }

    private IEnumerable<MapZoneInfo> BuildMapZonesFromDataManager()
    {
        if (dataManager == null || dataManager.targets == null || dataManager.targets.Length == 0)
            return defaultMapZones;

        List<MapZoneInfo> zones = new List<MapZoneInfo>();
        string[] defaultNames = { "Quầy trái cây", "Quầy nước uống 1", "Quầy nước uống 2", "Quầy bánh kẹo" };
        string[] zoneLabels = { "Khu A", "Khu B", "Khu C", "Khu D" };

        for (int i = 0; i < dataManager.targets.Length; i++)
        {
            Transform target = dataManager.targets[i];
            if (target == null)
                continue;

            zones.Add(new MapZoneInfo
            {
                zoneName = i < defaultNames.Length ? defaultNames[i] : target.name,
                relativePosition = i < zoneLabels.Length ? zoneLabels[i] : $"Khu {i + 1}"
            });
        }

        return zones.Count > 0 ? zones : defaultMapZones;
    }

    [ContextMenu("Level2/Test Map Intro")]
    private void ContextTestMapIntro() => RequestMapIntro();

    [ContextMenu("Level2/Test Time Up")]
    private void ContextTestTimeUp() => HandleTimeUp();
}