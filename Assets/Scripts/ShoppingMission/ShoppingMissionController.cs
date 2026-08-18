using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// Owns the shopping tasks for one play session.  Despite its legacy name, this is not a
/// sequential mission controller: the initial list is immutable at runtime and supplemental
/// tasks are notification-only additions.
/// </summary>
public class ShoppingMissionController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ListController listController;
    [SerializeField] private GameTimer gameTimer;

    [Header("Initial tasks (NoticeBoard / NPC list)")]
    [SerializeField] private List<ShoppingTaskItem> initialTasks = new List<ShoppingTaskItem>();
    [FormerlySerializedAs("autoShowNewMission")]
    [SerializeField] private bool autoShowInitialTasks = true;

    [Header("Supplemental tasks (notification only)")]
    [SerializeField] private List<ShoppingTaskItem> supplementalTaskCandidates = new List<ShoppingTaskItem>();
    [SerializeField, Min(0)] private float supplementalMinDelay = 30f;
    [SerializeField, Min(0)] private float supplementalMaxDelay = 90f;
    [SerializeField, Min(0)] private float supplementalNotificationDuration = 5f;
    [SerializeField, Min(1)] private int maxSupplementalTasks = 2;
    [SerializeField, Min(0)] private float supplementalCutoffSeconds = 30f;

    // Kept solely so an existing scene can be opened without editing its YAML.  It is migrated
    // once into initialTasks at runtime and is never processed sequentially.
    [FormerlySerializedAs("missions")]
    [SerializeField, HideInInspector] private List<ShoppingMission> legacyMissions = new List<ShoppingMission>();

    private readonly List<ShoppingTaskItem> supplementalTasks = new List<ShoppingTaskItem>();
    private readonly HashSet<string> supplementalKeys = new HashSet<string>(StringComparer.Ordinal);
    private Coroutine supplementalRoutine;
    private bool initialized;
    private bool isShowingSupplemental;

    public IReadOnlyList<ShoppingTaskItem> InitialTasks => initialTasks;
    public IReadOnlyList<ShoppingTaskItem> SupplementalTasks => supplementalTasks;
    public event Action<IReadOnlyList<ShoppingTaskItem>> OnInitialTasksRendered;
    public event Action<ShoppingTaskItem> OnSupplementalTaskAdded;
    public event Action<ShoppingTaskItem> OnSupplementalTaskShown;

    private void Awake()
    {
        if (listController == null)
            listController = FindListController();
        if (gameTimer == null)
            gameTimer = FindFirstObjectByType<GameTimer>();
        MigrateLegacyInitialTasks();
    }

    private void Start()
    {
        InitializeSession();
    }

    private void OnDisable()
    {
        if (supplementalRoutine != null)
        {
            StopCoroutine(supplementalRoutine);
            supplementalRoutine = null;
        }
    }

    public void InitializeSession()
    {
        if (initialized)
            return;

        initialized = true;
        RenderInitialTasks();
        if (supplementalTaskCandidates.Count > 0 && supplementalRoutine == null)
            supplementalRoutine = StartCoroutine(SupplementalTaskLoop());
    }

    /// <summary>Renders only the immutable initial list. Called by NoticeBoard/NPC via ListController.ShowList.</summary>
    public void RenderInitialTasks()
    {
        if (listController == null)
            return;

        listController.RenderTasks(initialTasks);
        listController.ResetViewLimit();
        OnInitialTasksRendered?.Invoke(initialTasks);
        if (autoShowInitialTasks)
            listController.ShowListAutomatically();
    }

    public bool TryAddSupplementalTask(ShoppingTaskItem task)
    {
        if (task == null || string.IsNullOrWhiteSpace(task.itemName) ||
            isShowingSupplemental || supplementalTasks.Count >= maxSupplementalTasks ||
            RemainingTimeIsAtOrBelowCutoff())
            return false;

        string key = GetTaskKey(task);
        if (!supplementalKeys.Add(key))
            return false;

        supplementalTasks.Add(task);
        OnSupplementalTaskAdded?.Invoke(task);
        StartCoroutine(ShowSupplementalTaskOnce(task));
        return true;
    }

    private IEnumerator SupplementalTaskLoop()
    {
        while (supplementalTasks.Count < maxSupplementalTasks)
        {
            if (RemainingTimeIsAtOrBelowCutoff())
                yield break;

            float minDelay = Mathf.Min(supplementalMinDelay, supplementalMaxDelay);
            float maxDelay = Mathf.Max(supplementalMinDelay, supplementalMaxDelay);
            yield return new WaitForSeconds(UnityEngine.Random.Range(minDelay, maxDelay));

            if (RemainingTimeIsAtOrBelowCutoff())
                yield break;

            ShoppingTaskItem candidate = PickCandidate();
            if (candidate == null)
                yield break;
            TryAddSupplementalTask(candidate);
        }
    }

    private IEnumerator ShowSupplementalTaskOnce(ShoppingTaskItem task)
    {
        isShowingSupplemental = true;
        if (listController != null)
            listController.ShowNotification($"New task: {task.itemName} x{Mathf.Max(1, task.requiredQuantity)}", supplementalNotificationDuration);
        OnSupplementalTaskShown?.Invoke(task);
        yield return new WaitForSeconds(supplementalNotificationDuration);
        isShowingSupplemental = false;
    }

    private ShoppingTaskItem PickCandidate()
    {
        List<ShoppingTaskItem> eligible = new List<ShoppingTaskItem>();
        foreach (ShoppingTaskItem candidate in supplementalTaskCandidates)
        {
            if (candidate != null && !string.IsNullOrWhiteSpace(candidate.itemName) &&
                !supplementalKeys.Contains(GetTaskKey(candidate)))
                eligible.Add(candidate);
        }
        return eligible.Count == 0 ? null : eligible[UnityEngine.Random.Range(0, eligible.Count)];
    }

    private bool RemainingTimeIsAtOrBelowCutoff() =>
        gameTimer != null && gameTimer.RemainingSeconds <= supplementalCutoffSeconds;

    private void MigrateLegacyInitialTasks()
    {
        if (initialTasks.Count != 0 || legacyMissions == null)
            return;

        foreach (ShoppingMission legacyMission in legacyMissions)
        {
            if (legacyMission?.items == null)
                continue;
            initialTasks.AddRange(legacyMission.items);
        }
    }

    private static string GetTaskKey(ShoppingTaskItem task) =>
        $"{task.itemName}\u001f{Mathf.Max(1, task.requiredQuantity)}";

    private static ListController FindListController()
    {
        foreach (ListController candidate in FindObjectsByType<ListController>(FindObjectsInactive.Exclude, FindObjectsSortMode.None))
            if (candidate != null && candidate.HasListContainer)
                return candidate;
        return FindFirstObjectByType<ListController>();
    }

    // Test seam.
    public void ConfigureForTests(List<ShoppingTaskItem> initial, List<ShoppingTaskItem> supplemental = null, ListController view = null, GameTimer timer = null)
    {
        initialTasks = initial ?? new List<ShoppingTaskItem>();
        supplementalTaskCandidates = supplemental ?? new List<ShoppingTaskItem>();
        listController = view;
        gameTimer = timer;
        supplementalTasks.Clear();
        supplementalKeys.Clear();
        initialized = false;
    }
}
