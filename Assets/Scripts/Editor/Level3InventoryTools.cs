using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

/// <summary>Author and validate the small Level 3 inventory using connected existing prefabs.</summary>
[InitializeOnLoad]
public static class Level3InventoryTools
{
    const string ScenePath = "Assets/Scenes/Scene-level-3.unity";
    const string Request = "Library/Level3Inventory.request";
    const string Result = "Library/Level3Inventory-result.txt";
    const string Group = "ShoppingInventory";
    static double nextPoll;
    static readonly string[] Paths = {
        "Assets/Prefabs/Carrot.prefab",
        "Assets/Prefabs/tomate.prefab",
        "Assets/Prefabs/Banana.prefab",
        "Assets/Prefabs/Cute Supermarket Lite/Prefabs/apple_green.prefab",
        "Assets/Prefabs/Cute Supermarket Lite/Prefabs/bottle1_water.prefab",
        "Assets/Prefabs/sok.prefab",
        "Assets/Prefabs/Cute Supermarket Lite/Prefabs/can_food_square_red.prefab",
        "Assets/Prefabs/Milk.prefab",
        "Assets/Prefabs/fish 1.prefab",
        "Assets/Prefabs/Cute Supermarket Lite/Prefabs/roll_toilet.prefab"
    };
    static readonly string[] Categories = {
        "Vegetables", "Vegetables", "Fruit", "Fruit", "Beverages",
        "Beverages", "Packaged food", "Chilled food", "Fresh food", "Household"
    };
    static Level3InventoryTools()
    {
        EditorApplication.update += Poll;
        EditorApplication.playModeStateChanged += PlayChanged;
    }
    static void Poll()
    {
        if (EditorApplication.timeSinceStartup < nextPoll || EditorApplication.isCompiling || EditorApplication.isUpdating) return;
        nextPoll = EditorApplication.timeSinceStartup + 1;
        if (!File.Exists(Request)) return;
        string action = File.ReadAllText(Request).Trim();
        File.Delete(Request);
        try
        {
            if (action == "install") Install();
            else if (action == "validate") Validate();
            else if (action == "snapshot")
            {
                var scene = Level3();
                EditorSceneManager.SaveScene(scene, "Temp/Level3Inventory-current.unity", true);
                File.WriteAllText(Result, "Snapshot saved; dirty=" + scene.isDirty);
            }
            else if (action == "save")
            {
                Require(EditorSceneManager.SaveScene(Level3()), "Save failed");
                Validate();
            }
            else if (action == "regression") RunRegression();
            else if (action == "integrateui") IntegrateUi();
            else if (action == "preview") Preview();
            else if (action == "relocate") { PlaceOnDisplays(Level3()); EditorSceneManager.SaveScene(Level3()); Validate(); }
            else if (action == "playtest" || action == "playtesttimeout")
            {
                Require(!EditorApplication.isPlayingOrWillChangePlaymode, "Already in Play Mode.");
                Require(SceneManager.sceneCount == 1 && SceneManager.GetActiveScene().path == ScenePath,
                    "Runtime check requires Level 3 loaded independently.");
                // Play Mode preserves unsaved editor state; this check never saves the scene.
                File.WriteAllText("Library/Level3Inventory-playcheck.txt", "");
                SessionState.SetBool("Level3Checkout.TimeoutTest", action == "playtesttimeout");
                SessionState.SetBool("Level3Inventory.Playcheck", true);
                EditorApplication.isPlaying = true;
            }
        }
        catch (Exception e) { File.WriteAllText(Result, e.ToString()); Debug.LogException(e); }
    }
    static Scene Level3()
    {
        var scene = SceneManager.GetSceneByPath(ScenePath);
        Require(scene.isLoaded, "Open Scene-level-3 before using this tool.");
        Require(!EditorApplication.isPlayingOrWillChangePlaymode, "Stop Play Mode first.");
        return scene;
    }
    static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }
    static string Hierarchy(Transform t) => t.parent == null ? t.name : Hierarchy(t.parent) + "/" + t.name;
    static SelectableItem[] Products(Scene scene) => scene.GetRootGameObjects()
        .SelectMany(r => r.GetComponentsInChildren<SelectableItem>(true)).ToArray();
    static Bounds BoundsOf(GameObject go)
    {
        var renderers = go.GetComponentsInChildren<Renderer>().Where(r => r.enabled).ToArray();
        Require(renderers.Length > 0, "Product has no renderer: " + go.name);
        Bounds bounds = renderers[0].bounds;
        foreach (var renderer in renderers.Skip(1)) bounds.Encapsulate(renderer.bounds);
        return bounds;
    }
    [MenuItem("Tools/Level 3/Install initial shopping inventory")]
    public static void Install()
    {
        var scene = Level3();
        Require(!scene.isDirty, "Scene has unsaved edits. Save/review them first.");
        var shop = scene.GetRootGameObjects().Single(g => g.name == "supermarket");
        var old = scene.GetRootGameObjects().Single(g => g.name == "Supermarket");
        Require(shop.activeInHierarchy && !old.activeSelf, "Unexpected shop activity.");
        Require(shop.transform.Find(Group) == null, "Inventory already exists; use Validate. Existing placements will not be overwritten.");
        // Resolve and check every source before touching the scene.
        var prefabs = Paths.Select(p => AssetDatabase.LoadAssetAtPath<GameObject>(p)).ToArray();
        var list = scene.GetRootGameObjects().SelectMany(r => r.GetComponentsInChildren<ListController>(true))
            .Single(c => c.HasListContainer);
        var listData = new SerializedObject(list);
        var pool = listData.FindProperty("availablePrefabs");
        var views = Enumerable.Range(0, pool.arraySize)
            .Select(i => pool.GetArrayElementAtIndex(i).objectReferenceValue as GameObject).ToArray();
        var selectedViews = new GameObject[prefabs.Length];
        for (int i = 0; i < prefabs.Length; i++)
        {
            Require(prefabs[i] != null, "Missing prefab " + Paths[i]);
            var items = prefabs[i].GetComponentsInChildren<SelectableItem>(true);
            Require(items.Length == 1, "Expected one product identity in " + Paths[i]);
            selectedViews[i] = views.SingleOrDefault(v => v != null && v.name == items[0].itemName);
            Require(selectedViews[i] != null, "No matching list view: " + items[0].itemName);
            Require(prefabs[i].GetComponentInChildren<PokeObjects>(true) != null, "Missing PokeObjects");
            Require(prefabs[i].GetComponentInChildren<XRBaseInteractable>(true) != null, "Missing XR interactable");
        }
        File.Copy(ScenePath, "Library/Level3Inventory-before.unity", true);
        int undoGroup = Undo.GetCurrentGroup();
        Undo.SetCurrentGroupName("Integrate Level 3 shopping inventory");
        try
        {
            var container = new GameObject(Group);
            SceneManager.MoveGameObjectToScene(container, scene);
            Undo.RegisterCreatedObjectUndo(container, "Create inventory");
            container.transform.SetParent(shop.transform, false);
            for (int i = 0; i < prefabs.Length; i++)
            {
                var product = (GameObject)PrefabUtility.InstantiatePrefab(prefabs[i], scene);
                Undo.RegisterCreatedObjectUndo(product, "Place product");
                product.transform.SetParent(container.transform, false);
                product.name = Categories[i] + " - " + prefabs[i].name;
                product.SetActive(true);
                product.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
                // Scale each existing asset uniformly by rendered size, preserving its collider/mesh relationship.
                var bounds = BoundsOf(product);
                product.transform.localScale *= 0.20f / Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z);
                bounds = BoundsOf(product);
                var item = product.GetComponentInChildren<SelectableItem>();
                item.stockQuantity = 12;
                // Several source products carry a checkout trigger script. It is irrelevant on shelf goods.
                foreach (var counter in product.GetComponentsInChildren<CheckoutCounter>(true))
                    Undo.DestroyObjectImmediate(counter);
                PrefabUtility.RecordPrefabInstancePropertyModifications(product.transform);
                PrefabUtility.RecordPrefabInstancePropertyModifications(product);
                PrefabUtility.RecordPrefabInstancePropertyModifications(item);
            }
            PlaceOnDisplays(scene);
            Undo.RecordObject(list, "Limit Level 3 list to integrated catalog");
            pool.arraySize = selectedViews.Length;
            for (int i = 0; i < selectedViews.Length; i++)
                pool.GetArrayElementAtIndex(i).objectReferenceValue = selectedViews[i];
            listData.FindProperty("limitLegacyListToAvailableStock").boolValue = true;
            listData.ApplyModifiedProperties();
            EditorSceneManager.MarkSceneDirty(scene);
            IntegrateUi();
            Validate();
            Require(EditorSceneManager.SaveScene(scene), "Scene save failed");
            Undo.CollapseUndoOperations(undoGroup);
        }
        catch
        {
            Undo.RevertAllDownToGroup(undoGroup);
            throw;
        }
    }
    [MenuItem("Tools/Level 3/Validate shopping inventory")]
    public static void Validate()
    {
        var scene = Level3();
        var old = scene.GetRootGameObjects().Single(g => g.name == "Supermarket");
        Require(!old.activeSelf, "Old supermarket must remain inactive");
        var shop = scene.GetRootGameObjects().Single(g => g.name == "supermarket");
        var group = shop.transform.Find(Group);
        Require(group != null, "Missing inventory container");
        if (Level3VisualRevisionTools.IsRevised(group)) { File.WriteAllText(Result, Level3VisualRevisionTools.Validate(scene)); return; }
        var products = group.GetComponentsInChildren<SelectableItem>(true);
        int expected = group.Find("S01_Vegetables") != null ? 14 : 10;
        Require(products.Length == expected && products.Select(p => p.itemName).Distinct().Count() == expected,
            "Unexpected distinct product count");
        Require(Products(scene).Count(p => p.isActiveAndEnabled) == expected, "Unexpected additional active products");
        var report = new StringBuilder("STATIC VALIDATION PASS\n");
        foreach (var item in products)
        {
            Require(item.isActiveAndEnabled && item.stockQuantity == 12, "Inactive item or unexpected stock");
            var xr = item.GetComponent<XRBaseInteractable>();
            Require(xr != null && xr.enabled && item.GetComponent<PokeObjects>() != null, "Broken interaction components");
            Require(item.GetComponent<Collider>() != null && item.GetComponent<Rigidbody>() != null, "Missing physics");
            Require(item.GetComponents<Collider>().Any(c => c.enabled && !c.isTrigger), "No enabled solid collider");
            Require(xr.selectEntered.GetPersistentEventCount() == 1 &&
                xr.selectEntered.GetPersistentMethodName(0) == "OnPokeStart" &&
                xr.selectEntered.GetPersistentTarget(0) == item.GetComponent<PokeObjects>(), "Invalid selection callback");
            Require(item.GetComponent<CheckoutCounter>() == null, "Shelf product must not trigger checkout");
            Require(PrefabUtility.GetCorrespondingObjectFromSource(item) != null, "Prefab linkage lost");
            var b = BoundsOf(item.gameObject);
            Require(b.size.magnitude > 0 && b.size.x <= 0.201f && b.size.y <= 0.201f && b.size.z <= 0.201f,
                "Unexpected product scale");
            report.AppendLine($"{Hierarchy(item.transform)} | {item.itemName} | stock={item.stockQuantity} | pos={item.transform.position:F3} | bounds={b.center:F3}/{b.size:F3} | prefab={PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(item.gameObject)}");
        }
        var list = scene.GetRootGameObjects().SelectMany(r => r.GetComponentsInChildren<ListController>(true))
            .Single(c => c.HasListContainer && c.gameObject.activeInHierarchy);
        var data = new SerializedObject(list);
        Require(data.FindProperty("limitLegacyListToAvailableStock").boolValue, "Stock restriction disabled");
        var pool = data.FindProperty("availablePrefabs");
        Require(pool.arraySize == expected, "Unexpected catalog size");
        for (int i = 0; i < pool.arraySize; i++)
        {
            var view = pool.GetArrayElementAtIndex(i).objectReferenceValue as GameObject;
            Require(view != null && products.Any(p => p.itemName == view.name), "Unavailable list product");
        }
        report.AppendLine("Catalog="+expected+"; initial list=8; per-type stock=12; legacy flag enabled only on Level 3 main list.");
        File.WriteAllText(Result, report.ToString());
    }
    static void PlaceOnDisplays(Scene scene)
    {
        var shop = scene.GetRootGameObjects().Single(g => g.name == "supermarket");
        var group = shop.transform.Find(Group);
        Require(group != null, "Missing inventory group");
        var items = group.GetComponentsInChildren<SelectableItem>();
        Require(items.Length == 10, "Expected ten integrated products");
        var source = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Supermercado/shelfMarker2.fbx");
        Require(source != null, "Missing reusable produce shelf");
        for (int row = 0; row < 2; row++)
        {
            string name = "DisplayShelf " + (row + 1);
            var shelfTransform = group.Find(name);
            GameObject shelf;
            if (shelfTransform == null)
            {
                shelf = (GameObject)PrefabUtility.InstantiatePrefab(source, scene);
                Undo.RegisterCreatedObjectUndo(shelf, "Add inventory display shelf");
                shelf.name = name;
                shelf.transform.SetParent(group, false);
            }
            else shelf = shelfTransform.gameObject;
            shelf.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            shelf.transform.localScale = Vector3.one;
            var bounds = BoundsOf(shelf);
            shelf.transform.localScale = new Vector3(1.8f / bounds.size.x, 0.4f / bounds.size.y, 0.65f / bounds.size.z);
            bounds = BoundsOf(shelf);
            Vector3 bottom = new Vector3(row == 0 ? -46.4f : -43.9f, 0.625f, 1.2f);
            shelf.transform.position += bottom - new Vector3(bounds.center.x, bounds.min.y, bounds.center.z);
            bounds = BoundsOf(shelf);
            // Use the real sloped display surfaces, including their recesses, for placement and ray interaction.
            foreach (var box in shelf.GetComponentsInChildren<BoxCollider>())
                if (PrefabUtility.GetCorrespondingObjectFromSource(box) == null)
                    Undo.DestroyObjectImmediate(box);
            foreach (var mesh in shelf.GetComponentsInChildren<MeshFilter>())
            {
                if (mesh.GetComponent<MeshCollider>() != null) continue;
                var collider = Undo.AddComponent<MeshCollider>(mesh.gameObject);
                collider.sharedMesh = mesh.sharedMesh;
                collider.convex = false;
            }
            // A shallow existing stock tray bridges the recessed produce bins, keeping small cans and fish visible.
            var trayTransform = group.Find("StockTray " + (row + 1));
            GameObject tray;
            if (trayTransform == null)
            {
                var traySource = AssetDatabase.LoadAssetAtPath<GameObject>(
                    "Assets/Prefabs/ThreeBox/Match3D Object Pack - Fruits and Vegetables/Resources/Prefabs/Add/StorageTray.prefab");
                Require(traySource != null, "Missing existing stock tray");
                tray = (GameObject)PrefabUtility.InstantiatePrefab(traySource, scene);
                Undo.RegisterCreatedObjectUndo(tray, "Add stock presentation tray");
                tray.name = "StockTray";
                tray.transform.SetParent(group, false);
            }
            else tray = trayTransform.gameObject;
            tray.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            tray.transform.localScale = Vector3.one;
            var trayBounds = BoundsOf(tray);
            tray.transform.localScale = new Vector3(1.65f / trayBounds.size.x, 0.025f / trayBounds.size.y,
                0.5f / trayBounds.size.z);
            trayBounds = BoundsOf(tray);
            tray.transform.position += new Vector3(bounds.center.x, bounds.max.y, bounds.center.z) -
                new Vector3(trayBounds.center.x, trayBounds.min.y, trayBounds.center.z);
            // Keep a direct unit-scale inventory parent to avoid inheriting the nonuniform shelf scale.
            tray.name = "StockTray " + (row + 1);
            foreach (var mesh in tray.GetComponentsInChildren<MeshFilter>())
            {
                if (mesh.GetComponent<MeshCollider>() != null) continue;
                var collider = Undo.AddComponent<MeshCollider>(mesh.gameObject);
                collider.sharedMesh = mesh.sharedMesh;
                collider.convex = false;
            }
            PrefabUtility.RecordPrefabInstancePropertyModifications(tray.transform);
            PrefabUtility.RecordPrefabInstancePropertyModifications(tray);
            Physics.SyncTransforms();
            PrefabUtility.RecordPrefabInstancePropertyModifications(shelf.transform);
            PrefabUtility.RecordPrefabInstancePropertyModifications(shelf);
            for (int col = 0; col < 5; col++)
            {
                var product = items[row * 5 + col].gameObject;
                Undo.RecordObject(product.transform, "Place stock on interior shelf");
                var itemBounds = BoundsOf(product);
                float x = bounds.center.x + (col - 2) * 0.3f;
                float top = float.NegativeInfinity;
                var ray = new Ray(new Vector3(x, bounds.max.y + 1, bounds.center.z), Vector3.down);
                foreach (var collider in tray.GetComponentsInChildren<MeshCollider>())
                    if (collider.Raycast(ray, out var hit, 3)) top = Mathf.Max(top, hit.point.y);
                Require(!float.IsNegativeInfinity(top), "No shelf surface below " + product.name);
                Vector3 target = new Vector3(x, top + 0.012f, bounds.center.z);
                product.transform.position += target - new Vector3(itemBounds.center.x, itemBounds.min.y, itemBounds.center.z);
                PrefabUtility.RecordPrefabInstancePropertyModifications(product.transform);
            }
        }
        EditorSceneManager.MarkSceneDirty(scene);
    }
    static void IntegrateUi()
    {
        var scene = Level3();
        var shop = scene.GetRootGameObjects().Single(g => g.name == "supermarket");
        var old = scene.GetRootGameObjects().Single(g => g.name == "Supermarket");
        Require(shop.transform.Find("ShoppingListUI") == null, "Shopping UI already integrated.");
        var source = old.GetComponentsInChildren<ListController>(true).Single(c => c.HasListContainer);
        var sourceRoot = PrefabUtility.GetOutermostPrefabInstanceRoot(source.gameObject);
        Require(sourceRoot != old, "Refusing to duplicate the old environment.");
        var clone = UnityEngine.Object.Instantiate(sourceRoot, shop.transform);
        Undo.RegisterCreatedObjectUndo(clone, "Create Level 3 shopping UI");
        clone.name = "ShoppingListUI";
        var list = clone.GetComponentsInChildren<ListController>(true).Single(c => c.HasListContainer);
        var data = new SerializedObject(list);
        var container = data.FindProperty("listContainer").objectReferenceValue as GameObject;
        clone.SetActive(true);
        list.gameObject.SetActive(true);
        clone.transform.SetPositionAndRotation(new Vector3(-48.19f, 1.3f, 4.8f), Quaternion.identity);
        var rect = clone.GetComponent<RectTransform>();
        float width = rect != null ? rect.rect.width : 600;
        clone.transform.localScale = Vector3.one * (0.85f / Mathf.Max(1, width));
        foreach (var canvas in clone.GetComponentsInChildren<Canvas>(true))
        { canvas.renderMode = RenderMode.WorldSpace; canvas.worldCamera = Camera.main; }
        // Remap only references outside the retained old hierarchy.
        var oldContainer = new SerializedObject(source).FindProperty("listContainer").objectReferenceValue;
        foreach (var behaviour in scene.GetRootGameObjects().SelectMany(r => r.GetComponentsInChildren<MonoBehaviour>(true)))
        {
            if (behaviour == null || behaviour.transform.IsChildOf(old.transform)) continue;
            var serialized = new SerializedObject(behaviour);
            var property = serialized.GetIterator();
            bool changed = false;
            while (property.NextVisible(true))
            {
                if (property.propertyType != SerializedPropertyType.ObjectReference) continue;
                var value = property.objectReferenceValue;
                if (value == source) { property.objectReferenceValue = list; changed = true; }
                else if (value == source.gameObject) { property.objectReferenceValue = list.gameObject; changed = true; }
                else if (value == oldContainer) { property.objectReferenceValue = container; changed = true; }
            }
            if (changed) serialized.ApplyModifiedProperties();
        }
        // Restore the source catalog from the exact pre-integration scene; it remains a reference.
        string original = File.ReadAllText("Library/Level3Inventory-before.unity");
        string component = System.Text.RegularExpressions.Regex.Match(original,
            @"(?ms)^--- !u!114 &1825740876[^\n]*\n(.*?)(?=^--- !u!|\z)").Value;
        string poolText = component.Split(new[] { "availablePrefabs:" }, StringSplitOptions.None)[1]
            .Split(new[] { "choicedItems:" }, StringSplitOptions.None)[0];
        var matches = System.Text.RegularExpressions.Regex.Matches(poolText, @"guid: (\w+)");
        var sourceData = new SerializedObject(source);
        var sourcePool = sourceData.FindProperty("availablePrefabs");
        sourcePool.arraySize = matches.Count;
        for (int i = 0; i < matches.Count; i++)
            sourcePool.GetArrayElementAtIndex(i).objectReferenceValue =
                AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(matches[i].Groups[1].Value));
        sourceData.FindProperty("limitLegacyListToAvailableStock").boolValue = false;
        sourceData.ApplyModifiedProperties();
        EditorSceneManager.MarkSceneDirty(scene);
        Require(EditorSceneManager.SaveScene(scene), "Save failed");
        Validate();
    }
    static void Preview()
    {
        Level3();
        var go = new GameObject("Temporary inventory preview camera");
        go.hideFlags = HideFlags.HideAndDontSave;
        var texture = new RenderTexture(1280, 720, 24);
        Texture2D image = null;
        var previous = RenderTexture.active;
        try
        {
            var camera = go.AddComponent<Camera>();
            float z = File.Exists("Library/Level3Inventory-preview-row2") ? 9.157f : 4.853f;
            var pose = File.Exists("Library/Level3Inventory-camera.txt")
                ? File.ReadAllText("Library/Level3Inventory-camera.txt").Split(',').Select(v => float.Parse(v, System.Globalization.CultureInfo.InvariantCulture)).ToArray()
                : new[] { -48.187f, 1.8f, 3.46f, -46.5f, 1.0f, 0f };
            go.transform.position = new Vector3(pose[0], pose[1], pose[2]);
            go.transform.LookAt(new Vector3(pose[3], pose[4], pose[5]));
            camera.fieldOfView = 65;
            camera.nearClipPlane = 0.05f;
            camera.targetTexture = texture;
            camera.Render();
            RenderTexture.active = texture;
            image = new Texture2D(1280, 720, TextureFormat.RGB24, false);
            image.ReadPixels(new Rect(0, 0, 1280, 720), 0, 0);
            image.Apply();
            File.WriteAllBytes("Library/Level3Inventory-preview.png", image.EncodeToPNG());
        }
        finally
        {
            RenderTexture.active = previous;
            UnityEngine.Object.DestroyImmediate(go);
            UnityEngine.Object.DestroyImmediate(texture);
            if (image != null) UnityEngine.Object.DestroyImmediate(image);
        }
    }
    static void RunRegression()
    {
        var output = new StringBuilder();
        foreach (string className in new[] { "ShoppingMissionControllerTests", "SupplementalShoppingTaskResultTests", "Level3InventoryListTests", "Level3CheckoutTests" })
        {
            var type = AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType(className)).FirstOrDefault(t => t != null);
            Require(type != null, "Missing test type " + className);
            foreach (var method in type.GetMethods().Where(m => m.GetCustomAttributes(false).Any(a => a.GetType().Name == "TestAttribute")))
            {
                var fixture = Activator.CreateInstance(type);
                try { method.Invoke(fixture, null); output.AppendLine("PASS " + className + "." + method.Name); }
                catch (Exception e) { output.AppendLine("FAIL " + method.Name + ": " + e); }
                finally { type.GetMethod("TearDown")?.Invoke(fixture, null); }
            }
        }
        File.WriteAllText("Library/Level3Inventory-regression.txt", output.ToString());
    }

    static void DrainPayment(PaymentManager payment, BindingFlags flags)
    {
        var routine = (System.Collections.IEnumerator)typeof(PaymentManager)
            .GetMethod("ResolvePaymentAttemptRoutine", flags).Invoke(payment, null);
        Drain(routine);
    }
    static void Drain(System.Collections.IEnumerator routine)
    {
        int count=0;
        while(routine.MoveNext())
        {
            if(++count>1000) throw new Exception("Coroutine did not terminate");
            if(routine.Current is System.Collections.IEnumerator nested) Drain(nested);
        }
    }

    static int frames;
    static void PlayChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.EnteredPlayMode && SessionState.GetBool("Level3Inventory.Playcheck", false))
        {
            frames = 0;
            EditorApplication.update += RuntimeCheck;
        }
    }
    static void RuntimeCheck()
    {
        if (!EditorApplication.isPlaying || ++frames < 60) return;
        EditorApplication.update -= RuntimeCheck;
        try
        {
            var scene = SceneManager.GetActiveScene();
            var manager = PokeManager.Instance;
            Require(manager != null, "PokeManager not initialized");
            var products = Products(scene).Where(p => p.isActiveAndEnabled).ToArray();
            int expected = scene.GetRootGameObjects().Single(g => g.name == "supermarket").transform.Find("ShoppingInventory/S01_Vegetables") != null ? 14 : 10;
            Require(products.Length == expected, "Active product count differs in Play Mode");
            var list = UnityEngine.Object.FindObjectsByType<ListController>(FindObjectsSortMode.None).Single(c => c.HasListContainer && c.gameObject.activeInHierarchy);
            Require(list.MainListGenerated && list.choicedItems.Count == 8, "Eight-row main list not initialized");
            foreach (var row in list.choicedItems)
            {
                string name = row.transform.Find("Name").GetComponent<TMPro.TMP_Text>().text;
                int quantity = int.Parse(row.transform.Find("Quantity").GetComponent<TMPro.TMP_Text>().text);
                Require(products.Any(p => p.itemName == name) && manager.GetAvailableQuantity(name) >= quantity,
                    "Impossible list request: " + name);
            }
            int added = 0;
            manager.OnItemSuccessfullyAdded += _ => added++;
            foreach (var item in products)
            {
                Require(manager.GetAvailableQuantity(item.itemName) == 12, "Initial stock duplicated/missing: " + item.itemName);
                var interactable = item.GetComponent<XRBaseInteractable>();
                interactable.selectEntered.Invoke(new SelectEnterEventArgs { interactableObject = (IXRSelectInteractable)interactable });
                Require(manager.GetAvailableQuantity(item.itemName) == 11 &&
                    manager.inventory[item.itemName].quantity == 1, "Pickup did not update exactly once");
            }
            Require(added == expected, "Duplicate pickup callbacks");
            var sample = products[0];
            for (int i = 0; i < 12; i++)
                sample.GetComponent<XRBaseInteractable>().selectEntered.Invoke(new SelectEnterEventArgs {
                    interactableObject = (IXRSelectInteractable)sample.GetComponent<XRBaseInteractable>() });
            Require(manager.GetAvailableQuantity(sample.itemName) == 0 && manager.inventory[sample.itemName].quantity == 12 &&
                added == expected + 11, "Stock exhaustion failed");

            var checkout = UnityEngine.Object.FindFirstObjectByType<VRCheckoutTeleport>();
            var payment = PaymentManager.Instance;
            Require(checkout != null && payment != null, "Missing checkout/payment");
            if (SessionState.GetBool("Level3Checkout.TimeoutTest", false)) GameTimer.Instance.SendMessage("TimeIsUp");
            else checkout.MoveToCheckout();
            int amount = CartManager.Instance.TotalPaid;
            Require(checkout.CheckoutStarted && !GameTimer.Instance.isRunning, "Checkout/timer state");
            Require(Vector3.Distance(checkout.mainCamera.position, checkout.poiChargeMoney.position) < .02f, "Wrong checkout destination");
            Require(checkout.movementScripts.All(p => p != null && !p.enabled) &&
                checkout.turnScripts.All(p => p != null && !p.enabled), "Locomotion not locked");
            checkout.MoveToCheckout();
            GameTimer.Instance.SendMessage("TimeIsUp");
            Require(CartManager.Instance.TotalPaid == amount && payment.requiredAmount == amount, "Duplicate billing");
            Require(!UnityEngine.SceneManagement.SceneManager.GetSceneByName("Dress_Game").isLoaded, "Unrelated scene loaded");
            payment.StopAllCoroutines();
            var flags = BindingFlags.Instance | BindingFlags.NonPublic;
            foreach (string field in new[] {"paymentSuccessTemplate", "paymentShortageTemplate", "insufficientWalletMessage"})
                typeof(PaymentManager).GetField(field, flags).SetValue(payment, "");
            typeof(PaymentManager).GetField("isPlayingCashierIntro", flags).SetValue(payment, false);
            DrainPayment(payment, flags);
            Require(!payment.HasConfirmedPayment, "Underpayment finalized despite remaining wallet");
            typeof(PaymentManager).GetField("currentAmount", flags).SetValue(payment, amount);
            DrainPayment(payment, flags);
            Require(payment.HasConfirmedPayment && payment.LastPaymentSummary.resultCode == "EXACT", "Exact payment failed");
            var result = UnityEngine.Object.FindFirstObjectByType<ListResultCompare>();
            Require(result != null && result.compareResults.Any(r => r.required), "Missing shopping results");
            foreach (var row in result.compareResults)
                Require(row.currentQuantity == (CartManager.Instance.bill.TryGetValue(row.itemName, out var billed) ? billed.quantity : 0), "Result lost billed quantity");
            var summary = payment.LastPaymentSummary;
            payment.ConfirmPayment();
            payment.ResetMoney();
            Require(payment.LastPaymentSummary == summary && payment.HasConfirmedPayment, "Finalized session reopened");
            File.AppendAllText("Library/Level3Checkout-playcheck.txt",
                "PASS programmatic (timeoutEntry="+SessionState.GetBool("Level3Checkout.TimeoutTest", false)+"): fresh stock/list/pickup; destination; timer; lock; duplicate/timeout idempotency; underpayment retry; exact finalization; bill-based results; repeat confirm/reset blocked; no Dress_Game.\n");

            File.AppendAllText("Library/Level3Inventory-playcheck.txt",
                "PASS: "+expected+" active products; stock 12 each; eight feasible list rows; actual persistent XR selection event -> PokeObjects -> PokeManager; "+expected+" pickups exactly once; exhaustion capped at 12.\n");
        }
        catch (Exception e)
        {
            File.AppendAllText("Library/Level3Inventory-playcheck.txt", "FAIL: " + e + "\n");
            Debug.LogException(e);
        }
        finally
        {
            SessionState.SetBool("Level3Inventory.Playcheck", false);
            EditorApplication.isPlaying = false;
        }
    }
}




