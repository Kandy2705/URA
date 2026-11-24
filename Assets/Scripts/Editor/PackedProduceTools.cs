using System;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class PackedProduceTools
{
    public const string DeckPath="Assets/Level3Layout/PackedProduceDeck.prefab";
    public static void Review()
    {
        Audit();
        ShelfFollowupTools.OpenForReview();
    }
    public static void Audit()
    {
        var scene=EditorSceneManager.OpenScene(Level3LayoutTools.ScenePath);
        var report=new StringBuilder();
        foreach(var root in scene.GetRootGameObjects())
            foreach(var renderer in root.GetComponentsInChildren<Renderer>())
            {
                var p=renderer.bounds.center;
                if(Mathf.Abs(p.x+44.7f)<.9f && Mathf.Abs(p.z+1.6f)<.5f && p.y>1 && p.y<1.7f)
                    report.AppendLine(renderer.GetType().Name+" "+Level3LayoutTools.PathOf(renderer.transform)+" "+renderer.bounds+" materials="+string.Join(",",renderer.sharedMaterials.Select(m=>m?m.name+":"+m.shader.name:"null")));
            }
        Physics.SyncTransforms();
        foreach(var spec in Level3LayoutTools.Sections.Where(s=>s.furniture=="Produce"))
        {
            var section=Level3MerchandisingTools.Inventory(scene).Find(spec.Name);
            var walls=Level3VisualRevisionTools.Furniture(section).GetComponentsInChildren<Collider>().Where(c=>c.enabled).ToArray();
            int checkedCount=0;
            foreach(var unit in section.Find("Products").GetComponentsInChildren<SelectableItem>())
            {
                var collider=unit.GetComponent<Collider>();
                foreach(var wall in walls)
                    if(Physics.ComputePenetration(collider,collider.transform.position,collider.transform.rotation,wall,wall.transform.position,wall.transform.rotation,out _,out float depth) && depth>.002f)
                        throw new Exception("Produce penetrates deck/wall: "+unit.name+" / "+wall.name+" depth="+depth);
                checkedCount++;
            }
            var colliders=section.Find("Products").GetComponentsInChildren<SelectableItem>().Select(p=>p.GetComponent<Collider>()).ToArray();
            for(int i=0;i<colliders.Length;i++) for(int j=i+1;j<colliders.Length;j++)
            {
                var a=colliders[i]; var b=colliders[j];
                if(Physics.ComputePenetration(a,a.transform.position,a.transform.rotation,b,b.transform.position,b.transform.rotation,out _,out float depth) && depth>.002f)
                    throw new Exception("Adjacent food colliders penetrate: "+a.name+" / "+b.name+" depth="+depth);
            }
            report.AppendLine("PASS "+spec.id+": all "+(colliders.Length*(colliders.Length-1)/2)+" food collider pairs checked, penetration <=2mm.");
            report.AppendLine("PASS "+spec.id+": "+checkedCount+" food colliders clear of enabled deck/wall colliders.");
        }
        File.WriteAllText("Evidence/PackedProduce/render-audit.txt",report.ToString());
    }
    public static Bounds DisplayBounds(GameObject item)
    {
        var inverse=Quaternion.Inverse(Quaternion.Euler(-18,0,0));
        bool first=true; Bounds bounds=default;
        foreach(var filter in item.GetComponentsInChildren<MeshFilter>())
            foreach(var vertex in filter.sharedMesh.vertices)
            {
                var point=inverse*filter.transform.TransformPoint(vertex);
                if(first) { bounds=new Bounds(point,Vector3.zero); first=false; } else bounds.Encapsulate(point);
            }
        return bounds;
    }
    static GameObject Cube(Transform parent,string name,Vector3 position,Vector3 size,Quaternion rotation,Material material)
    {
        var go=GameObject.CreatePrimitive(PrimitiveType.Cube); go.name=name;
        go.transform.SetParent(parent,false); go.transform.localPosition=position;
        go.transform.localRotation=rotation; go.transform.localScale=size;
        go.GetComponent<Renderer>().sharedMaterial=material;
        return go;
    }
    static GameObject Deck()
    {
        var existing=AssetDatabase.LoadAssetAtPath<GameObject>(DeckPath); if(existing) return existing;
        string materialPath="Assets/Level3Layout/PackedProduceWood.mat";
        var material=AssetDatabase.LoadAssetAtPath<Material>(materialPath);
        if(!material)
        {
            var shader=UnityEngine.Rendering.GraphicsSettings.currentRenderPipeline?Shader.Find("Universal Render Pipeline/Lit"):Shader.Find("Standard");
            if(!shader) shader=Shader.Find("Standard");
            material=new Material(shader){color=new Color(.40f,.27f,.14f)};
            if(material.HasProperty("_Glossiness")) material.SetFloat("_Glossiness",.12f);
            AssetDatabase.CreateAsset(material,materialPath);
        }
        var root=new GameObject("Packed Produce Deck");
        try
        {
            var slope=Quaternion.Euler(-18,0,0); var centre=new Vector3(0,.45f,0);
            Cube(root.transform,"Sloped Deck",centre,new Vector3(1.5f,.025f,.65f),slope,material);
            foreach(float x in new[]{-.744f,-.25f,.25f,.744f})
                Cube(root.transform,"Low Divider",centre+new Vector3(x,0,0)+slope*Vector3.up*.03f,new Vector3(.012f,.035f,.65f),slope,material);
            Cube(root.transform,"Front Rim",centre+slope*new Vector3(0,.03f,-.319f),new Vector3(1.5f,.035f,.012f),slope,material);
            Cube(root.transform,"Back Rail",new Vector3(0,.62f,.3125f),new Vector3(1.5f,.20f,.025f),Quaternion.identity,material);
            PrefabUtility.SaveAsPrefabAsset(root,DeckPath);
        }
        finally { UnityEngine.Object.DestroyImmediate(root); }
        return AssetDatabase.LoadAssetAtPath<GameObject>(DeckPath);
    }
    static Quaternion LongAxisAcross(SelectableItem item)
    {
        item.transform.rotation=Quaternion.identity;
        var s=Level3LayoutTools.BoundsOf(item.gameObject).size;
        if(s.z>=s.x && s.z>=s.y) return Quaternion.LookRotation(Vector3.right,s.x<s.y?Vector3.down:Vector3.forward);
        if(s.y>=s.x && s.y>=s.z) return Quaternion.LookRotation(s.x<s.z?Vector3.up:Vector3.forward,Vector3.right);
        return s.y<s.z?Quaternion.LookRotation(Vector3.down,Vector3.forward):Quaternion.identity;
    }
    public static void ApplyBatch()
    {
        var scene=EditorSceneManager.OpenScene(Level3LayoutTools.ScenePath);
        Directory.CreateDirectory("Evidence/PackedProduce");
        if(!File.Exists("Library/PackedProduce-before.unity")) File.Copy(Level3LayoutTools.ScenePath,"Library/PackedProduce-before.unity");
        var deckSource=Deck(); var inventory=Level3MerchandisingTools.Inventory(scene);
        var report=new StringBuilder();
        foreach(var root in scene.GetRootGameObjects())
            foreach(var renderer in root.GetComponentsInChildren<MeshRenderer>())
            {
                var p=renderer.bounds.center;
                if(Mathf.Abs(p.x+44.7f)<.85f && Mathf.Abs(p.z+1.6f)<.36f && p.y>1 && p.y<1.6f && !renderer.transform.IsChildOf(inventory))
                    report.AppendLine("EXTERNAL RENDERER "+Level3LayoutTools.PathOf(renderer.transform));
            }
        foreach(var spec in Level3LayoutTools.Sections.Where(s=>s.furniture=="Produce"))
        {
            var section=inventory.Find(spec.Name);
            // Legacy static filler is neither purchasable nor moved with the product rows.
            // Replace it with the linked, individually grabbable stock below.
            var oldFill=section.Find("Decorative Fruit Fill");
            if(oldFill) UnityEngine.Object.DestroyImmediate(oldFill.gameObject);
            var marker=section.Find("Packed Produce Layout");
            if(!marker) { marker=new GameObject("Packed Produce Layout").transform; marker.SetParent(section,false); }
            var group=section.Find("DisplayShelf Packed Produce");
            if(!group)
            {
                var original=Level3VisualRevisionTools.Furniture(section);
                group=new GameObject("DisplayShelf Packed Produce").transform; group.SetParent(section,true);
                group.SetPositionAndRotation(Vector3.zero,Quaternion.identity); group.localScale=Vector3.one;
                original.SetParent(group,true); original.name="Original Pedestal";
                PrefabUtility.RecordPrefabInstancePropertyModifications(original.gameObject);
                foreach(var renderer in original.GetComponentsInChildren<MeshRenderer>().Where(r=>r.name=="Box006"))
                {
                    renderer.enabled=false; PrefabUtility.RecordPrefabInstancePropertyModifications(renderer);
                    foreach(var collider in renderer.GetComponents<Collider>()) { collider.enabled=false; PrefabUtility.RecordPrefabInstancePropertyModifications(collider); }
                }
                var deck=(GameObject)PrefabUtility.InstantiatePrefab(deckSource,scene); deck.name="Packed Deck";
                deck.transform.SetParent(group,true); deck.transform.SetPositionAndRotation(new Vector3(spec.x,.62505f,spec.z),Quaternion.identity);
                deck.transform.localScale=new Vector3((spec.x < -47?1.5f:1.6f)/1.5f,1,1);
                PrefabUtility.RecordPrefabInstancePropertyModifications(deck.transform);
            }
            float width=spec.x < -47?1.5f:1.6f; int rowIndex=0;
            foreach(Transform row in section.Find("Products"))
            {
                var all=row.GetComponentsInChildren<SelectableItem>(); var owner=all.Single(i=>i.GetComponent<Level3StockFacing>().IsStockOwner);
                var units=all.OrderByDescending(i=>i==owner).ToList();
                var source=AssetDatabase.LoadAssetAtPath<GameObject>(PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(owner.gameObject));
                while(units.Count<30)
                {
                    var go=(GameObject)PrefabUtility.InstantiatePrefab(source,scene); go.transform.SetParent(row,false);
                    go.name=owner.name+" - Packed "+(units.Count+1);
                    ShelfGrabTools.Configure(go,false);
                    var item=go.GetComponent<SelectableItem>(); item.stockQuantity=1;
                    go.AddComponent<Level3StockFacing>().stockOwner=owner;
                    PrefabUtility.RecordPrefabInstancePropertyModifications(item); units.Add(item);
                }
                if(units.Count!=30) throw new Exception("Unexpected packed stock "+row.name);
                owner.stockQuantity=30; PrefabUtility.RecordPrefabInstancePropertyModifications(owner);
                for(int i=0;i<units.Count;i++)
                {
                    var item=units[i]; int columns=rowIndex==0?6:5, rows=30/columns;
                    float pitchX=(width/3-.04f)/columns;
                    float pitchZ=.46f/(rows-1);
                    float x=spec.x+(rowIndex-1)*width/3+(i%columns-(columns-1)*.5f)*pitchX;
                    float z=spec.z-.23f+(i/columns)*pitchZ;
                    item.transform.localScale=Vector3.one;
                    var rotation=rowIndex==0?Quaternion.Euler(0,90,0)*LongAxisAcross(item):Quaternion.identity;
                    var contact=Level3VisualRevisionTools.ContactSurface(section,x,z);
                    item.transform.rotation=Quaternion.FromToRotation(Vector3.up,contact.normal)*rotation;
                    var bounds=Level3LayoutTools.BoundsOf(item.gameObject);
                    float size=.30f;
                    item.transform.localScale*=size/Mathf.Max(bounds.size.x,bounds.size.y,bounds.size.z);
                    bounds=DisplayBounds(item.gameObject);
                    item.transform.localScale*=Mathf.Min(1,(pitchX-.003f)/bounds.size.x,(pitchZ/Mathf.Cos(18*Mathf.Deg2Rad)-.003f)/bounds.size.z);
                    bounds=Level3LayoutTools.BoundsOf(item.gameObject);
                    item.transform.position+=new Vector3(x,0,z)-new Vector3(bounds.center.x,0,bounds.center.z);
                    item.transform.position+=Vector3.up*((.002f-Level3ShelfFixTools.MinimumPlaneDistance(item.gameObject,contact))/contact.normal.y);
                    PrefabUtility.RecordPrefabInstancePropertyModifications(item.transform);
                    report.AppendLine(item.name+" size="+Level3LayoutTools.BoundsOf(item.gameObject).size.ToString("F3")+" renderers="+item.GetComponentsInChildren<Renderer>().Length);
                }
                report.AppendLine(spec.id+" "+row.name+" : 30 linked, individually grabbable units in a continuous close-spaced strip."); rowIndex++;
            }
        }
        Level3VisualRevisionTools.Validate(scene);
        report.AppendLine(Level3VisualRevisionTools.MeasureFacingAccess(scene));
        report.AppendLine(Level3LayoutTools.MeasureAccess(scene,false));
        EditorSceneManager.MarkSceneDirty(scene); EditorSceneManager.SaveScene(scene); AssetDatabase.SaveAssets();
        File.WriteAllText("Evidence/PackedProduce/validation.txt",report.ToString());
        ShelfFollowupTools.Capture("packed");
    }
}
