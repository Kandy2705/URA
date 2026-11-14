using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Shopping-list view. Legacy random-list APIs remain for scenes that do not use ShoppingMissionController.
/// </summary>
public class ListController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject listContainer;

    [Header("Legacy data / mission view prefabs")]
    [SerializeField] private List<GameObject> availablePrefabs = new List<GameObject>();
    [SerializeField] public List<GameObject> choicedItems = new List<GameObject>();

    [Header("Manual view limit")]
    [SerializeField] private int limit = 2;
    [SerializeField] private int currentLimit;
    [SerializeField] private bool enforceManualViewLimitForLegacyList;

    [Header("Legacy random list")]
    [SerializeField] private int spawnOnStart = 5;
    [SerializeField] private float showDuration = 10f;
    [SerializeField] private bool enableRandomTaskChange = false;
    [SerializeField] private float minDelay = 30f;
    [SerializeField] private float maxDelay = 90f;
    [Tooltip("Thời gian đệm sau khi NPC hoàn tất hướng dẫn/lời thoại trước khi cho phép đổi hàng.")]
    [SerializeField, Min(0f)] private float npcSpeechBufferSeconds = 3f;
    [SerializeField] private GameTimer randomChangeTimer;
    [SerializeField] private Level2MllmDialogueBridge randomChangeDialogueBridge;

    [Header("Notify UI")]
    [SerializeField] private GameObject notificationCanvas;
    [SerializeField] private TextMeshProUGUI notificationText;

    public GameObject NotificationCanvas => notificationCanvas;
    public TextMeshProUGUI NotificationText => notificationText;
    public bool HasListContainer => listContainer != null;
    public bool hasTriggeredRandomChange = true;
    public bool HasPendingRandomChange => hasTriggeredRandomChange;
    public event Action<string, GameObject, int> OnListChanged;
    public event Action<int> OnListShown;
    public event Action OnListHidden;
    public event Action<string, string, int, string> OnRandomListChange;
    public event Action<IReadOnlyList<ShoppingTaskItem>> OnInitialTasksRendered;
    public event Action OnMainListGenerated;
    public bool MainListGenerated { get; private set; }
    public IReadOnlyList<GameObject> RemainingPrefabs => availablePrefabs;

    private readonly List<GameObject> renderedItems = new List<GameObject>();
    private Coroutine currentRoutine;
    private int spawnCount;
    private string lastReplacedOldName;
    private string lastReplacedNewName;
    private int lastReplacedQuantity;

    private void Start()
    {
        if (HasMissionControllerInThisScene())
            return;

        EnsureMainListGenerated();
    }

    public void EnsureMainListGenerated()
    {
        if (MainListGenerated || listContainer == null)
            return;

        if (PokeManager.Instance == null)
        {
            Debug.LogWarning("[ListController] Cannot generate random tasks before inventory is available.", this);
            return;
        }

        for (int i = 0; i < spawnOnStart; i++)
            SpawnItemInList();
        MainListGenerated = true;
        OnMainListGenerated?.Invoke();
        ResetViewLimit();
        if (isActiveAndEnabled)
        {
            ShowListAutomatically();
            if (enableRandomTaskChange)
                StartCoroutine(RandomChangeCoroutine());
        }
    }

    /// <summary>Manual request; retained for NoticeBoardButtonFunction and NpcActionDispatcher.</summary>
    public void ShowList()
    {
        if (!isActiveAndEnabled || listContainer == null ||
            currentRoutine != null || (ShouldLimitManualViews() && currentLimit >= limit))
            return;

        currentLimit++;
        OnListShown?.Invoke(currentLimit);
        currentRoutine = StartCoroutine(ShowThenHide());
    }

    /// <summary>Used for the initial list. It deliberately does not consume a manual view.</summary>
    public void ShowListAutomatically()
    {
        if (listContainer == null)
            return;
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);
        currentRoutine = StartCoroutine(ShowThenHide());
    }

    // A stale scene persistent call references this method; keep it as a compatibility shim.
    public void ToggleList()
    {
        if (listContainer != null && listContainer.activeSelf)
        {
            if (currentRoutine != null)
                StopCoroutine(currentRoutine);
            listContainer.SetActive(false);
            currentRoutine = null;
            OnListHidden?.Invoke();
            return;
        }
        ShowList();
    }

    public void ResetViewLimit() => currentLimit = 0;
    public int GetClickNumber() => currentLimit;

    public void RenderTasks(IReadOnlyList<ShoppingTaskItem> tasks)
    {
        ClearRenderedItems();
        choicedItems.Clear();
        spawnCount = 0;
        if (tasks != null)
        {
            foreach (ShoppingTaskItem task in tasks)
                SpawnMissionItem(task);
        }
        OnInitialTasksRendered?.Invoke(tasks);
    }

    public void ShowNotification(string message, float duration = 3f)
    {
        if (notificationText != null)
            notificationText.text = message;
        if (notificationCanvas == null)
            return;
        notificationCanvas.SetActive(true);
        StartCoroutine(HideNotificationAfter(duration));
    }

    private bool ShouldLimitManualViews() =>
        enforceManualViewLimitForLegacyList || HasMissionControllerInThisScene();

    private bool HasMissionControllerInThisScene()
    {
        foreach (ShoppingMissionController controller in FindObjectsByType<ShoppingMissionController>(FindObjectsInactive.Exclude, FindObjectsSortMode.None))
            if (controller.gameObject.scene == gameObject.scene)
                return true;
        return false;
    }

    private IEnumerator ShowThenHide()
    {
        if (listContainer == null)
        {
            currentRoutine = null;
            yield break;
        }
        listContainer.SetActive(true);
        yield return new WaitForSeconds(showDuration);
        listContainer.SetActive(false);
        currentRoutine = null;
        OnListHidden?.Invoke();
    }

    private void SpawnMissionItem(ShoppingTaskItem task)
    {
        if (task == null)
            return;
        GameObject prefab = task.viewPrefab != null ? task.viewPrefab : FindPrefab(task.itemName);
        if (prefab == null || listContainer == null)
        {
            Debug.LogWarning($"[ListController] Missing view prefab for mission item '{task.itemName}'.");
            return;
        }
        GameObject item = Instantiate(prefab, listContainer.transform);
        ConfigureItemView(item, task.itemName, task.requiredQuantity);
    }

    private GameObject FindPrefab(string itemName)
    {
        foreach (GameObject prefab in availablePrefabs)
        {
            if (prefab != null && string.Equals(prefab.name, itemName, StringComparison.Ordinal))
                return prefab;
        }
        return null;
    }

    private void SpawnItemInList()
    {
        if (listContainer == null || !TryChooseStockedPrefab(out GameObject prefab, out int quantity))
            return;
        ConfigureItemView(Instantiate(prefab, listContainer.transform), prefab.name, quantity);
        availablePrefabs.Remove(prefab);
    }

    private bool TryChooseStockedPrefab(out GameObject prefab, out int quantity)
    {
        prefab = null;
        quantity = 0;
        if (PokeManager.Instance == null)
            return false;

        HashSet<string> listedNames = new HashSet<string>(StringComparer.Ordinal);
        foreach (GameObject row in choicedItems)
        {
            TMP_Text nameText = row != null ? row.transform.Find("Name")?.GetComponent<TMP_Text>() : null;
            if (nameText != null)
                listedNames.Add(nameText.text);
        }

        List<GameObject> candidates = new List<GameObject>();
        foreach (GameObject candidate in availablePrefabs)
        {
            if (candidate != null && !listedNames.Contains(candidate.name) &&
                PokeManager.Instance.GetAvailableQuantity(candidate.name) > 0)
                candidates.Add(candidate);
        }

        if (candidates.Count == 0)
            return false;

        prefab = candidates[UnityEngine.Random.Range(0, candidates.Count)];
        int maxQuantity = Mathf.Min(9, PokeManager.Instance.GetAvailableQuantity(prefab.name));
        quantity = UnityEngine.Random.Range(1, maxQuantity + 1);
        return true;
    }

    private void ConfigureItemView(GameObject item, string itemName, int quantity)
    {
        if (item == null)
            return;
        renderedItems.Add(item);
        choicedItems.Add(item);
        item.transform.localPosition = new Vector3(0f, 20f + spawnCount * -25f, 0f);
        item.transform.localRotation = Quaternion.identity;
        TMP_Text nameText = item.transform.Find("Name")?.GetComponent<TMP_Text>();
        TMP_Text quantityText = item.transform.Find("Quantity")?.GetComponent<TMP_Text>();
        if (nameText != null) nameText.text = itemName;
        if (quantityText != null) quantityText.text = Mathf.Max(1, quantity).ToString();
        spawnCount++;
    }

    private void ClearRenderedItems()
    {
        foreach (GameObject item in renderedItems)
        {
            if (item != null)
                Destroy(item);
        }
        renderedItems.Clear();
    }

    public string ReplaceRandomItemWithUniquePrefab()
    {
        lastReplacedQuantity = 0;
        if (choicedItems.Count == 0 || !TryChooseStockedPrefab(out GameObject prefab, out int quantity))
            return "Không có sản phẩm còn hàng phù hợp để thay đổi nhiệm vụ.";
        GameObject target = choicedItems[UnityEngine.Random.Range(0, choicedItems.Count)];
        TMP_Text oldText = target.transform.Find("Name")?.GetComponent<TMP_Text>();
        lastReplacedOldName = oldText != null ? oldText.text : target.name;
        lastReplacedNewName = prefab.name;
        lastReplacedQuantity = quantity;
        ConfigureExistingItem(target, lastReplacedNewName, lastReplacedQuantity);
        availablePrefabs.Remove(prefab);
        OnListChanged?.Invoke(lastReplacedOldName, prefab, lastReplacedQuantity);
        return $"Đã đổi {lastReplacedOldName} → {lastReplacedNewName} (x{lastReplacedQuantity})";
    }

    private static void ConfigureExistingItem(GameObject item, string name, int quantity)
    {
        TMP_Text nameText = item.transform.Find("Name")?.GetComponent<TMP_Text>();
        TMP_Text quantityText = item.transform.Find("Quantity")?.GetComponent<TMP_Text>();
        if (nameText != null) nameText.text = name;
        if (quantityText != null) quantityText.text = quantity.ToString();
    }

    private IEnumerator RandomChangeCoroutine()
    {
        // Let every scene component finish Start before checking timer/intro state.
        yield return null;
        if (randomChangeTimer == null)
            randomChangeTimer = FindSceneComponent<GameTimer>();
        if (randomChangeDialogueBridge == null)
            randomChangeDialogueBridge = FindSceneComponent<Level2MllmDialogueBridge>();
        float quietSeconds = 0f;
        float delayRemaining = 0f;
        bool delayStarted = false;

        while (hasTriggeredRandomChange)
        {
            if ((randomChangeTimer != null && !randomChangeTimer.isRunning) ||
                (PaymentManager.Instance != null && PaymentManager.Instance.IsCheckoutInProgress))
                yield break;

            bool dialogueBusy = randomChangeDialogueBridge != null && randomChangeDialogueBridge.isActiveAndEnabled &&
                randomChangeDialogueBridge.IsDialogueBusy;
            quietSeconds = dialogueBusy ? 0f : quietSeconds + Time.deltaTime;
            bool ready = !dialogueBusy && quietSeconds >= npcSpeechBufferSeconds;

            if (!delayStarted && ready)
            {
                delayRemaining = UnityEngine.Random.Range(Mathf.Min(minDelay, maxDelay), Mathf.Max(minDelay, maxDelay));
                delayStarted = true;
            }
            else if (delayStarted)
            {
                delayRemaining -= Time.deltaTime;
            }

            if (delayStarted && delayRemaining <= 0f && ready)
                break;

            yield return null;
        }

        if (!hasTriggeredRandomChange)
            yield break;
        string discountInfo = ItemsManager.DiscountRandomItem();
        string message = ReplaceRandomItemWithUniquePrefab();
        hasTriggeredRandomChange = false;
        if (lastReplacedQuantity > 0)
            OnRandomListChange?.Invoke(lastReplacedOldName, lastReplacedNewName, lastReplacedQuantity, discountInfo);
        ShowNotification(message);
    }

    private T FindSceneComponent<T>() where T : Component
    {
        foreach (T component in FindObjectsByType<T>(FindObjectsInactive.Exclude, FindObjectsSortMode.None))
            if (component.gameObject.scene == gameObject.scene)
                return component;
        return null;
    }

    private IEnumerator HideNotificationAfter(float duration)
    {
        yield return new WaitForSeconds(duration);
        if (notificationCanvas != null)
            notificationCanvas.SetActive(false);
    }
}
