using System;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Level3ShelfFixTools
{
    public const string Evidence="Evidence/Level3Prompt4_5_Fix";
    public static bool IsPad(Transform t) => t.name=="Merchandising Support" || t.name.StartsWith("StockTray",StringComparison.Ordinal) || t.name=="Stock Tray";
    public static void Probe()
    {
        var scene=Level3MerchandisingTools.Open(); var inventory=Level3MerchandisingTools.Inventory(scene);
        Directory.CreateDirectory(Evidence);
        if(File.Exists(Evidence+"/before-counts.txt")) throw new Exception("Baseline already captured.");
        File.Copy(Level3LayoutTools.ScenePath,"Library/Level3ShelfFix-before.unity",false);
        File.WriteAllText(Evidence+"/before-counts.txt",Level3MerchandisingTools.Counts(scene));
        Capture("before");
        var b=new StringBuilder();
        foreach(Transform section in inventory)
        {
            foreach(var t in section.Cast<Transform>().Where(IsPad)) { b.AppendLine("REMOVE PAD "+Level3LayoutTools.PathOf(t)); t.gameObject.SetActive(false); }
        }
        foreach(var id in new[]{"S01","S03","S08"})
        {
            var spec=Level3LayoutTools.Sections.Single(s=>s.id==id); var section=inventory.Find(spec.Name);
            section.Find("Products").gameObject.SetActive(false); section.Find("Decorative Stock").gameObject.SetActive(false); section.Find("Category Sign").gameObject.SetActive(false);
            foreach(var r in section.GetComponentsInChildren<Renderer>()) b.AppendLine("FURNITURE "+Level3LayoutTools.PathOf(r.transform)+" "+r.bounds);
            for(float z=-.30f;z<=.301f;z+=.05f) foreach(float x in new[]{-.55f,0,.55f})
            { try { b.AppendLine(id+" x="+x.ToString("F2")+" z="+z.ToString("F2")+" top="+Level3MerchandisingTools.SupportHeight(section,spec.x+x,spec.z+z).ToString("F4")); } catch(Exception) {b.AppendLine(id+" missing surface "+x+","+z);} }
            Render(id+"-bare-front",new Vector3(spec.x+.1f,1.45f,spec.z-1.1f),new Vector3(spec.x,.95f,spec.z),false);
            Render(id+"-bare-back",new Vector3(spec.x+.1f,1.45f,spec.z+1.1f),new Vector3(spec.x,.95f,spec.z),false);
            Render(id+"-bare-side",new Vector3(spec.x+1.1f,1.45f,spec.z-.15f),new Vector3(spec.x,.95f,spec.z),false);
        }
        File.WriteAllText(Evidence+"/geometry-probe.txt",b.ToString());
        // Probe mutations are intentionally not saved.
    }
    static Transform Furniture(Transform section) => section.Cast<Transform>().Single(t=>t.name=="Gondola Display" || t.name=="Chilled Display" || t.name.StartsWith("DisplayShelf",StringComparison.Ordinal));
    static void OrientShelf(Transform furniture,Level3LayoutTools.Section spec,float angle)
    {
        var b=Level3LayoutTools.BoundsOf(furniture.gameObject);
        furniture.RotateAround(b.center,Vector3.right,angle);
        b=Level3LayoutTools.BoundsOf(furniture.gameObject);
        var scale=furniture.localScale; scale.x*=1.45f/b.size.x; scale.y*=.65f/b.size.z; scale.z*=.55f/b.size.y; furniture.localScale=scale;
        b=Level3LayoutTools.BoundsOf(furniture.gameObject);
        furniture.position+=new Vector3(spec.x,.62505f,spec.z)-new Vector3(b.center.x,b.min.y,b.center.z);
        PrefabUtility.RecordPrefabInstancePropertyModifications(furniture);
    }
    public static void OrientationProbe()
    {
        var scene=Level3MerchandisingTools.Open(); var spec=Level3LayoutTools.Sections.Single(s=>s.id=="S03");
        var section=Level3MerchandisingTools.Inventory(scene).Find(spec.Name);
        foreach(var t in section.Cast<Transform>().Where(t=>IsPad(t) || t.name=="Products" || t.name=="Decorative Stock" || t.name=="Category Sign")) t.gameObject.SetActive(false);
        var furniture=Furniture(section); var originalPos=furniture.position; var originalRot=furniture.rotation; var originalScale=furniture.localScale;
        var report=new StringBuilder();
        foreach(float angle in new[]{90f,-90f})
        {
            furniture.SetPositionAndRotation(originalPos,originalRot); furniture.localScale=originalScale;
            OrientShelf(furniture,spec,angle);
            Render("shelf-oriented-"+angle,new Vector3(spec.x+.6f,1.5f,spec.z-1.15f),new Vector3(spec.x,.95f,spec.z),false);
            for(float z=-.28f;z<.30f;z+=.04f) report.AppendLine(angle+" z="+z+" surface="+Level3MerchandisingTools.SupportHeight(section,spec.x,spec.z+z));
        }
        File.WriteAllText(Evidence+"/shelf-orientation.txt",report.ToString());
    }
    static Vector2 ClosestSegment(Vector2 point,Vector2 a,Vector2 b) { var d=b-a; return a+d*Mathf.Clamp01(Vector2.Dot(point-a,d)/Mathf.Max(d.sqrMagnitude,1e-10f)); }
    public struct Contact { public Vector3 point,normal; }
    public static Contact Surface(Transform section,float x,float z,float ceiling=10,float radius=0)
    {
        float best=float.NegativeInfinity; Contact contact=default;
        foreach(var f in Furniture(section).GetComponentsInChildren<MeshFilter>())
        {
            bool rack=section.name.StartsWith("S06") || section.name.StartsWith("S07") || section.name.StartsWith("S08") || section.name.StartsWith("S09");
            if(rack && f.name!="Box007") continue;
            var vertices=f.sharedMesh.vertices; var tris=f.sharedMesh.triangles;
            for(int i=0;i<tris.Length;i+=3)
            {
                var a=f.transform.TransformPoint(vertices[tris[i]]); var b=f.transform.TransformPoint(vertices[tris[i+1]]); var c=f.transform.TransformPoint(vertices[tris[i+2]]);
                var normal=Vector3.Cross(b-a,c-a).normalized; if(normal.y<0) normal=-normal; if(normal.y<.5f) continue;
                float den=(b.z-c.z)*(a.x-c.x)+(c.x-b.x)*(a.z-c.z); if(Mathf.Abs(den)<1e-10f) continue;
                float px=x,pz=z;
                float u=((b.z-c.z)*(px-c.x)+(c.x-b.x)*(pz-c.z))/den;
                float v=((c.z-a.z)*(px-c.x)+(a.x-c.x)*(pz-c.z))/den;
                if(u<0 || v<0 || 1-u-v<0)
                {
                    if(radius<=0) continue;
                    var q=new Vector2(x,z); var av=new Vector2(a.x,a.z); var bv=new Vector2(b.x,b.z); var cv=new Vector2(c.x,c.z);
                    var candidates=new[]{ClosestSegment(q,av,bv),ClosestSegment(q,bv,cv),ClosestSegment(q,cv,av)};
                    var closest=candidates.OrderBy(p=>(p-q).sqrMagnitude).First();
                    if((closest-q).sqrMagnitude>radius*radius) continue;
                    px=closest.x; pz=closest.y;
                    u=((b.z-c.z)*(px-c.x)+(c.x-b.x)*(pz-c.z))/den;
                    v=((c.z-a.z)*(px-c.x)+(a.x-c.x)*(pz-c.z))/den;
                }
                float y=u*a.y+v*b.y+(1-u-v)*c.y;
                if(y>ceiling+.001f || y<best-.001f) continue;
                if(Mathf.Abs(y-best)<.001f && new Vector2(contact.point.x-x,contact.point.z-z).sqrMagnitude<=new Vector2(px-x,pz-z).sqrMagnitude) continue;
                best=y; contact=new Contact{point=new Vector3(px,y,pz),normal=normal};
            }
        }
        if(float.IsNegativeInfinity(best)) throw new Exception("No original furniture support: "+section.name+" x="+x+" z="+z+" ceiling="+ceiling);
        return contact;
    }
    public static float MinimumPlaneDistance(GameObject product,Contact contact)
    {
        float distance=float.PositiveInfinity;
        foreach(var f in product.GetComponentsInChildren<MeshFilter>()) foreach(var vertex in f.sharedMesh.vertices)
            distance=Mathf.Min(distance,Vector3.Dot(f.transform.TransformPoint(vertex)-contact.point,contact.normal));
        return distance;
    }
    static void Place(GameObject product,Transform section,float x,float z,float size,Quaternion rotation,float ceiling=10)
    {
        product.transform.rotation=rotation;
        var bounds=Level3LayoutTools.BoundsOf(product); product.transform.localScale*=size/Mathf.Max(bounds.size.x,bounds.size.y,bounds.size.z);
        bounds=Level3LayoutTools.BoundsOf(product);
        product.transform.position+=new Vector3(x,0,z)-new Vector3(bounds.center.x,bounds.min.y,bounds.center.z);
        bool chilled=section.name.StartsWith("S06") || section.name.StartsWith("S07") || section.name.StartsWith("S08") || section.name.StartsWith("S09");
        if(chilled && z-section.position.z>.15f) { bounds=Level3LayoutTools.BoundsOf(product); float targetZ=section.position.z+.235f-bounds.extents.z; product.transform.position+=Vector3.forward*(targetZ-bounds.center.z); z=targetZ; }
        bounds=Level3LayoutTools.BoundsOf(product);
        var contact=Surface(section,x,z,ceiling,chilled?Mathf.Max(.025f,Mathf.Min(bounds.extents.x,bounds.extents.z)):.0f);
        if(chilled) contact.normal=Vector3.up;
        if(section.name.StartsWith("S01") || section.name.StartsWith("S02"))
        {
            product.transform.rotation=Quaternion.FromToRotation(Vector3.up,contact.normal)*rotation;
            bounds=Level3LayoutTools.BoundsOf(product); product.transform.localScale*=size/Mathf.Max(bounds.size.x,bounds.size.y,bounds.size.z);
            bounds=Level3LayoutTools.BoundsOf(product); product.transform.position+=new Vector3(x,0,z)-new Vector3(bounds.center.x,bounds.min.y,bounds.center.z);
        }
        product.transform.position+=Vector3.up*((.002f-MinimumPlaneDistance(product,contact))/contact.normal.y);
        PrefabUtility.RecordPrefabInstancePropertyModifications(product.transform);
    }
    static void CollisionForFurniture(Transform section,string type)
    {
        var body=section.Find("Display Body Collider").GetComponent<BoxCollider>();
        float height=type=="Produce"?.17f:(type=="Chilled"?.065f:.04f);
        body.size=new Vector3(1.45f,height,.65f); body.transform.position=new Vector3(body.transform.position.x,.62505f+height*.5f,body.transform.position.z);
        foreach(var f in Furniture(section).GetComponentsInChildren<MeshFilter>())
        {
            var collider=f.GetComponent<MeshCollider>(); if(collider==null) collider=f.gameObject.AddComponent<MeshCollider>();
            collider.sharedMesh=f.sharedMesh; collider.convex=false; collider.enabled=true; collider.isTrigger=false;
            PrefabUtility.RecordPrefabInstancePropertyModifications(collider);
        }
    }
    public static void Apply()
    {
        var scene=Level3MerchandisingTools.Open(); var inventory=Level3MerchandisingTools.Inventory(scene);
        if(!File.Exists(Evidence+"/before-counts.txt")) throw new Exception("Probe saved baseline first.");
        if(!inventory.Cast<Transform>().Any(t=>t.Cast<Transform>().Any(IsPad))) throw new Exception("Pads already removed; refusing duplicate repair.");
        var report=new StringBuilder("Section\tDecorative facings\tAnchor positions\n");
        foreach(var spec in Level3LayoutTools.Sections)
        {
            var section=inventory.Find(spec.Name);
            foreach(var pad in section.Cast<Transform>().Where(IsPad).ToArray()) UnityEngine.Object.DestroyImmediate(pad.gameObject);
            if(spec.furniture=="Shelf") OrientShelf(Furniture(section),spec,-90);
            CollisionForFurniture(section,spec.furniture);
            section.Find("Approach").position=new Vector3(spec.x+.1f,.62505f,spec.z-.75f);
            var anchors=section.GetComponentsInChildren<SelectableItem>();
            for(int i=0;i<anchors.Length;i++)
            {
                float ax=spec.x+(anchors.Length==2?(i==0?-.32f:.32f):0);
                float az=spec.z+(spec.furniture=="Produce"?-.17f:(spec.furniture=="Chilled"?.21f:-.20f));
                var rotation=spec.id=="S08"?Quaternion.Euler(0,90,90):Quaternion.identity;
                Place(anchors[i].gameObject,section,ax,az,.20f,rotation);
            }
            var group=section.Find("Decorative Stock"); var facings=group.Cast<Transform>().ToArray(); int cursor=0;
            var occupied=anchors.Select(a=>Level3LayoutTools.BoundsOf(a.gameObject)).ToList();
            int tiers=spec.furniture=="Shelf"?3:(spec.furniture=="Chilled"?2:1);
            for(int tier=0;tier<tiers && cursor<facings.Length;tier++)
            {
                var rows=spec.furniture=="Shelf"?new[]{-.20f}:(spec.furniture=="Produce"?new[]{-.23f,-.12f,.12f,.23f}:new[]{-.19f,-.07f,.04f});
                var columns=spec.furniture=="Produce"?new[]{-.60f,-.34f,0,.34f,.60f}:new[]{-.54f,-.36f,-.18f,0,.18f,.36f,.54f};
                var slots=rows.SelectMany(z=>columns.Select(x=>new Vector2(x,z))).ToList();
                if(spec.furniture=="Chilled") slots=columns.Select(x=>new Vector2(x,.21f)).Concat(new[]{new Vector2(.55f,-.18f),new Vector2(.55f,-.03f),new Vector2(.55f,.1f)}).ToList();
                foreach(var slot in slots)
                {
                    float x=slot.x,z=slot.y;
                    if(cursor>=facings.Length) break;
                    var facing=facings[cursor].gameObject;
                    float ceiling=10;
                    try {
                        for(int t=0;t<tier;t++) ceiling=Surface(section,spec.x+x,spec.z+z,ceiling,spec.furniture=="Chilled"?.04f:0).point.y-.06f;
                        float size=spec.furniture=="Produce"?.13f:((spec.id=="S07" || spec.id=="S08" || spec.id=="S11" || spec.id=="S12")?.14f:.14f);
                        var rotation=spec.id=="S08"?Quaternion.Euler(0,90,90):(facing.name.Contains("Carrot")?Quaternion.Euler(0,0,65):Quaternion.identity);
                        Place(facing,section,spec.x+x,spec.z+z,size,rotation,ceiling);
                    } catch(Exception) { continue; }
                    var fb=Level3LayoutTools.BoundsOf(facing);
                    var expanded=fb; expanded.Expand(.025f);
                    if(occupied.Any(b=>b.Intersects(expanded))) continue;
                    var body=section.Find("Display Body Collider").GetComponent<BoxCollider>().bounds;
                    if(fb.min.x<body.min.x+.02f || fb.max.x>body.max.x-.02f || fb.min.z<body.min.z+.015f || fb.max.z>body.max.z-.015f) continue;
                    occupied.Add(fb); cursor++;
                }
            }
            foreach(var unused in facings.Skip(cursor)) UnityEngine.Object.DestroyImmediate(unused.gameObject);
            if(cursor<5) throw new Exception("Too little direct stock "+spec.id+": "+cursor);
            if(spec.id=="S08")
            {
                section.Find("Category Sign").GetComponent<TextMesh>().text="S08 Seafood\nWhole fish";
                section.Find("Category Sign").GetComponent<TextMesh>().characterSize=.014f;
                anchors[0].name="Whole fish - Interactive"; PrefabUtility.RecordPrefabInstancePropertyModifications(anchors[0].gameObject);
                int i=0; foreach(Transform fish in group) fish.name="Facing "+(++i).ToString("D2")+" - Whole fish";
            }
            report.AppendLine(spec.id+"\t"+cursor+"\t"+string.Join(";",anchors.Select(a=>a.transform.position.ToString("F3"))));
        }
        Physics.SyncTransforms();
        File.WriteAllText(Evidence+"/stocking-layout.tsv",report.ToString());
        var surface=scene.GetRootGameObjects().Single(g=>g.name=="supermarket").transform.Find("supermarket").GetComponent<Unity.AI.Navigation.NavMeshSurface>();
        surface.RemoveData(); surface.navMeshData=null; surface.BuildNavMesh();
        AssetDatabase.CreateAsset(surface.navMeshData,"Assets/Scenes/Scene-level-3/NavMesh-Level3ShelfFix.asset");
        EditorSceneManager.MarkSceneDirty(scene); EditorSceneManager.SaveScene(scene); AssetDatabase.SaveAssets();
        File.WriteAllText(Evidence+"/after-counts.txt",Level3MerchandisingTools.Counts(scene));
        File.WriteAllText(Evidence+"/accessibility.txt",Level3LayoutTools.MeasureAccess(scene,false));
    }
    public static void RackProbe()
    {
        var scene=Level3MerchandisingTools.Open(); var section=Level3MerchandisingTools.Inventory(scene).Find("S08_Seafood"); var spec=Level3LayoutTools.Sections.Single(s=>s.id=="S08");
        foreach(var t in section.Cast<Transform>().Where(t=>IsPad(t) || t.name=="Products" || t.name=="Decorative Stock")) t.gameObject.SetActive(false);
        var report=new StringBuilder("mesh\tup triangles\tY bands and XZ ranges\n");
        foreach(var f in Furniture(section).GetComponentsInChildren<MeshFilter>())
        {
            var vertices=f.sharedMesh.vertices; var tris=f.sharedMesh.triangles; var bands=new System.Collections.Generic.Dictionary<int,Bounds>(); int count=0;
            for(int i=0;i<tris.Length;i+=3)
            {
                var a=f.transform.TransformPoint(vertices[tris[i]]); var b=f.transform.TransformPoint(vertices[tris[i+1]]); var c=f.transform.TransformPoint(vertices[tris[i+2]]);
                var normal=Vector3.Cross(b-a,c-a).normalized; if(Mathf.Abs(normal.y)<.5f) continue; count++;
                int key=Mathf.RoundToInt(Mathf.Max(a.y,b.y,c.y)*1000); var bounds=new Bounds(a,Vector3.zero); bounds.Encapsulate(b); bounds.Encapsulate(c);
                if(bands.TryGetValue(key,out var existing)) { existing.Encapsulate(bounds); bounds=existing; } bands[key]=bounds;
            }
            report.AppendLine(f.name+" up="+count);
            foreach(var band in bands.OrderByDescending(p=>p.Key)) report.AppendLine(band.Key+" min="+(band.Value.min-new Vector3(spec.x,0,spec.z)).ToString("F4")+" max="+(band.Value.max-new Vector3(spec.x,0,spec.z)).ToString("F4"));
        }
        File.WriteAllText(Evidence+"/rack-surfaces.txt",report.ToString());
    }
    public static void RackContacts()
    {
        var scene=Level3MerchandisingTools.Open(); var inventory=Level3MerchandisingTools.Inventory(scene); var b=new StringBuilder();
        foreach(var spec in Level3LayoutTools.Sections.Where(s=>s.furniture=="Chilled"))
        {
            var section=inventory.Find(spec.Name);
            b.AppendLine(spec.id+" filters="+string.Join(",",Furniture(section).GetComponentsInChildren<MeshFilter>().Select(f=>"["+f.name+"]")));
            foreach(float x in new[]{-.4f,0,.4f}) foreach(float z in new[]{-.2f,0,.2f})
            {
                try { var c=Surface(section,spec.x+x,spec.z+z,1,.7f); b.AppendLine("request="+x+","+z+" hit="+(c.point-new Vector3(spec.x,0,spec.z))); } catch(Exception e) {b.AppendLine(e.Message);}
            }
        }
        File.WriteAllText(Evidence+"/rack-contacts.txt",b.ToString());
    }
    static void FitUnderShelf(GameObject product,Transform section)
    {
        if(!section.name.StartsWith("S03") && !section.name.StartsWith("S04") && !section.name.StartsWith("S05") && !section.name.StartsWith("S10") && !section.name.StartsWith("S11") && !section.name.StartsWith("S12")) return;
        var b=Level3LayoutTools.BoundsOf(product); float ceiling=10,above=float.PositiveInfinity;
        for(int i=0;i<12;i++)
        {
            Contact c; try { c=Surface(section,b.center.x,b.center.z,ceiling); } catch(Exception) { break; }
            if(c.point.y>b.min.y+.006f) above=Mathf.Min(above,c.point.y); else break;
            ceiling=c.point.y-.003f;
        }
        if(float.IsPositiveInfinity(above) || b.max.y<above-.004f) return;
        float scale=(above-b.min.y-.005f)/b.size.y;
        if(scale<=.2f) throw new Exception("No useful shelf headroom "+product.name);
        product.transform.localScale*=scale;
        var resized=Level3LayoutTools.BoundsOf(product);
        product.transform.position+=new Vector3(b.center.x,b.min.y,b.center.z)-new Vector3(resized.center.x,resized.min.y,resized.center.z);
    }
    public static void RefineStock()
    {
        var scene=Level3MerchandisingTools.Open(); var inventory=Level3MerchandisingTools.Inventory(scene);
        foreach(var spec in Level3LayoutTools.Sections)
        {
            var section=inventory.Find(spec.Name); var group=section.Find("Decorative Stock");
            if(spec.furniture=="Shelf")
            {
                foreach(Transform facing in group)
                {
                    var b=Level3LayoutTools.BoundsOf(facing.gameObject);
                    if(b.min.y<1.0f)
                    {
                        float targetZ=spec.z-.317f+b.extents.z;
                        facing.position+=Vector3.forward*(targetZ-b.center.z);
                        var c=Surface(section,b.center.x,targetZ,b.min.y+.025f);
                        facing.position+=Vector3.up*((.002f-MinimumPlaneDistance(facing.gameObject,c))/c.normal.y);
                    }
                    FitUnderShelf(facing.gameObject,section);
                }
            }
            if(spec.furniture!="Produce") continue;
            var originals=group.Cast<Transform>().ToArray();
            for(int i=originals.Length;i<40;i++)
            {
                var copy=UnityEngine.Object.Instantiate(originals[i%originals.Length].gameObject,group); copy.name="Facing "+(i+1).ToString("D2")+" - "+originals[i%originals.Length].name.Split(new[]{" - "},StringSplitOptions.None).Last();
            }
            var candidates=new System.Collections.Generic.List<Vector2>();
            foreach(float bin in new[]{-.32f,0,.32f}) foreach(float dx in new[]{-.065f,.065f}) foreach(float z in new[]{-.23f,-.10f,.10f,.23f}) candidates.Add(new Vector2(bin+dx,z));
            foreach(float x in new[]{-.60f,.60f}) foreach(float z in new[]{-.23f,-.10f,.10f,.23f}) candidates.Add(new Vector2(x,z));
            var anchors=section.GetComponentsInChildren<SelectableItem>(); var occupied=anchors.Select(a=>Level3LayoutTools.BoundsOf(a.gameObject)).ToList();
            var facings=group.Cast<Transform>().ToArray(); int cursor=0;
            foreach(var position in candidates)
            {
                if(cursor>=facings.Length) break;
                var facing=facings[cursor].gameObject;
                Place(facing,section,spec.x+position.x,spec.z+position.y,.10f,facing.name.Contains("Carrot")?Quaternion.Euler(0,0,65):Quaternion.identity);
                var b=Level3LayoutTools.BoundsOf(facing); var expanded=b; expanded.Expand(.012f);
                if(occupied.Any(o=>o.Intersects(expanded))) continue;
                occupied.Add(b); cursor++;
            }
            foreach(var unused in facings.Skip(cursor)) UnityEngine.Object.DestroyImmediate(unused.gameObject);
        }
        EditorSceneManager.MarkSceneDirty(scene); EditorSceneManager.SaveScene(scene); AssetDatabase.SaveAssets();
        Audit(); Capture("final");
    }
    public static string Validate(Scene scene)
    {
        var inventory=Level3MerchandisingTools.Inventory(scene); var b=new StringBuilder("Section\tFacings\tInteractive\n");
        if(inventory.GetComponentsInChildren<Transform>(true).Any(IsPad)) throw new Exception("Removable support pad remains.");
        if(inventory.childCount!=12) throw new Exception("Section count differs.");
        var anchors=inventory.GetComponentsInChildren<SelectableItem>(true);
        if(anchors.Length!=14 || anchors.Sum(a=>a.stockQuantity)!=168 || anchors.Any(a=>a.stockQuantity!=12)) throw new Exception("Stock changed.");
        foreach(var spec in Level3LayoutTools.Sections)
        {
            var section=inventory.Find(spec.Name); var group=section.Find("Decorative Stock");
            if(group==null || group.childCount<5) throw new Exception("Insufficient direct stock: "+spec.id);
            if(group.GetComponentsInChildren<Component>(true).Any(c=>!(c is Transform || c is MeshFilter || c is MeshRenderer || (c is Unity.AI.Navigation.NavMeshModifier && c.transform==group && ((Unity.AI.Navigation.NavMeshModifier)c).ignoreFromBuild)))) throw new Exception("Decorative gameplay/physics component: "+spec.id);
            var stockBounds=group.Cast<Transform>().Select(t=>Level3LayoutTools.BoundsOf(t.gameObject)).ToArray();
            for(int i=0;i<stockBounds.Length;i++) for(int j=i+1;j<stockBounds.Length;j++) if(stockBounds[i].Intersects(stockBounds[j])) throw new Exception("Decorative bounds overlap: "+spec.id);
            foreach(var renderer in group.GetComponentsInChildren<MeshRenderer>()) if(renderer.sharedMaterials.Any(m=>m==null || !AssetDatabase.Contains(m))) throw new Exception("Non-shared material.");
            foreach(var filter in group.GetComponentsInChildren<MeshFilter>()) if(filter.sharedMesh==null || !AssetDatabase.Contains(filter.sharedMesh)) throw new Exception("Non-shared mesh.");
            foreach(var product in section.GetComponentsInChildren<SelectableItem>().Select(a=>a.gameObject).Concat(group.Cast<Transform>().Select(t=>t.gameObject))) ValidateSupport(section,product);
            b.AppendLine(spec.id+"\t"+group.childCount+"\t"+section.GetComponentsInChildren<SelectableItem>().Length);
        }
        return b.ToString();
    }
    public static void ValidateSupport(Transform section,GameObject product)
    {
        var b=Level3LayoutTools.BoundsOf(product); var body=section.Find("Display Body Collider").GetComponent<BoxCollider>().bounds;
        if(b.min.x<body.min.x+.005f || b.max.x>body.max.x-.005f || b.min.z<body.min.z+.005f || b.max.z>body.max.z-.005f) throw new Exception("Outside original display footprint: "+product.name);
        bool chilled=section.name.StartsWith("S06") || section.name.StartsWith("S07") || section.name.StartsWith("S08") || section.name.StartsWith("S09");
        var contact=Surface(section,b.center.x,b.center.z,b.min.y+.06f,chilled?Mathf.Max(.025f,Mathf.Min(b.extents.x,b.extents.z)):0);
        if(chilled) contact.normal=Vector3.up;
        float gap=MinimumPlaneDistance(product,contact);
        if(gap<-.003f || gap>.007f) throw new Exception("Original furniture contact gap="+gap+" product="+product.name);
        if(section.name.StartsWith("S03") || section.name.StartsWith("S04") || section.name.StartsWith("S05") || section.name.StartsWith("S10") || section.name.StartsWith("S11") || section.name.StartsWith("S12"))
        {
            var top=Surface(section,b.center.x,b.center.z,b.max.y-.002f);
            if(top.point.y>b.min.y+.008f) throw new Exception("Product penetrates another shelf board: "+product.name);
        }
    }
    [MenuItem("Tools/Level 3/Direct shelf stocking/Validate")]
    public static void Audit()
    {
        var scene=Level3MerchandisingTools.Open(); File.WriteAllText(Evidence+"/direct-stock-validation.tsv",Validate(scene));
        File.WriteAllText(Evidence+"/after-counts.txt",Level3MerchandisingTools.Counts(scene));
        File.WriteAllText(Evidence+"/accessibility.txt",Level3LayoutTools.MeasureAccess(scene,false));
        Level3StabilityTools.Inspect();
        File.Copy("Library/Level3Stability-static.txt",Evidence+"/static-references.txt",true);
        File.Copy("Library/Level3Inventory-result.txt",Evidence+"/inventory-validation.txt",true);
    }
    [MenuItem("Tools/Level 3/Direct shelf stocking/Capture final views")]
    public static void After() {Level3MerchandisingTools.Open(); Capture("final");}
    public static void Capture(string prefix)
    {
        Render(prefix+"-overview",new Vector3(-46.6f,2.75f,1.5f),new Vector3(-46.6f,0,1.5f),true,1000,1800,10.5f);
        Render(prefix+"-north-aisle",new Vector3(-46.5f,1.79f,3.5f),new Vector3(-46.5f,1.15f,9),false);
        Render(prefix+"-south-aisle",new Vector3(-46.5f,1.79f,3.5f),new Vector3(-46.5f,1.15f,-4.3f),false);
        foreach(var spec in Level3LayoutTools.Sections) Render(prefix+"-"+spec.id,new Vector3(spec.x+.1f,1.79f,spec.z-1.1f),new Vector3(spec.x,1.05f,spec.z),false);
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

























