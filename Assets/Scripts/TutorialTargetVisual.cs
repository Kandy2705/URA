using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// Owns only the temporary visuals for the exact tutorial item. It never changes a
/// shared Milk material and it creates no XR-interactable or collider components.
/// </summary>
public sealed class TutorialTargetVisual : MonoBehaviour
{
    [Header("Marker")]
    [SerializeField] private float markerVerticalOffset = 1.5f;
    [SerializeField] private float markerScale = 0.35f;
    [SerializeField] private float markerFontSize = 18f;
    [SerializeField] private string markerText = "\u25BC";

    [Header("Item Highlight")]
    [Tooltip("White HDR emission used only when the Milk material already supports enabled emission.")]
    [SerializeField, Min(0f)] private float highlightEmissionIntensity = 0.65f;
    [Tooltip("Used only when the imported Milk materials cannot safely receive an emission property block.")]
    [SerializeField, Range(0.01f, 0.35f)] private float fallbackShellAlpha = 0.12f;
    [SerializeField, Min(1f)] private float fallbackShellScale = 1.08f;

    private readonly List<RendererHighlightState> rendererStates = new();

    private Transform target;
    private Transform marker;
    private Camera playerCamera;
    private GameObject fallbackShell;
    private Material fallbackShellMaterial;
    private bool highlightActive;

    private sealed class RendererHighlightState
    {
        public Renderer Renderer;
        public MaterialPropertyBlock[] OriginalBlocks;
        public bool[] UsesEmission;
    }

    /// <summary>Supplies the scene's XR camera so the marker is billboarded correctly.</summary>
    public void SetPlayerCamera(Camera camera)
    {
        playerCamera = camera;
    }

    public void SetTarget(SelectableItem item)
    {
        RestoreHighlight();
        DestroyFallbackShell();
        target = item != null ? item.transform : null;
        CacheRendererStates();
        Debug.Log($"[TutorialTargetVisual] Target -> {(target != null ? target.name : "None")}");
    }

    public void SetVisualsActive(bool active)
    {
        if (!active)
        {
            if (marker != null)
                marker.gameObject.SetActive(false);
            RestoreHighlight();
            return;
        }

        if (target == null)
        {
            Debug.LogWarning("[TutorialTargetVisual] Cannot enable visuals because the tutorial item is missing.");
            return;
        }

        EnsureMarker();
        if (marker != null)
        {
            marker.gameObject.SetActive(true);
            UpdateMarker();
            Debug.Log("[TutorialTargetVisual] Marker activated");
        }
        else
        {
            Debug.LogError("[TutorialTargetVisual] Marker creation failed; continuing without the marker.");
        }

        ApplyHighlight();
    }

    private void LateUpdate()
    {
        if (marker != null && marker.gameObject.activeSelf)
            UpdateMarker();
    }

    private void OnDisable()
    {
        SetVisualsActive(false);
    }

    private void OnDestroy()
    {
        RestoreHighlight();
        DestroyFallbackShell();
    }

    private void EnsureMarker()
    {
        if (marker != null)
            return;

        GameObject markerObject = new("WhiteTutorialArrowMarker");
        Transform newMarker = markerObject.transform;
        newMarker.SetParent(transform, false);
        newMarker.localScale = Vector3.one * markerScale;
        markerObject.layer = target != null ? target.gameObject.layer : gameObject.layer;

        TextMeshPro text = markerObject.AddComponent<TextMeshPro>();
        if (TMP_Settings.defaultFontAsset == null)
        {
            Debug.LogError("[TutorialTargetVisual] No default TMP font asset is configured.");
            Destroy(markerObject);
            return;
        }

        text.font = TMP_Settings.defaultFontAsset;
        text.text = markerText;
        text.alignment = TextAlignmentOptions.Center;
        text.color = Color.white;
        text.fontSize = markerFontSize;
        text.enableWordWrapping = false;
        text.enabled = true;

        MeshRenderer meshRenderer = text.GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            meshRenderer.enabled = true;
            meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
            meshRenderer.receiveShadows = false;
            meshRenderer.sortingOrder = 10;
        }

        marker = newMarker;
        Debug.Log("[TutorialTargetVisual] Marker created");
    }

    private void UpdateMarker()
    {
        if (marker == null || target == null)
            return;

        marker.position = target.position + Vector3.up * markerVerticalOffset;
        if (playerCamera == null)
            playerCamera = Camera.main;
        if (playerCamera != null)
            marker.rotation = Quaternion.LookRotation(playerCamera.transform.position - marker.position, Vector3.up);
    }

    private void CacheRendererStates()
    {
        rendererStates.Clear();
        if (target == null)
            return;

        foreach (Renderer renderer in target.GetComponentsInChildren<Renderer>(true))
        {
            Material[] materials = renderer.sharedMaterials;
            MaterialPropertyBlock[] blocks = new MaterialPropertyBlock[materials.Length];
            bool[] supportsSafeEmission = new bool[materials.Length];

            for (int materialIndex = 0; materialIndex < materials.Length; materialIndex++)
            {
                blocks[materialIndex] = new MaterialPropertyBlock();
                renderer.GetPropertyBlock(blocks[materialIndex], materialIndex);

                Material material = materials[materialIndex];
                supportsSafeEmission[materialIndex] = material != null &&
                    material.HasProperty("_EmissionColor") && material.IsKeywordEnabled("_EMISSION");

                if (material != null)
                    Debug.Log($"[TutorialTargetVisual] Renderer={renderer.name}, material={material.name}, shader={material.shader.name}, safeEmission={supportsSafeEmission[materialIndex]}");
            }

            rendererStates.Add(new RendererHighlightState
            {
                Renderer = renderer,
                OriginalBlocks = blocks,
                UsesEmission = supportsSafeEmission
            });
        }
    }

    private void ApplyHighlight()
    {
        if (highlightActive)
            return;

        bool emissionApplied = false;
        Color whiteEmission = Color.white * highlightEmissionIntensity;

        foreach (RendererHighlightState state in rendererStates)
        {
            if (state.Renderer == null)
                continue;

            for (int materialIndex = 0; materialIndex < state.UsesEmission.Length; materialIndex++)
            {
                if (!state.UsesEmission[materialIndex])
                    continue;

                MaterialPropertyBlock block = new();
                state.Renderer.GetPropertyBlock(block, materialIndex);
                block.SetColor("_EmissionColor", whiteEmission);
                state.Renderer.SetPropertyBlock(block, materialIndex);
                emissionApplied = true;
            }
        }

        if (emissionApplied)
        {
            highlightActive = true;
            Debug.Log($"[TutorialTargetVisual] Safe emission highlight enabled (intensity={highlightEmissionIntensity}).");
            return;
        }

        EnsureFallbackShell();
        if (fallbackShell != null)
        {
            fallbackShell.SetActive(true);
            highlightActive = true;
            Debug.Log("[TutorialTargetVisual] Milk materials have no enabled safe emission; fallback shell enabled.");
        }
        else
        {
            Debug.LogWarning("[TutorialTargetVisual] No safe emission or fallback shell is available; marker-only guidance remains active.");
        }
    }

    private void RestoreHighlight()
    {
        foreach (RendererHighlightState state in rendererStates)
        {
            if (state.Renderer == null)
                continue;

            for (int materialIndex = 0; materialIndex < state.OriginalBlocks.Length; materialIndex++)
                state.Renderer.SetPropertyBlock(state.OriginalBlocks[materialIndex], materialIndex);
        }

        if (fallbackShell != null)
            fallbackShell.SetActive(false);
        highlightActive = false;
    }

    private void EnsureFallbackShell()
    {
        if (fallbackShell != null || target == null)
            return;

        Bounds bounds = new(target.position, Vector3.zero);
        bool hasBounds = false;
        foreach (RendererHighlightState state in rendererStates)
        {
            if (state.Renderer == null)
                continue;
            if (!hasBounds)
            {
                bounds = state.Renderer.bounds;
                hasBounds = true;
            }
            else
            {
                bounds.Encapsulate(state.Renderer.bounds);
            }
        }

        if (!hasBounds)
        {
            Debug.LogWarning("[TutorialTargetVisual] Tutorial Milk has no renderer bounds for a fallback highlight shell.");
            return;
        }

        Shader shader = Shader.Find("Standard");
        if (shader == null)
        {
            Debug.LogWarning("[TutorialTargetVisual] Standard shader is unavailable for the fallback highlight shell.");
            return;
        }

        fallbackShell = GameObject.CreatePrimitive(PrimitiveType.Cube);
        fallbackShell.name = "TutorialMilkHighlightShell";
        fallbackShell.layer = target.gameObject.layer;
        Collider shellCollider = fallbackShell.GetComponent<Collider>();
        if (shellCollider != null)
            Destroy(shellCollider);

        fallbackShell.transform.position = bounds.center;
        fallbackShell.transform.rotation = target.rotation;
        fallbackShell.transform.localScale = bounds.size * fallbackShellScale;
        fallbackShell.transform.SetParent(target, true);

        fallbackShellMaterial = new Material(shader)
        {
            name = "TutorialMilkHighlightShellMaterial"
        };
        fallbackShellMaterial.SetFloat("_Mode", 3f);
        fallbackShellMaterial.SetInt("_SrcBlend", (int)BlendMode.SrcAlpha);
        fallbackShellMaterial.SetInt("_DstBlend", (int)BlendMode.One);
        fallbackShellMaterial.SetInt("_ZWrite", 0);
        fallbackShellMaterial.EnableKeyword("_ALPHAPREMULTIPLY_ON");
        fallbackShellMaterial.renderQueue = (int)RenderQueue.Transparent;
        fallbackShellMaterial.SetColor("_Color", new Color(1f, 1f, 1f, fallbackShellAlpha));
        fallbackShellMaterial.SetColor("_EmissionColor", Color.white * highlightEmissionIntensity);
        fallbackShellMaterial.EnableKeyword("_EMISSION");

        MeshRenderer shellRenderer = fallbackShell.GetComponent<MeshRenderer>();
        shellRenderer.sharedMaterial = fallbackShellMaterial;
        shellRenderer.shadowCastingMode = ShadowCastingMode.Off;
        shellRenderer.receiveShadows = false;
        fallbackShell.SetActive(false);
    }

    private void DestroyFallbackShell()
    {
        if (fallbackShell != null)
            Destroy(fallbackShell);
        fallbackShell = null;

        if (fallbackShellMaterial != null)
            Destroy(fallbackShellMaterial);
        fallbackShellMaterial = null;
    }
}
