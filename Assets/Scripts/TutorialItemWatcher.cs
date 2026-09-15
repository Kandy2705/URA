using UnityEngine;

/// <summary>
/// Cầu nối giữa sự kiện chọn item và QuestManager trong tutorial.
/// Gọi CheckItem từ hệ thống tương tác khi người chơi bấm/chạm một item.
/// </summary>
public class TutorialItemWatcher : MonoBehaviour
{
    public static TutorialItemWatcher Instance { get; private set; }

    [SerializeField] private SelectableItem targetSampleItem;
    [SerializeField] private QuestManager questManager;

    private bool isCompleted;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    public void CheckItem(SelectableItem item)
    {
        if (isCompleted || item == null)
            return;

        if (item == targetSampleItem)
        {
            isCompleted = true;

            if (questManager != null)
                questManager.OnSampleItemGrabbed();
        }
        else
        {
            if (questManager != null)
                questManager.OnWrongItemTouched();
        }
    }
}
