using UnityEngine;

[CreateAssetMenu(fileName = "BackendApiConfig", menuName = "URA/Backend API Config")]
public class BackendApiConfig : ScriptableObject
{
    [Header("Server")]
    [Tooltip("Base URL không có dấu / ở cuối.")]
    public string baseUrl = "https://vr-supermarket-ai-service-cqcybyc4fahxf5fp.southeastasia-01.azurewebsites.net";

    [Header("Paths")]
    public string generateDialoguePath = "/api/v1/dialogue/generate_refactored";
    public string loginPath = "/auth/login";
    public string helloPath = "/dialogue/hello";

    [Header("Auth (optional)")]
    [Tooltip("Bật để gửi Authorization: Bearer header nếu bearerToken không rỗng.")]
    public bool sendBearerToken = false;

    [Tooltip("Bearer token — cấu hình trong Inspector, không hardcode trong code.")]
    public string bearerToken = "";

    [Header("Request")]
    [Min(1f)]
    public float timeoutSeconds = 30f;

    [Header("Rate limit / anti-spam")]
    [Tooltip("Khoảng cách tối thiểu giữa 2 lần gọi generate_refactored.")]
    [Min(0f)]
    public float minimumSendIntervalSeconds = 4f;

    [Tooltip("Cooldown riêng cho voice_chat_trigger — thường ngắn hơn.")]
    [Min(0f)]
    public float voiceChatMinimumIntervalSeconds = 2f;

    [Tooltip("Ghi log khi request bị skip do gate.")]
    public bool logApiSkips = true;

    [Header("Fallback khi API lỗi")]
    [TextArea(2, 4)]
    public string fallbackDialogue = "Xin lỗi, tôi chưa thể phản hồi lúc này. Bạn hãy tiếp tục thử nhé.";

    public string fallbackAction = "Anim_Greet";

    public string GenerateDialogueUrl => CombineUrl(baseUrl, generateDialoguePath);
    public string LoginUrl => CombineUrl(baseUrl, loginPath);
    public string HelloUrl => CombineUrl(baseUrl, helloPath);

    private static string CombineUrl(string baseUrl, string path)
    {
        if (string.IsNullOrWhiteSpace(baseUrl))
            return path ?? string.Empty;

        baseUrl = baseUrl.TrimEnd('/');
        if (string.IsNullOrWhiteSpace(path))
            return baseUrl;

        if (!path.StartsWith("/"))
            path = "/" + path;

        return baseUrl + path;
    }
}