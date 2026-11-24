using System;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>Explicit authoring and resolved-reference validation; never runs automatically.</summary>
public static class Level3StabilityTools
{
    const string ScenePath = "Assets/Scenes/Scene-level-3.unity";
    static Scene OpenLevel3()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
            throw new InvalidOperationException("Stop Play Mode before authoring or inspecting saved state.");
        for (int i = 0; i < SceneManager.sceneCount; i++)
            if (SceneManager.GetSceneAt(i).isDirty)
                throw new InvalidOperationException("Review/save unsaved scenes before switching scenes.");
        return EditorSceneManager.OpenScene(ScenePath);
    }

    [MenuItem("Tools/Level 3/Validate initialization references")]
    public static void Inspect()
    {
        var scene = OpenLevel3();
        var output = new StringBuilder("Level 3 resolved Edit Mode references\n");
        int missing = 0, broken = 0;
        foreach (var root in scene.GetRootGameObjects())
        foreach (var transform in root.GetComponentsInChildren<Transform>(true))
        {
            int count = GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(transform.gameObject);
            missing += count;
            if (count > 0) output.AppendLine("MISSING " + PathOf(transform) + " count=" + count);
            foreach (var component in transform.GetComponents<MonoBehaviour>())
            {
                if (component == null) continue;
                output.AppendLine("COMP " + component.GetType().Name + " active=" + component.isActiveAndEnabled + " path=" + PathOf(transform));
                var property = new SerializedObject(component).GetIterator();
                while (property.NextVisible(true))
                {
                    if (property.propertyType != SerializedPropertyType.ObjectReference) continue;
                    if (property.objectReferenceValue == null && property.objectReferenceInstanceIDValue != 0)
                    {
                        broken++;
                        output.AppendLine("BROKEN " + PathOf(transform) + ":" + property.propertyPath);
                    }
                    output.AppendLine(" REF " + property.propertyPath + "=" +
                        (property.objectReferenceValue == null ? "null" : property.objectReferenceValue.name));
                }
            }
        }
        output.AppendLine("SUMMARY missingScripts=" + missing + "; brokenSerializedReferences=" + broken);
        File.WriteAllText("Library/Level3Stability-static.txt", output.ToString());
        if (missing != 0 || broken != 0) throw new InvalidOperationException("Resolved scene has missing scripts/references. See Library/Level3Stability-static.txt.");
        Level3InventoryTools.Validate();
    }

    public static void RepairScene()
    {
        var scene = OpenLevel3();
        var npc = scene.GetRootGameObjects().Single(g => g.name == "NPC_2");
        // Isolate only this scene from the missing development helper in the shared variant.
        if (PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(npc) == "Assets/Prefabs/NPC_2.prefab")
            PrefabUtility.UnpackPrefabInstance(npc, PrefabUnpackMode.OutermostRoot, InteractionMode.AutomatedAction);
        GameObjectUtility.RemoveMonoBehavioursWithMissingScript(npc);
        var followers = npc.GetComponents<NPCFollowerNavMesh>();
        var keep = followers.Single(f => Mathf.Approximately(f.followDistance, 1.2f));
        foreach (var follower in followers.Where(f => f != keep)) UnityEngine.Object.DestroyImmediate(follower);
        // The scene-configured follower owns movement; do not write competing wander destinations.
        npc.GetComponent<NPCWander>().enabled = false;
        var dispatcher = new SerializedObject(npc.GetComponent<NpcActionDispatcher>());
        dispatcher.FindProperty("targetAnimator").objectReferenceValue = npc.GetComponent<Animator>();
        dispatcher.ApplyModifiedPropertiesWithoutUndo();
        var bridge = scene.GetRootGameObjects().SelectMany(r => r.GetComponentsInChildren<Level2MllmDialogueBridge>(true)).Single();
        bridge.enabled = false;
        EditorUtility.SetDirty(bridge);
        EditorSceneManager.MarkSceneDirty(scene);
        if (!EditorSceneManager.SaveScene(scene)) throw new InvalidOperationException("Scene save failed.");
        Inspect();
    }

    public static void RepairUiReferences()
    {
        var scene = OpenLevel3();
        int repaired = 0;
        foreach (var root in scene.GetRootGameObjects())
        foreach (var component in root.GetComponentsInChildren<MonoBehaviour>(true))
        {
            if (component == null) continue;
            var path = PathOf(component.transform);
            bool legacyImage = component is UnityEngine.UI.Image && path.StartsWith("CanvasUI/Scroll UI Sample/Scroll View/Scrollbar ", StringComparison.Ordinal);
            bool paymentScroll = component is UnityEngine.UI.ScrollRect && path == "PaymentUI Variant/Scroll UI Sample/Scroll View";
            if (!legacyImage && !paymentScroll) continue;
            var serialized = new SerializedObject(component);
            foreach (var name in legacyImage ? new[] { "m_Sprite" } : new[] { "m_HorizontalScrollbar", "m_VerticalScrollbar" })
            {
                var property = serialized.FindProperty(name);
                if (property.objectReferenceValue != null || property.objectReferenceInstanceIDValue == 0) continue;
                // These optional targets are already absent. Clear the dangling ID, not a valid reference.
                property.objectReferenceValue = null;
                repaired++;
            }
            if (serialized.ApplyModifiedPropertiesWithoutUndo())
                PrefabUtility.RecordPrefabInstancePropertyModifications(component);
        }
        EditorSceneManager.MarkSceneDirty(scene);
        if (!EditorSceneManager.SaveScene(scene)) throw new InvalidOperationException("Scene save failed.");
        File.WriteAllText("Library/Level3Stability-ui-repair.txt", "Cleared optional dangling UI references: " + repaired);
        Inspect();
    }
    static string PathOf(Transform transform) => transform.parent == null ? transform.name : PathOf(transform.parent) + "/" + transform.name;
}

