using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class NpcDialoguePresenter : MonoBehaviour
{
    [Header("UI (screen / notification — optional)")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private TMP_Text speakerLabel;

    [Header("Bubble trên đầu NPC")]
    [SerializeField] private bool showHeadBubble = true;
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

    private void Awake()
    {
        if (useTts)
            EnsureAudioSource();

        EnsureHeadBubble();
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

        Camera cam = Camera.main;
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
        if (!showHeadBubble || npcHeadAnchor == null || _headBubbleRoot != null)
            return;

        _headBubbleRoot = new GameObject("NpcDialogueHeadBubble");
        _headBubbleRoot.transform.SetParent(npcHeadAnchor, false);
        _headBubbleRoot.transform.localPosition = new Vector3(0f, headOffsetY, 0f);

        Canvas canvas = _headBubbleRoot.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;

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

    private void ShowHeadBubble(string dialogue)
    {
        EnsureHeadBubble();
        if (_headBubbleRoot == null || _headBubbleText == null)
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

        string requestUrl =
            $"https://translate.google.com/translate_tts?ie=UTF-8&client=tw-ob&tl={ttsLanguage}&q={UnityWebRequest.EscapeURL(text)}";

        using (UnityWebRequest request = UnityWebRequestMultimedia.GetAudioClip(requestUrl, AudioType.MPEG))
        {
            request.timeout = 10;
            request.SetRequestHeader("User-Agent", "Mozilla/5.0");
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogWarning($"[NpcDialoguePresenter] TTS failed: {request.error}");
                _isSpeaking = false;
                yield break;
            }

            AudioClip clip = DownloadHandlerAudioClip.GetContent(request);
            if (clip == null)
            {
                _isSpeaking = false;
                yield break;
            }

            audioSource.Stop();
            audioSource.PlayOneShot(clip);
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
