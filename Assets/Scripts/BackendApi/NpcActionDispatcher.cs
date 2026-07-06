using System;
using System.Collections.Generic;
using UnityEngine;

public class NpcActionDispatcher : MonoBehaviour
{
    [Serializable]
    public class ActionMapping
    {
        [Tooltip("Action code từ backend, ví dụ Anim_Greet")]
        public string backendAction;

        [Tooltip("Animator Trigger name — để trống nếu không dùng trigger.")]
        public string animatorTrigger;

        [Tooltip("Animator Bool name — set true rồi tự reset sau delay.")]
        public string animatorBool;

        [Tooltip("Audio clip phát khi action là Audio_* hoặc cần âm thanh kèm anim.")]
        public AudioClip audioClip;

        [Tooltip("Gọi ListController.ShowList khi action khớp.")]
        public bool showShoppingList;
    }

    [Header("Targets")]
    [SerializeField] private Animator targetAnimator;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private ListController shoppingListController;

    [Header("Mappings")]
    [SerializeField] private List<ActionMapping> actionMappings = new List<ActionMapping>
    {
        new ActionMapping { backendAction = MllmActionCodes.AnimGreet, animatorTrigger = "Greet" },
        new ActionMapping { backendAction = MllmActionCodes.AnimPointForward, animatorTrigger = "PointForward" },
        new ActionMapping { backendAction = MllmActionCodes.AnimPointUI, animatorTrigger = "PointUI", showShoppingList = true },
        new ActionMapping { backendAction = MllmActionCodes.AnimExplain, animatorTrigger = "Explain" },
        new ActionMapping { backendAction = MllmActionCodes.AnimAlert, animatorTrigger = "Alert" },
        new ActionMapping { backendAction = MllmActionCodes.AnimShakeHead, animatorTrigger = "ShakeHead" },
        new ActionMapping { backendAction = MllmActionCodes.AnimNpcOffer, animatorTrigger = "NPCOffer" },
        new ActionMapping { backendAction = MllmActionCodes.AudioPhoneCall, animatorTrigger = "PhoneCall" },
        new ActionMapping { backendAction = MllmActionCodes.AudioBroadcast, animatorTrigger = "Broadcast" }
    };

    [SerializeField] private float boolResetDelay = 1.5f;

    private readonly Dictionary<string, ActionMapping> _lookup = new Dictionary<string, ActionMapping>(StringComparer.OrdinalIgnoreCase);

    private void Awake()
    {
        RebuildLookup();
        ResolveTargets();
    }

    public void Configure(Animator animator, ListController listControllerRef, AudioSource source = null)
    {
        if (animator != null)
            targetAnimator = animator;
        if (listControllerRef != null)
            shoppingListController = listControllerRef;
        if (source != null)
            audioSource = source;

        ResolveTargets();
    }

    private void ResolveTargets()
    {
        if (targetAnimator == null)
            targetAnimator = FindActiveNpcAnimator();

        if (audioSource == null && targetAnimator != null)
            audioSource = targetAnimator.GetComponent<AudioSource>();
    }

    public void Dispatch(string actionCode)
    {
        if (string.IsNullOrWhiteSpace(actionCode))
        {
            Debug.LogWarning("[NpcActionDispatcher] action rỗng, bỏ qua.");
            return;
        }

        if (!_lookup.TryGetValue(actionCode, out ActionMapping mapping))
        {
            Debug.LogWarning($"[NpcActionDispatcher] Chưa map action '{actionCode}'. Thêm mapping trong Inspector hoặc Animator.");
            TryDispatchByConvention(actionCode);
            return;
        }

        ApplyMapping(mapping, actionCode);
    }

    private void ApplyMapping(ActionMapping mapping, string actionCode)
    {
        Debug.Log($"[NpcActionDispatcher] Dispatch '{actionCode}'");

        if (targetAnimator != null)
        {
            if (!string.IsNullOrWhiteSpace(mapping.animatorTrigger) && HasAnimatorTrigger(targetAnimator, mapping.animatorTrigger))
                targetAnimator.SetTrigger(mapping.animatorTrigger);
            else if (!string.IsNullOrWhiteSpace(mapping.animatorTrigger))
                Debug.LogWarning($"[NpcActionDispatcher] Animator không có trigger '{mapping.animatorTrigger}'.");

            if (!string.IsNullOrWhiteSpace(mapping.animatorBool) && HasAnimatorBool(targetAnimator, mapping.animatorBool))
            {
                targetAnimator.SetBool(mapping.animatorBool, true);
                StartCoroutine(ResetBoolAfterDelay(mapping.animatorBool, boolResetDelay));
            }
        }

        if (mapping.audioClip != null && audioSource != null)
            audioSource.PlayOneShot(mapping.audioClip);

        if (mapping.showShoppingList && shoppingListController != null)
            shoppingListController.ShowList();
    }

    private void TryDispatchByConvention(string actionCode)
    {
        if (targetAnimator == null)
            return;

        if (actionCode.StartsWith("Anim_", StringComparison.OrdinalIgnoreCase))
        {
            string triggerName = actionCode.Substring("Anim_".Length);
            if (HasAnimatorTrigger(targetAnimator, triggerName))
            {
                targetAnimator.SetTrigger(triggerName);
                Debug.Log($"[NpcActionDispatcher] Convention trigger '{triggerName}'");
            }
        }
        else if (actionCode.StartsWith("Audio_", StringComparison.OrdinalIgnoreCase))
        {
            Debug.Log($"[NpcActionDispatcher] Audio action '{actionCode}' — gán audioClip trong mapping nếu cần.");
        }
    }

    private System.Collections.IEnumerator ResetBoolAfterDelay(string boolName, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (targetAnimator != null && HasAnimatorBool(targetAnimator, boolName))
            targetAnimator.SetBool(boolName, false);
    }

    private void RebuildLookup()
    {
        _lookup.Clear();
        foreach (ActionMapping mapping in actionMappings)
        {
            if (mapping == null || string.IsNullOrWhiteSpace(mapping.backendAction))
                continue;

            _lookup[mapping.backendAction] = mapping;
        }
    }

    private Animator FindActiveNpcAnimator()
    {
        Animator guideNpcAnimator = NpcSceneResolver.FindNpcAnimator();
        if (guideNpcAnimator != null)
            return guideNpcAnimator;

        NPCAnimator npcAnimator = FindObjectOfType<NPCAnimator>();
        if (npcAnimator != null)
            return npcAnimator.GetComponent<Animator>();

        Animator[] animators = FindObjectsOfType<Animator>(true);
        foreach (Animator animator in animators)
        {
            if (animator != null && animator.gameObject.activeInHierarchy)
                return animator;
        }

        return null;
    }

    private static bool HasAnimatorTrigger(Animator animator, string triggerName)
    {
        foreach (AnimatorControllerParameter parameter in animator.parameters)
        {
            if (parameter.type == AnimatorControllerParameterType.Trigger && parameter.name == triggerName)
                return true;
        }

        return false;
    }

    private static bool HasAnimatorBool(Animator animator, string boolName)
    {
        foreach (AnimatorControllerParameter parameter in animator.parameters)
        {
            if (parameter.type == AnimatorControllerParameterType.Bool && parameter.name == boolName)
                return true;
        }

        return false;
    }

    private void OnValidate()
    {
        RebuildLookup();
    }
}