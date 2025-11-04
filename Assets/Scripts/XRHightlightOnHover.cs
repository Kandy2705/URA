using System.Linq;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable))]
public class XRHighlightOnHover : MonoBehaviour
{
    [Header("Highlight Settings")]
    public MeshRenderer targetRenderer;          // Renderer muốn sáng (kéo thả vào)
    public Material highlightMat;                // Material sáng
    private Material originalMat;                // Lưu lại material gốc

    private void Start()
    {
        // Nếu chưa gán thủ công, tự tìm renderer trong con có tên "Sphere"
        if (targetRenderer == null)
        {
            targetRenderer = GetComponentsInChildren<MeshRenderer>()
                .FirstOrDefault(r => r.gameObject.name.Contains("Sphere"));
        }

        if (targetRenderer != null)
        {
            originalMat = targetRenderer.material;
            Debug.Log("✅ Target Renderer: " + targetRenderer.gameObject.name);
        }
        else
        {
            Debug.LogWarning("⚠️ Không tìm thấy MeshRenderer cho highlight trong " + gameObject.name);
        }

        // Đăng ký sự kiện hover
        var interactable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable>();
        interactable.hoverEntered.AddListener(OnHoverEnter);
        interactable.hoverExited.AddListener(OnHoverExit);
    }

    private void OnDestroy()
    {
        var interactable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable>();
        interactable.hoverEntered.RemoveListener(OnHoverEnter);
        interactable.hoverExited.RemoveListener(OnHoverExit);
    }

    private void OnHoverEnter(HoverEnterEventArgs args)
    {
        Debug.Log("Hover Enter: " + gameObject.name);
        ApplyHighlight(true);
    }

    private void OnHoverExit(HoverExitEventArgs args)
    {
        Debug.Log("Hover Exit: " + gameObject.name);
        ApplyHighlight(false);
    }

    private void ApplyHighlight(bool active)
    {
        if (targetRenderer == null) return;

        if (active)
        {
            // Tạo instance vật liệu mới để tránh thay đổi global
            Material matInstance = new Material(highlightMat);
            matInstance.EnableKeyword("_EMISSION");
            matInstance.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
            matInstance.SetColor("_EmissionColor", Color.yellow * 5f);

            targetRenderer.material = matInstance;  // dùng material, không sharedMaterial
            DynamicGI.SetEmissive(targetRenderer, Color.yellow * 5f);
            DynamicGI.UpdateEnvironment();
        }
        else
        {
            targetRenderer.material = originalMat;
            DynamicGI.SetEmissive(targetRenderer, Color.black);
            DynamicGI.UpdateEnvironment();
        }
    }
}
