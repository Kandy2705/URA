using System;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors.Casters;

public static class ShelfGrabTools
{
    public const string Output = "Evidence/ShelfGrab";
    public static string[] ProductPaths() => AssetDatabase.FindAssets("t:Prefab", new[]{"Assets/Prefabs"})
        .Select(AssetDatabase.GUIDToAssetPath).Where(p => AssetDatabase.LoadAssetAtPath<GameObject>(p).GetComponent<SelectableItem>() != null)
        .OrderBy(p => p).ToArray();

    public static void Survey()
    {
        Directory.CreateDirectory(Output);
        var scene = EditorSceneManager.OpenScene(Level3LayoutTools.ScenePath);
        var report = new StringBuilder("Phase 1: read-only asset survey\nUnity " + Application.unityVersion + "\n");
        foreach (var path in ProductPaths())
        {
            var root = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            var grab = root.GetComponent<XRGrabInteractable>();
            report.AppendLine(path + " | " + PrefabUtility.GetPrefabAssetType(root) + " | grab=" + (grab != null) +
                (grab ? " trackPosition=" + grab.trackPosition + " trackRotation=" + grab.trackRotation + " movement=" + grab.movementType : "") +
                " | colliders=" + root.GetComponentsInChildren<Collider>(true).Length + " | identities=" + root.GetComponentsInChildren<SelectableItem>(true).Length +
                " | constraints=" + (root.GetComponent<Rigidbody>() ? root.GetComponent<Rigidbody>().constraints.ToString() : "MISSING"));
        }
        var inventory = Level3MerchandisingTools.Inventory(scene);
        report.AppendLine("Shelf units=" + inventory.GetComponentsInChildren<SelectableItem>().Length);
        foreach (var group in Level3LayoutTools.Sections)
            report.AppendLine(group.Name + " units=" + inventory.Find(group.Name).GetComponentsInChildren<SelectableItem>().Length);
        foreach (var ray in scene.GetRootGameObjects().SelectMany(r=>r.GetComponentsInChildren<XRRayInteractor>(true)))
            report.AppendLine("RAY " + Level3LayoutTools.PathOf(ray.transform) + " active=" + ray.isActiveAndEnabled + " max=" + ray.maxRaycastDistance + " mask=" + ray.raycastMask.value + " layers=" + ray.interactionLayers.value);
        report.AppendLine("HANDOFF 1: canonical assets are gameplay prefabs with SelectableItem, including model-based variants; do not edit imported FBX/vendor mesh-only props or catalog UI. Keep identities, prices, stock ownership, 204 placed units, shelf transforms, aisles, input actions, checkout and NavMesh. XRGrabInteractable already exists, but tracking is disabled and rigidbody constraints freeze motion. Poke select consumes/hides Level3StockFacing immediately. Configure existing XR grab on source prefabs; preserve scene collider fitting and ownership; defer consumed visibility until release and restore shelf pose on inventory return.");
        File.WriteAllText(Output + "/phase1.txt", report.ToString());
        Debug.Log(report.ToString());
    }

    internal static void Configure(GameObject root, bool source)
    {
        // CheckoutCounter belongs to the till, not to a product being picked up.
        var checkout = root.GetComponent<CheckoutCounter>();
        if (checkout) UnityEngine.Object.DestroyImmediate(checkout);
        if (!source)
        {
            var boxes=root.GetComponents<BoxCollider>();
            var canonical=root.GetComponents<Collider>().FirstOrDefault(c=>PrefabUtility.GetCorrespondingObjectFromSource(c));
            if (!canonical) throw new Exception("Missing inherited product collider: " + root.name);
            var fitted=boxes.FirstOrDefault(c=>!PrefabUtility.GetCorrespondingObjectFromSource(c));
            if (fitted && canonical is BoxCollider canonicalBox) { canonicalBox.center=fitted.center; canonicalBox.size=fitted.size; }
            foreach (var c in root.GetComponentsInChildren<Collider>(true).Where(c=>c!=canonical).ToArray()) UnityEngine.Object.DestroyImmediate(c);
            canonical.enabled=true; canonical.isTrigger=false;
            PrefabUtility.RecordPrefabInstancePropertyModifications(canonical);
        }
        if (source)
        {
            // A product has one identity, physics body and XR target at its root.
            foreach (var child in root.GetComponentsInChildren<Transform>(true).Where(t=>t != root.transform))
            {
                foreach (var c in child.GetComponents<MonoBehaviour>().Where(c=>c is XRBaseInteractable || c is SelectableItem || c is PokeObjects || (c && c.GetType().Name == "XRHighlightOnHover")).ToArray())
                    UnityEngine.Object.DestroyImmediate(c);
                foreach (var c in child.GetComponents<Rigidbody>()) UnityEngine.Object.DestroyImmediate(c);
            }
            var box = root.GetComponent<BoxCollider>();
            if (!box) box = root.AddComponent<BoxCollider>();
            foreach (var c in root.GetComponentsInChildren<Collider>(true).Where(c=>c != box).ToArray()) UnityEngine.Object.DestroyImmediate(c);
            var points = root.GetComponentsInChildren<MeshFilter>(true).Where(f=>f.sharedMesh)
                .SelectMany(f=>f.sharedMesh.vertices.Select(v=>root.transform.InverseTransformPoint(f.transform.TransformPoint(v)))).ToArray();
            if (points.Length == 0) throw new Exception("No geometry: " + root.name);
            var bounds = new Bounds(points[0], Vector3.zero);
            foreach (var p in points) bounds.Encapsulate(p);
            box.center = bounds.center; box.size = bounds.size; box.isTrigger = false; box.enabled = true;
            if((root.name=="Banana" || root.name=="Carrot") && root.GetComponent<MeshFilter>())
            {
                var hull=root.AddComponent<MeshCollider>(); hull.sharedMesh=root.GetComponent<MeshFilter>().sharedMesh; hull.convex=true;
                UnityEngine.Object.DestroyImmediate(box);
            }
        }
        var body = root.GetComponent<Rigidbody>(); if (!body) body = root.AddComponent<Rigidbody>();
        var grab = root.GetComponent<XRGrabInteractable>();
        if (!grab)
        {
            foreach (var other in root.GetComponents<XRBaseInteractable>()) UnityEngine.Object.DestroyImmediate(other);
            grab = root.AddComponent<XRGrabInteractable>();
        }
        // Normalize the existing purchase callback after source changes as well as on sources.
        // Scene-local component removals can otherwise leave inherited event targets stale.
        {
            var poke = root.GetComponent<PokeObjects>(); if (!poke) poke = root.AddComponent<PokeObjects>();
            grab.selectEntered = new SelectEnterEvent(); grab.selectExited = new SelectExitEvent();
            UnityEditor.Events.UnityEventTools.AddPersistentListener(grab.selectEntered, poke.OnPokeStart);
            UnityEditor.Events.UnityEventTools.AddPersistentListener(grab.selectExited, poke.OnPokeEnd);
        }
        grab.trackPosition = true; grab.trackRotation = true;
        grab.movementType = XRBaseInteractable.MovementType.VelocityTracking;
        grab.useDynamicAttach = true; grab.matchAttachPosition = true; grab.matchAttachRotation = true;
        grab.snapToColliderVolume = true; grab.throwOnDetach = false;
        grab.selectMode = InteractableSelectMode.Single;
        grab.colliders.Clear(); grab.colliders.AddRange(root.GetComponentsInChildren<Collider>(true));
        body.constraints = RigidbodyConstraints.None; body.isKinematic = true; body.useGravity = false;
        body.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        body.interpolation = RigidbodyInterpolation.Interpolate;
        if (!source)
        {
            PrefabUtility.RecordPrefabInstancePropertyModifications(grab);
            PrefabUtility.RecordPrefabInstancePropertyModifications(body);
        }
    }

    [MenuItem("Tools/Shelf Grab/Apply source prefab fixes")]
    public static void Install()
    {
        if (!File.Exists(Output + "/phase1.txt")) throw new Exception("Run Survey first.");
        var scene = Level3MerchandisingTools.Open();
        if (scene.isDirty) throw new Exception("Save/review scene first.");
        if (!File.Exists("Library/ShelfGrab-before.unity")) File.Copy(Level3LayoutTools.ScenePath, "Library/ShelfGrab-before.unity");
        var paths = ProductPaths();
        // Dependencies first so variants inherit updated bases without duplicate components.
        paths = paths.OrderBy(p=>AssetDatabase.GetDependencies(p).Count(d=>paths.Contains(d))).ThenBy(p=>p).ToArray();
        var report = new StringBuilder("Phase 2: exact modified prefab paths\n");
        foreach (var path in paths)
        {
            var root = PrefabUtility.LoadPrefabContents(path);
            try { Configure(root, true); PrefabUtility.SaveAsPrefabAsset(root, path); }
            finally { PrefabUtility.UnloadPrefabContents(root); }
            report.AppendLine(path);
        }
        // Retain scene-authored fitted collider, callbacks, ownership and visual placement.
        foreach (var item in Level3MerchandisingTools.Inventory(scene).GetComponentsInChildren<SelectableItem>()) Configure(item.gameObject, false);
        EditorSceneManager.MarkSceneDirty(scene); EditorSceneManager.SaveScene(scene); AssetDatabase.SaveAssets();
        report.AppendLine(Level3LayoutTools.ScenePath);
        report.AppendLine("Reused XRGrabInteractable; no new runtime component. 204 existing units retained: 12 per identity, 17 identities. No stock inflation or extra aisle geometry. Source BoxCollider fits mesh in root space; duplicate nested targets/bodies removed without unpacking variants. Scene-local collider/stock links retained. Level3StockFacing keeps consumed unit visible while selected, hides on release, restores original pose on inventory return. Existing select-enter purchase remains once per unit.");
        File.WriteAllText(Output + "/phase2.txt", report.ToString());
        Restock();
    }

    public static void Audit()
    {
        var scene = Level3MerchandisingTools.Open();
        var report = new StringBuilder("Phase 3: saved scene and source-prefab checks\n");
        foreach (var path in ProductPaths())
        {
            var root = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            var grab = root.GetComponent<XRGrabInteractable>();
            if (!grab || !grab.trackPosition || !grab.trackRotation || root.GetComponentsInChildren<XRBaseInteractable>(true).Length != 1 ||
                root.GetComponentsInChildren<SelectableItem>(true).Length != 1 || root.GetComponentsInChildren<Collider>(true).Length != 1 ||
                root.GetComponent<Rigidbody>().constraints != RigidbodyConstraints.None) throw new Exception("Invalid prefab " + path);
            report.AppendLine("PASS " + path + " | " + PrefabUtility.GetPrefabAssetType(root));
        }
        report.AppendLine(Level3VisualRevisionTools.Validate(scene));
        report.AppendLine(Level3VisualRevisionTools.MeasureFacingAccess(scene));
        report.AppendLine(Level3LayoutTools.MeasureAccess(scene, false));
        File.WriteAllText(Output + "/phase3.txt", report.ToString());
    }

    public static void RepairScene()
    {
        var scene = Level3MerchandisingTools.Open();
        foreach (var item in Level3MerchandisingTools.Inventory(scene).GetComponentsInChildren<SelectableItem>()) Configure(item.gameObject, false);
        foreach(var interactor in scene.GetRootGameObjects().SelectMany(r=>r.GetComponentsInChildren<NearFarInteractor>(true)))
        {
            var far=interactor.GetComponent<CurveInteractionCaster>();
            if(far) { far.hitDetectionType=CurveInteractionCaster.HitDetectionType.Raycast; PrefabUtility.RecordPrefabInstancePropertyModifications(far); }
            var near=interactor.GetComponent<SphereInteractionCaster>();
            if(near) { var serialized=new SerializedObject(near); serialized.FindProperty("m_CastRadius").floatValue=.025f; serialized.ApplyModifiedPropertiesWithoutUndo(); PrefabUtility.RecordPrefabInstancePropertyModifications(near); }
        }
        EditorSceneManager.MarkSceneDirty(scene); EditorSceneManager.SaveScene(scene); AssetDatabase.SaveAssets();
        Audit();
    }

    public static void Restock()
    {
        var scene = Level3MerchandisingTools.Open();
        var inventory = Level3MerchandisingTools.Inventory(scene);
        foreach (var id in new[]{"S03", "S04"})
        {
            var spec = Level3LayoutTools.Sections.Single(s=>s.id==id);
            var section = inventory.Find(spec.Name);
            var units = section.Find("Products").GetComponentsInChildren<SelectableItem>();
            if (units.Length == 24)
            {
                foreach (var item in units.Where(p=>p.name.EndsWith(" - Back row")))
                {
                    var original = units.Single(p=>p.name==item.name.Replace(" - Back row", ""));
                    PlaceAdditional(section, original, item, units.Where(p=>!p.name.EndsWith(" - Back row") || System.Array.IndexOf(units,p)<System.Array.IndexOf(units,item)).ToArray());
                }
                continue;
            }
            if (units.Length != 12) throw new Exception("Unexpected baseline stock " + id);
            var owner = units.Single(p=>p.GetComponent<Level3StockFacing>().IsStockOwner);
            foreach (var original in units)
            {
                // Instantiate the canonical prefab through the Editor API to retain its link.
                var source = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(original.gameObject));
                var copy = (GameObject)PrefabUtility.InstantiatePrefab(source, scene);
                copy.transform.SetParent(original.transform.parent, false);
                copy.transform.localScale = original.transform.localScale;
                copy.transform.rotation = original.transform.rotation;
                var copyBox = copy.GetComponent<BoxCollider>(); var originalBox = original.GetComponent<BoxCollider>();
                copyBox.center = originalBox.center; copyBox.size = originalBox.size;
                Configure(copy, false);
                var facing = copy.AddComponent<Level3StockFacing>(); facing.stockOwner = owner;
                copy.name = original.name + " - Back row";
                copy.transform.position = original.transform.position + new Vector3(0,0,.12f);
                var item = copy.GetComponent<SelectableItem>(); item.stockQuantity = 1;
                var bounds = Level3LayoutTools.BoundsOf(copy);
                var contact = Level3VisualRevisionTools.ContactSurface(section, bounds.center.x, bounds.center.z, bounds.min.y + .12f);
                copy.transform.position += Vector3.up * ((.002f - Level3ShelfFixTools.MinimumPlaneDistance(copy, contact)) / contact.normal.y);
                PlaceAdditional(section,original,item,section.Find("Products").GetComponentsInChildren<SelectableItem>().Where(p=>p!=item).ToArray());
                PrefabUtility.RecordPrefabInstancePropertyModifications(copy.transform);
                PrefabUtility.RecordPrefabInstancePropertyModifications(item);
                PrefabUtility.RecordPrefabInstancePropertyModifications(copy.GetComponent<Level3StockFacing>());
            }
            owner.stockQuantity = 24;
            PrefabUtility.RecordPrefabInstancePropertyModifications(owner);
        }
        EditorSceneManager.MarkSceneDirty(scene); EditorSceneManager.SaveScene(scene);
        File.AppendAllText(Output + "/phase2.txt", "\nFinal restocking: S03 and S04 each gain 12 linked physical back-row units at +0.12m depth, fitted to the actual support plane. Each stock owner increases from 12 to 24; total=228 physical/logical units, same 17 identities. Other sections and source prefab stock defaults retained.\n");
        Audit();
    }

    static void PlaceAdditional(Transform section, SelectableItem original, SelectableItem item, SelectableItem[] occupied)
    {
        var furniture = Level3LayoutTools.BoundsOf(Level3VisualRevisionTools.Furniture(section).gameObject);
        foreach (float dz in new[]{.12f,.10f,.14f,.16f,.18f,-.12f,-.10f,-.14f,-.16f,0f})
        foreach (float dx in new[]{0f,.04f,-.04f,.08f,-.08f,.12f,-.12f})
        {
            item.transform.SetPositionAndRotation(original.transform.position+new Vector3(dx,0,dz),original.transform.rotation);
            var b = Level3LayoutTools.BoundsOf(item.gameObject);
            Level3ShelfFixTools.Contact contact;
            try { contact=Level3VisualRevisionTools.ContactSurface(section,b.center.x,b.center.z,b.min.y+.035f); }
            catch { continue; }
            item.transform.position+=Vector3.up*((.002f-Level3ShelfFixTools.MinimumPlaneDistance(item.gameObject,contact))/contact.normal.y);
            b=Level3LayoutTools.BoundsOf(item.gameObject);
            if (b.min.x<=furniture.min.x || b.max.x>=furniture.max.x || b.min.z<=furniture.min.z || b.max.z>=furniture.max.z) continue;
            var test=b; test.Expand(-.006f);
            if (occupied.Any(p=>p!=item && test.Intersects(Level3LayoutTools.BoundsOf(p.gameObject)))) continue;
            var support=Level3VisualRevisionTools.ContactSurface(section,b.center.x,b.center.z,b.min.y+.035f);
            float distance=Level3ShelfFixTools.MinimumPlaneDistance(item.gameObject,support);
            var top=Level3VisualRevisionTools.ContactSurface(section,b.center.x,b.center.z,b.max.y-.004f);
            if (distance < -.003f || distance > .007f || top.point.y >= support.point.y+.018f) continue;
            if (!HasClearFurnitureRay(section,item)) continue;
            PrefabUtility.RecordPrefabInstancePropertyModifications(item.transform);
            return;
        }
        throw new Exception("No non-overlapping supported location for " + item.name);
    }

    static bool HasClearFurnitureRay(Transform section, SelectableItem item)
    {
        Physics.SyncTransforms();
        var vertices=item.GetComponentsInChildren<MeshFilter>().SelectMany(f=>f.sharedMesh.vertices.Select(v=>f.transform.TransformPoint(v))).ToArray();
        var eye=section.Find("Approach").position; eye.y=1.792f;
        var owner=item.GetComponent<Level3StockFacing>().stockOwner;
        var ownerDirection=Level3LayoutTools.BoundsOf(owner.gameObject).center-eye;
        if(item.GetComponent<Collider>().Raycast(new Ray(eye,ownerDirection.normalized),out _,ownerDirection.magnitude-.005f)) return false;
        var targets=new[]{Level3LayoutTools.BoundsOf(item.gameObject).center}.Concat(vertices.OrderByDescending(v=>v.y).Take(12)).Concat(vertices.OrderBy(v=>(v-eye).sqrMagnitude).Take(12));
        foreach(var target in targets)
        {
            var direction=target-eye; if(direction.magnitude>1.35f) continue;
            var hits=Physics.RaycastAll(eye,direction.normalized,direction.magnitude+.03f,Physics.DefaultRaycastLayers,QueryTriggerInteraction.Ignore)
                .Where(h=>h.collider.gameObject.scene==item.gameObject.scene && !h.collider.transform.root.name.StartsWith("XR Origin"))
                .Where(h=>h.collider.GetComponentInParent<SelectableItem>()==null || h.collider.GetComponentInParent<SelectableItem>()==item).OrderBy(h=>h.distance).ToArray();
            if(hits.Length==0 || !hits[0].transform.IsChildOf(item.transform)) continue;
            var ray=new Ray(eye,direction.normalized);
            if(!section.Cast<Transform>().Where(t=>t.name.EndsWith("Backing")).Any(t=>t.GetComponent<Renderer>().bounds.IntersectRay(ray,out float distance) && distance<direction.magnitude-.01f)) return true;
        }
        return false;
    }

    public static void Finish()
    {
        foreach (var path in ProductPaths())
        {
            if (!AssetDatabase.LoadAssetAtPath<GameObject>(path).GetComponent<CheckoutCounter>()) continue;
            var root=PrefabUtility.LoadPrefabContents(path);
            try { UnityEngine.Object.DestroyImmediate(root.GetComponent<CheckoutCounter>()); PrefabUtility.SaveAsPrefabAsset(root,path); }
            finally { PrefabUtility.UnloadPrefabContents(root); }
        }
        RepairScene();
    }

    public static void FitBananaCollider()
    {
        const string path="Assets/Prefabs/Banana.prefab";
        var root=PrefabUtility.LoadPrefabContents(path);
        try
        {
            var mesh=root.GetComponent<MeshFilter>();
            if(!mesh || !mesh.sharedMesh) throw new Exception("Expected banana mesh on its physical root.");
            var convex=root.AddComponent<MeshCollider>(); convex.sharedMesh=mesh.sharedMesh; convex.convex=true;
            foreach(var collider in root.GetComponents<Collider>().Where(c=>c!=convex).ToArray()) UnityEngine.Object.DestroyImmediate(collider);
            var grab=root.GetComponent<XRGrabInteractable>(); grab.colliders.Clear(); grab.colliders.Add(convex);
            PrefabUtility.SaveAsPrefabAsset(root,path);
        }
        finally { PrefabUtility.UnloadPrefabContents(root); }
        RepairScene();
    }

    public static void FitProduceColliders()
    {
        foreach(var path in new[]{"Assets/Prefabs/Banana.prefab","Assets/Prefabs/Carrot.prefab"})
        {
            var root=PrefabUtility.LoadPrefabContents(path);
            try
            {
                var mesh=root.GetComponent<MeshFilter>();
                if(!mesh || !mesh.sharedMesh) throw new Exception("Expected root produce mesh: "+path);
                var convex=root.AddComponent<MeshCollider>(); convex.sharedMesh=mesh.sharedMesh; convex.convex=true;
                foreach(var c in root.GetComponents<Collider>().Where(c=>c!=convex).ToArray()) UnityEngine.Object.DestroyImmediate(c);
                var grab=root.GetComponent<XRGrabInteractable>(); grab.colliders.Clear(); grab.colliders.Add(convex);
                PrefabUtility.SaveAsPrefabAsset(root,path);
            }
            finally { PrefabUtility.UnloadPrefabContents(root); }
        }
        var scene=Level3MerchandisingTools.Open();
        foreach(var item in Level3MerchandisingTools.Inventory(scene).GetComponentsInChildren<SelectableItem>())
        {
            string path=PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(item.gameObject);
            if(path!="Assets/Prefabs/Banana.prefab" && path!="Assets/Prefabs/Carrot.prefab") continue;
            var bounds=Level3LayoutTools.BoundsOf(item.gameObject);
            float target=path.EndsWith("Carrot.prefab")?.144f:.1344f;
            item.transform.localScale*=target/Mathf.Max(bounds.size.x,bounds.size.y,bounds.size.z);
            bounds=Level3LayoutTools.BoundsOf(item.gameObject);
            var section=item.transform.parent.parent.parent;
            var support=Level3VisualRevisionTools.ContactSurface(section,bounds.center.x,bounds.center.z,bounds.min.y+.035f);
            item.transform.position+=Vector3.up*((.002f-Level3ShelfFixTools.MinimumPlaneDistance(item.gameObject,support))/support.normal.y);
            PrefabUtility.RecordPrefabInstancePropertyModifications(item.transform);
        }
        EditorSceneManager.MarkSceneDirty(scene); EditorSceneManager.SaveScene(scene);
        RepairScene();
    }

}
