using System;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using Unity.AI.Navigation;

public static class Level3VisualFixTools
{
    const string Evidence = "Evidence/Level3VisualFix";
    const string FixturePath = "Assets/Prefabs/Supermercado/shelfMarker4.fbx";

    public static void RepairFruitAndCapture()
    {
        var scene = Level3MerchandisingTools.Open();
        FillFruitTrays(scene);
        RebuildNavMesh(scene);
        EditorSceneManager.SaveScene(scene);
        AssetDatabase.SaveAssets();
        ReloadSavedScene();
        AuditAndCapture("after-reload");
    }
    public static void ApplyAndCapture()
    {
        var scene = Level3MerchandisingTools.Open();
        if (!File.Exists("Library/Level3VisualFix-before.unity")) File.Copy(Level3LayoutTools.ScenePath, "Library/Level3VisualFix-before.unity", false);

        ReplaceYellowCarts(scene);
        FillFruitTrays(scene);
        RebuildNavMesh(scene);
        EditorSceneManager.SaveScene(scene);
        AssetDatabase.SaveAssets();
        ReloadSavedScene();
        AuditAndCapture("after-reload");
    }

    static void ReplaceYellowCarts(Scene scene)
    {
        var shop = scene.GetRootGameObjects().Single(g => g.name == "supermarket");
        var obstacles = shop.transform.Find("Prompt5 Obstacles");
        if (!obstacles || obstacles.childCount != 2) throw new Exception("Expected both Prompt 5 obstacles.");
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(FixturePath);
        if (!prefab) throw new Exception("Existing narrow shelf fixture is unavailable: " + FixturePath);
        foreach (Transform old in obstacles.Cast<Transform>().ToArray())
        {
            if (!old.name.Contains("Cart")) throw new Exception("Refusing unexpected obstacle: " + old.name);
            var oldBounds = Level3LayoutTools.BoundsOf(old.gameObject);
            var center = oldBounds.center;
            var size = oldBounds.size;
            var rotation = old.rotation;
            var name = old.name.Replace("Obstacle Cart", "Obstacle Display");
            var root = new GameObject(name);
            SceneManager.MoveGameObjectToScene(root, scene);
            root.transform.SetParent(obstacles, true);
            root.transform.SetPositionAndRotation(center, rotation);
            var model = (GameObject)PrefabUtility.InstantiatePrefab(prefab, scene);
            model.name = "Existing shelf fixture";
            model.transform.SetParent(root.transform, false);
            var bounds = Level3LayoutTools.BoundsOf(model);
            model.transform.localScale = Vector3.Scale(model.transform.localScale, new Vector3(size.x / bounds.size.x, size.y / bounds.size.y, size.z / bounds.size.z));
            bounds = Level3LayoutTools.BoundsOf(model);
            model.transform.position += center - bounds.center;
            foreach (var collider in model.GetComponentsInChildren<Collider>(true)) UnityEngine.Object.DestroyImmediate(collider);
            foreach (var renderer in model.GetComponentsInChildren<Renderer>(true)) renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
            var body = root.AddComponent<BoxCollider>();
            body.center = Vector3.zero;
            body.size = size;
            body.isTrigger = false;
            var nav = root.AddComponent<NavMeshModifier>();
            nav.overrideArea = true;
            nav.area = NavMesh.GetAreaFromName("Not Walkable");
            UnityEngine.Object.DestroyImmediate(old.gameObject);
        }
    }

    static void FillFruitTrays(Scene scene)
    {
        var inventory = Level3MerchandisingTools.Inventory(scene);
        var section = inventory.Find("S02_FreshFruits");
        var group = section.Find("Decorative Fruit Fill");
        if (group) throw new Exception("Decorative fruit facings already exist.");
        group = new GameObject("Decorative Fruit Fill").transform;
        group.SetParent(section, true);
        int copies = 0;
        foreach (var row in section.Find("Products").Cast<Transform>())
        {
            var units = row.GetComponentsInChildren<SelectableItem>().OrderBy(p => p.name, StringComparer.Ordinal).ToArray();
            if (units.Length != 12) throw new Exception("Unexpected fruit units in " + row.name + ": " + units.Length);
            foreach (var index in new[] { 1, 3, 5, 7, 9, 11 })
            {
                var source = units[index];
                var offset = new Vector3(index % 4 < 2 ? -.018f : .018f, .002f, index % 2 == 0 ? -.015f : .015f);
                var facing = new GameObject(row.name + " decorative facing " + (index + 1).ToString("D2"));
                facing.transform.SetParent(group, true);
                foreach (var filter in source.GetComponentsInChildren<MeshFilter>())
                {
                    var sourceRenderer = filter.GetComponent<MeshRenderer>();
                    if (!sourceRenderer || !filter.sharedMesh) continue;
                    var meshObject = new GameObject("Static fruit mesh " + (++copies).ToString("D2"));
                    meshObject.transform.SetParent(facing.transform, true);
                    meshObject.transform.SetPositionAndRotation(filter.transform.position + offset, filter.transform.rotation);
                    meshObject.transform.localScale = filter.transform.lossyScale * .72f;
                    meshObject.isStatic = true;
                    var copyFilter = meshObject.AddComponent<MeshFilter>();
                    copyFilter.sharedMesh = filter.sharedMesh;
                    var copyRenderer = meshObject.AddComponent<MeshRenderer>();
                    copyRenderer.sharedMaterials = sourceRenderer.sharedMaterials;
                    copyRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                    copyRenderer.receiveShadows = false;
                }
            }
        }
        if (copies < 12) throw new Exception("Expected at least twelve shared-mesh fruit facings, got " + copies);
    }
    static void RebuildNavMesh(Scene scene)
    {
        Physics.SyncTransforms();
        var surface = scene.GetRootGameObjects().Single(g => g.name == "supermarket").transform.Find("supermarket").GetComponent<NavMeshSurface>();
        var previous = surface.navMeshData;
        var path = previous ? AssetDatabase.GetAssetPath(previous) : null;
        surface.RemoveData();
        surface.navMeshData = null;
        surface.BuildNavMesh();
        var generated = surface.navMeshData;
        if (previous && !string.IsNullOrEmpty(path))
        {
            surface.RemoveData();
            EditorUtility.CopySerialized(generated, previous);
            surface.navMeshData = previous;
            surface.AddData();
            EditorUtility.SetDirty(previous);
            UnityEngine.Object.DestroyImmediate(generated);
        }
        else
        {
            const string newPath = "Assets/Scenes/Scene-level-3/NavMesh-Level3VisualFix.asset";
            AssetDatabase.CreateAsset(generated, newPath);
            surface.navMeshData = generated;
            surface.AddData();
        }
        EditorUtility.SetDirty(surface);
        PrefabUtility.RecordPrefabInstancePropertyModifications(surface);
        EditorSceneManager.MarkSceneDirty(scene);
    }

    static void ReloadSavedScene()
    {
        EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        EditorSceneManager.OpenScene(Level3LayoutTools.ScenePath, OpenSceneMode.Single);
    }

    public static void AuditAndCapture(string prefix)
    {
        Directory.CreateDirectory(Evidence);
        var scene = Level3MerchandisingTools.Open();
        var fruitBounds = new StringBuilder("Name\tBounds\tLocalScale\tPosition\n");
        foreach (var item in scene.GetRootGameObjects().SelectMany(g => g.GetComponentsInChildren<SelectableItem>(true)).Where(i => i.name.Contains("Nho - 0")))
            fruitBounds.AppendLine(item.name + "\t" + Level3LayoutTools.BoundsOf(item.gameObject) + "\t" + item.transform.localScale + "\t" + item.transform.position);
        File.WriteAllText(Evidence + "/grape-bounds.txt", fruitBounds.ToString());
        var validation = Level3VisualRevisionTools.Validate(scene);
        var obstacles = Level3ObstacleTools.Validate();
        var result = new StringBuilder("Saved scene reloaded before capture.\n" + validation + "\n" + obstacles);
        File.WriteAllText(Evidence + "/validation.txt", result.ToString());
        Level3VisualRevisionTools.Capture(prefix);
        Level3VisualRevisionTools.Render(prefix + "-obstacle-area", new Vector3(-46.5f, 1.792f, 4.5f), new Vector3(-46.5f, 1.20f, 1.4f), false);
        Level3VisualRevisionTools.Render(prefix + "-obstacles-overview", new Vector3(-46.55f, 3.2f, 1.2f), new Vector3(-46.55f, .7f, 1.2f), true, 1200, 1800, 7f);
        File.WriteAllText(Evidence + "/inventory-counts.txt", Level3MerchandisingTools.Counts(scene) + "\n" + validation);
        File.WriteAllText(Evidence + "/route-validation.txt", obstacles);
    }
}













