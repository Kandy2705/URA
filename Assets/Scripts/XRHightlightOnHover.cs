using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable))]
public class XRHighlightOnHover : MonoBehaviour
{
    [Header("Highlight Settings")]
    [Tooltip("Danh sách MeshRenderer muốn sáng. Nếu để trống, tự động lấy tất cả children.")]
    public List<MeshRenderer> targetRenderers = new List<MeshRenderer>();

    [Tooltip("Material có Emission (Standard Shader, bật Emission Color)")]
    public Material highlightMat;

    private List<Material> originalMats = new List<Material>();
    private List<Material> matInstances = new List<Material>();
    private Coroutine pulseRoutine;
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable interactable;

    private void Start()
    {
        if (targetRenderers == null || targetRenderers.Count == 0)
        {
            targetRenderers = GetComponentsInChildren<MeshRenderer>(true).ToList();
            Debug.Log($"[XRHighlightOnHover] Auto-detected {targetRenderers.Count} MeshRenderers in {gameObject.name}");
        }

        if (targetRenderers.Count == 0)
        {
            Debug.LogWarning($"Không tìm thấy MeshRenderer nào trong {gameObject.name}");
            return;
        }

        originalMats.Clear();
        foreach (var r in targetRenderers)
        {
            if (r != null)
                originalMats.Add(r.material);
        }

        interactable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable>();
        interactable.hoverEntered.AddListener(OnHoverEnter);
        interactable.hoverExited.AddListener(OnHoverExit);
    }

    private void OnDestroy()
    {
        if (interactable == null) return;
        interactable.hoverEntered.RemoveListener(OnHoverEnter);
        interactable.hoverExited.RemoveListener(OnHoverExit);
    }

    private void OnHoverEnter(HoverEnterEventArgs args)
    {
        StartHighlight();
    }

    private void OnHoverExit(HoverExitEventArgs args)
    {
        StopHighlight();
    }


    private void StartHighlight()
    {
        if (targetRenderers.Count == 0 || highlightMat == null) return;

        matInstances.Clear();

        foreach (var r in targetRenderers)
        {
            if (r == null) continue;

            var mat = new Material(highlightMat);
            mat.EnableKeyword("_EMISSION");
            mat.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
            mat.SetColor("_EmissionColor", Color.yellow * 3f);

            r.material = mat;
            matInstances.Add(mat);
        }

        if (pulseRoutine != null)
            StopCoroutine(pulseRoutine);

        pulseRoutine = StartCoroutine(PulseEmission());
    }

    private void StopHighlight()
    {
        if (pulseRoutine != null)
        {
            StopCoroutine(pulseRoutine);
            pulseRoutine = null;
        }

        for (int i = 0; i < targetRenderers.Count; i++)
        {
            if (targetRenderers[i] != null && i < originalMats.Count)
                targetRenderers[i].material = originalMats[i];
        }
    }

    private IEnumerator PulseEmission()
    {
        float speed = 2f;
        Color baseColor = Color.yellow * 2f;
        Color brightColor = Color.yellow * 6f;

        while (true)
        {
            float t = Mathf.PingPong(Time.time * speed, 1f);
            Color glow = Color.Lerp(baseColor, brightColor, t);

            foreach (var mat in matInstances)
            {
                if (mat != null)
                    mat.SetColor("_EmissionColor", glow);
            }

            yield return null;
        }
    }
}
