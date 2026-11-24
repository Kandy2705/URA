using System;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.AI.Navigation;
using UnityEngine.AI;

public static class Level3ObstacleTools
{
    const string ScenePath = "Assets/Scenes/Scene-level-3.unity";
    const string CartPath = "Assets/Prefabs/Cute Supermarket Lite/Prefabs/cart_orange.prefab";
    const string Evidence = "Evidence/Level3Prompt5";
    static Scene Open()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode) throw new Exception("Stop Play Mode first.");
        foreach (var scene in Enumerable.Range(0, SceneManager.sceneCount).Select(SceneManager.GetSceneAt))
            if (scene.isDirty) throw new Exception("Unsaved scene; review before switching.");
        var scene3 = SceneManager.GetSceneByPath(ScenePath);
        return scene3.isLoaded ? scene3 : EditorSceneManager.OpenScene(ScenePath);
    }
    public static void ProbeCart()
    {
        var scene = Open(); Directory.CreateDirectory(Evidence);
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(CartPath);
        var cart = (GameObject)PrefabUtility.InstantiatePrefab(prefab, scene);
        try
        {
            var b = Level3LayoutTools.BoundsOf(cart);
            File.WriteAllText(Evidence + "/cart-probe.txt", "renderer=" + b + "\n" + string.Join("\n", cart.GetComponentsInChildren<Collider>(true).Select(c => c.GetType().Name + " enabled=" + c.enabled + " trigger=" + c.isTrigger + " bounds=" + c.bounds)));
        }
        finally { UnityEngine.Object.DestroyImmediate(cart); }
    }
    static GameObject AddCart(Scene scene, Transform parent, string name, Vector3 center, Vector3 size)
    {
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(CartPath);
        var cart = (GameObject)PrefabUtility.InstantiatePrefab(prefab, scene);
        cart.name = name;
        cart.transform.SetParent(parent, true);
        var before = Level3LayoutTools.BoundsOf(cart);
        cart.transform.localScale = Vector3.Scale(cart.transform.localScale, new Vector3(size.x / before.size.x, size.y / before.size.y, size.z / before.size.z));
        var after = Level3LayoutTools.BoundsOf(cart);
        cart.transform.position += center - new Vector3(after.center.x, after.min.y + size.y * .5f, after.center.z);
        foreach (var renderer in cart.GetComponentsInChildren<Renderer>(true)) renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
        var colliders = cart.GetComponentsInChildren<Collider>(true);
        if (colliders.Length == 0 || colliders.Any(c => !c.enabled || c.isTrigger)) throw new Exception("Cart must retain enabled solid collision.");
        return cart;
    }
    static void BakeNavMesh(Scene scene)
    {
        Physics.SyncTransforms();
        var surface = scene.GetRootGameObjects().Single(g => g.name == "supermarket").transform.Find("supermarket").GetComponent<NavMeshSurface>();
        surface.RemoveData(); surface.navMeshData = null; surface.BuildNavMesh();
        const string path = "Assets/Scenes/Scene-level-3/NavMesh-Level3Obstacles.asset";
        var existing = AssetDatabase.LoadAssetAtPath<NavMeshData>(path);
        if (existing)
        {
            var temporary = surface.navMeshData; surface.RemoveData(); EditorUtility.CopySerialized(temporary, existing);
            surface.navMeshData = existing; surface.AddData(); EditorUtility.SetDirty(existing); UnityEngine.Object.DestroyImmediate(temporary);
        }
        else { var data = surface.navMeshData; AssetDatabase.CreateAsset(data, path); surface.navMeshData = data; surface.AddData(); }
        EditorUtility.SetDirty(surface); PrefabUtility.RecordPrefabInstancePropertyModifications(surface);
        EditorSceneManager.MarkSceneDirty(scene);
    }
    public static void Apply()
    {
        var scene = Open(); Directory.CreateDirectory(Evidence);
        var shop = scene.GetRootGameObjects().Single(g => g.name == "supermarket");
        if (shop.transform.Find("Prompt5 Obstacles") != null) throw new Exception("Prompt 5 obstacles already exist.");
        var root = new GameObject("Prompt5 Obstacles"); SceneManager.MoveGameObjectToScene(root, scene); root.transform.SetParent(shop.transform, false);
        // Alternating cart placements form a two-turn chicane. The opposite-side opening remains at least 1.12 m.
        AddCart(scene, root.transform, "Obstacle Cart A - West Offset", new Vector3(-47.00f, .62505f, 2.55f), new Vector3(.65f, .95f, .68f));
        AddCart(scene, root.transform, "Obstacle Cart B - East Offset", new Vector3(-45.94f, .62505f, -.95f), new Vector3(.65f, .95f, .68f));
        BakeNavMesh(scene);
        var access = Level3LayoutTools.MeasureAccess(scene, false);
        EditorSceneManager.SaveScene(scene); AssetDatabase.SaveAssets();
        File.WriteAllText(Evidence + "/initial-validation.txt", access);
        Level3VisualRevisionTools.Audit();
    }
    public static void RefinePositions()
    {
        var scene = Open(); var obstacles = scene.GetRootGameObjects().Single(g => g.name == "supermarket").transform.Find("Prompt5 Obstacles");
        obstacles.Find("Obstacle Cart A - West Offset").position = new Vector3(-47.00f, .62505f, 3.7f);
        obstacles.Find("Obstacle Cart B - East Offset").position = new Vector3(-45.94f, .62505f, -.95f);
        foreach (Transform cart in obstacles) { var bounds = Level3LayoutTools.BoundsOf(cart.gameObject); var scale = cart.localScale; scale.x *= .40f/bounds.size.x; cart.localScale = scale; PrefabUtility.RecordPrefabInstancePropertyModifications(cart); }
        BakeNavMesh(scene); EditorSceneManager.SaveScene(scene); AssetDatabase.SaveAssets();
    }
    static string DirectBlock(Transform rig, Transform obstacles, Transform camera, Transform target, CharacterController[] capsules, Scene scene)
    {
        var offset = camera.position - rig.position;
        var destination = target.position - offset;
        var movement = destination - rig.position;
        foreach (var cc in capsules)
        {
            var scale = cc.transform.lossyScale;
            float radius = cc.radius * Mathf.Max(Mathf.Abs(scale.x), Mathf.Abs(scale.z)) + .02f;
            float half = Mathf.Max(radius, cc.height * Mathf.Abs(scale.y) * .5f);
            var center = cc.transform.TransformPoint(cc.center) + Vector3.up * .025f;
            var a = center + Vector3.up * (half - radius);
            var b = center - Vector3.up * (half - radius);
            foreach (var hit in Physics.CapsuleCastAll(a, b, radius, movement.normalized, movement.magnitude, ~0, QueryTriggerInteraction.Ignore).OrderBy(h => h.distance))
                if (hit.collider.gameObject.scene == scene && hit.collider.transform.IsChildOf(obstacles)) return hit.collider.transform.root.name + "/" + hit.collider.name;
        }
        return null;
    }
    public static string Validate()
    {
        var scene = Open(); Physics.SyncTransforms();
        var obstacles = scene.GetRootGameObjects().Single(g => g.name == "supermarket").transform.Find("Prompt5 Obstacles");
        if (!obstacles || obstacles.childCount != 2) throw new Exception("Expected exactly two Prompt 5 obstacles.");
        foreach (Transform cart in obstacles)
        {
            var colliders = cart.GetComponentsInChildren<Collider>(true);
            if (colliders.Length == 0 || colliders.Any(c => !c.enabled || c.isTrigger)) throw new Exception("Obstacle collision missing: " + cart.name);
        }
        var access = Level3LayoutTools.MeasureAccess(scene, false);
        var facing = Level3VisualRevisionTools.MeasureFacingAccess(scene);
        var rig = scene.GetRootGameObjects().Single(g => g.name == "XR Origin (XR Rig)").transform;
        var camera = rig.GetComponentsInChildren<Camera>(true).Single(c => c.CompareTag("MainCamera")).transform;
        var capsules = rig.GetComponentsInChildren<CharacterController>(true);
        var inventory = scene.GetRootGameObjects().Single(g => g.name == "supermarket").transform.Find("ShoppingInventory");
        var report = new StringBuilder("Two existing solid retail-display fixture colliders; alternate-side chicane. Minimum free-side opening is approximately 1.25 m by placed bounds; NavMesh agent radius 0.5 m.\n");
        foreach (var sectionName in new[] { "S06_Dairy", "S02_FreshFruits" })
        {
            var section = inventory.Find(sectionName);
            var direct = DirectBlock(rig, obstacles, camera, section.Find("Approach"), capsules, scene);
            if (direct == null) throw new Exception("Expected direct approach to be blocked for " + sectionName);
            report.AppendLine(sectionName + " direct approach blocked by " + direct + "; collision-aware alternate path remains connected.");
        }
        report.AppendLine("\n" + access + "\n" + facing);
        File.WriteAllText(Evidence + "/validation.txt", report.ToString());
        return report.ToString();
    }
    public static void RenderEvidence()
    {
        var scene = Open();
        Level3LayoutTools.Render("obstacles-overview", new Vector3(-46.55f, 3.2f, 1.2f), new Vector3(-46.55f, .7f, 1.2f), true, 1200, 1800, 7f);
        Level3LayoutTools.Render("obstacles-north-eye", new Vector3(-46.45f, 1.792f, 4.1f), new Vector3(-46.45f, 1.25f, 1.7f), false);
        Level3LayoutTools.Render("obstacles-south-eye", new Vector3(-46.45f, 1.792f, 1.2f), new Vector3(-46.45f, 1.25f, -2.0f), false);
        File.WriteAllText(Evidence + "/validation.txt", Validate());
    }
}

