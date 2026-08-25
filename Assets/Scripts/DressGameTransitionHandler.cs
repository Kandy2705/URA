using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Handles transition from Dress_Game scene to Scene-level-2 when player clicks the continue/payment button.
/// Automatically hooks up button listeners upon loading Dress_Game scene.
/// </summary>
public class DressGameTransitionHandler : MonoBehaviour
{
    private const string DressSceneName = "Dress_Game";
    private const string Level2SceneName = "Scene-level-2";

    [SerializeField] private Button transitionButton;
    private static bool isTransitioning = false;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void InitSceneLoadedCallback()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == DressSceneName)
        {
            isTransitioning = false;
            BindButtonsInDressScene(scene);
        }
        else if (scene.name == Level2SceneName)
        {
            isTransitioning = false;
        }
    }

    private static void BindButtonsInDressScene(Scene dressScene)
    {
        if (!dressScene.IsValid() || !dressScene.isLoaded)
            return;

        GameObject[] rootObjects = dressScene.GetRootGameObjects();
        int boundCount = 0;

        foreach (GameObject root in rootObjects)
        {
            if (root == null) continue;

            Button[] buttons = root.GetComponentsInChildren<Button>(true);
            foreach (Button btn in buttons)
            {
                if (btn != null)
                {
                    btn.onClick.RemoveListener(HandleTransitionClicked);
                    btn.onClick.AddListener(HandleTransitionClicked);
                    boundCount++;
                    Debug.Log($"[DressGameTransitionHandler] Đã gán sự kiện chuyển màn cho Button '{btn.gameObject.name}' (Parent: '{btn.transform.parent?.name}') trong scene '{dressScene.name}'.");
                }
            }
        }

        Debug.Log($"[DressGameTransitionHandler] Tổng cộng đã gắn {boundCount} button trong scene '{dressScene.name}'.");
    }

    private void Awake()
    {
        if (transitionButton == null)
            transitionButton = GetComponent<Button>();

        if (transitionButton != null)
        {
            transitionButton.onClick.RemoveListener(HandleTransitionClicked);
            transitionButton.onClick.AddListener(HandleTransitionClicked);
        }
    }

    private void Start()
    {
        // Thử tìm và bind lại các button trong scene hiện tại nếu chưa được bind
        Scene currentScene = gameObject.scene;
        if (currentScene.IsValid() && currentScene.name == DressSceneName)
        {
            BindButtonsInDressScene(currentScene);
        }
    }

    public void TransitionToLevel2()
    {
        HandleTransitionClicked();
    }

    public static void HandleTransitionClicked()
    {
        if (isTransitioning)
        {
            Debug.Log("[DressGameTransitionHandler] Đang trong quá trình chuyển sang Level 2, bỏ qua click lặp lại.");
            return;
        }

        isTransitioning = true;
        Debug.Log("[DressGameTransitionHandler] Người chơi đã bấm nút chuyển màn! Đang chuyển từ Dress_Game sang Scene-level-2...");

        // Dừng timer hiện tại nếu có
        GameTimer timer = GameTimer.Instance != null ? GameTimer.Instance : Object.FindObjectOfType<GameTimer>();
        if (timer != null)
        {
            timer.isRunning = false;
        }

        if (!Application.CanStreamedLevelBeLoaded(Level2SceneName))
        {
            Debug.LogError($"[DressGameTransitionHandler] Lỗi: Scene '{Level2SceneName}' không có trong Build Settings!");
            isTransitioning = false;
            return;
        }

        // Chuyển sang Level 2 ở chế độ Single (giải phóng Level 1 và Dress_Game)
        SceneManager.LoadScene(Level2SceneName, LoadSceneMode.Single);
        Debug.Log($"[DressGameTransitionHandler] Đã gọi LoadScene('{Level2SceneName}', LoadSceneMode.Single) thành công.");
    }
}
