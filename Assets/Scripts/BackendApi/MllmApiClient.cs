using System;
using System.Collections;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

public class MllmApiClient : MonoBehaviour
{
    public static MllmApiClient Instance { get; private set; }

    [SerializeField] private BackendApiConfig config;

    private string _runtimeBearerToken;
    private bool _isRequestInFlight;
    private readonly MllmApiSendGate _sendGate = new MllmApiSendGate();

    public BackendApiConfig Config => config;
    public bool IsRequestInFlight => _isRequestInFlight;
    public MllmApiSendGate SendGate => _sendGate;

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

    public void SetConfig(BackendApiConfig apiConfig)
    {
        config = apiConfig;
        ApplyGateConfig();
    }

    private void ApplyGateConfig()
    {
        if (config == null)
            return;

        _sendGate.MinimumSendIntervalSeconds = config.minimumSendIntervalSeconds;
        _sendGate.VoiceChatMinimumIntervalSeconds = config.voiceChatMinimumIntervalSeconds;
        _sendGate.LogSkips = config.logApiSkips;
    }

    public void SetBearerToken(string token)
    {
        _runtimeBearerToken = token;
    }

    public string GetEffectiveBearerToken()
    {
        if (!string.IsNullOrWhiteSpace(_runtimeBearerToken))
            return _runtimeBearerToken;

        if (config != null && !string.IsNullOrWhiteSpace(config.bearerToken))
            return config.bearerToken;

        return null;
    }

    public void GenerateDialogue(MllmGenerateDialogueRequest request, Action<MllmApiCallResult> onComplete, bool bypassGate = false)
    {
        StartCoroutine(GenerateDialogueRoutine(request, onComplete, bypassGate));
    }

    public bool TryGenerateDialogue(MllmGenerateDialogueRequest request, Action<MllmApiCallResult> onComplete, bool bypassGate = false)
    {
        string pendingFingerprint = null;

        if (!bypassGate)
        {
            ApplyGateConfig();
            MllmApiSendGate.Evaluation evaluation = _sendGate.Evaluate(request, _isRequestInFlight);
            if (!evaluation.canSend)
            {
                onComplete?.Invoke(MllmApiCallResult.FromSkip(evaluation.skipReason));
                return false;
            }

            pendingFingerprint = evaluation.fingerprint;
            _isRequestInFlight = true;
            if (!string.IsNullOrEmpty(pendingFingerprint))
                _sendGate.MarkSendStarted(pendingFingerprint);
        }
        else
        {
            _isRequestInFlight = true;
        }

        StartCoroutine(GenerateDialogueRoutine(request, onComplete, bypassGate, gateAlreadyPassed: !bypassGate));
        return true;
    }

    public void PingHello(Action<bool, string> onComplete)
    {
        StartCoroutine(PingHelloRoutine(onComplete));
    }

    private IEnumerator GenerateDialogueRoutine(
        MllmGenerateDialogueRequest request,
        Action<MllmApiCallResult> onComplete,
        bool bypassGate,
        bool gateAlreadyPassed = false)
    {
        if (config == null)
        {
            Debug.LogError("[MllmApiClient] BackendApiConfig chưa được gán.");
            onComplete?.Invoke(MllmApiCallResult.FromError(0, "BackendApiConfig chưa được gán."));
            _isRequestInFlight = false;
            yield break;
        }

        if (request == null || request.content == null)
        {
            onComplete?.Invoke(MllmApiCallResult.FromError(0, "Request hoặc content bị null."));
            _isRequestInFlight = false;
            yield break;
        }

        if (!bypassGate && !gateAlreadyPassed)
        {
            ApplyGateConfig();
            MllmApiSendGate.Evaluation evaluation = _sendGate.Evaluate(request, _isRequestInFlight);
            if (!evaluation.canSend)
            {
                onComplete?.Invoke(MllmApiCallResult.FromSkip(evaluation.skipReason));
                _isRequestInFlight = false;
                yield break;
            }

            _isRequestInFlight = true;
            if (!string.IsNullOrEmpty(evaluation.fingerprint))
                _sendGate.MarkSendStarted(evaluation.fingerprint);
        }
        else if (bypassGate && !_isRequestInFlight)
        {
            _isRequestInFlight = true;
        }

        string json = JsonConvert.SerializeObject(request, new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Include
        });

        Debug.Log($"[MllmApiClient] POST {config.GenerateDialogueUrl}\nPayload: {json}");

        NpcDialoguePresenter presenter = NpcDialoguePresenter.Instance ?? FindObjectOfType<NpcDialoguePresenter>();
        if (presenter != null)
        {
            presenter.ShowApiRequesting(config.GenerateDialogueUrl);
        }

        using (UnityWebRequest webRequest = BuildJsonPostRequest(config.GenerateDialogueUrl, json))
        {
            yield return webRequest.SendWebRequest();
            MllmApiCallResult result = ParseGenerateDialogueResponse(webRequest, presenter);
            onComplete?.Invoke(result);
        }

        _isRequestInFlight = false;
    }

    private IEnumerator PingHelloRoutine(Action<bool, string> onComplete)
    {
        if (config == null)
        {
            onComplete?.Invoke(false, "BackendApiConfig chưa được gán.");
            yield break;
        }

        NpcDialoguePresenter presenter = NpcDialoguePresenter.Instance ?? FindObjectOfType<NpcDialoguePresenter>();
        if (presenter != null)
        {
            presenter.ShowApiRequesting(config.HelloUrl);
        }

        using (UnityWebRequest webRequest = UnityWebRequest.Get(config.HelloUrl))
        {
            ApplyAuthHeader(webRequest);
            webRequest.timeout = Mathf.CeilToInt(config.timeoutSeconds);
            yield return webRequest.SendWebRequest();

            bool ok = webRequest.result == UnityWebRequest.Result.Success;
            string message = ok
                ? webRequest.downloadHandler.text
                : BuildHttpErrorMessage((int)webRequest.responseCode, webRequest.error, webRequest.downloadHandler?.text, webRequest.result);

            Debug.Log(ok
                ? $"[MllmApiClient] Hello OK: {message}"
                : $"[MllmApiClient] Hello failed: {message}");

            if (presenter != null)
            {
                if (ok)
                {
                    presenter.ShowHeadBubbleDebug($"[PING SUCCESS]\nHTTP: 200\n\n{message}", 3f);
                }
                else if (webRequest.result == UnityWebRequest.Result.ConnectionError)
                {
                    presenter.ShowNetworkError(webRequest.error ?? "Cannot resolve destination host");
                }
                else
                {
                    presenter.ShowApiError((int)webRequest.responseCode, webRequest.result.ToString(), webRequest.error, webRequest.downloadHandler?.text);
                }
            }

            onComplete?.Invoke(ok, message);
        }
    }

    private UnityWebRequest BuildJsonPostRequest(string url, string json)
    {
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
        UnityWebRequest webRequest = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST);
        webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
        webRequest.downloadHandler = new DownloadHandlerBuffer();
        webRequest.SetRequestHeader("Content-Type", "application/json");
        webRequest.SetRequestHeader("Accept", "application/json");
        ApplyAuthHeader(webRequest);
        webRequest.timeout = Mathf.CeilToInt(config.timeoutSeconds);
        return webRequest;
    }

    private void ApplyAuthHeader(UnityWebRequest webRequest)
    {
        if (config == null || !config.sendBearerToken)
            return;

        string token = GetEffectiveBearerToken();
        if (!string.IsNullOrWhiteSpace(token))
            webRequest.SetRequestHeader("Authorization", "Bearer " + token);
    }

    private MllmApiCallResult ParseGenerateDialogueResponse(UnityWebRequest webRequest, NpcDialoguePresenter presenter = null)
    {
        if (presenter == null)
            presenter = NpcDialoguePresenter.Instance ?? FindObjectOfType<NpcDialoguePresenter>();

        string body = webRequest.downloadHandler?.text;
        int statusCode = (int)webRequest.responseCode;
        bool isTimeout = webRequest.result == UnityWebRequest.Result.ConnectionError &&
                         !string.IsNullOrEmpty(webRequest.error) &&
                         webRequest.error.IndexOf("timeout", StringComparison.OrdinalIgnoreCase) >= 0;
        bool isNetworkError = webRequest.result == UnityWebRequest.Result.ConnectionError;

        if (webRequest.result != UnityWebRequest.Result.Success)
        {
            string message = BuildHttpErrorMessage(statusCode, webRequest.error, body, webRequest.result);
            Debug.LogError($"[MllmApiClient] Generate dialogue failed ({statusCode}): {message}");

            if (presenter != null)
            {
                if (isTimeout)
                    presenter.ShowApiTimeout();
                else if (isNetworkError)
                    presenter.ShowNetworkError(webRequest.error ?? "Cannot resolve destination host");
                else
                    presenter.ShowApiError(statusCode, webRequest.result.ToString(), webRequest.error, body);
            }

            MllmApiCallResult errorResult = MllmApiCallResult.FromError(statusCode, message, body);
            errorResult.isTimeout = isTimeout;
            errorResult.isNetworkError = isNetworkError;
            return errorResult;
        }

        if (presenter != null)
        {
            presenter.ShowApiSuccess(statusCode, body);
        }

        try
        {
            MllmGenerateDialogueResponse response = JsonConvert.DeserializeObject<MllmGenerateDialogueResponse>(body);
            if (response?.result == null)
            {
                if (presenter != null)
                    presenter.ShowJsonError(body, "Response thiếu field 'result'.");

                return MllmApiCallResult.FromError(statusCode, "Response thiếu field 'result'.", body);
            }

            Debug.Log(
                $"[MllmApiClient] OK ({statusCode}) | action={response.result.action} | " +
                $"latency={response.latency_seconds:F2}s | model={response.model_latency_seconds:F2}s\n" +
                $"dialogue: {response.result.dialogue}");

            if (presenter != null)
            {
                presenter.ShowParseSuccess(response.result.dialogue);
            }

            return MllmApiCallResult.FromSuccess(response, body, statusCode);
        }
        catch (Exception ex)
        {
            Debug.LogError($"[MllmApiClient] Không parse được response: {ex.Message}\nBody: {body}");

            if (presenter != null)
            {
                presenter.ShowJsonError(body, ex.Message);
            }

            return MllmApiCallResult.FromError(statusCode, $"Parse error: {ex.Message}", body);
        }
    }

    private static string BuildHttpErrorMessage(int statusCode, string transportError, string body, UnityWebRequest.Result result)
    {
        if (result == UnityWebRequest.Result.ConnectionError &&
            !string.IsNullOrEmpty(transportError) &&
            transportError.IndexOf("timeout", StringComparison.OrdinalIgnoreCase) >= 0)
        {
            return $"Timeout — server không phản hồi trong thời gian cho phép. ({transportError})";
        }

        if (result == UnityWebRequest.Result.ConnectionError)
            return $"Network error: {transportError}";

        switch (statusCode)
        {
            case 400:
                return $"400 Bad Request — payload không hợp lệ. {AppendBody(body)}";
            case 401:
                return $"401 Unauthorized — cần Bearer token hoặc login. {AppendBody(body)}";
            case 404:
                return $"404 Not Found — bệnh nhân/appointment không tìm thấy hoặc chưa có active appointment. {AppendBody(body)}";
            case 422:
                return $"422 Validation Error — kiểm tra citizen_id, level, event_code. {AppendBody(body)}";
            case 500:
                return $"500 Internal Server Error — lỗi backend/AI. {AppendBody(body)}";
            default:
                return $"HTTP {statusCode}. {transportError} {AppendBody(body)}".Trim();
        }
    }

    private static string AppendBody(string body)
    {
        return string.IsNullOrWhiteSpace(body) ? string.Empty : $"Body: {body}";
    }
}