using System;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.AI.Navigation;

public static class Level3LayoutTools
{
    public const string ScenePath = "Assets/Scenes/Scene-level-3.unity";
    public const string Evidence = "Evidence/Level3Prompt4";
    static readonly string[] NewProducts = { "Assets/Prefabs/turkey.prefab", "Assets/Prefabs/IceCream.prefab", "Assets/Prefabs/shaker-salt.prefab", "Assets/Prefabs/plate.prefab" };
    public static string PathOf(Transform t) => t.parent == null ? t.name : PathOf(t.parent) + "/" + t.name;
    public static Bounds BoundsOf(GameObject go)
    {
        var renderers = go.GetComponentsInChildren<Renderer>(true).Where(r => !(r is ParticleSystemRenderer)).ToArray();
        if (renderers.Length == 0) throw new Exception("No renderer: " + go.name);
        var bounds = renderers[0].bounds;
        foreach (var renderer in renderers.Skip(1)) bounds.Encapsulate(renderer.bounds);
        return bounds;
    }
    static Scene Open(string path)
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode) throw new Exception("Stop Play Mode first.");
        for (int i = 0; i < SceneManager.sceneCount; i++) if (SceneManager.GetSceneAt(i).isDirty) throw new Exception("Unsaved scene; review before switching.");
        var loaded=SceneManager.GetSceneByPath(path);
        return loaded.isLoaded ? loaded : EditorSceneManager.OpenScene(path);
    }
    public static void Probe()
    {
        Directory.CreateDirectory(Evidence);
        var scene = Open(ScenePath);
        var b = new StringBuilder();
        foreach (var root in scene.GetRootGameObjects()) b.AppendLine("ROOT " + root.name + " active=" + root.activeSelf + " position=" + root.transform.position.ToString("F3"));
        var shop = scene.GetRootGameObjects().Single(g => g.name == "supermarket");
        foreach (var r in shop.GetComponentsInChildren<Renderer>(true))
            b.AppendLine("RENDER " + PathOf(r.transform) + " active=" + r.gameObject.activeInHierarchy + " center=" + r.bounds.center.ToString("F3") + " size=" + r.bounds.size.ToString("F3") + " mesh=" + r.GetComponent<MeshFilter>()?.sharedMesh?.name);
        foreach (var root in scene.GetRootGameObjects()) foreach (var c in root.GetComponentsInChildren<Collider>(true))
            if (c.enabled && c.gameObject.activeInHierarchy)
                b.AppendLine("COLLIDER " + c.GetType().Name + " " + PathOf(c.transform) + " trigger=" + c.isTrigger + " bounds=" + c.bounds);
        foreach (var root in scene.GetRootGameObjects()) foreach (var c in root.GetComponentsInChildren<CharacterController>(true))
            b.AppendLine("CAPSULE " + PathOf(c.transform) + " scale=" + c.transform.lossyScale + " radius=" + c.radius + " height=" + c.height + " center=" + c.center);
        foreach (var root in scene.GetRootGameObjects()) foreach (var c in root.GetComponentsInChildren<NavMeshSurface>(true))
            b.AppendLine("NAVSURFACE " + PathOf(c.transform) + " active=" + c.isActiveAndEnabled + " collect=" + c.collectObjects + " layers=" + c.layerMask.value + " geometry=" + c.useGeometry + " data=" + AssetDatabase.GetAssetPath(c.navMeshData));
        foreach (var root in scene.GetRootGameObjects()) foreach (var item in root.GetComponentsInChildren<SelectableItem>(false))
            b.AppendLine("PRODUCT " + PathOf(item.transform) + " identity=" + item.itemName + " stock=" + item.stockQuantity + " position=" + item.transform.position.ToString("F3"));
        File.WriteAllText(Evidence + "/geometry-before.txt", b.ToString());
        var catalog = new StringBuilder();
        foreach (var p in NewProducts.Concat(new[] { "Assets/Prefabs/Supermercado/shelfMarker.fbx", "Assets/Prefabs/Supermercado/Frezer.fbx", "Assets/Prefabs/Supermercado/Frezer2.fbx" }))
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(p);
            if (prefab == null) { catalog.AppendLine("MISSING " + p); continue; }
            var instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, scene);
            try {
                catalog.AppendLine("ASSET " + p + " bounds=" + BoundsOf(instance));
                foreach (var item in instance.GetComponentsInChildren<SelectableItem>(true))
                    catalog.AppendLine(" ITEM " + item.itemName + " stock=" + item.stockQuantity + " components=" + string.Join(",", item.GetComponents<Component>().Select(c => c==null?"MISSING":c.GetType().Name)));
            } finally { UnityEngine.Object.DestroyImmediate(instance); }
        }
        File.WriteAllText(Evidence + "/candidate-assets.txt", catalog.ToString());
        // Temporary probe instances are never saved.
        var camera = scene.GetRootGameObjects().SelectMany(r => r.GetComponentsInChildren<Camera>(true)).FirstOrDefault(c => c.CompareTag("MainCamera"));
        if (camera != null) File.WriteAllText(Evidence + "/spawn.txt", "camera=" + camera.transform.position + "; forward=" + camera.transform.forward);
        Render("before-overhead", new Vector3(-44, 18, 0), new Vector3(-44, 0, 0), true);
        Render("before-eye", new Vector3(-48.18f, 1.65f, 4.85f), new Vector3(-44, 1, 0), false);
        // Discard temporary edit dirtiness before inspecting Level 2; never save it.
        scene = EditorSceneManager.OpenScene("Assets/Scenes/Scene-level-2.unity");
        var l2 = new StringBuilder("path\tidentity\tx\ty\tz\tfixture\n");
        foreach (var root in scene.GetRootGameObjects()) foreach (var item in root.GetComponentsInChildren<SelectableItem>(true))
            l2.AppendLine(PathOf(item.transform) + "\t" + item.itemName + "\t" + item.transform.position.x.ToString("F3") + "\t" + item.transform.position.y.ToString("F3") + "\t" + item.transform.position.z.ToString("F3") + "\t" + (item.transform.parent == null ? "" : PathOf(item.transform.parent)));
        File.WriteAllText(Evidence + "/level2-products.tsv", l2.ToString());
    }
    public static void SpatialProbe()
    {
        var scene=Open(ScenePath);
        var b=new StringBuilder();
        var shop=scene.GetRootGameObjects().Single(g=>g.name=="supermarket");
        foreach(var r in shop.GetComponentsInChildren<Renderer>(true))
        {
            var box=r.bounds;
            if(box.center.x > -53.5f && box.center.x < -42.4f && box.center.z > -9 && box.center.z < 11.7f)
                b.AppendLine(PathOf(r.transform)+" center="+box.center.ToString("F3")+" size="+box.size.ToString("F3"));
        }
        Physics.SyncTransforms();
        // Probe floor and current capsule clearance for candidate fixture centers.
        foreach(float z in new[]{-4.8f,-1.4f,5.8f,9.1f}) foreach(float x in new[]{-51.4f,-47.8f,-44.4f})
        {
            var origin=new Vector3(x,2,z);
            var hits=Physics.RaycastAll(origin,Vector3.down,3,~0,QueryTriggerInteraction.Ignore).OrderBy(h=>h.distance);
            b.AppendLine("CANDIDATE "+origin+" hits="+string.Join(";",hits.Select(h=>PathOf(h.transform)+" @"+h.point.y)));
        }
        File.WriteAllText(Evidence+"/interior-geometry.txt",b.ToString());
        Render("before-cutaway",new Vector3(-47.8f,2.75f,1.3f),new Vector3(-47.8f,0,1.3f),true);
        Render("before-east-eye",new Vector3(-44,1.65f,7),new Vector3(-50,1.2f,-3),false);
        // Identify list view names for the new goods without renaming their existing identities.
        var views=new StringBuilder();
        foreach(var root in scene.GetRootGameObjects()) foreach(var list in root.GetComponentsInChildren<ListController>(true))
        {
            var pool=new SerializedObject(list).FindProperty("availablePrefabs");
            for(int i=0;i<pool.arraySize;i++)
            {
                var view=pool.GetArrayElementAtIndex(i).objectReferenceValue;
                if(view!=null) views.AppendLine(view.name+"\t"+AssetDatabase.GetAssetPath(view));
            }
        }
        File.WriteAllText(Evidence+"/available-list-views.tsv",views.ToString());
    }
    public class Section
    {
        public string id, category, furniture;
        public float x, z;
        public string[] productPaths;
        public Section(string id, string category, float x, float z, string furniture, params string[] paths)
        { this.id=id; this.category=category; this.x=x; this.z=z; this.furniture=furniture; productPaths=paths; }
        public string Name => id + "_" + category.Replace(" ", "");
    }
    public static readonly Section[] Sections = {
        new Section("S01","Vegetables",-48.2f,6f,"Produce", "Assets/Prefabs/Carrot.prefab","Assets/Prefabs/tomate.prefab"),
        new Section("S02","Fresh Fruits",-44.7f,-1.6f,"Produce","Assets/Prefabs/Banana.prefab","Assets/Prefabs/Cute Supermarket Lite/Prefabs/apple_green.prefab"),
        new Section("S03","Bottled Water",-44.7f,-6.5f,"Shelf","Assets/Prefabs/Cute Supermarket Lite/Prefabs/bottle1_water.prefab"),
        new Section("S04","Juice",-48.2f,8.5f,"Shelf","Assets/Prefabs/sok.prefab"),
        new Section("S05","Packaged Foods",-48.2f,-1.6f,"Shelf","Assets/Prefabs/Cute Supermarket Lite/Prefabs/can_food_square_red.prefab"),
        new Section("S06","Dairy",-44.7f,4f,"Chilled","Assets/Prefabs/Milk.prefab"),
        new Section("S07","Fresh Meat",-44.7f,-4.3f,"Chilled","Assets/Prefabs/turkey.prefab"),
        new Section("S08","Seafood",-44.7f,6f,"Chilled","Assets/Prefabs/fish 1.prefab"),
        new Section("S09","Frozen Foods",-44.7f,8.5f,"Chilled","Assets/Prefabs/IceCream.prefab"),
        new Section("S10","Condiments",-48.2f,1.1f,"Shelf","Assets/Prefabs/shaker-salt.prefab"),
        new Section("S11","Household Paper",-48.2f,-4.3f,"Shelf","Assets/Prefabs/Cute Supermarket Lite/Prefabs/roll_toilet.prefab"),
        new Section("S12","Kitchenware",-44.7f,1.1f,"Shelf","Assets/Prefabs/plate.prefab")
    };
    static void AddStructuralColliders(Scene scene)
    {
        var model=scene.GetRootGameObjects().Single(g=>g.name=="supermarket").transform.Find("supermarket");
        foreach(string name in new[]{"Cube.165","Cube.313"})
        {
            var part=model.Find(name);
            if(part==null || part.GetComponent<MeshFilter>()==null) throw new Exception("Structural mesh changed: "+name);
            var collider=part.GetComponent<MeshCollider>();
            if(collider==null) collider=part.gameObject.AddComponent<MeshCollider>();
            collider.sharedMesh=part.GetComponent<MeshFilter>().sharedMesh;
            collider.convex=false; collider.isTrigger=false; collider.enabled=true;
            PrefabUtility.RecordPrefabInstancePropertyModifications(collider);
        }
        Physics.SyncTransforms();
    }
    public static void Plan()
    {
        var scene=Open(ScenePath);
        AddStructuralColliders(scene);
        var shop=scene.GetRootGameObjects().Single(g=>g.name=="supermarket");
        var inventory=shop.transform.Find("ShoppingInventory");
        var b=new StringBuilder("# Prompt 4 geometry-validated spatial plan\n\n");
        b.AppendLine("Measured interior: the primary shopping hall lies east of the x=-49.62 glazed partition, inside the x=-43.36 east shell. Floor top is y=0.625. Roof-cut views show the western annex contains planters, tables and entrance furniture. Those modeled elements are retained. Twelve compact displays use two banks (x=-48.2 and -44.7), preserving the broad spawn/UI cross-aisle between z=1.1 and z=6.0, and the south cashier zone. No placement uses the exterior parking or annex planters.\n");
        b.AppendLine("| ID | Category | Center x,z | Furniture | Existing interactive product prefabs |\n|---|---|---|---|---|");
        foreach(var section in Sections)
        {
            var pos=new Vector3(section.x,.62505f,section.z);
            var blockers=Physics.OverlapBox(pos+Vector3.up*.35f,new Vector3(.725f,.30f,.325f),Quaternion.identity,~0,QueryTriggerInteraction.Ignore)
                .Where(c=>!c.transform.IsChildOf(inventory)).ToArray();
            if(blockers.Length>0) throw new Exception("Plan intersects actual colliders: "+section.id+" "+string.Join(",",blockers.Select(c=>PathOf(c.transform))));
            if(!Physics.Raycast(pos+Vector3.up*.15f,Vector3.down,out var hit,.3f,~0,QueryTriggerInteraction.Ignore) || Mathf.Abs(hit.point.y-.62505f)>.02f)
                throw new Exception("Missing floor at "+section.id);
            foreach(var path in section.productPaths)
                if(AssetDatabase.LoadAssetAtPath<GameObject>(path)==null) throw new Exception("Missing product "+path);
            b.AppendLine("| "+section.id+" | "+section.category+" | "+section.x.ToString("F2",System.Globalization.CultureInfo.InvariantCulture)+", "+section.z.ToString("F2",System.Globalization.CultureInfo.InvariantCulture)+" | "+section.furniture+" | "+string.Join("; ",section.productPaths)+" |");
        }
        b.AppendLine("\nPre-placement checks: 12 supported floor samples; no proposed fixture overlaps the existing structural colliders (moving Prompt 1 displays excluded). Typical fixture width 1.45m/depth 0.65m yields a 2.05m central aisle and at least 1.45m between successive display rows. Player root capsule is 0.392m diameter/1.0m high; the additional camera capsule and its tracked offset require separate compound collision checks after placement.\n");
        b.AppendLine("Preserve all ten original product identities with aggregate stock 12 each; add only turkey, IceCream, shaker-salt and plate, stock 12 each (14 identities/168 units total). Repair only new instance callbacks where source targets are null. Preserve the case/whitespace of existing identities through Level 3 view variants where needed. No category-aware distribution rule is added. Reuse the two existing produce displays/trays; ten further small existing furniture instances complete the section fixtures. Rebuild the existing Level 3 NavMeshSurface against effective colliders into a separate Level 3 nav asset.\n");
        File.WriteAllText(Evidence+"/spatial-plan.md",b.ToString());
    }
    const string TrayPath = "Assets/Prefabs/ThreeBox/Match3D Object Pack - Fruits and Vegetables/Resources/Prefabs/Add/StorageTray.prefab";
    static GameObject InstantiateAsset(string path, Scene scene, Transform parent, string name)
    {
        var asset=AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if(asset==null) throw new Exception("Missing asset "+path);
        var go=(GameObject)PrefabUtility.InstantiatePrefab(asset,scene);
        go.name=name; go.transform.SetParent(parent,true);
        return go;
    }
    static void Fit(GameObject go, Vector3 size, Vector3 bottom)
    {
        go.transform.SetPositionAndRotation(Vector3.zero,Quaternion.identity);
        go.transform.localScale=Vector3.one;
        var bounds=BoundsOf(go);
        go.transform.localScale=new Vector3(size.x/bounds.size.x,size.y/bounds.size.y,size.z/bounds.size.z);
        bounds=BoundsOf(go);
        go.transform.position += bottom-new Vector3(bounds.center.x,bounds.min.y,bounds.center.z);
        PrefabUtility.RecordPrefabInstancePropertyModifications(go.transform);
    }
    static void IgnoreNavigation(GameObject go)
    {
        var modifier=go.GetComponent<NavMeshModifier>();
        if(modifier==null) modifier=go.AddComponent<NavMeshModifier>();
        modifier.ignoreFromBuild=true;
    }
    static void RepairNewProduct(SelectableItem item)
    {
        var poke=item.GetComponent<PokeObjects>();
        if(poke==null) poke=item.gameObject.AddComponent<PokeObjects>();
        var xr=item.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable>();
        if(xr==null) throw new Exception("No interactable: "+item.name);
        while(xr.selectEntered.GetPersistentEventCount()>0) UnityEditor.Events.UnityEventTools.RemovePersistentListener(xr.selectEntered,0);
        while(xr.selectExited.GetPersistentEventCount()>0) UnityEditor.Events.UnityEventTools.RemovePersistentListener(xr.selectExited,0);
        UnityEditor.Events.UnityEventTools.AddPersistentListener(xr.selectEntered,poke.OnPokeStart);
        UnityEditor.Events.UnityEventTools.AddPersistentListener(xr.selectExited,poke.OnPokeEnd);
        PrefabUtility.RecordPrefabInstancePropertyModifications(xr);
        foreach(var trigger in item.GetComponents<CheckoutCounter>()) UnityEngine.Object.DestroyImmediate(trigger);
    }
    static void Sign(Transform parent, Section section, float height)
    {
        var sign=new GameObject("Category Sign"); sign.transform.SetParent(parent,true);
        sign.transform.position=new Vector3(section.x,height,section.z+.27f);
        IgnoreNavigation(sign);
        var text=sign.AddComponent<TextMesh>();
        text.text=section.id+"  "+section.category;
        text.font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize=64; text.characterSize=.019f; text.anchor=TextAnchor.MiddleCenter;
        text.alignment=TextAlignment.Center; text.color=Color.white;
        var renderer=sign.GetComponent<MeshRenderer>(); renderer.sharedMaterial=text.font.material;
    }
    public static void Install()
    {
        if(!File.Exists(Evidence+"/spatial-plan.md")) throw new Exception("Run geometry plan first.");
        var scene=Open(ScenePath);
        var shop=scene.GetRootGameObjects().Single(g=>g.name=="supermarket");
        var inventory=shop.transform.Find("ShoppingInventory");
        if(inventory.Find("S01_Vegetables")!=null) throw new Exception("Sections already authored; refusing duplicate installation.");
        var existing=inventory.GetComponentsInChildren<SelectableItem>(true);
        if(existing.Length!=10) throw new Exception("Prompt 1 baseline differs.");
        AddStructuralColliders(scene);
        var oldList=scene.GetRootGameObjects().Single(g=>g.name=="Supermarket").GetComponentsInChildren<ListController>(true).Single(l=>l.HasListContainer);
        var oldPool=new SerializedObject(oldList).FindProperty("availablePrefabs");
        var oldViews=Enumerable.Range(0,oldPool.arraySize).Select(i=>oldPool.GetArrayElementAtIndex(i).objectReferenceValue as GameObject).Where(v=>v!=null).ToArray();
        var activeList=shop.GetComponentsInChildren<ListController>(true).Single(l=>l.HasListContainer && l.gameObject.activeInHierarchy);
        var views=new System.Collections.Generic.List<GameObject>();
        Directory.CreateDirectory("Assets/Level3Layout/Views"); AssetDatabase.Refresh();
        int produce=0;
        foreach(var section in Sections)
        {
            var root=new GameObject(section.Name); root.transform.SetParent(inventory,true);
            root.transform.position=new Vector3(section.x,.62505f,section.z);
            GameObject furniture, tray=null;
            if(section.furniture=="Produce")
            {
                produce++;
                furniture=inventory.Find("DisplayShelf "+produce).gameObject;
                tray=inventory.Find("StockTray "+produce).gameObject;
                furniture.transform.SetParent(root.transform,true); tray.transform.SetParent(root.transform,true);
                Fit(furniture,new Vector3(1.45f,.4f,.65f),root.transform.position);
            }
            else
            {
                furniture=InstantiateAsset(section.furniture=="Chilled"?"Assets/Prefabs/Supermercado/Frezer2.fbx":"Assets/Prefabs/Supermercado/shelfMarker.fbx",scene,root.transform,section.furniture=="Chilled"?"Chilled Display":"Gondola Display");
                Fit(furniture,new Vector3(1.45f,section.furniture=="Chilled"?.45f:.55f,.65f),root.transform.position);
                if(section.furniture=="Chilled") tray=InstantiateAsset(TrayPath,scene,root.transform,"Stock Tray");
            }
            // Replace only display-instance collision overrides with a fitted primitive body.
            foreach(var collider in furniture.GetComponentsInChildren<Collider>()) collider.enabled=false;
            var body=new GameObject("Display Body Collider"); body.transform.SetParent(root.transform,true);
            var bounds=BoundsOf(furniture);
            body.transform.position=bounds.center;
            var box=body.AddComponent<BoxCollider>(); box.size=bounds.size;
            var nav=body.AddComponent<NavMeshModifier>(); nav.overrideArea=true; nav.area=1;
            IgnoreNavigation(furniture);
            float top=bounds.max.y;
            if(tray!=null)
            {
                foreach(var collider in tray.GetComponentsInChildren<Collider>()) collider.enabled=false;
                Fit(tray,new Vector3(1.3f,.025f,.5f),new Vector3(section.x,top,section.z));
                // The thin tray is visible support; the body collider extends to its top.
                top=BoundsOf(tray).max.y;
                box.size=new Vector3(bounds.size.x,top-.62505f,bounds.size.z);
                body.transform.position=new Vector3(section.x,(top+.62505f)*.5f,section.z);
                IgnoreNavigation(tray);
            }
            var productGroup=new GameObject("Products"); productGroup.transform.SetParent(root.transform,false); IgnoreNavigation(productGroup);
            for(int i=0;i<section.productPaths.Length;i++)
            {
                var path=section.productPaths[i];
                var item=existing.SingleOrDefault(p=>PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(p.gameObject)==path);
                bool added=item==null;
                if(added) item=InstantiateAsset(path,scene,productGroup.transform,Path.GetFileNameWithoutExtension(path)).GetComponentInChildren<SelectableItem>(true);
                else item.transform.SetParent(productGroup.transform,true);
                if(item==null) throw new Exception("Missing item: "+path);
                if(added) RepairNewProduct(item);
                item.stockQuantity=12;
                item.transform.SetPositionAndRotation(Vector3.zero,Quaternion.identity);
                item.transform.localScale=Vector3.one;
                var productBounds=BoundsOf(item.gameObject);
                item.transform.localScale=Vector3.one*(.2f/Mathf.Max(productBounds.size.x,productBounds.size.y,productBounds.size.z));
                productBounds=BoundsOf(item.gameObject);
                float x=section.x+(i-(section.productPaths.Length-1)*.5f)*.38f;
                item.transform.position+=new Vector3(x,top+.012f,section.z-.08f)-new Vector3(productBounds.center.x,productBounds.min.y,productBounds.center.z);
                PrefabUtility.RecordPrefabInstancePropertyModifications(item.transform);
                PrefabUtility.RecordPrefabInstancePropertyModifications(item);
                var view=oldViews.FirstOrDefault(v=>v.name==item.itemName);
                if(view==null)
                {
                    // Legacy identity casing/trailing spaces are retained. Create a local view variant.
                    var source=oldViews.FirstOrDefault(v=>string.Equals(v.name.Trim(),item.itemName.Trim(),StringComparison.OrdinalIgnoreCase));
                    if(source==null) throw new Exception("No list view for exact identity: ["+item.itemName+"]");
                    var temporary=(GameObject)PrefabUtility.InstantiatePrefab(source,scene);
                    temporary.name=item.itemName;
                    view=PrefabUtility.SaveAsPrefabAsset(temporary,"Assets/Level3Layout/Views/"+item.itemName+".prefab");
                    UnityEngine.Object.DestroyImmediate(temporary);
                }
                views.Add(view);
            }
            var approach=new GameObject("Approach"); approach.transform.SetParent(root.transform,true);
            // Camera-floor marker; collision validation transforms this into the actual rig/root pose.
            approach.transform.position=new Vector3(section.x+.1f,.62505f,section.z-.85f);
            Sign(root.transform,section,top+.34f);
        }
        var listData=new SerializedObject(activeList); var pool=listData.FindProperty("availablePrefabs");
        pool.arraySize=views.Count;
        for(int i=0;i<views.Count;i++) pool.GetArrayElementAtIndex(i).objectReferenceValue=views[i];
        listData.ApplyModifiedPropertiesWithoutUndo(); PrefabUtility.RecordPrefabInstancePropertyModifications(activeList);
        Physics.SyncTransforms();
        // Keep the existing Level 3 surface; bake actual solid geometry and exclude movable stock/signs.
        var surface=shop.GetComponentsInChildren<NavMeshSurface>(true).Single();
        surface.useGeometry=UnityEngine.AI.NavMeshCollectGeometry.PhysicsColliders;
        surface.BuildNavMesh();
        if(surface.navMeshData==null) throw new Exception("NavMesh build failed.");
        AssetDatabase.CreateAsset(surface.navMeshData,"Assets/Scenes/Scene-level-3/NavMesh-Level3Sections.asset");
        EditorUtility.SetDirty(surface); PrefabUtility.RecordPrefabInstancePropertyModifications(surface);
        EditorSceneManager.MarkSceneDirty(scene);
        if(!EditorSceneManager.SaveScene(scene)) throw new Exception("Scene save failed.");
        AssetDatabase.SaveAssets();
        Describe();
    }
    public static void Describe()
    {
        var scene=Open(ScenePath);
        var inventory=scene.GetRootGameObjects().Single(g=>g.name=="supermarket").transform.Find("ShoppingInventory");
        var b=new StringBuilder("ID\tCategory\tPath\tPosition\tProducts\tStock\n");
        foreach(var section in Sections)
        {
            var root=inventory.Find(section.Name);
            var products=root.GetComponentsInChildren<SelectableItem>(true);
            b.AppendLine(section.id+"\t"+section.category+"\t"+PathOf(root)+"\t"+root.position.ToString("F3",System.Globalization.CultureInfo.InvariantCulture)+"\t"+string.Join(";",products.Select(p=>"["+p.itemName+"] @ "+p.transform.position.ToString("F3",System.Globalization.CultureInfo.InvariantCulture)))+"\t"+products.Sum(p=>p.stockQuantity));
        }
        File.WriteAllText(Evidence+"/section-inventory.tsv",b.ToString());
    }
    public static void AdjustBackSections()
    {
        var scene=Open(ScenePath);
        var inventory=scene.GetRootGameObjects().Single(g=>g.name=="supermarket").transform.Find("ShoppingInventory");
        foreach(var id in new[]{"S03","S06"})
        {
            var spec=Sections.Single(s=>s.id==id);
            inventory.Find(spec.Name).position=new Vector3(spec.x,.62505f,spec.z);
        }
        var surface=scene.GetRootGameObjects().Single(g=>g.name=="supermarket").GetComponentsInChildren<NavMeshSurface>(true).Single();
        surface.BuildNavMesh();
        AssetDatabase.CreateAsset(surface.navMeshData,"Assets/Scenes/Scene-level-3/NavMesh-Level3Sections.asset");
        EditorUtility.SetDirty(surface); PrefabUtility.RecordPrefabInstancePropertyModifications(surface);
        EditorSceneManager.MarkSceneDirty(scene);
        if(!EditorSceneManager.SaveScene(scene)) throw new Exception("Save failed");
        AssetDatabase.SaveAssets();
        File.WriteAllText(Evidence+"/plan-refinement.txt","Route validation found the internal north wall between the original row z=8.5 and z=10.6. Collision-volume overlap alone did not classify inside/outside. Moved S03 water to the front-right bay (-44.7,-6.5) and S06 dairy to the right-side bay (-44.7,4.0), preserving the original spawn/UI cross-aisle and cashier. Both sections, their displays, stock and approach markers move together. Rebuilt the same Level 3 surface. Final layout is five west-bank and seven east-bank sections.");
        ValidateAndRender();
    }
    public static string MeasureAccess(Scene scene, bool writeEvidence)
    {
        Physics.SyncTransforms();
        var rig=scene.GetRootGameObjects().Single(g=>g.name=="XR Origin (XR Rig)").transform;
        var camera=rig.GetComponentsInChildren<Camera>(true).Single(c=>c.CompareTag("MainCamera"));
        var capsules=rig.GetComponentsInChildren<CharacterController>(true);
        var inventory=scene.GetRootGameObjects().Single(g=>g.name=="supermarket").transform.Find("ShoppingInventory");
        var offset=camera.transform.position-rig.position;
        const float step=.15f,minX=-49.35f,minZ=-6.7f;
        const int width=38,height=120;
        var clear=new bool[width*height]; var reached=new bool[clear.Length]; var parent=new int[clear.Length];
        Func<int,Vector3> position=index=>new Vector3(minX+(index%width)*step,rig.position.y,minZ+(index/width)*step);
        Func<Vector3,int> nearest=p=>Mathf.Clamp(Mathf.RoundToInt((p.z-minZ)/step),0,height-1)*width+Mathf.Clamp(Mathf.RoundToInt((p.x-minX)/step),0,width-1);
        Func<Collider,bool> blocker=c=>c!=null && c.gameObject.scene==scene && !c.transform.IsChildOf(rig) && !c.isTrigger;
        Func<Vector3,bool> floor=p=>Physics.Raycast(new Vector3(p.x,.77505f,p.z),Vector3.down,out var hit,.3f,~0,QueryTriggerInteraction.Ignore) && Mathf.Abs(hit.point.y-.62505f)<.02f;
        Func<Vector3,Vector3,bool> sweep=(from,to)=>
        {
            var delta=from-rig.position+Vector3.up*.025f;
            var move=to-from;
            foreach(var capsule in capsules)
            {
                var scale=capsule.transform.lossyScale;
                float radius=capsule.radius*Mathf.Max(Mathf.Abs(scale.x),Mathf.Abs(scale.z));
                float half=Mathf.Max(radius,capsule.height*Mathf.Abs(scale.y)*.5f);
                var center=capsule.transform.TransformPoint(capsule.center)+delta;
                var a=center+Vector3.up*(half-radius); var b=center-Vector3.up*(half-radius);
                if(move.sqrMagnitude<.000001f)
                {
                    if(Physics.OverlapCapsule(a,b,radius+.02f,~0,QueryTriggerInteraction.Ignore).Any(blocker)) return false;
                }
                else if(Physics.CapsuleCastAll(a,b,radius+.02f,move.normalized,move.magnitude,~0,QueryTriggerInteraction.Ignore).Any(h=>blocker(h.collider))) return false;
            }
            return floor(to) && floor((from+to)*.5f);
        };
        for(int i=0;i<clear.Length;i++) clear[i]=sweep(position(i),position(i));
        int start=nearest(rig.position);
        if(!clear[start] || !sweep(rig.position,position(start))) throw new Exception("Spawn compound capsules obstructed.");
        var queue=new System.Collections.Generic.Queue<int>(); queue.Enqueue(start); reached[start]=true; parent[start]=-1;
        while(queue.Count>0)
        {
            int current=queue.Dequeue(); int x=current%width,z=current/width;
            foreach(int next in new[]{x>0?current-1:-1,x<width-1?current+1:-1,z>0?current-width:-1,z<height-1?current+width:-1})
            {
                if(next<0 || reached[next] || !clear[next]) continue;
                if(!sweep(position(current),position(next))) continue;
                reached[next]=true; parent[next]=current; queue.Enqueue(next);
            }
        }
        var report=new StringBuilder("Collision-aware access: two existing CharacterController capsules, actual world offsets, +0.02m radius margin, 0.15m grid with swept edges and floor samples. Current rig orientation/tracked pose only.\n");
        report.AppendLine("Root="+rig.position.ToString("F3")+" cameraOffset="+offset.ToString("F3")+" reachedNodes="+reached.Count(v=>v));
        foreach(var c in capsules) report.AppendLine("CAPSULE "+PathOf(c.transform)+" radiusWorld="+(c.radius*Mathf.Abs(c.transform.lossyScale.x))+" heightWorld="+(c.height*Mathf.Abs(c.transform.lossyScale.y)));
        if(writeEvidence) File.WriteAllLines(Evidence+"/collision-grid.tsv",Enumerable.Range(0,clear.Length).Select(i=>position(i).ToString("F3",System.Globalization.CultureInfo.InvariantCulture)+"\t"+clear[i]+"\t"+reached[i]));
        var paths=new System.Collections.Generic.List<Vector3[]>();
        foreach(var section in Sections)
        {
            var root=inventory.Find(section.Name);
            var approach=root.Find("Approach").position;
            var rootTarget=new Vector3(approach.x-offset.x,rig.position.y,approach.z-offset.z);
            int goal=nearest(rootTarget);
            if(!reached[goal] || !sweep(position(goal),rootTarget)) throw new Exception("No compound-capsule path to "+section.id+" rootTarget="+rootTarget+" nearest="+position(goal)+" nearestClear="+clear[goal]+" reached="+reached[goal]+" exactClear="+sweep(rootTarget,rootTarget)+" endpointSweep="+sweep(position(goal),rootTarget));
            var route=new System.Collections.Generic.List<Vector3>{rootTarget};
            for(int cursor=goal;cursor>=0;cursor=parent[cursor]) route.Add(position(cursor));
            route.Reverse(); paths.Add(route.ToArray());
            float length=0; for(int i=1;i<route.Count;i++) length+=Vector3.Distance(route[i-1],route[i]);
            var backApproach=root.Find("Approach Back");
            if(backApproach!=null)
            {
                var point=backApproach.position;var backTarget=new Vector3(point.x-offset.x,rig.position.y,point.z-offset.z);int backGoal=nearest(backTarget);
                if(!reached[backGoal] || !sweep(position(backGoal),backTarget)) throw new Exception("No compound-capsule path to rear produce approach: "+section.id);
                report.AppendLine(section.id+" rear produce approach reachable, root="+backTarget.ToString("F3"));
            }
            bool facingsVerified=false;
            foreach(var item in Level3VisualRevisionTools.Owners(root))
            {
                var target=BoundsOf(item.gameObject).center;
                var eye=new Vector3(approach.x,camera.transform.position.y,approach.z);
                var direction=target-eye;
                var hits=Physics.RaycastAll(eye,direction.normalized,direction.magnitude+.02f,Physics.DefaultRaycastLayers,QueryTriggerInteraction.Ignore)
                    .Where(h=>blocker(h.collider)).OrderBy(h=>h.distance).ToArray();
                if(hits.Length==0 || !hits[0].transform.IsChildOf(item.transform))
                {
                    if(section.furniture!="Produce") throw new Exception("Product line of sight obstructed: "+section.id+" "+item.itemName+" first="+(hits.Length==0?"none":PathOf(hits[0].transform)));
                    // A low fruit's centre can be behind a bin rim. Require the stronger
                    // full per-unit mesh target/depletion audit rather than treating it as unreachable.
                    if(!facingsVerified) { Level3VisualRevisionTools.MeasureFacingAccess(scene); facingsVerified=true; }
                    report.AppendLine(section.id+" owner centre occluded; all individual mesh targets verified in depletion order.");
                }
                if(direction.magnitude>1.35f) throw new Exception("Long product approach ray: "+section.id);
                report.AppendLine(section.id+" "+item.itemName+" root="+rootTarget.ToString("F3")+" route="+length.ToString("F2",System.Globalization.CultureInfo.InvariantCulture)+"m targetRange="+direction.magnitude.ToString("F2",System.Globalization.CultureInfo.InvariantCulture)+"m");
            }
            var npc=scene.GetRootGameObjects().Single(g=>g.name=="NPC_2");
            if(!UnityEngine.AI.NavMesh.SamplePosition(npc.transform.position,out var npcHit,.3f,UnityEngine.AI.NavMesh.AllAreas)) throw new Exception("NPC spawn off NavMesh.");
            if(!UnityEngine.AI.NavMesh.SamplePosition(approach,out var approachHit,.6f,UnityEngine.AI.NavMesh.AllAreas)) throw new Exception("NPC approach off NavMesh: "+section.id);
            var navPath=new UnityEngine.AI.NavMeshPath();
            if(!UnityEngine.AI.NavMesh.CalculatePath(npcHit.position,approachHit.position,UnityEngine.AI.NavMesh.AllAreas,navPath) || navPath.status!=UnityEngine.AI.NavMeshPathStatus.PathComplete)
                throw new Exception("NPC path incomplete: "+section.id);
        }
        var checkout=scene.GetRootGameObjects().SelectMany(r=>r.GetComponentsInChildren<VRCheckoutTeleport>(true)).Single();
        int best=-1; float distance=float.PositiveInfinity;
        for(int i=0;i<reached.Length;i++) if(reached[i])
        {
            var eye=position(i)+offset;
            float d=Vector2.Distance(new Vector2(eye.x,eye.z),new Vector2(checkout.poiChargeMoney.position.x,checkout.poiChargeMoney.position.z));
            if(d<distance){distance=d;best=i;}
        }
        if(best<0 || distance>1f) throw new Exception("Checkout customer zone not reachable within 1m: "+distance);
        report.AppendLine("CHECKOUT reachable camera-floor point="+(position(best)+offset).ToString("F3")+" horizontal distance to unchanged POI="+distance.ToString("F3",System.Globalization.CultureInfo.InvariantCulture)+"m");
        report.AppendLine("PASS: 12 section approaches, stock-owner product rays <=1.35m, NPC NavMesh paths to all 12 approaches, spawn and customer-side checkout access.");
        if(writeEvidence)
        {
            File.WriteAllText(Evidence+"/accessibility.txt",report.ToString());
            WriteMap(inventory,paths,rig.position);
        }
        return report.ToString();
    }
    static void WriteMap(Transform inventory,System.Collections.Generic.List<Vector3[]> paths,Vector3 spawn)
    {
        Func<Vector3,string> point=p=>((p.x+50f)*85f).ToString("F1",System.Globalization.CultureInfo.InvariantCulture)+","+((12f-p.z)*60f).ToString("F1",System.Globalization.CultureInfo.InvariantCulture);
        var svg=new StringBuilder("<svg xmlns='http://www.w3.org/2000/svg' width='700' height='1240' viewBox='0 0 700 1240'><rect width='700' height='1240' fill='#111e29'/><text x='30' y='30' fill='white' font-size='20'>Level 3: twelve stocked sections</text><text x='30' y='52' fill='#bacbd6' font-size='12'>Measured world layout; green lines are collision-checked rig-root routes.</text>");
        foreach(var route in paths) svg.Append("<polyline fill='none' stroke='#52bf9a' stroke-opacity='.45' stroke-width='2' points='").Append(string.Join(" ",route.Select(point))).Append("'/>");
        foreach(var section in Sections)
        {
            var pos=inventory.Find(section.Name).position;
            float x=(pos.x+50)*85,z=(12-pos.z)*60;
            string color=section.furniture=="Chilled"?"#92cbdf":section.furniture=="Produce"?"#a8d697":"#e3c49a";
            svg.AppendFormat(System.Globalization.CultureInfo.InvariantCulture,"<rect x='{0:F1}' y='{1:F1}' width='123.25' height='39' rx='3' fill='{2}'/><text x='{3:F1}' y='{4:F1}' fill='#102631' text-anchor='middle' font-size='12'>{5}</text><text x='{3:F1}' y='{6:F1}' fill='#102631' text-anchor='middle' font-size='11'>{7}</text>",x-61.625f,z-19.5f,color,x,z-3,section.id,z+12,section.category);
        }
        svg.Append("<circle cx='").Append(point(spawn).Split(',')[0]).Append("' cy='").Append(point(spawn).Split(',')[1]).Append("' r='7' fill='#f6c95d'/>");
        svg.Append("<text x='30' y='1160' fill='white' font-size='12'>Yellow: unchanged rig spawn. South checkout: unchanged anchors/cashier.</text><text x='30' y='1180' fill='#bacbd6' font-size='12'>Collision checks retain both capsule offsets; this is not hardware XR validation.</text></svg>");
        File.WriteAllText(Evidence+"/layout-and-routes.svg",svg.ToString());
    }
    public static void ValidateAndRender()
    {
        var scene=Open(ScenePath);
        Describe();
        Render("layout-cutaway",new Vector3(-46.6f,2.75f,1.5f),new Vector3(-46.6f,0,1.5f),true,1000,1800,10.5f);
        Render("north-aisle-eye",new Vector3(-46.5f,1.79f,3.5f),new Vector3(-46.5f,1.15f,9),false);
        Render("south-aisle-eye",new Vector3(-46.5f,1.79f,3.5f),new Vector3(-46.5f,1.15f,-4.3f),false);
        foreach(var id in new[]{"S01","S07","S10","S12"})
        {
            var section=Sections.Single(s=>s.id==id);
            Render(id+"-display-eye",new Vector3(section.x+.1f,1.79f,section.z-1.1f),new Vector3(section.x,1.15f,section.z),false);
        }
        MeasureAccess(scene,true);
    }
    public static void RepairCatalogNames()
    {
        var scene=Open(ScenePath);
        var inventory=scene.GetRootGameObjects().Single(g=>g.name=="supermarket").transform.Find("ShoppingInventory");
        foreach(var id in new[]{"S10","S12"})
        {
            var section=Sections.Single(s=>s.id==id);
            var item=inventory.Find(section.Name).GetComponentInChildren<SelectableItem>();
            string oldPath="Assets/Level3Layout/Views/"+id+"_ItemView.prefab";
            string error=AssetDatabase.RenameAsset(oldPath,item.itemName);
            if(!string.IsNullOrEmpty(error)) throw new Exception(error);
            var path="Assets/Level3Layout/Views/"+item.itemName+".prefab";
            AssetDatabase.ImportAsset(path,ImportAssetOptions.ForceSynchronousImport);
            var view=AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if(view==null || view.name!=item.itemName) throw new Exception("Exact view identity still differs: ["+item.itemName+"] -> ["+view?.name+"]");
        }
        AssetDatabase.SaveAssets();
        FinalAudit();
    }
    public static void FinalAudit()
    {
        Level3StabilityTools.Inspect();
        var scene=SceneManager.GetSceneByPath(ScenePath);
        MeasureAccess(scene,true);
        Describe();
        var b=new StringBuilder("Current serialized XR interactor configuration\n");
        foreach(var root in scene.GetRootGameObjects()) foreach(var component in root.GetComponentsInChildren<MonoBehaviour>(true))
        {
            if(component==null || (!component.GetType().Name.Contains("Interactor") && !component.GetType().Name.Contains("RayCaster"))) continue;
            b.AppendLine(component.GetType().Name+" active="+component.isActiveAndEnabled+" path="+PathOf(component.transform));
            var property=new SerializedObject(component).GetIterator();
            while(property.NextVisible(true))
            {
                if(property.propertyType==SerializedPropertyType.Float && (property.name.IndexOf("Distance",StringComparison.OrdinalIgnoreCase)>=0 || property.name.IndexOf("Radius",StringComparison.OrdinalIgnoreCase)>=0))
                    b.AppendLine(" "+property.propertyPath+"="+property.floatValue);
            }
        }
        File.WriteAllText(Evidence+"/xr-interactor-configuration.txt",b.ToString());
        CompareLevel2();
    }
    public static void CompareLevel2()
    {
        var scene=Open("Assets/Scenes/Scene-level-2.unity");
        var items=scene.GetRootGameObjects().SelectMany(r=>r.GetComponentsInChildren<SelectableItem>(true)).Where(p=>p.isActiveAndEnabled).ToArray();
        var report=new StringBuilder("Category\tLevel2 bounds min\tLevel2 bounds max\tLevel2 product count\tLevel2 paths\tLevel3 section center\n");
        foreach(var section in Sections)
        {
            var matching=items.Where(p=>section.productPaths.Contains(PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(p.gameObject))).ToArray();
            if(matching.Length==0) throw new Exception("No active Level 2 comparison for "+section.id);
            var min=matching.Select(p=>p.transform.position).Aggregate(Vector3.Min); var max=matching.Select(p=>p.transform.position).Aggregate(Vector3.Max);
            report.AppendLine(section.category+"\t"+min.ToString("F3",System.Globalization.CultureInfo.InvariantCulture)+"\t"+max.ToString("F3",System.Globalization.CultureInfo.InvariantCulture)+"\t"+matching.Length+"\t"+string.Join(";",matching.Select(p=>PathOf(p.transform)).Distinct())+"\t"+new Vector3(section.x,.62505f,section.z).ToString("F3",System.Globalization.CultureInfo.InvariantCulture));
        }
        File.WriteAllText(Evidence+"/level2-level3-comparison.tsv",report.ToString());
    }
    public static void Render(string name, Vector3 position, Vector3 target, bool orthographic, int width=1600, int height=1200, float size=11.5f)
    {
        var go = new GameObject("Temporary layout evidence camera") { hideFlags = HideFlags.HideAndDontSave };
        var previous = RenderTexture.active;
        var texture = new RenderTexture(width, height, 24);
        Texture2D image = null;
        try {
            var camera = go.AddComponent<Camera>();
            camera.transform.position = position;
            camera.transform.LookAt(target, orthographic ? Vector3.forward : Vector3.up);
            camera.orthographic = orthographic; camera.orthographicSize = size;
            camera.nearClipPlane = .03f; camera.farClipPlane = 80;
            camera.clearFlags = CameraClearFlags.SolidColor; camera.backgroundColor = new Color(.15f,.18f,.22f);
            camera.targetTexture = texture;
            camera.Render(); RenderTexture.active = texture;
            image = new Texture2D(texture.width,texture.height,TextureFormat.RGB24,false);
            image.ReadPixels(new Rect(0,0,texture.width,texture.height),0,0); image.Apply();
            File.WriteAllBytes(Evidence + "/" + name + ".png", image.EncodeToPNG());
        } finally { RenderTexture.active=previous; if(image!=null) UnityEngine.Object.DestroyImmediate(image); UnityEngine.Object.DestroyImmediate(texture); UnityEngine.Object.DestroyImmediate(go); }
    }
}









