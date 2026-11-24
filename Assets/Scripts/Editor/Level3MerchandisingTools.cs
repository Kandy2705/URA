using System;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public static class Level3MerchandisingTools
{
    public const string Evidence = "Evidence/Level3Prompt4_5";
    public static Scene Open()
    {
        if(EditorApplication.isPlayingOrWillChangePlaymode) throw new Exception("Stop Play Mode first.");
        for(int i=0;i<SceneManager.sceneCount;i++) if(SceneManager.GetSceneAt(i).isDirty) throw new Exception("Unsaved scene; save/review it first.");
        var scene=SceneManager.GetSceneByPath(Level3LayoutTools.ScenePath);
        return scene.isLoaded ? scene : EditorSceneManager.OpenScene(Level3LayoutTools.ScenePath);
    }
    public static Transform Inventory(Scene scene) => scene.GetRootGameObjects().Single(g=>g.name=="supermarket").transform.Find("ShoppingInventory");
    public static string Counts(Scene scene)
    {
        var gos=scene.GetRootGameObjects().SelectMany(g=>g.GetComponentsInChildren<Transform>(true)).Where(t=>t.gameObject.activeInHierarchy).Select(t=>t.gameObject).ToArray();
        return "Active GameObjects="+gos.Length+"; Renderers="+gos.Sum(g=>g.GetComponents<Renderer>().Length)+"; Colliders="+gos.Sum(g=>g.GetComponents<Collider>().Length)+"; Rigidbodies="+gos.Sum(g=>g.GetComponents<Rigidbody>().Length)+"; XRInteractables="+gos.Sum(g=>g.GetComponents<XRBaseInteractable>().Length)+"; SelectableItems="+gos.Sum(g=>g.GetComponents<SelectableItem>().Length);
    }
    [MenuItem("Tools/Level 3/Merchandising/1 Capture before")]
    public static void Before()
    {
        var scene=Open(); Directory.CreateDirectory(Evidence);
        if(Inventory(scene).GetComponentsInChildren<SelectableItem>().Length!=14) throw new Exception("Expected fourteen anchors.");
        if(Inventory(scene).GetComponentsInChildren<Transform>().Any(t=>t.name=="Decorative Stock")) throw new Exception("Already stocked; baseline capture refused.");
        if(File.Exists(Evidence+"/before-counts.txt")) throw new Exception("Baseline already captured.");
        File.Copy(Level3LayoutTools.ScenePath,"Library/Level3Prompt4_5-before.unity",false);
        File.WriteAllText(Evidence+"/before-counts.txt",Counts(scene));
        var b=new StringBuilder();
        foreach(var spec in Level3LayoutTools.Sections)
        {
            var section=Inventory(scene).Find(spec.Name);
            foreach(var r in section.GetComponentsInChildren<Renderer>()) b.AppendLine(Level3LayoutTools.PathOf(r.transform)+" "+r.bounds);
            foreach(var item in section.GetComponentsInChildren<SelectableItem>()) b.AppendLine("ANCHOR "+item.itemName+" stock="+item.stockQuantity+" hover="+item.GetComponentsInChildren<XRHighlightOnHover>().Length+" source="+PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(item.gameObject));
        }
        File.WriteAllText(Evidence+"/baseline-displays.txt",b.ToString());
        Capture(scene,"before");
    }
    // Copy only render components by construction. No prefab scripts, events or physics enter decorations.
    static GameObject Visual(GameObject source,Transform parent,string name)
    {
        var go=new GameObject(name); go.transform.SetParent(parent,false); go.isStatic=true;
        foreach(var filter in source.GetComponentsInChildren<MeshFilter>(true))
        {
            var renderer=filter.GetComponent<MeshRenderer>();
            if(filter.sharedMesh==null || renderer==null || !renderer.enabled) continue;
            var child=new GameObject(filter.name); child.transform.SetParent(go.transform,false); child.isStatic=true;
            var relative=source.transform.worldToLocalMatrix*filter.transform.localToWorldMatrix;
            child.transform.localPosition=relative.GetColumn(3); child.transform.localRotation=relative.rotation; child.transform.localScale=relative.lossyScale;
            child.AddComponent<MeshFilter>().sharedMesh=filter.sharedMesh;
            var copy=child.AddComponent<MeshRenderer>(); copy.sharedMaterials=renderer.sharedMaterials;
            copy.shadowCastingMode=UnityEngine.Rendering.ShadowCastingMode.Off;
        }
        return go;
    }
    // Vertical intersection with actual rendered support triangles, not the fitted collision box.
    public static float SupportHeight(Transform section,float x,float z)
    {
        float top=float.NegativeInfinity;
        foreach(var filter in section.GetComponentsInChildren<MeshFilter>())
        {
            if(filter.transform.IsChildOf(section.Find("Products")) || (section.Find("Decorative Stock")!=null && filter.transform.IsChildOf(section.Find("Decorative Stock")))) continue;
            var mesh=filter.sharedMesh; if(mesh==null) continue;
            var vertices=mesh.vertices; var triangles=mesh.triangles;
            for(int i=0;i<triangles.Length;i+=3)
            {
                var a=filter.transform.TransformPoint(vertices[triangles[i]]);
                var b=filter.transform.TransformPoint(vertices[triangles[i+1]]);
                var c=filter.transform.TransformPoint(vertices[triangles[i+2]]);
                float den=(b.z-c.z)*(a.x-c.x)+(c.x-b.x)*(a.z-c.z);
                if(Mathf.Abs(den)<.00000001f) continue;
                float u=((b.z-c.z)*(x-c.x)+(c.x-b.x)*(z-c.z))/den;
                float v=((c.z-a.z)*(x-c.x)+(a.x-c.x)*(z-c.z))/den;
                float w=1-u-v;
                if(u>=-.00001f && v>=-.00001f && w>=-.00001f) top=Mathf.Max(top,u*a.y+v*b.y+w*c.y);
            }
        }
        if(float.IsNegativeInfinity(top)) throw new Exception("No rendered support: "+section.name+" "+x+","+z);
        return top;
    }
    static string Extra(string id)
    {
        switch(id) {
            case "S01": return "Assets/Prefabs/lettuce.prefab";
            case "S02": return "Assets/Prefabs/grapes.prefab";
            case "S07": return "Assets/Prefabs/meat-sausage.prefab";
            case "S10": return "Assets/Prefabs/Cute Supermarket Lite/Prefabs/dressing_mayo.prefab";
            case "S12": return "Assets/Prefabs/kenney_food-kit/Models/FBX format/cup.fbx";
            default: return null;
        }
    }
    static void AddShelfSupport(Transform section,Level3LayoutTools.Section spec)
    {
        var source=AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/ThreeBox/Match3D Object Pack - Fruits and Vegetables/Resources/Prefabs/Add/StorageTray.prefab");
        if(source==null) throw new Exception("Missing support tray source.");
        var tray=Visual(source,section,"Merchandising Support");
        tray.AddComponent<Unity.AI.Navigation.NavMeshModifier>().ignoreFromBuild=true;
        var bounds=Level3LayoutTools.BoundsOf(tray);
        tray.transform.localScale=new Vector3(1.3f/bounds.size.x,.025f/bounds.size.y,.5f/bounds.size.z);
        bounds=Level3LayoutTools.BoundsOf(tray);
        var body=section.Find("Display Body Collider").GetComponent<BoxCollider>().bounds;
        tray.transform.position+=new Vector3(spec.x,body.max.y+.003f,spec.z)-new Vector3(bounds.center.x,bounds.min.y,bounds.center.z);
        foreach(var anchor in section.GetComponentsInChildren<SelectableItem>())
        {
            var ab=Level3LayoutTools.BoundsOf(anchor.gameObject);
            anchor.transform.position+=Vector3.up*(SupportHeight(section,ab.center.x,ab.center.z)+.012f-ab.min.y);
            PrefabUtility.RecordPrefabInstancePropertyModifications(anchor.transform);
        }
    }
    [MenuItem("Tools/Level 3/Merchandising/2 Stock displays")]
    public static void Install()
    {
        var scene=Open(); var inventory=Inventory(scene);
        if(!File.Exists(Evidence+"/before-counts.txt")) throw new Exception("Capture baseline first.");
        if(inventory.GetComponentsInChildren<Transform>(true).Any(t=>t.name=="Decorative Stock")) throw new Exception("Refusing duplicate stocking.");
        foreach(var spec in Level3LayoutTools.Sections)
        {
            var section=inventory.Find(spec.Name);
            var anchors=section.GetComponentsInChildren<SelectableItem>();
            if(spec.furniture=="Shelf") AddShelfSupport(section,spec);
            var group=new GameObject("Decorative Stock"); group.transform.SetParent(section,false); group.isStatic=true;
            // Same exclusion used by the existing Products group. This is not collision geometry.
            group.AddComponent<Unity.AI.Navigation.NavMeshModifier>().ignoreFromBuild=true;
            var sources=anchors.Select(a=>a.gameObject).ToList();
            var path=Extra(spec.id); var extra=path==null?null:AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if(path!=null && extra==null) throw new Exception("Missing visual source "+path);
            if(extra!=null) sources.Add(extra);
            bool bulky=spec.id=="S07" || spec.id=="S08" || spec.id=="S11" || spec.id=="S12";
            var xs=bulky?new[]{-.50f,-.25f,0,.25f,.50f}:new[]{-.54f,-.36f,-.18f,0,.18f,.36f,.54f};
            var zs=bulky?new[]{-.15f,.15f}:new[]{-.16f,0,.16f};
            int number=0;
            foreach(float z in zs) foreach(float x in xs)
            {
                var source=sources[number%sources.Count];
                var facing=Visual(source,group.transform,"Facing "+(number+1).ToString("D2")+" - "+source.name);
                var bounds=Level3LayoutTools.BoundsOf(facing);
                float scale=Mathf.Min((bulky?.20f:.165f)/Mathf.Max(bounds.size.x,bounds.size.y,bounds.size.z),.135f/bounds.size.z);
                facing.transform.localScale*=scale; bounds=Level3LayoutTools.BoundsOf(facing);
                float top=SupportHeight(section,spec.x+x,spec.z+z);
                facing.transform.position+=new Vector3(spec.x+x,top+.004f,spec.z+z)-new Vector3(bounds.center.x,bounds.min.y,bounds.center.z);
                bounds=Level3LayoutTools.BoundsOf(facing);
                if(anchors.Any(a=> {var ab=Level3LayoutTools.BoundsOf(a.gameObject);ab.Expand(new Vector3(.07f,0,.07f));return bounds.min.x<ab.max.x && bounds.max.x>ab.min.x && bounds.min.z<ab.max.z && bounds.max.z>ab.min.z;}))
                { UnityEngine.Object.DestroyImmediate(facing); continue; }
                number++;
            }
            if(number<5) throw new Exception("Insufficient facings "+spec.id+": "+number);
        }
        EditorSceneManager.MarkSceneDirty(scene); EditorSceneManager.SaveScene(scene); AssetDatabase.SaveAssets();
        Audit();
    }
    public static void RenderingBudget()
    {
        var scene=Open(); var inventory=Inventory(scene);
        var groups=inventory.GetComponentsInChildren<Transform>().Where(t=>t.name=="Decorative Stock" || t.name=="Merchandising Support").ToArray();
        var filters=groups.SelectMany(g=>g.GetComponentsInChildren<MeshFilter>()).ToArray();
        var renderers=groups.SelectMany(g=>g.GetComponentsInChildren<MeshRenderer>()).ToArray();
        var mats=renderers.SelectMany(r=>r.sharedMaterials).Distinct().ToArray();
        var b=new StringBuilder();
        b.AppendLine("Decorative facings="+groups.Where(g=>g.name=="Decorative Stock").Sum(g=>g.childCount));
        b.AppendLine("Support trays="+groups.Count(g=>g.name=="Merchandising Support"));
        b.AppendLine("Added GameObjects="+groups.Sum(g=>g.GetComponentsInChildren<Transform>().Length));
        b.AppendLine("Added renderers="+renderers.Length+"; shared mesh assets="+filters.Select(f=>f.sharedMesh).Distinct().Count()+"; shared materials="+mats.Length);
        b.AppendLine("Rendered mesh vertices (all copies)="+filters.Sum(f=>(long)f.sharedMesh.vertexCount));
        b.AppendLine("Rendered triangles (all copies)="+filters.Sum(f=>(long)f.sharedMesh.triangles.Length/3));
        b.AppendLine("Material slots (before batching/culling; not measured draw calls)="+renderers.Sum(r=>r.sharedMaterials.Length));
        b.AppendLine("Instancing-enabled shared materials="+mats.Count(m=>m.enableInstancing));
        b.AppendLine("Added colliders="+groups.Sum(g=>g.GetComponentsInChildren<Collider>(true).Length)+"; added rigidbodies="+groups.Sum(g=>g.GetComponentsInChildren<Rigidbody>(true).Length)+"; added interactables="+groups.Sum(g=>g.GetComponentsInChildren<XRBaseInteractable>(true).Length));
        b.AppendLine("Static flags set; source meshes/materials reused; decorative shadow casting off. No frame-time, stereo draw-call or hardware performance acceptance is inferred from these counts.");
        File.WriteAllText(Evidence+"/rendering-budget.txt",b.ToString());
    }
    public static string Validate(Scene scene)
    {
        if(Level3VisualRevisionTools.IsRevised(Inventory(scene))) return Level3VisualRevisionTools.Validate(scene);
        if(!Inventory(scene).GetComponentsInChildren<Transform>(true).Any(Level3ShelfFixTools.IsPad)) return Level3ShelfFixTools.Validate(scene);
        var inventory=Inventory(scene); var b=new StringBuilder("Section\tDecorative facings\tRenderers\tTypes\n");
        var anchors=inventory.GetComponentsInChildren<SelectableItem>(true);
        if(anchors.Length!=14 || anchors.Sum(a=>a.stockQuantity)!=168) throw new Exception("Stock changed.");
        foreach(var spec in Level3LayoutTools.Sections)
        {
            var section=inventory.Find(spec.Name); var group=section.Find("Decorative Stock");
            if(group==null || group.childCount<5) throw new Exception("Missing meaningful stock: "+spec.id);
            if(group.GetComponentsInChildren<Component>(true).Any(c=>!(c is Transform || c is MeshFilter || c is MeshRenderer || (c is Unity.AI.Navigation.NavMeshModifier && c.transform==group && ((Unity.AI.Navigation.NavMeshModifier)c).ignoreFromBuild)))) throw new Exception("Unexpected decorative component "+spec.id);
            var body=section.Find("Display Body Collider").GetComponent<BoxCollider>().bounds;
            var previous=new System.Collections.Generic.List<Bounds>();
            foreach(Transform facing in group)
            {
                var bounds=Level3LayoutTools.BoundsOf(facing.gameObject);
                if(bounds.min.x<body.min.x+.04f || bounds.max.x>body.max.x-.04f || bounds.min.z<body.min.z+.04f || bounds.max.z>body.max.z-.04f || Mathf.Abs(bounds.min.y-SupportHeight(section,bounds.center.x,bounds.center.z)-.004f)>.003f) throw new Exception("Unsupported/outside facing "+facing.name);
                if(previous.Any(p=>p.Intersects(bounds))) throw new Exception("Overlapping decorative bounds "+spec.id);
                previous.Add(bounds);
                foreach(var filter in facing.GetComponentsInChildren<MeshFilter>()) if(filter.sharedMesh==null || !AssetDatabase.Contains(filter.sharedMesh)) throw new Exception("Mesh not shared asset.");
                foreach(var renderer in facing.GetComponentsInChildren<MeshRenderer>()) if(renderer.sharedMaterials.Any(m=>m==null || !AssetDatabase.Contains(m))) throw new Exception("Material not shared asset.");
            }
            b.AppendLine(spec.id+"\t"+group.childCount+"\t"+group.GetComponentsInChildren<Renderer>().Length+"\t"+string.Join(";",group.Cast<Transform>().Select(t=>t.name.Substring(t.name.IndexOf(" - ")+3)).Distinct()));
        }
        return b.ToString();
    }
    [MenuItem("Tools/Level 3/Merchandising/3 Audit")]
    public static void Audit()
    {
        if(Level3VisualRevisionTools.IsRevised(Inventory(Open()))) { Level3VisualRevisionTools.Audit(); return; }
        if(!Inventory(Open()).GetComponentsInChildren<Transform>(true).Any(Level3ShelfFixTools.IsPad)) { Level3ShelfFixTools.Audit(); return; }
        var scene=Open(); Directory.CreateDirectory(Evidence);
        File.WriteAllText(Evidence+"/stocking-validation.tsv",Validate(scene));
        File.WriteAllText(Evidence+"/after-counts.txt",Counts(scene));
        File.WriteAllText(Evidence+"/accessibility.txt",Level3LayoutTools.MeasureAccess(scene,false));
        RenderingBudget();
        Level3StabilityTools.Inspect();
        File.Copy("Library/Level3Stability-static.txt",Evidence+"/static-references.txt",true);
        File.Copy("Library/Level3Inventory-result.txt",Evidence+"/inventory-validation.txt",true);
    }
    [MenuItem("Tools/Level 3/Merchandising/4 Capture after")]
    public static void After() { var scene=Open(); if(Level3VisualRevisionTools.IsRevised(Inventory(scene))) { Level3VisualRevisionTools.CaptureSaved(); return; } if(!Inventory(scene).GetComponentsInChildren<Transform>(true).Any(Level3ShelfFixTools.IsPad)) { Level3ShelfFixTools.After(); return; } Validate(scene); Capture(scene,"after"); }
    static void Capture(Scene scene,string prefix)
    {
        Render(prefix+"-overview",new Vector3(-46.6f,2.75f,1.5f),new Vector3(-46.6f,0,1.5f),true,1000,1800,10.5f);
        Render(prefix+"-north-aisle",new Vector3(-46.5f,1.79f,3.5f),new Vector3(-46.5f,1.15f,9),false);
        Render(prefix+"-south-aisle",new Vector3(-46.5f,1.79f,3.5f),new Vector3(-46.5f,1.15f,-4.3f),false);
        foreach(var spec in Level3LayoutTools.Sections) Render(prefix+"-"+spec.id,new Vector3(spec.x+.1f,1.79f,spec.z-1.1f),new Vector3(spec.x,1.15f,spec.z),false);
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


















