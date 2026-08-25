using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MllmDialogueOrchestrator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BackendApiConfig config;
    [SerializeField] private MllmApiClient apiClient;
    [SerializeField] private GameSessionContext sessionContext;
    [SerializeField] private NpcActionDispatcher actionDispatcher;
    [SerializeField] private NpcDialoguePresenter dialoguePresenter;

    [Header("Behaviour")]
    [SerializeField] private bool useFallbackOnError = true;
    [SerializeField] private bool logReasoning = true;

    public event Action<MllmApiCallResult> OnDialogueCompleted;

    private void Awake()
    {
        ResolveReferences();
    }

    public void Configure(
        BackendApiConfig apiConfig,
        MllmApiClient client = null,
        GameSessionContext session = null,
        NpcActionDispatcher dispatcher = null,
        NpcDialoguePresenter presenter = null)
    {
        if (apiConfig != null)
            config = apiConfig;
        if (client != null)
            apiClient = client;
        if (session != null)
            sessionContext = session;
        if (dispatcher != null)
            actionDispatcher = dispatcher;
        if (presenter != null)
            dialoguePresenter = presenter;

        ResolveReferences();
    }

    private void ResolveReferences()
    {
        if (apiClient == null)
            apiClient = MllmApiClient.Instance ?? FindObjectOfType<MllmApiClient>();

        if (apiClient != null && config != null)
            apiClient.SetConfig(config);

        if (sessionContext == null)
            sessionContext = GameSessionContext.Instance ?? FindObjectOfType<GameSessionContext>();

        if (actionDispatcher == null)
            actionDispatcher = FindObjectOfType<NpcActionDispatcher>();

        if (dialoguePresenter == null)
            dialoguePresenter = FindObjectOfType<NpcDialoguePresenter>();
    }

    public void RequestDialogue(MllmGenerateDialogueRequest request, Action<MllmApiCallResult> onComplete = null, bool bypassGate = false)
    {
        if (apiClient == null)
        {
            Debug.LogError("[MllmDialogueOrchestrator] MllmApiClient chưa có trong scene.");
            MllmApiCallResult error = MllmApiCallResult.FromError(0, "MllmApiClient chưa có trong scene.");
            HandleResult(error, onComplete);
            return;
        }

        if (request == null)
        {
            MllmApiCallResult error = MllmApiCallResult.FromError(0, "Request null.");
            HandleResult(error, onComplete);
            return;
        }

        if (sessionContext != null && !sessionContext.HasValidCitizenId() && string.IsNullOrWhiteSpace(request.citizen_id))
        {
            Debug.LogWarning("[MllmDialogueOrchestrator] citizen_id chưa được cấu hình — backend có thể trả 404/422.");
        }

        apiClient.TryGenerateDialogue(request, result => HandleResult(result, onComplete), bypassGate);
    }

    public void RequestDialogueFromSession(
        string eventCode,
        string eventDetails,
        Newtonsoft.Json.Linq.JObject contextData = null,
        Action<MllmApiCallResult> onComplete = null,
        bool bypassGate = false)
    {
        MllmGenerateDialogueRequest request = MllmDialogueRequestFactory.BuildFromSession(
            sessionContext,
            eventCode,
            eventDetails,
            contextData);

        RequestDialogue(request, onComplete, bypassGate);
    }

    private void HandleResult(MllmApiCallResult result, Action<MllmApiCallResult> onComplete)
    {
        if (result.wasSkipped)
        {
            Debug.Log($"[MllmDialogueOrchestrator] API skipped — {result.skipReason}");
            if (dialoguePresenter != null && dialoguePresenter.ShowApiDebugOnHeadBubble)
            {
                dialoguePresenter.ShowHeadBubbleDebug($"[API SKIPPED]\nLý do: {result.skipReason}", 2.5f);
            }
            OnDialogueCompleted?.Invoke(result);
            onComplete?.Invoke(result);
            return;
        }

        if (result.success)
        {
            ApplySuccess(result);
        }
        else if (useFallbackOnError)
        {
            ApplyFallback(result);
        }
        else
        {
            Debug.LogWarning($"[MllmDialogueOrchestrator] API lỗi, không dùng fallback: {result.errorMessage}");
        }

        OnDialogueCompleted?.Invoke(result);
        onComplete?.Invoke(result);
    }

    private void ApplySuccess(MllmApiCallResult result)
    {
        MllmGenerateDialogueResponse response = result.response;
        if (response == null || response.result == null)
            return;

        if (sessionContext != null && !string.IsNullOrWhiteSpace(response.appointment_uid))
            sessionContext.SetAppointmentUid(response.appointment_uid);

        if (logReasoning && !string.IsNullOrWhiteSpace(response.result.reasoning))
            Debug.Log($"[MllmDialogueOrchestrator] reasoning: {response.result.reasoning}");

        StartCoroutine(TransitionToDialogueRoutine(response));

        if (actionDispatcher != null && !string.IsNullOrWhiteSpace(response.result.action))
            actionDispatcher.Dispatch(response.result.action);
    }

    private IEnumerator TransitionToDialogueRoutine(MllmGenerateDialogueResponse response)
    {
        if (dialoguePresenter != null && dialoguePresenter.ShowApiDebugOnHeadBubble)
        {
            yield return new WaitForSeconds(1.5f);
        }

        if (dialoguePresenter != null)
        {
            if (response.result.dialogue_map != null && response.result.dialogue_map.Count > 0)
                dialoguePresenter.PresentMap(response.result.dialogue_map);
            else
                dialoguePresenter.Present(response.result.dialogue);
        }
    }

    private void ApplyFallback(MllmApiCallResult result)
    {
        string fallbackDialogue = config != null ? config.fallbackDialogue : "Xin lỗi, hệ thống tạm thời không phản hồi.";
        string fallbackAction = config != null ? config.fallbackAction : MllmActionCodes.AnimGreet;

        Debug.LogWarning(
            $"[MllmDialogueOrchestrator] Dùng fallback do lỗi ({result.statusCode}): {result.errorMessage}");

        result.usedFallback = true;
        result.response = new MllmGenerateDialogueResponse
        {
            result = new MllmAgentResult
            {
                dialogue = fallbackDialogue,
                action = fallbackAction,
                reasoning = "Fallback local do API lỗi."
            }
        };

        if (dialoguePresenter != null && !dialoguePresenter.ShowApiDebugOnHeadBubble)
            dialoguePresenter.Present(fallbackDialogue);

        if (actionDispatcher != null && !string.IsNullOrWhiteSpace(fallbackAction))
            actionDispatcher.Dispatch(fallbackAction);
    }
}