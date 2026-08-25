using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class NpcDialoguePresenter : MonoBehaviour
{
    public static NpcDialoguePresenter Instance { get; private set; }

    [Header("UI (screen / notification — optional)")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private TMP_Text speakerLabel;

    [Header("Bubble trên đầu NPC")]
    [SerializeField] private bool showHeadBubble = true;
    [SerializeField] private bool showApiDebugOnHeadBubble = false;
    [SerializeField] private GameObject headBubbleInstance;
    [SerializeField] private TMP_Text headBubbleText;
    [SerializeField] private Camera renderCamera;
    [SerializeField] private Transform npcHeadAnchor;
    [SerializeField] private float headOffsetY = 2.1f;
    [SerializeField] private float headBubbleWorldWidth = 3.5f;
    [SerializeField] private int headFontSize = 28;
    [SerializeField] private bool billboardToCamera = true;

    [Header("Audio / TTS")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private bool useTts = false;
    [SerializeField] private string ttsLanguage = "vi";

    [Header("Display")]
    [SerializeField] private float defaultDisplaySeconds = 8f;
    [SerializeField] private bool hidePanelWhenEmpty = true;

    private Coroutine _displayRoutine;
    private GameObject _headBubbleRoot;
    private TMP_Text _headBubbleText;
    private bool _isSpeaking;

    public bool IsSpeaking => _isSpeaking;
    public bool ShowApiDebugOnHeadBubble => showApiDebugOnHeadBubble;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        if (useTts)
            EnsureAudioSource();

        EnsureHeadBubble();
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    public void ConfigureTts(bool enabled, string language = "vi")
    {
        useTts = enabled;
        if (!string.IsNullOrWhiteSpace(language))
            ttsLanguage = language;

        if (useTts)
            EnsureAudioSource();
    }

    private void LateUpdate()
    {
        if (!billboardToCamera || _headBubbleRoot == null || !_headBubbleRoot.activeSelf)
            return;

        Camera cam = renderCamera != null ? renderCamera : Camera.main;
        if (cam == null)
            return;

        Vector3 forward = _headBubbleRoot.transform.position - cam.transform.position;
        if (forward.sqrMagnitude < 0.001f)
            return;

        _headBubbleRoot.transform.rotation = Quaternion.LookRotation(forward.normalized, Vector3.up);
    }

    public void Configure(GameObject panel, TMP_Text text, TMP_Text speaker = null)
    {
        if (panel != null)
            dialoguePanel = panel;
        if (text != null)
            dialogueText = text;
        if (speaker != null)
            speakerLabel = speaker;
    }

    public void ConfigureHeadAnchor(Transform anchor, float offsetY = -1f)
    {
        if (anchor != null)
            npcHeadAnchor = anchor;

        if (offsetY >= 0f)
            headOffsetY = offsetY;

        EnsureHeadBubble();
    }

    public void Present(string dialogue, string speakerName = "NPC")
    {
        if (string.IsNullOrWhiteSpace(dialogue))
        {
            Debug.LogWarning("[NpcDialoguePresenter] dialogue rỗng.");
            return;
        }

        ShowSingle(dialogue, speakerName, defaultDisplaySeconds);
    }

    public void PresentMap(Dictionary<string, string> dialogueMap, float displaySeconds = -1f)
    {
        if (dialogueMap == null || dialogueMap.Count == 0)
        {
            Debug.Log("[NpcDialoguePresenter] dialogue_map rỗng — dùng dialogue đơn.");
            return;
        }

        float duration = displaySeconds > 0f ? displaySeconds : defaultDisplaySeconds;
        List<string> lines = new List<string>();
        foreach (KeyValuePair<string, string> pair in dialogueMap)
            lines.Add($"[{pair.Key}] {pair.Value}");

        ShowSingle(string.Join("\n", lines), "NPC", duration);
    }

    public void ShowSingle(string dialogue, string speakerName, float displaySeconds)
    {
        if (_displayRoutine != null)
        {
            StopCoroutine(_displayRoutine);
            _isSpeaking = false;
        }

        _displayRoutine = StartCoroutine(DisplayRoutine(dialogue, speakerName, displaySeconds));
    }

    public void Hide()
    {
        if (_displayRoutine != null)
        {
            StopCoroutine(_displayRoutine);
            _displayRoutine = null;
            _isSpeaking = false;
        }

        if (UseScreenOverlay())
        {
            if (dialogueText != null)
                dialogueText.text = string.Empty;

            if (speakerLabel != null)
                speakerLabel.text = string.Empty;

            if (hidePanelWhenEmpty && dialoguePanel != null)
                dialoguePanel.SetActive(false);
        }

        HideHeadBubble();
    }

    private bool UseScreenOverlay()
    {
        if (dialoguePanel == null && dialogueText == null)
            return false;

        // Đã có bubble trên đầu NPC — không hiện thêm text trước màn hình
        if (showHeadBubble && npcHeadAnchor != null)
            return false;

        return true;
    }

    private IEnumerator DisplayRoutine(string dialogue, string speakerName, float displaySeconds)
    {
        Debug.Log($"[NpcDialoguePresenter] {speakerName}: {dialogue}");

        if (showHeadBubble && npcHeadAnchor != null)
            ShowHeadBubble(dialogue);
        else
            HideHeadBubble();

        if (UseScreenOverlay())
        {
            if (dialoguePanel != null)
                dialoguePanel.SetActive(true);

            if (speakerLabel != null)
                speakerLabel.text = speakerName;

            if (dialogueText != null)
                dialogueText.text = dialogue;
        }

        if (useTts && audioSource != null)
            yield return PlayTts(dialogue);

        yield return new WaitForSeconds(displaySeconds);

        Hide();
        _displayRoutine = null;
    }

    private void EnsureHeadBubble()
    {
        if (!showHeadBubble)
            return;

        if (_headBubbleRoot != null && _headBubbleText != null)
            return;

        // 1. Kiểm tra nếu đã kéo sẵn trong Inspector
        if (headBubbleInstance != null)
        {
            _headBubbleRoot = headBubbleInstance;
        }

        // 2. Tìm prefab NpcDialogueHeadBubble gắn trực tiếp trên NPC / anchor
        if (_headBubbleRoot == null && npcHeadAnchor != null)
        {
            Transform[] children = npcHeadAnchor.GetComponentsInChildren<Transform>(true);
            foreach (Transform child in children)
            {
                if (child.name == "NpcDialogueHeadBubble")
                {
                    _headBubbleRoot = child.gameObject;
                    break;
                }
            }
        }

        // 3. Tìm trên transform hiện tại hoặc trong Scene
        if (_headBubbleRoot == null)
        {
            Transform[] children = GetComponentsInChildren<Transform>(true);
            foreach (Transform child in children)
            {
                if (child.name == "NpcDialogueHeadBubble")
                {
                    _headBubbleRoot = child.gameObject;
                    break;
                }
            }
        }

        if (_headBubbleRoot == null)
        {
            GameObject foundInScene = GameObject.Find("NpcDialogueHeadBubble");
            if (foundInScene != null)
                _headBubbleRoot = foundInScene;
        }

        // 4. Nếu tìm thấy NpcDialogueHeadBubble có sẵn, chỉ lấy Text component.
        // Không chỉnh Canvas/RectTransform để giữ nguyên cấu hình đã setup trong Inspector.
        if (_headBubbleRoot != null)
        {
            if (headBubbleText != null)
            {
                _headBubbleText = headBubbleText;
            }
            else
            {
                _headBubbleText = _headBubbleRoot.GetComponentInChildren<TMP_Text>(true);
            }

            _headBubbleRoot.SetActive(false);
            return;
        }

        // 5. Fallback tạo động nếu chưa có prefab
        if (npcHeadAnchor == null)
            return;

        _headBubbleRoot = new GameObject("NpcDialogueHeadBubble");
        _headBubbleRoot.transform.SetParent(npcHeadAnchor, false);
        _headBubbleRoot.transform.localPosition = new Vector3(0f, headOffsetY, 0f);

        Canvas canvas = _headBubbleRoot.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.worldCamera = renderCamera != null ? renderCamera : Camera.main;
        canvas.sortingLayerName = "UI VR";

        RectTransform rootRect = _headBubbleRoot.GetComponent<RectTransform>();
        float pixelWidth = 500f;
        float pixelHeight = 220f;
        rootRect.sizeDelta = new Vector2(pixelWidth, pixelHeight);
        rootRect.localScale = Vector3.one * (headBubbleWorldWidth / pixelWidth);

        GameObject backgroundObject = new GameObject("Background");
        backgroundObject.transform.SetParent(_headBubbleRoot.transform, false);
        RectTransform backgroundRect = backgroundObject.AddComponent<RectTransform>();
        backgroundRect.anchorMin = Vector2.zero;
        backgroundRect.anchorMax = Vector2.one;
        backgroundRect.offsetMin = Vector2.zero;
        backgroundRect.offsetMax = Vector2.zero;

        Image backgroundImage = backgroundObject.AddComponent<Image>();
        backgroundImage.color = new Color(0.05f, 0.08f, 0.14f, 0.88f);
        backgroundImage.raycastTarget = false;

        GameObject textObject = new GameObject("DialogueText");
        textObject.transform.SetParent(_headBubbleRoot.transform, false);

        RectTransform textRect = textObject.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = new Vector2(16f, 16f);
        textRect.offsetMax = new Vector2(-16f, -16f);

        _headBubbleText = textObject.AddComponent<TextMeshProUGUI>();
        _headBubbleText.fontSize = headFontSize;
        _headBubbleText.color = Color.white;
        _headBubbleText.alignment = TextAlignmentOptions.Center;
        _headBubbleText.enableWordWrapping = true;
        _headBubbleText.overflowMode = TextOverflowModes.Truncate;
        _headBubbleText.raycastTarget = false;

        _headBubbleRoot.SetActive(false);
    }

    public void ShowHeadBubbleDebug(string message, float holdSeconds = -1f)
    {
        if (!showApiDebugOnHeadBubble)
            return;

        EnsureHeadBubble();
        if (_headBubbleRoot == null)
            return;

        if (_headBubbleText == null)
            _headBubbleText = _headBubbleRoot.GetComponentInChildren<TMP_Text>(true);

        if (_headBubbleText == null)
            return;

        if (_displayRoutine != null)
        {
            StopCoroutine(_displayRoutine);
            _displayRoutine = null;
        }

        _headBubbleText.text = message;
        _headBubbleRoot.SetActive(true);

        if (holdSeconds > 0f)
        {
            _displayRoutine = StartCoroutine(HoldDebugMessageRoutine(holdSeconds));
        }
    }

    private IEnumerator HoldDebugMessageRoutine(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        HideHeadBubble();
        _displayRoutine = null;
    }

    public void ShowApiRequesting(string url)
    {
        string message = $"[API DEBUG]\nĐang gửi request...\n\nRequesting:\n{url}";
        ShowHeadBubbleDebug(message);
    }

    public void ShowApiSuccess(int statusCode, string responseSnippet)
    {
        string snippet = responseSnippet;
        if (!string.IsNullOrEmpty(snippet) && snippet.Length > 200)
            snippet = snippet.Substring(0, 200) + "...";

        string message = $"[API SUCCESS]\nHTTP: {statusCode}\n\nResponse:\n{snippet}";
        ShowHeadBubbleDebug(message);
    }

    public void ShowParseSuccess(string dialogue)
    {
        string message = $"[PARSE SUCCESS]\n\nNPC:\n{dialogue}";
        ShowHeadBubbleDebug(message);
    }

    public void ShowApiError(int statusCode, string resultType, string error, string responseBody)
    {
        string bodySnippet = responseBody;
        if (!string.IsNullOrEmpty(bodySnippet) && bodySnippet.Length > 200)
            bodySnippet = bodySnippet.Substring(0, 200) + "...";

        string message = $"[API ERROR]\n\nHTTP: {statusCode}\nResult: {resultType}\n\nError:\n{error}\n\nResponse:\n{bodySnippet}";
        // Giữ lỗi tối thiểu 10 giây (12s)
        ShowHeadBubbleDebug(message, 12f);
    }

    public void ShowApiTimeout()
    {
        string message = "[API TIMEOUT]\n\nKhông thể kết nối Backend.";
        ShowHeadBubbleDebug(message, 12f);
    }

    public void ShowNetworkError(string detail)
    {
        string message = $"[NETWORK ERROR]\n\n{detail}";
        ShowHeadBubbleDebug(message, 12f);
    }

    public void ShowJsonError(string rawResponse, string exceptionMessage)
    {
        string snippet = rawResponse;
        if (!string.IsNullOrEmpty(snippet) && snippet.Length > 150)
            snippet = snippet.Substring(0, 150) + "...";

        string message = $"[JSON ERROR]\n\nRaw Response:\n{snippet}\n\nException:\n{exceptionMessage}";
        ShowHeadBubbleDebug(message, 12f);
    }

    public void ShowTtsDebug(string status, string error = null)
    {
        if (!showApiDebugOnHeadBubble)
            return;

        string message = string.IsNullOrEmpty(error)
            ? $"[TTS DEBUG]\n{status}"
            : $"[TTS ERROR]\n{status}\n\nError:\n{error}";
        ShowHeadBubbleDebug(message, 4f);
    }

    private void ShowHeadBubble(string dialogue)
    {
        EnsureHeadBubble();
        if (_headBubbleRoot == null)
            return;

        if (_headBubbleText == null)
            _headBubbleText = _headBubbleRoot.GetComponentInChildren<TMP_Text>(true);

        if (_headBubbleText == null)
            return;

        _headBubbleText.text = dialogue;
        _headBubbleRoot.SetActive(true);
    }

    private void HideHeadBubble()
    {
        if (_headBubbleText != null)
            _headBubbleText.text = string.Empty;

        if (_headBubbleRoot != null)
            _headBubbleRoot.SetActive(false);
    }

    private IEnumerator PlayTts(string text)
    {
        _isSpeaking = true;
        EnsureAudioSource();
        if (audioSource == null)
        {
            Debug.LogWarning("[NpcDialoguePresenter] Không tạo được AudioSource để phát TTS.");
            _isSpeaking = false;
            yield break;
        }

        Debug.Log($"[NpcDialoguePresenter] NPC bắt đầu gọi TTS: text='{text}'");

        string encodedText = UnityWebRequest.EscapeURL(text);
        string requestUrl =
            $"https://translate.google.com/translate_tts?ie=UTF-8&client=tw-ob&tl={ttsLanguage}&q={encodedText}";

        Debug.Log($"[NpcDialoguePresenter] TTS URL: {requestUrl}");

        using (UnityWebRequest request = UnityWebRequestMultimedia.GetAudioClip(requestUrl, AudioType.MPEG))
        {
            request.timeout = 10;
            request.SetRequestHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");
            request.SetRequestHeader("Referer", "https://translate.google.com/");
            request.SetRequestHeader("Accept", "*/*");

            yield return request.SendWebRequest();

            Debug.Log($"[NpcDialoguePresenter] TTS Result: {request.result}");
            Debug.Log($"[NpcDialoguePresenter] TTS Error: {request.error}");
            Debug.Log($"[NpcDialoguePresenter] TTS HTTP Code: {request.responseCode}");

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"[NpcDialoguePresenter] Google TTS: {request.responseCode} | {request.error}");
                ShowTtsDebug($"HTTP {request.responseCode}", request.error);
                _isSpeaking = false;
                yield break;
            }

            AudioClip clip = DownloadHandlerAudioClip.GetContent(request);
            if (clip == null)
            {
                Debug.LogError("[NpcDialoguePresenter] Không tạo được AudioClip từ TTS stream.");
                ShowTtsDebug("Clip decode error");
                _isSpeaking = false;
                yield break;
            }

            audioSource.Stop();
            audioSource.clip = clip;
            audioSource.Play();
            yield return new WaitForSeconds(clip.length);
            _isSpeaking = false;
        }
    }

    private void EnsureAudioSource()
    {
        if (audioSource != null)
            return;

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.spatialBlend = 0f;
    }
}
