#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Tạo lại Scene-Tutorial bằng các component Unity chuẩn. Chạy từ menu
/// URA > Rebuild VR Tutorial Scene khi cần cập nhật nội dung/hình minh họa.
/// </summary>
public static class TutorialSceneBuilder
{
    private const string ScenePath = "Assets/Scenes/Scene-Tutorial.unity";
    private const string TriggerImagePath = "Assets/Tutorial/ControllerGuides/vr-trigger.png";
    private const string GripImagePath = "Assets/Tutorial/ControllerGuides/vr-side-grip.png";
    private const string MoveMenuImagePath = "Assets/Tutorial/ControllerGuides/vr-thumbstick-quick-menu.png";
    private const string FontPath = "Assets/TextMesh Pro/Resources/Fonts & Materials/LiberationSans SDF.asset";

    private static readonly Color BackgroundColor = new Color32(13, 24, 38, 255);
    private static readonly Color CardColor = new Color32(25, 43, 62, 252);
    private static readonly Color Cyan = new Color32(68, 210, 220, 255);
    private static readonly Color Yellow = new Color32(255, 198, 62, 255);
    private static readonly Color PrimaryButton = new Color32(23, 132, 151, 255);
    private static readonly Color SecondaryButton = new Color32(52, 70, 89, 255);

    private sealed class PageContent
    {
        public string ObjectName;
        public string Step;
        public string Title;
        public string Body;
        public string Tip;
        public Sprite Illustration;
    }

    [MenuItem("URA/Rebuild VR Tutorial Scene")]
    public static void RebuildFromMenu()
    {
        Rebuild();
        Selection.activeObject = AssetDatabase.LoadAssetAtPath<SceneAsset>(ScenePath);
        EditorGUIUtility.PingObject(Selection.activeObject);
    }

    /// <summary>Entry point dành cho Unity batch mode.</summary>
    public static void RebuildForBatchMode()
    {
        Rebuild();
    }

    /// <summary>Render trang đầu ra PNG để kiểm tra bố cục tự động.</summary>
    public static void CapturePreviewForBatchMode()
    {
        Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        foreach (GameObject page in Resources.FindObjectsOfTypeAll<GameObject>()
                     .Where(item => item.scene == scene && item.name.StartsWith("Page_")))
            page.SetActive(page.name == "Page_01_Move");

        Canvas.ForceUpdateCanvases();
        Camera camera = Camera.main;
        if (camera == null)
            throw new System.InvalidOperationException("Scene-Tutorial không có Main Camera.");

        const int width = 1920;
        const int height = 1080;
        RenderTexture renderTexture = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);
        Texture2D preview = new Texture2D(width, height, TextureFormat.RGBA32, false);
        camera.targetTexture = renderTexture;
        camera.Render();
        RenderTexture.active = renderTexture;
        preview.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        preview.Apply();
        File.WriteAllBytes("/tmp/ura-tutorial-preview.png", preview.EncodeToPNG());

        camera.targetTexture = null;
        RenderTexture.active = null;
        Object.DestroyImmediate(preview);
        renderTexture.Release();
        Object.DestroyImmediate(renderTexture);
        Debug.Log("[TutorialSceneBuilder] Preview: /tmp/ura-tutorial-preview.png");
    }

    private static void Rebuild()
    {
        AssetDatabase.Refresh();
        ConfigureTutorialTexture(TriggerImagePath);
        ConfigureTutorialTexture(GripImagePath);
        ConfigureTutorialTexture(MoveMenuImagePath);

        Sprite triggerSprite = AssetDatabase.LoadAssetAtPath<Sprite>(TriggerImagePath);
        Sprite gripSprite = AssetDatabase.LoadAssetAtPath<Sprite>(GripImagePath);
        Sprite moveMenuSprite = AssetDatabase.LoadAssetAtPath<Sprite>(MoveMenuImagePath);
        TMP_FontAsset font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FontPath);

        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        Camera tutorialCamera = CreateCamera();
        CreateEventSystem();

        Canvas canvas = CreateCanvas(tutorialCamera);
        CreateImage("Background", canvas.transform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, BackgroundColor);

        GameObject controllerObject = new GameObject("TutorialController", typeof(RectTransform));
        controllerObject.transform.SetParent(canvas.transform, false);
        TutorialSceneController controller = controllerObject.AddComponent<TutorialSceneController>();

        List<PageContent> contents = new List<PageContent>
        {
            new PageContent
            {
                ObjectName = "Page_01_Move",
                Step = "BƯỚC 1 / 5",
                Title = "Di chuyển bằng tay cầm",
                Body = "1. Dùng tay cầm BÊN TRÁI.\n\n2. Đặt ngón cái lên cần tròn.\n\n3. Đẩy nhẹ về hướng muốn đi.\n\n4. Thả ngón tay để dừng lại.",
                Tip = "Đi chậm từng chút. Bạn không cần vung tay hoặc bước thật.",
                Illustration = moveMenuSprite
            },
            new PageContent
            {
                ObjectName = "Page_02_Trigger",
                Step = "BƯỚC 2 / 5",
                Title = "Cò trước: chỉ và chọn",
                Body = "1. Duỗi tay cầm về phía nút hoặc đồ vật.\n\n2. Nhìn tia chỉ đến đúng mục cần chọn.\n\n3. Dùng NGÓN TRỎ bóp nhẹ cò phía trước.\n\n4. Thả cò sau khi chọn xong.",
                Tip = "Hãy bóp cò ngay bây giờ để sang bước kế tiếp.",
                Illustration = triggerSprite
            },
            new PageContent
            {
                ObjectName = "Page_03_Grip",
                Step = "BƯỚC 3 / 5",
                Title = "Nút bóp bên hông",
                Body = "1. Nút này nằm ở cạnh tay cầm.\n\n2. Dùng NGÓN GIỮA bóp và GIỮ nút.\n\n3. Nút dùng để cầm/nắm vật thể khi trò chơi cho phép.\n\n4. Thả nút để buông vật thể.",
                Tip = "Bóp vừa đủ, không cần siết tay cầm quá mạnh.",
                Illustration = gripSprite
            },
            new PageContent
            {
                ObjectName = "Page_04_QuickMenu",
                Step = "BƯỚC 4 / 5",
                Title = "Mở thanh công cụ nhanh",
                Body = "1. Đưa tay cầm lên trước mặt.\n\n2. Bóp và GIỮ nút bên hông để mở thanh công cụ.\n\n3. Vẫn giữ nút, hướng tia đến chức năng cần dùng.\n\n4. Bóp cò để chọn. Thả nút bên hông để đóng.",
                Tip = "Trong game, thanh công cụ chỉ hiện khi bạn đang giữ nút.",
                Illustration = moveMenuSprite
            },
            new PageContent
            {
                ObjectName = "Page_05_ListAndItem",
                Step = "BƯỚC 5 / 5",
                Title = "Xem danh sách và lấy sản phẩm",
                Body = "1. Mở thanh công cụ nhanh.\n\n2. Chọn DANH SÁCH bằng cò trước. Danh sách hiện khoảng 10 giây.\n\n3. Đến gần đúng sản phẩm trên kệ.\n\n4. Đưa đầu tay cầm chạm vào sản phẩm để thêm vào giỏ.",
                Tip = "Thấy bảng thông báo đã lấy nghĩa là sản phẩm đã vào giỏ.",
                Illustration = triggerSprite
            }
        };

        List<Button> practiceButtons = new List<Button>();
        List<GameObject> pages = contents.Select(content =>
        {
            Button practiceButton;
            GameObject page = CreatePage(canvas.transform, content, font, out practiceButton);
            practiceButtons.Add(practiceButton);
            return page;
        }).ToList();
        GameObject footer = CreateFooter(canvas.transform, font, out Button previous, out Button next,
            out Button start, out TMP_Text indicator, out Image progress);
        Button skip = CreateButton("BtnSkip", canvas.transform, "Bỏ qua hướng dẫn", font,
            new Vector2(0.82f, 0.91f), new Vector2(0.96f, 0.975f), SecondaryButton, 30);
        GameObject practiceOverlay = CreatePracticeOverlay(canvas.transform, font, out TMP_Text practiceAction,
            out TMP_Text practiceStatus, out TMP_Text practiceCountdown, out Button practiceClose);

        SerializedObject serializedController = new SerializedObject(controller);
        serializedController.FindProperty("nextSceneName").stringValue = "Scene-level-1";
        SerializedProperty pageProperty = serializedController.FindProperty("tutorialPages");
        pageProperty.arraySize = pages.Count;
        for (int i = 0; i < pages.Count; i++)
            pageProperty.GetArrayElementAtIndex(i).objectReferenceValue = pages[i];
        serializedController.FindProperty("btnNext").objectReferenceValue = next;
        serializedController.FindProperty("btnPrev").objectReferenceValue = previous;
        serializedController.FindProperty("btnSkip").objectReferenceValue = skip;
        serializedController.FindProperty("btnStart").objectReferenceValue = start;
        serializedController.FindProperty("txtPageIndicator").objectReferenceValue = indicator;
        serializedController.FindProperty("imgProgressBar").objectReferenceValue = progress;
        SerializedProperty practiceButtonProperty = serializedController.FindProperty("practiceButtons");
        practiceButtonProperty.arraySize = practiceButtons.Count;
        for (int i = 0; i < practiceButtons.Count; i++)
            practiceButtonProperty.GetArrayElementAtIndex(i).objectReferenceValue = practiceButtons[i];
        serializedController.FindProperty("practiceOverlay").objectReferenceValue = practiceOverlay;
        serializedController.FindProperty("txtPracticeAction").objectReferenceValue = practiceAction;
        serializedController.FindProperty("txtPracticeStatus").objectReferenceValue = practiceStatus;
        serializedController.FindProperty("txtPracticeCountdown").objectReferenceValue = practiceCountdown;
        serializedController.FindProperty("btnPracticeClose").objectReferenceValue = practiceClose;
        SerializedProperty actionProperty = serializedController.FindProperty("practiceActionPerPage");
        actionProperty.arraySize = contents.Count;
        for (int i = 0; i < contents.Count; i++)
            actionProperty.GetArrayElementAtIndex(i).enumValueIndex = i switch
            {
                0 => (int)TutorialSceneController.PracticeAction.Thumbstick,
                1 => (int)TutorialSceneController.PracticeAction.Trigger,
                2 => (int)TutorialSceneController.PracticeAction.Grip,
                3 => (int)TutorialSceneController.PracticeAction.QuickMenu,
                _ => (int)TutorialSceneController.PracticeAction.Trigger
            };
        serializedController.FindProperty("fadeDuration").floatValue = 0.2f;
        serializedController.FindProperty("triggerToAdvance").boolValue = true;
        serializedController.FindProperty("triggerPressThreshold").floatValue = 0.65f;
        serializedController.ApplyModifiedPropertiesWithoutUndo();

        footer.transform.SetAsLastSibling();
        skip.transform.SetAsLastSibling();
        EditorSceneManager.SaveScene(scene, ScenePath);
        AddTutorialToBuildSettings();
        AssetDatabase.SaveAssets();
        Debug.Log("[TutorialSceneBuilder] Đã tạo lại Scene-Tutorial và thêm scene vào đầu Build Settings.");
    }

    private static void ConfigureTutorialTexture(string path)
    {
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer == null)
            return;

        bool changed = importer.textureType != TextureImporterType.Sprite ||
                       importer.spriteImportMode != SpriteImportMode.Single ||
                       !importer.alphaIsTransparency || importer.maxTextureSize != 2048;
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Single;
        importer.alphaIsTransparency = true;
        importer.mipmapEnabled = false;
        importer.maxTextureSize = 2048;
        importer.textureCompression = TextureImporterCompression.CompressedHQ;
        if (changed)
            importer.SaveAndReimport();
    }

    private static Camera CreateCamera()
    {
        GameObject cameraObject = new GameObject("Main Camera", typeof(Camera), typeof(AudioListener));
        cameraObject.tag = "MainCamera";
        Camera camera = cameraObject.GetComponent<Camera>();
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = BackgroundColor;
        cameraObject.transform.position = Vector3.zero;
        return camera;
    }

    private static void CreateEventSystem()
    {
        GameObject eventSystem = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
        eventSystem.GetComponent<EventSystem>().sendNavigationEvents = true;
    }

    private static Canvas CreateCanvas(Camera tutorialCamera)
    {
        GameObject canvasObject = new GameObject("Canvas", typeof(RectTransform), typeof(Canvas),
            typeof(CanvasScaler), typeof(GraphicRaycaster));
        Canvas canvas = canvasObject.GetComponent<Canvas>();
        // World Space là chế độ hiển thị ổn định cho cả hai mắt trong kính VR.
        // Canvas được gắn vào camera để bảng hướng dẫn luôn nằm trước mặt người dùng.
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.worldCamera = tutorialCamera;
        RectTransform canvasRect = canvasObject.GetComponent<RectTransform>();
        canvasRect.SetParent(tutorialCamera.transform, false);
        canvasRect.sizeDelta = new Vector2(1920f, 1080f);
        canvasRect.localPosition = new Vector3(0f, 0f, 2f);
        canvasRect.localRotation = Quaternion.identity;
        canvasRect.localScale = Vector3.one * 0.001f;
        CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;
        return canvas;
    }

    private static GameObject CreatePage(Transform parent, PageContent content, TMP_FontAsset font,
        out Button practiceButton)
    {
        GameObject page = CreateImage(content.ObjectName, parent, new Vector2(0.055f, 0.13f),
            new Vector2(0.945f, 0.91f), Vector2.zero, Vector2.zero, CardColor);
        page.AddComponent<CanvasGroup>();

        TMP_Text step = CreateText("TxtStep", page.transform, content.Step, font, 28, FontStyles.Bold,
            Cyan, TextAlignmentOptions.Left, new Vector2(0.045f, 0.88f), new Vector2(0.35f, 0.965f));
        step.characterSpacing = 4f;

        CreateText("TxtTitle", page.transform, content.Title, font, 58, FontStyles.Bold,
            Color.white, TextAlignmentOptions.Left, new Vector2(0.045f, 0.75f), new Vector2(0.95f, 0.89f));

        GameObject imageFrame = CreateImage("IllustrationFrame", page.transform, new Vector2(0.045f, 0.19f),
            new Vector2(0.47f, 0.73f), Vector2.zero, Vector2.zero, new Color32(10, 20, 31, 255));
        Image illustration = CreateImage("Illustration", imageFrame.transform, new Vector2(0.035f, 0.035f),
            new Vector2(0.965f, 0.965f), Vector2.zero, Vector2.zero, Color.white).GetComponent<Image>();
        illustration.sprite = content.Illustration;
        illustration.preserveAspect = true;
        illustration.raycastTarget = false;

        TMP_Text body = CreateText("TxtBody", page.transform, content.Body, font, 38, FontStyles.Normal,
            Color.white, TextAlignmentOptions.TopLeft, new Vector2(0.52f, 0.28f), new Vector2(0.95f, 0.73f));
        body.lineSpacing = 5f;
        body.enableAutoSizing = true;
        body.fontSizeMin = 29f;
        body.fontSizeMax = 38f;

        GameObject tipPanel = CreateImage("TipPanel", page.transform, new Vector2(0.52f, 0.12f),
            new Vector2(0.95f, 0.255f), Vector2.zero, Vector2.zero, new Color32(64, 53, 23, 255));
        TMP_Text tip = CreateText("TxtTip", tipPanel.transform, "GỢI Ý:  " + content.Tip, font, 29,
            FontStyles.Bold, new Color32(255, 226, 139, 255), TextAlignmentOptions.MidlineLeft,
            new Vector2(0.04f, 0.12f), new Vector2(0.96f, 0.88f));
        tip.enableAutoSizing = true;
        tip.fontSizeMin = 23f;
        tip.fontSizeMax = 29f;

        practiceButton = CreateButton("BtnPractice", page.transform, "THỬ NGAY", font,
            new Vector2(0.78f, 0.035f), new Vector2(0.95f, 0.105f), PrimaryButton, 25);

        page.SetActive(false);
        return page;
    }

    private static GameObject CreatePracticeOverlay(Transform parent, TMP_FontAsset font,
        out TMP_Text action, out TMP_Text status, out TMP_Text countdown, out Button close)
    {
        GameObject overlay = CreateImage("PracticeOverlay", parent, Vector2.zero, Vector2.one,
            Vector2.zero, Vector2.zero, new Color(0.01f, 0.03f, 0.06f, 0.78f));
        overlay.GetComponent<Image>().raycastTarget = true;

        GameObject card = CreateImage("PracticeCard", overlay.transform, new Vector2(0.61f, 0.14f),
            new Vector2(0.95f, 0.86f), Vector2.zero, Vector2.zero, new Color32(25, 51, 67, 255));
        CreateText("TxtPracticeHeading", card.transform, "THỬ NGAY", font, 46, FontStyles.Bold,
            Yellow, TextAlignmentOptions.Center, new Vector2(0.06f, 0.79f), new Vector2(0.94f, 0.95f));
        action = CreateText("TxtPracticeAction", card.transform, "", font, 34, FontStyles.Bold,
            Color.white, TextAlignmentOptions.Center, new Vector2(0.08f, 0.45f), new Vector2(0.92f, 0.77f));
        action.enableAutoSizing = true;
        action.fontSizeMin = 26f;
        action.fontSizeMax = 34f;
        status = CreateText("TxtPracticeStatus", card.transform, "", font, 27, FontStyles.Normal,
            new Color32(189, 222, 231, 255), TextAlignmentOptions.Center,
            new Vector2(0.08f, 0.29f), new Vector2(0.92f, 0.44f));
        countdown = CreateText("TxtPracticeCountdown", card.transform, "", font, 28, FontStyles.Bold,
            Yellow, TextAlignmentOptions.Center, new Vector2(0.08f, 0.18f), new Vector2(0.92f, 0.3f));
        close = CreateButton("BtnPracticeClose", card.transform, "ĐÓNG", font,
            new Vector2(0.33f, 0.035f), new Vector2(0.67f, 0.16f), SecondaryButton, 25);

        overlay.SetActive(false);
        return overlay;
    }

    private static GameObject CreateFooter(Transform parent, TMP_FontAsset font, out Button previous,
        out Button next, out Button start, out TMP_Text indicator, out Image progress)
    {
        GameObject footer = CreateImage("Footer", parent, new Vector2(0f, 0f), new Vector2(1f, 0.125f),
            Vector2.zero, Vector2.zero, new Color32(9, 18, 29, 255));

        previous = CreateButton("BtnPrev", footer.transform, "TRƯỚC", font,
            new Vector2(0.045f, 0.2f), new Vector2(0.18f, 0.82f), SecondaryButton, 30);
        next = CreateButton("BtnNext", footer.transform, "TIẾP", font,
            new Vector2(0.82f, 0.2f), new Vector2(0.955f, 0.82f), PrimaryButton, 30);
        start = CreateButton("BtnStart", footer.transform, "BẮT ĐẦU", font,
            new Vector2(0.79f, 0.2f), new Vector2(0.955f, 0.82f), new Color32(32, 143, 92, 255), 30);

        indicator = CreateText("TxtPageIndicator", footer.transform, "1 / 5", font, 28,
            FontStyles.Bold, Color.white, TextAlignmentOptions.Center,
            new Vector2(0.45f, 0.54f), new Vector2(0.55f, 0.92f));

        CreateText("TxtTriggerHint", footer.transform, "BÓP CÒ để sang bước tiếp", font, 25,
            FontStyles.Normal, new Color32(187, 207, 219, 255), TextAlignmentOptions.Center,
            new Vector2(0.31f, 0.1f), new Vector2(0.69f, 0.47f));

        GameObject progressBackground = CreateImage("ProgressBackground", footer.transform,
            new Vector2(0.225f, 0.58f), new Vector2(0.42f, 0.76f), Vector2.zero, Vector2.zero,
            new Color32(39, 57, 73, 255));
        progress = CreateImage("ProgressFill", progressBackground.transform, Vector2.zero, Vector2.one,
            Vector2.zero, Vector2.zero, Cyan).GetComponent<Image>();
        progress.type = Image.Type.Filled;
        progress.fillMethod = Image.FillMethod.Horizontal;
        progress.fillOrigin = 0;
        progress.fillAmount = 0f;
        progress.raycastTarget = false;

        return footer;
    }

    private static Button CreateButton(string name, Transform parent, string label, TMP_FontAsset font,
        Vector2 anchorMin, Vector2 anchorMax, Color color, int fontSize)
    {
        GameObject buttonObject = CreateImage(name, parent, anchorMin, anchorMax, Vector2.zero, Vector2.zero, color);
        Image image = buttonObject.GetComponent<Image>();
        image.raycastTarget = true;
        Button button = buttonObject.AddComponent<Button>();
        button.targetGraphic = image;
        button.transition = Selectable.Transition.ColorTint;
        Navigation navigation = button.navigation;
        navigation.mode = Navigation.Mode.None;
        button.navigation = navigation;

        CreateText("Text", buttonObject.transform, label, font, fontSize, FontStyles.Bold, Color.white,
            TextAlignmentOptions.Center, new Vector2(0.04f, 0.06f), new Vector2(0.96f, 0.94f));
        return button;
    }

    private static GameObject CreateImage(string name, Transform parent, Vector2 anchorMin,
        Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax, Color color)
    {
        GameObject gameObject = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        gameObject.layer = LayerMask.NameToLayer("UI");
        RectTransform rect = gameObject.GetComponent<RectTransform>();
        rect.SetParent(parent, false);
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.offsetMin = offsetMin;
        rect.offsetMax = offsetMax;
        Image image = gameObject.GetComponent<Image>();
        image.color = color;
        image.raycastTarget = false;
        return gameObject;
    }

    private static TMP_Text CreateText(string name, Transform parent, string value, TMP_FontAsset font,
        float fontSize, FontStyles fontStyle, Color color, TextAlignmentOptions alignment,
        Vector2 anchorMin, Vector2 anchorMax)
    {
        GameObject textObject = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer),
            typeof(TextMeshProUGUI));
        textObject.layer = LayerMask.NameToLayer("UI");
        RectTransform rect = textObject.GetComponent<RectTransform>();
        rect.SetParent(parent, false);
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        TextMeshProUGUI text = textObject.GetComponent<TextMeshProUGUI>();
        if (font != null)
            text.font = font;
        text.text = value;
        text.fontSize = fontSize;
        text.fontStyle = fontStyle;
        text.color = color;
        text.alignment = alignment;
        text.textWrappingMode = TextWrappingModes.Normal;
        text.overflowMode = TextOverflowModes.Overflow;
        text.raycastTarget = false;
        return text;
    }

    private static void AddTutorialToBuildSettings()
    {
        List<EditorBuildSettingsScene> scenes = EditorBuildSettings.scenes
            .Where(item => item.path != ScenePath)
            .ToList();
        scenes.Insert(0, new EditorBuildSettingsScene(ScenePath, true));
        EditorBuildSettings.scenes = scenes.ToArray();
    }
}
#endif
