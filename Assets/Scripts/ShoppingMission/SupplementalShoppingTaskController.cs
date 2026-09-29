using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>Owns temporary shopping requests without changing the main shopping list.</summary>
public class SupplementalShoppingTaskController : MonoBehaviour
{
    [SerializeField] private ListController listController;
    [SerializeField] private GameTimer gameTimer;
    [SerializeField] private PokeManager pokeManager;
    [SerializeField, Min(0f)] private float minDelay = 30f;
    [SerializeField, Min(0f)] private float maxDelay = 90f;
    [SerializeField, Min(0f)] private float notificationDuration = 8f;
    [SerializeField, Min(0f)] private float cutoffSeconds = 240f;

    private readonly List<ShoppingTaskItem> supplementalTasks = new List<ShoppingTaskItem>();
    public IReadOnlyList<ShoppingTaskItem> SupplementalTasks => supplementalTasks;
    public event Action<ShoppingTaskItem> OnSupplementalTaskAdded;

    private IEnumerator Start()
    {
        // The scene must explicitly point to the UI controller, not the ListItem controller.
        if (listController == null || !listController.HasListContainer || gameTimer == null || pokeManager == null ||
            listController.NotificationCanvas == null || listController.NotificationText == null)
        {
            Debug.LogWarning("[SupplementalShoppingTaskController] Missing shopping-list, timer, inventory, or popup reference.", this);
            yield break;
        }

        ActivateShoppingListUi();
        yield return null;
        listController.EnsureMainListGenerated();

        if (!listController.MainListGenerated)
        {
            Debug.LogWarning("[SupplementalShoppingTaskController] Request skipped because the main list was not generated.", this);
            yield break;
        }

        int requestCount = UnityEngine.Random.Range(1, 3);
        for (int i = 0; i < requestCount; i++)
        {
            float delay = UnityEngine.Random.Range(Mathf.Min(minDelay, maxDelay), Mathf.Max(minDelay, maxDelay));
            yield return new WaitForSeconds(delay);

            if (!gameTimer.isRunning || gameTimer.RemainingSeconds <= cutoffSeconds)
                yield break;

            GameObject selectedPrefab = ChooseAvailablePrefab();
            if (selectedPrefab == null)
            {
                Debug.LogWarning("[SupplementalShoppingTaskController] No collectible item with available stock was found outside the main list.", this);
                yield break;
            }

            ShoppingTaskItem task = new ShoppingTaskItem(selectedPrefab.name, 1) { viewPrefab = selectedPrefab };
            supplementalTasks.Add(task);
            OnSupplementalTaskAdded?.Invoke(task);
            yield return ShowPopupOnce(task);
        }
    }

    private void ActivateShoppingListUi()
    {
        // Scene 3 disables the UI Sample prefab root; enable only the shopping UI it contains.
        for (Transform node = listController.transform.parent; node != null; node = node.parent)
        {
            if (node.name != "UI Sample")
                continue;
            node.gameObject.SetActive(true);
            break;
        }
        listController.gameObject.SetActive(true);
        if (!listController.gameObject.activeInHierarchy)
            Debug.LogWarning("[SupplementalShoppingTaskController] Shopping-list UI is still inactive; check its scene hierarchy.", this);
    }

    private IEnumerator ShowPopupOnce(ShoppingTaskItem task)
    {
        GameObject popup = listController.NotificationCanvas;
        Transform popupTransform = popup.transform;
        Vector3 originalPosition = popupTransform.position;
        Quaternion originalRotation = popupTransform.rotation;
        Vector3 originalScale = popupTransform.localScale;
        Camera camera = Camera.main;
        if (camera == null && Camera.allCamerasCount > 0)
            camera = Camera.allCameras[0];
        if (camera != null)
        {
            popupTransform.position = camera.transform.position + camera.transform.forward * 1.5f;
            // The prefab canvas has its own rotation; preserve that offset so its front faces the player.
            popupTransform.rotation = camera.transform.rotation * originalRotation;
            popupTransform.localScale = originalScale * 0.25f;
        }

        listController.NotificationText.text = $"Mua thêm 1 {task.itemName} nhé!";
        popup.SetActive(true);
        yield return new WaitForSeconds(notificationDuration);
        popup.SetActive(false);
        popupTransform.SetPositionAndRotation(originalPosition, originalRotation);
        popupTransform.localScale = originalScale;
    }

    private GameObject ChooseAvailablePrefab()
    {
        HashSet<string> mainItemNames = new HashSet<string>(StringComparer.Ordinal);
        foreach (GameObject row in listController.choicedItems)
        {
            if (row == null)
                continue;
            TMP_Text name = row.transform.Find("Name")?.GetComponent<TMP_Text>();
            if (name != null)
                mainItemNames.Add(name.text);
        }

        HashSet<string> collectibleNames = new HashSet<string>(StringComparer.Ordinal);
        foreach (SelectableItem product in FindObjectsByType<SelectableItem>(FindObjectsInactive.Exclude, FindObjectsSortMode.None))
        {
            if (product != null && product.gameObject.scene == gameObject.scene &&
                product.isActiveAndEnabled && !string.IsNullOrWhiteSpace(product.itemName))
                collectibleNames.Add(product.itemName);
        }

        List<GameObject> candidates = new List<GameObject>();
        HashSet<string> candidateNames = new HashSet<string>(StringComparer.Ordinal);
        foreach (GameObject prefab in listController.RemainingPrefabs)
        {
            if (prefab == null || !candidateNames.Add(prefab.name) || mainItemNames.Contains(prefab.name) ||
                supplementalTasks.Exists(task => task.itemName == prefab.name) ||
                pokeManager.inventory.ContainsKey(prefab.name) ||
                !collectibleNames.Contains(prefab.name) || pokeManager.GetAvailableQuantity(prefab.name) < 1)
                continue;
            candidates.Add(prefab);
        }

        return candidates.Count == 0 ? null : candidates[UnityEngine.Random.Range(0, candidates.Count)];
    }
}
