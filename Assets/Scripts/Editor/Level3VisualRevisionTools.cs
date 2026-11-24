using System;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
public static class Level3VisualRevisionTools
{
    public const string Evidence="Evidence/Level3VisualRevision";
    public static void Probe()
    {
        var scene=Level3MerchandisingTools.Open(); Directory.CreateDirectory(Evidence);
        if(File.Exists(Evidence+"/baseline.txt")) throw new Exception("Revision baseline already exists.");
        File.Copy(Level3LayoutTools.ScenePath,"Library/Level3VisualRevision-before.unity",false);
        var b=new StringBuilder(Level3MerchandisingTools.Counts(scene));
        foreach(var cc in scene.GetRootGameObjects().SelectMany(g=>g.GetComponentsInChildren<CharacterController>(true)).Where(c=>c.enabled && c.gameObject.activeInHierarchy)) b.AppendLine("CAPSULE "+Level3LayoutTools.PathOf(cc.transform)+" "+cc.bounds);
        foreach(var camera in scene.GetRootGameObjects().SelectMany(g=>g.GetComponentsInChildren<Camera>(true)).Where(c=>c.gameObject.activeInHierarchy)) b.AppendLine("CAMERA "+Level3LayoutTools.PathOf(camera.transform)+" "+camera.transform.position+" scale="+camera.transform.lossyScale);
        foreach(Transform section in Level3MerchandisingTools.Inventory(scene)) foreach(var child in section.Cast<Transform>().Where(t=>t.name=="Gondola Display" || t.name=="Chilled Display" || t.name.StartsWith("DisplayShelf"))) b.AppendLine(section.name+" FURNITURE "+Level3LayoutTools.BoundsOf(child.gameObject));
        foreach(var path in new[]{"Assets/Prefabs/lettuce.prefab","Assets/Prefabs/grapes.prefab","Assets/Prefabs/meat-sausage.prefab","Assets/Prefabs/Cute Supermarket Lite/Prefabs/dressing_mayo.prefab","Assets/Prefabs/cup.prefab"})
        {
            var source=AssetDatabase.LoadAssetAtPath<GameObject>(path); var item=source?.GetComponent<SelectableItem>();
            b.AppendLine("TYPE "+path+" identity="+item?.itemName+" price="+item?.price+" XR="+source?.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable>());
        }
        var old=scene.GetRootGameObjects().Single(g=>g.name=="Supermarket");
        foreach(var list in old.GetComponentsInChildren<ListController>(true))
        {
            var pool=new SerializedObject(list).FindProperty("availablePrefabs"); if(pool==null) continue;
            for(int i=0;i<pool.arraySize;i++) {var view=pool.GetArrayElementAtIndex(i).objectReferenceValue; if(view) b.AppendLine("VIEW "+view.name+" | "+AssetDatabase.GetAssetPath(view));}
        }
        File.WriteAllText(Evidence+"/baseline.txt",b.ToString()); Capture("before");
    }
    public static void FurnitureProbe()
    {
        var scene=Level3MerchandisingTools.Open(); var spec=Level3LayoutTools.Sections.Single(s=>s.id=="S08");
        var section=Level3MerchandisingTools.Inventory(scene).Find(spec.Name); section.gameObject.SetActive(false);
        var report=new StringBuilder();
        foreach(var name in new[]{"Frezer","Frezer2","shelfMarker3","shelfMarker4"})
        {
            var go=(GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Supermercado/"+name+".fbx"),scene);
            go.transform.SetPositionAndRotation(Vector3.zero,Quaternion.identity); go.transform.localScale=Vector3.one;
            var b=Level3LayoutTools.BoundsOf(go); report.AppendLine(name+" ORIGINAL "+b);
            go.transform.localScale=new Vector3(1.6f/b.size.x,.9f/b.size.y,.7f/b.size.z);
            b=Level3LayoutTools.BoundsOf(go); go.transform.position+=new Vector3(spec.x,.62505f,spec.z)-new Vector3(b.center.x,b.min.y,b.center.z);
            foreach(var f in go.GetComponentsInChildren<MeshFilter>()) report.AppendLine(f.name+" "+f.GetComponent<Renderer>().bounds);
            Render("candidate-"+name,new Vector3(spec.x+.65f,1.792f,spec.z-1.15f),new Vector3(spec.x,1.15f,spec.z),false);
            UnityEngine.Object.DestroyImmediate(go);
        }
        File.WriteAllText(Evidence+"/furniture-candidates.txt",report.ToString());
    }
    public static readonly string[] Categories={"Rau củ","Trái cây","Nước suối","Nước trái cây","Thực phẩm đóng gói","Sữa","Thịt tươi","Hải sản","Đồ đông lạnh","Gia vị","Đồ gia dụng","Dụng cụ nhà bếp"};
    public static bool IsRevised(Transform inventory) => inventory.GetComponentsInChildren<Level3StockFacing>(true).Length>0;
    public static SelectableItem[] Owners(Transform root) => root.GetComponentsInChildren<SelectableItem>(true).Where(p=>p.GetComponent<Level3StockFacing>()==null || p.GetComponent<Level3StockFacing>().IsStockOwner).ToArray();
    public static Transform Furniture(Transform section) => section.Cast<Transform>().Single(t=>t.name=="Gondola Display" || t.name=="Chilled Display" || t.name.StartsWith("DisplayShelf"));
    static void Fit(Transform t,Vector3 size,Vector3 bottom)
    {
        var b=Level3LayoutTools.BoundsOf(t.gameObject); t.localScale=Vector3.Scale(t.localScale,new Vector3(size.x/b.size.x,size.y/b.size.y,size.z/b.size.z));
        b=Level3LayoutTools.BoundsOf(t.gameObject); t.position+=bottom-new Vector3(b.center.x,b.min.y,b.center.z); PrefabUtility.RecordPrefabInstancePropertyModifications(t);
    }
    static GameObject Instance(string path,Transform parent,Scene scene)
    {
        var go=(GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(path),scene); go.transform.SetParent(parent,true); return go;
    }
    static void Interaction(SelectableItem item,SelectableItem owner)
    {
        // Some extra source prefabs repeat gameplay components on mesh children.
        // Keep their geometry, but offer exactly one interactable/identity per physical root.
        foreach(var child in item.GetComponentsInChildren<Transform>(true).Where(t=>t!=item.transform))
        {
            foreach(var mb in child.GetComponents<MonoBehaviour>().Where(m=>m!=null && m.GetType().Name=="XRHighlightOnHover").ToArray()) UnityEngine.Object.DestroyImmediate(mb);
            foreach(var mb in child.GetComponents<MonoBehaviour>().Where(m=>m is UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable).ToArray()) UnityEngine.Object.DestroyImmediate(mb);
            foreach(var mb in child.GetComponents<MonoBehaviour>().Where(m=>m!=null).ToArray()) UnityEngine.Object.DestroyImmediate(mb);
        }
        var box=item.GetComponent<BoxCollider>(); if(!box) box=item.gameObject.AddComponent<BoxCollider>();
        foreach(var c in item.GetComponentsInChildren<Collider>(true).Where(c=>c!=box).ToArray()) UnityEngine.Object.DestroyImmediate(c);
        foreach(var rbChild in item.GetComponentsInChildren<Rigidbody>(true).Where(r=>r.transform!=item.transform).ToArray()) UnityEngine.Object.DestroyImmediate(rbChild);
        var vertices=item.GetComponentsInChildren<MeshFilter>().SelectMany(f=>f.sharedMesh.vertices.Select(v=>item.transform.InverseTransformPoint(f.transform.TransformPoint(v)))).ToArray();
        var localBounds=new Bounds(vertices[0],Vector3.zero); foreach(var vertex in vertices) localBounds.Encapsulate(vertex);
        box.center=localBounds.center; box.size=localBounds.size; box.isTrigger=false; box.enabled=true;
        var counter=item.GetComponent<CheckoutCounter>(); if(counter) UnityEngine.Object.DestroyImmediate(counter);
        var poke=item.GetComponent<PokeObjects>(); if(!poke) poke=item.gameObject.AddComponent<PokeObjects>();
        var xr=item.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable>();
        xr.colliders.Clear(); xr.colliders.Add(box);
        var so=new SerializedObject(xr); so.FindProperty("m_SelectEntered.m_PersistentCalls.m_Calls").ClearArray(); so.FindProperty("m_SelectExited.m_PersistentCalls.m_Calls").ClearArray(); so.ApplyModifiedPropertiesWithoutUndo();
        UnityEditor.Events.UnityEventTools.AddPersistentListener(xr.selectEntered,poke.OnPokeStart);
        UnityEditor.Events.UnityEventTools.AddPersistentListener(xr.selectExited,poke.OnPokeEnd);
        var facing=item.GetComponent<Level3StockFacing>(); if(!facing) facing=item.gameObject.AddComponent<Level3StockFacing>(); facing.stockOwner=owner;
        item.stockQuantity=item==owner?12:1;
        var rb=item.GetComponent<Rigidbody>(); rb.isKinematic=true; rb.useGravity=false;
        foreach(var r in item.GetComponentsInChildren<Renderer>()) r.shadowCastingMode=UnityEngine.Rendering.ShadowCastingMode.Off;
        EditorUtility.SetDirty(item); EditorUtility.SetDirty(facing); EditorUtility.SetDirty(xr); PrefabUtility.RecordPrefabInstancePropertyModifications(item); PrefabUtility.RecordPrefabInstancePropertyModifications(xr); PrefabUtility.RecordPrefabInstancePropertyModifications(rb);
    }
    public static void Install()
    {
        var scene=Level3MerchandisingTools.Open(); var inventory=Level3MerchandisingTools.Inventory(scene);
        if(!File.Exists(Evidence+"/baseline.txt") || IsRevised(inventory)) throw new Exception("Capture baseline once; revision must not already be installed.");
        var dimensions=new StringBuilder("Section\tBefore\tAfter\tSource\n");
        foreach(var spec in Level3LayoutTools.Sections)
        {
            var section=inventory.Find(spec.Name); var old=Furniture(section); var before=Level3LayoutTools.BoundsOf(old.gameObject);
            var decor=section.Find("Decorative Stock"); if(decor) UnityEngine.Object.DestroyImmediate(decor.gameObject);
            var sign=section.Find("Category Sign"); if(sign) UnityEngine.Object.DestroyImmediate(sign.gameObject);
            if(spec.furniture!="Produce")
            {
                UnityEngine.Object.DestroyImmediate(old.gameObject);
                old=Instance("Assets/Prefabs/Supermercado/shelfMarker4.fbx",section,scene).transform;
                old.name=spec.furniture=="Chilled"?"Chilled Display":"Gondola Display";
            }
            float height=spec.furniture=="Produce"?.72f:(spec.furniture=="Chilled"?.85f:1.05f);
            Fit(old,new Vector3(1.6f,height,.70f),new Vector3(spec.x,.62505f,spec.z));
            foreach(var mf in old.GetComponentsInChildren<MeshFilter>())
            {
                var c=mf.GetComponent<MeshCollider>(); if(!c) c=mf.gameObject.AddComponent<MeshCollider>(); c.sharedMesh=mf.sharedMesh; c.convex=false; c.enabled=true;
            }
            var modifier=old.GetComponent<Unity.AI.Navigation.NavMeshModifier>(); if(!modifier) modifier=old.gameObject.AddComponent<Unity.AI.Navigation.NavMeshModifier>(); modifier.ignoreFromBuild=true;
            var body=section.Find("Display Body Collider").GetComponent<BoxCollider>(); float baseHeight=spec.furniture=="Produce"?.306f:.08f;
            body.size=new Vector3(1.6f,baseHeight,.70f); body.transform.position=new Vector3(spec.x,.62505f+baseHeight*.5f,spec.z);
            dimensions.AppendLine(spec.id+"\t"+before.size+"\t"+Level3LayoutTools.BoundsOf(old.gameObject).size+"\t"+PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(old.gameObject));
            var products=section.Find("Products"); var owners=products.GetComponentsInChildren<SelectableItem>().ToList();
            string extra=spec.id=="S01"?"lettuce":(spec.id=="S02"?"grapes":(spec.id=="S07"?"meat-sausage":null));
            if(extra!=null) owners.Add(Instance("Assets/Prefabs/"+extra+".prefab",products,scene).GetComponent<SelectableItem>());
            for(int row=0;row<owners.Count;row++)
            {
                var owner=owners[row]; string source=PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(owner.gameObject);
                string label=ProductLabel(spec.id,row);
                var group=new GameObject("Row - "+label); group.transform.SetParent(products,false); owner.transform.SetParent(group.transform,true);
                for(int unit=0;unit<12;unit++)
                {
                    var item=unit==0?owner:Instance(source,group.transform,scene).GetComponent<SelectableItem>();
                    item.gameObject.name=label+" - "+(unit+1).ToString("D2"); Interaction(item,owner);
                }
            }
            CategorySign(section,spec,Categories[int.Parse(spec.id.Substring(1))-1],height);
            section.Find("Approach").position=new Vector3(spec.x+.1f,.62505f,spec.z-.85f);
        }
        // Only add identities whose complete existing list views match exactly.
        var list=scene.GetRootGameObjects().SelectMany(g=>g.GetComponentsInChildren<ListController>(true)).Single(l=>l.isActiveAndEnabled && l.HasListContainer);
        var oldList=scene.GetRootGameObjects().Single(g=>g.name=="Supermarket").GetComponentsInChildren<ListController>(true).Single(l=>l.HasListContainer);
        var data=new SerializedObject(list); var pool=data.FindProperty("availablePrefabs"); var oldPool=new SerializedObject(oldList).FindProperty("availablePrefabs");
        foreach(var owner in Owners(inventory))
        {
            if(Enumerable.Range(0,pool.arraySize).Any(i=>pool.GetArrayElementAtIndex(i).objectReferenceValue?.name==owner.itemName)) continue;
            var match=Enumerable.Range(0,oldPool.arraySize).Select(i=>oldPool.GetArrayElementAtIndex(i).objectReferenceValue).Single(v=>v!=null && v.name==owner.itemName);
            pool.InsertArrayElementAtIndex(pool.arraySize); pool.GetArrayElementAtIndex(pool.arraySize-1).objectReferenceValue=match;
        }
        data.ApplyModifiedPropertiesWithoutUndo();
        PositionUnits(inventory); File.WriteAllText(Evidence+"/dimensions.tsv",dimensions.ToString());
        BakeAndSave(scene); Capture("after-first");
    }
    public static string ProductLabel(string id,int row)
    {
        switch(id) {
            case "S01":return new[]{"Cà rốt","Cà chua","Bắp cải"}[row]; case "S02":return new[]{"Chuối","Táo xanh","Nho"}[row];
            case "S03":return "Nước suối"; case "S04":return "Nước trái cây"; case "S05":return "Đồ hộp"; case "S06":return "Sữa";
            case "S07":return row==0?"Gà tây":"Xúc xích"; case "S08":return "Cá"; case "S09":return "Kem"; case "S10":return "Tiêu"; case "S11":return "Giấy vệ sinh"; default:return "Đĩa tròn";
        }
    }
    static Vector2 ClosestSegment(Vector2 point,Vector2 a,Vector2 b) { var d=b-a; return a+d*Mathf.Clamp01(Vector2.Dot(point-a,d)/Mathf.Max(d.sqrMagnitude,1e-10f)); }

    public static Level3ShelfFixTools.Contact ContactSurface(Transform section,float x,float z,float ceiling=10,float radius=0)
    {
        float best=float.NegativeInfinity; Level3ShelfFixTools.Contact contact=default;
        foreach(var f in Furniture(section).GetComponentsInChildren<MeshFilter>())
        {


            if(f.TryGetComponent<Renderer>(out var visible) && !visible.enabled) continue;
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
                best=y; contact=new Level3ShelfFixTools.Contact{point=new Vector3(px,y,pz),normal=normal};
            }
        }
        if(float.IsNegativeInfinity(best)) throw new Exception("No original furniture support: "+section.name+" x="+x+" z="+z+" ceiling="+ceiling);
        return contact;
    }

    static void Label(Transform parent,string name,string text,Vector3 position,float width,float height,Color color)
    {
        var go=new GameObject(name); go.transform.SetParent(parent,true); go.transform.position=position;
        var tm=go.AddComponent<TextMesh>(); tm.font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf"); tm.text=text; tm.fontSize=80; tm.characterSize=.01f; tm.anchor=TextAnchor.MiddleCenter; tm.alignment=TextAlignment.Center; tm.color=color;
        go.GetComponent<MeshRenderer>().sharedMaterial=tm.font.material;
        var b=go.GetComponent<Renderer>().bounds; go.transform.localScale=Vector3.one*Mathf.Min(width/b.size.x,height/b.size.y);
        var back=UnityEngine.Object.Instantiate(go,parent); back.name=name+" Back"; back.transform.position+=Vector3.forward*.012f; back.transform.rotation=Quaternion.Euler(0,180,0);
        var panel=GameObject.CreatePrimitive(PrimitiveType.Cube); panel.name=name+" Backing"; panel.transform.SetParent(parent,true); UnityEngine.Object.DestroyImmediate(panel.GetComponent<Collider>());
        panel.transform.position=position+Vector3.forward*.006f; panel.transform.localScale=new Vector3(width+.025f,height+.02f,.004f);
        const string materialPath="Assets/Level3Layout/SignBacking.mat";
        var mat=AssetDatabase.LoadAssetAtPath<Material>(materialPath);
        if(!mat) {mat=new Material(Shader.Find("Unlit/Color"));mat.color=new Color(.025f,.12f,.13f);AssetDatabase.CreateAsset(mat,materialPath);}
        panel.GetComponent<Renderer>().sharedMaterial=mat; var nav=panel.AddComponent<Unity.AI.Navigation.NavMeshModifier>();nav.ignoreFromBuild=true;
        foreach(var t in new[]{go,back}) {var m=t.AddComponent<Unity.AI.Navigation.NavMeshModifier>();m.ignoreFromBuild=true;}
    }
    static void CategorySign(Transform section,Level3LayoutTools.Section spec,string text,float height)
    {
        Label(section,"Category Sign",text,new Vector3(spec.x,1.85f,spec.z-.30f),.72f,.07f,Color.white);
    }
    static float ProductSize(string id,int row)
    {
        switch(id) {
            case "S01":return row==0?.15f:(row==1?.10f:.13f); case "S02":return row==0?.14f:(row==1?.10f:.13f);
            case "S03":return .24f; case "S04":return .21f; case "S05":return .18f; case "S06":return .21f;
            case "S07":return row==0?.22f:.115f; case "S08":return .23f; case "S09":return .18f; case "S10":return .17f; case "S11":return .21f; default:return .22f;
        }
    }
    public static void PositionUnits(Transform inventory)
    {
        var positions=new StringBuilder("Section\tRow\tIdentity\tPrice\tPhysical\tLogical\tSupport min/max\n");
        foreach(var spec in Level3LayoutTools.Sections)
        {
            var section=inventory.Find(spec.Name); var products=section.Find("Products"); var groups=products.Cast<Transform>().ToArray();
            section.Find("Approach").position=new Vector3(spec.x+(spec.x < -47?.1f:0),.62505f,spec.z-.85f);
            if(spec.furniture=="Produce")
            {
                var back=section.Find("Approach Back"); if(!back) {back=new GameObject("Approach Back").transform;back.SetParent(section,true);}
                back.position=new Vector3(spec.x+(spec.x < -47?.1f:0),.62505f,spec.z+.85f);
            }
            foreach(var text in section.Cast<Transform>().Where(t=>t.name.StartsWith("Row Label")).ToArray()) UnityEngine.Object.DestroyImmediate(text.gameObject);
            for(int row=0;row<groups.Length;row++)
            {
                var units=groups[row].GetComponentsInChildren<SelectableItem>(); if(units.Length!=12) throw new Exception("Unexpected row units "+spec.id+" "+groups[row].name+" count="+units.Length+" paths="+string.Join(";",units.Select(p=>Level3LayoutTools.PathOf(p.transform)))); float min=float.PositiveInfinity,max=0;
                for(int u=0;u<units.Length;u++)
                {
                    float x,z,ceiling=10; var item=units[u];
                    if(spec.furniture=="Produce")
                    {
                        if(row==1) {x=(u%3-1)*.11f;z=new[]{-.25f,-.10f,.10f,.25f}[u/3];}
                        else if(u<8) {x=(row==0?-.353f:.353f)+(u%2==0?-.075f:.075f);z=new[]{-.25f,-.10f,.10f,.25f}[u/2];}
                        else {x=row==0?-.665f:.665f;z=new[]{-.25f,-.10f,.10f,.25f}[u-8];}
                    }
                    else
                    {
                        ceiling=.62505f+(spec.furniture=="Chilled"?.85f:1.05f)*.82f;
                        x=(u%6-2.5f)*.24f; z=groups.Length>1?(u<6?-.23f:.06f):(u<6?-.12f:-.20f);
                        if(spec.id=="S07" && row==1) {x=(u-5.5f)*.12f;z=-.20f;}
                        if((groups.Length>1 && row==1) || (groups.Length==1 && u>=6)) ceiling=.62505f+(spec.furniture=="Chilled"?.85f:1.05f)*.36f;
                    }
                    var rotation=spec.id=="S08"?Quaternion.Euler(0,90,90):(spec.id=="S01" && row==0?Quaternion.Euler(0,0,65):Quaternion.identity);
                    if(spec.id=="S07" && row==1) rotation=Quaternion.Euler(0,90,0);
                    item.transform.rotation=rotation; var b=Level3LayoutTools.BoundsOf(item.gameObject); item.transform.localScale*=ProductSize(spec.id,row)/Mathf.Max(b.size.x,b.size.y,b.size.z);
                    b=Level3LayoutTools.BoundsOf(item.gameObject); item.transform.position+=new Vector3(spec.x+x,0,spec.z+z)-new Vector3(b.center.x,b.min.y,b.center.z);
                    var contact=ContactSurface(section,spec.x+x,spec.z+z,ceiling);
                    item.transform.rotation=Quaternion.FromToRotation(Vector3.up,contact.normal)*rotation;
                    b=Level3LayoutTools.BoundsOf(item.gameObject); item.transform.localScale*=ProductSize(spec.id,row)/Mathf.Max(b.size.x,b.size.y,b.size.z);
                    b=Level3LayoutTools.BoundsOf(item.gameObject); item.transform.position+=new Vector3(spec.x+x,0,spec.z+z)-new Vector3(b.center.x,b.min.y,b.center.z);
                    item.transform.position+=Vector3.up*((.002f-Level3ShelfFixTools.MinimumPlaneDistance(item.gameObject,contact))/contact.normal.y);
                    min=Mathf.Min(min,contact.point.y);max=Mathf.Max(max,contact.point.y); PrefabUtility.RecordPrefabInstancePropertyModifications(item.transform);
                }
                var owner=units.Single(p=>p.GetComponent<Level3StockFacing>().IsStockOwner);
                float labelX=spec.furniture=="Produce"?spec.x+(row-1)*.52f:spec.x;
                float labelY=spec.furniture=="Produce"?.94f:min-.025f;
                Label(section,"Row Label "+row,ProductLabel(spec.id,row),new Vector3(labelX,labelY,spec.z-.365f),spec.furniture=="Produce"?.30f:.65f,.03f,new Color(1,.90f,.53f));
                positions.AppendLine(spec.id+"\t"+ProductLabel(spec.id,row)+"\t["+owner.itemName+"]\t"+owner.price+"\t"+units.Length+"\t12\t"+min.ToString("F3")+"/"+max.ToString("F3"));
            }
        }
        File.WriteAllText(Evidence+"/rows.tsv",positions.ToString());
    }
    static void BakeAndSave(Scene scene)
    {
        Physics.SyncTransforms(); var surface=scene.GetRootGameObjects().Single(g=>g.name=="supermarket").transform.Find("supermarket").GetComponent<Unity.AI.Navigation.NavMeshSurface>();
        surface.overrideVoxelSize=true; surface.voxelSize=.08f;
        surface.RemoveData(); surface.navMeshData=null; surface.BuildNavMesh();
        const string navPath="Assets/Scenes/Scene-level-3/NavMesh-Level3VisualRevision.asset";
        var existing=AssetDatabase.LoadAssetAtPath<UnityEngine.AI.NavMeshData>(navPath);
        if(existing) {var temporary=surface.navMeshData;surface.RemoveData();EditorUtility.CopySerialized(temporary,existing);surface.navMeshData=existing;surface.AddData();EditorUtility.SetDirty(existing);UnityEngine.Object.DestroyImmediate(temporary);}
        else AssetDatabase.CreateAsset(surface.navMeshData,navPath);
        EditorUtility.SetDirty(surface); PrefabUtility.RecordPrefabInstancePropertyModifications(surface); EditorSceneManager.MarkSceneDirty(scene); EditorSceneManager.SaveScene(scene); AssetDatabase.SaveAssets();
    }
    public static void Refine()
    {
        var scene=Level3MerchandisingTools.Open(); var inventory=Level3MerchandisingTools.Inventory(scene);
        foreach(var spec in Level3LayoutTools.Sections)
        {
            var section=inventory.Find(spec.Name);
            foreach(var name in new[]{"Category Sign","Category Sign Back","Category Sign Backing"})
            {
                var old=section.Find(name); if(old) UnityEngine.Object.DestroyImmediate(old.gameObject);
            }
            var height=spec.furniture=="Produce"?.72f:(spec.furniture=="Chilled"?.85f:1.05f);
            CategorySign(section,spec,Categories[int.Parse(spec.id.Substring(1))-1],height);
        }
        PositionUnits(inventory); ApplyTextMaterials(scene); EditorSceneManager.MarkSceneDirty(scene); EditorSceneManager.SaveScene(scene); Capture("refined");
    }
    static void ApplyTextMaterials(Scene scene)
    {
        var labels=Level3MerchandisingTools.Inventory(scene).GetComponentsInChildren<TextMesh>();
        var font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf"); font.RequestCharactersInTexture(string.Join("",labels.Select(l=>l.text)),80);
        const string path="Assets/Level3Layout/WorldSignText.mat";
        var material=AssetDatabase.LoadAssetAtPath<Material>(path); if(!material) {material=new Material(Shader.Find("Level3/WorldSignText"));AssetDatabase.CreateAsset(material,path);}
        material.mainTexture=font.material.mainTexture; EditorUtility.SetDirty(material);
        foreach(var label in labels) label.GetComponent<Renderer>().sharedMaterial=material;
        AssetDatabase.SaveAssets();
    }
    static void Require(bool condition,string message) {if(!condition) throw new Exception(message);}
    public static string Validate(Scene scene)
    {
        var inventory=Level3MerchandisingTools.Inventory(scene); var owners=Owners(inventory); var units=inventory.GetComponentsInChildren<SelectableItem>();
        Require(inventory.childCount==12 && owners.Length==17 && units.Length==336,"Expected twelve sections, seventeen owners and 336 physical units");
        Require(!inventory.GetComponentsInChildren<Transform>(true).Any(Level3ShelfFixTools.IsPad),"Added support pad remains");
        Require(owners.Sum(p=>p.stockQuantity)==336 && owners.All(p=>p.stockQuantity==(p.transform.IsChildOf(inventory.Find("S01_Vegetables")) || p.transform.IsChildOf(inventory.Find("S02_FreshFruits")) || p.transform.IsChildOf(inventory.Find("S03_BottledWater")) || p.transform.IsChildOf(inventory.Find("S04_Juice")) ? (p.transform.IsChildOf(inventory.Find("S01_Vegetables")) || p.transform.IsChildOf(inventory.Find("S02_FreshFruits")) ? 30 : 24) : 12)),"Logical stock must match the restocked shelves");
        var output=new StringBuilder("PASS shared stock: 17 identities, 336 physical units, 336 logical units; proxies contribute zero.\nSection\tPhysical units\tOwners\n");
        foreach(var spec in Level3LayoutTools.Sections)
        {
            var section=inventory.Find(spec.Name); var goods=section.GetComponentsInChildren<SelectableItem>();
            var fb=Level3LayoutTools.BoundsOf(Furniture(section).gameObject);
            Require(Mathf.Abs(fb.size.x-(spec.x < -47?1.5f:1.6f))<.003f && Mathf.Abs(fb.size.z-.65f)<.003f && Mathf.Abs(fb.min.y-.62505f)<.003f,"Furniture bounds "+spec.id);
            foreach(var row in section.Find("Products").Cast<Transform>())
            {
                var products=row.GetComponentsInChildren<SelectableItem>(); Require(products.Length==(spec.furniture=="Produce" ? 30 : (spec.id=="S03" || spec.id=="S04" ? 24 : 12)),"Expected physical stock per row");
                Require(products.Select(p=>p.itemName).Distinct().Count()==1,"Mixed identities within row");
                var owner=products.Single(p=>p.GetComponent<Level3StockFacing>().IsStockOwner);
                foreach(var item in products)
                {
                    var facing=item.GetComponent<Level3StockFacing>(); Require(facing!=null && facing.stockOwner==owner,"Broken owner link");
                    var source=PrefabUtility.GetCorrespondingObjectFromSource(item); Require(source && item.itemName==source.itemName && item.price==source.price,"Source identity/price changed");
                    var xr=item.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable>();
                    Require(xr && xr.enabled && item.GetComponent<PokeObjects>() && item.GetComponent<Rigidbody>(),"Missing interaction component");
                    Require(item.GetComponentsInChildren<SelectableItem>(true).Length==1 && item.GetComponentsInChildren<UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable>(true).Length==1,"Nested interaction duplication");
                    Require(xr.selectEntered.GetPersistentEventCount()==1 && xr.selectEntered.GetPersistentMethodName(0)=="OnPokeStart" && xr.selectEntered.GetPersistentTarget(0)==item.GetComponent<PokeObjects>(),"Duplicate or wrong enter callback");
                    Require(xr.selectExited.GetPersistentEventCount()==1 && xr.selectExited.GetPersistentTarget(0)==item.GetComponent<PokeObjects>(),"Wrong exit callback");
                    Require(item.GetComponentsInChildren<CheckoutCounter>(true).Length==0,"Unexpected checkout component");
                    var b=Level3LayoutTools.BoundsOf(item.gameObject); Require(b.min.x>fb.min.x && b.max.x<fb.max.x && b.min.z>fb.min.z && b.max.z<fb.max.z,"Product outside furniture "+item.name);
                    var contact=ContactSurface(section,b.center.x,b.center.z,b.min.y+.035f);
                    float distance=Level3ShelfFixTools.MinimumPlaneDistance(item.gameObject,contact);
                    Require(distance>=-.003f && distance<=.007f,"Unsupported product "+spec.id+" "+item.name+" gap="+distance);
                    // A second surface inside the product would indicate shelf/frame penetration.
                    var top=ContactSurface(section,b.center.x,b.center.z,b.max.y-.004f);
                    Require(top.point.y<contact.point.y+.018f,"Shelf intersects product "+spec.id+" "+item.name);
                }
            }
            var sorted=goods.Select(p=>new{p,b=spec.furniture=="Produce" && section.Find("Packed Produce Layout") ? PackedProduceTools.DisplayBounds(p.gameObject) : Level3LayoutTools.BoundsOf(p.gameObject)}).ToArray();
            for(int i=0;i<sorted.Length;i++) for(int j=i+1;j<sorted.Length;j++)
            {
                var a=sorted[i].b; a.Expand(-.006f); Require(!a.Intersects(sorted[j].b),"Overlapping products "+spec.id+" "+sorted[i].p.name+" / "+sorted[j].p.name);
            }
            var category=section.Find("Category Sign").GetComponent<TextMesh>(); Require(category.text==Categories[int.Parse(spec.id.Substring(1))-1],"Vietnamese category label");
            foreach(var label in section.GetComponentsInChildren<TextMesh>())
            {
                Require(!System.Text.RegularExpressions.Regex.IsMatch(label.text,@"S\d\d"),"Technical ID on player sign");
                Require(label.GetComponent<Renderer>().sharedMaterial.shader.name=="Level3/WorldSignText","Sign must respect scene depth");
                Require(label.font.HasCharacter('\u1ee7') && label.font.HasCharacter('\u1edb'),"Font lacks Vietnamese glyphs");
            }
            output.AppendLine(spec.id+"\t"+goods.Length+"\t"+Owners(section).Length);
        }
        var list=scene.GetRootGameObjects().SelectMany(g=>g.GetComponentsInChildren<ListController>(true)).Single(l=>l.isActiveAndEnabled && l.HasListContainer);
        var data=new SerializedObject(list); var pool=data.FindProperty("availablePrefabs"); Require(pool.arraySize==17,"Catalog must have seventeen exact views");
        var names=Enumerable.Range(0,pool.arraySize).Select(i=>pool.GetArrayElementAtIndex(i).objectReferenceValue?.name).ToArray();
        Require(names.All(n=>n!=null) && names.Distinct().Count()==17 && names.All(n=>owners.Any(o=>o.itemName==n)),"Catalog identity mismatch");
        Require(data.FindProperty("spawnOnStart").intValue==8 && data.FindProperty("limitLegacyListToAvailableStock").boolValue,"Eight stock-aware rows required");
        return output.ToString();
    }
    public static string MeasureFacingAccess(Scene scene)
    {
        Physics.SyncTransforms(); var b=new StringBuilder("Geometric targeting from collision-validated front/rear approaches. Targets include mesh vertices because bin rims can hide an item's center while leaving its upper surface selectable. Furniture/signs remain; remove only a targeted unit. Saved camera height, no controller input.\n");
        var inventory=Level3MerchandisingTools.Inventory(scene);var disabled=new System.Collections.Generic.List<Collider>();
        try {
            foreach(var spec in Level3LayoutTools.Sections)
            {
                var section=inventory.Find(spec.Name);
                var eyes=new[]{section.Find("Approach"),section.Find("Approach Back"),section.Find("Approach Left"),section.Find("Approach Right"),section.Find("Approach Back Left"),section.Find("Approach Back Right")}.Where(t=>t!=null).Select(t=>new Vector3(t.position.x,1.792f,t.position.z)).ToArray();
                var remaining=section.GetComponentsInChildren<SelectableItem>().ToList(); int total=remaining.Count;
                while(remaining.Count>0)
                {
                    SelectableItem visible=null;
                    foreach(var item in remaining)
                    {
                        var vertices=item.GetComponentsInChildren<MeshFilter>().SelectMany(f=>f.sharedMesh.vertices.Select(v=>f.transform.TransformPoint(v))).ToArray();
                        foreach(var eye in eyes)
                        {
                            var targets=new[]{Level3LayoutTools.BoundsOf(item.gameObject).center}.Concat(vertices.OrderByDescending(v=>v.y).Take(12)).Concat(vertices.OrderBy(v=>(v-eye).sqrMagnitude).Take(12));
                            foreach(var target in targets)
                            {
                                var direction=target-eye; if(direction.magnitude>1.35f) continue;
                                var hits=Physics.RaycastAll(eye,direction.normalized,direction.magnitude+.03f,Physics.DefaultRaycastLayers,QueryTriggerInteraction.Ignore).Where(h=>h.collider.gameObject.scene==scene && !h.collider.transform.root.name.StartsWith("XR Origin")).OrderBy(h=>h.distance).ToArray();
                                if(hits.Length==0 || !hits[0].transform.IsChildOf(item.transform)) continue;
                                var ray=new Ray(eye,direction.normalized);
                                bool obscured=section.Cast<Transform>().Where(t=>t.name.EndsWith("Backing")).Any(t=>t.GetComponent<Renderer>().bounds.IntersectRay(ray,out float distance) && distance<direction.magnitude-.01f);
                                if(!obscured) {visible=item;break;}
                            }
                            if(visible) break;
                        }
                        if(visible) break;
                    }
                    if(!visible) throw new Exception("Unreachable physical units "+spec.id+": "+string.Join(",",remaining.Select(p=>p.name)));
                    foreach(var c in visible.GetComponentsInChildren<Collider>().Where(c=>c.enabled)) {disabled.Add(c);c.enabled=false;}
                    remaining.Remove(visible);Physics.SyncTransforms();
                }
                b.AppendLine(spec.id+" all "+total+" units targetable in depletion order; rays <=1.35m");
            }
        } finally {foreach(var c in disabled) if(c) c.enabled=true;Physics.SyncTransforms();}
        return b.AppendLine("PASS: all "+InventoryUnitCount(scene)+" physical units can be targeted in a collision-clear depletion order.").ToString();
    }
    static int InventoryUnitCount(Scene scene) => Level3MerchandisingTools.Inventory(scene).GetComponentsInChildren<SelectableItem>().Length;
    [MenuItem("Tools/Level 3/Visual revision/Validate saved scene")]
    public static void Audit()
    {
        var scene=Level3MerchandisingTools.Open(); File.WriteAllText(Evidence+"/validation.txt",Validate(scene));
        File.WriteAllText(Evidence+"/accessibility.txt",Level3LayoutTools.MeasureAccess(scene,false)+"\n"+MeasureFacingAccess(scene));
        File.WriteAllText(Evidence+"/after-counts.txt",Level3MerchandisingTools.Counts(scene));
        Level3StabilityTools.Inspect(); File.Copy("Library/Level3Stability-static.txt",Evidence+"/static-references.txt",true);
        File.Copy("Library/Level3Inventory-result.txt",Evidence+"/inventory-validation.txt",true);
    }
    public static void LiftAndRefine()
    {
        var scene=Level3MerchandisingTools.Open(); var inventory=Level3MerchandisingTools.Inventory(scene);
        Require(!File.Exists(Evidence+"/shelf-lift.txt"),"Shelf lift already applied");
        foreach(var spec in Level3LayoutTools.Sections.Where(s=>s.furniture!="Produce"))
        {
            var trays=Furniture(inventory.Find(spec.Name)).GetComponentsInChildren<MeshFilter>().Single(f=>f.name=="Object002").transform;
            trays.position+=Vector3.up*(spec.furniture=="Chilled"?.10f:.15f); PrefabUtility.RecordPrefabInstancePropertyModifications(trays);
        }
        PositionUnits(inventory); ApplyTextMaterials(scene); EditorSceneManager.MarkSceneDirty(scene); EditorSceneManager.SaveScene(scene);
        File.WriteAllText(Evidence+"/shelf-lift.txt","Moved the original two-tier Object002 mesh within the existing frame: general +0.15m; chilled +0.10m. No added surface; frame bases remain on the floor.");
        Capture("lifted");
    }
    public static void FinalizeAuthoring() { Refine(); CaptureSaved(); Audit(); }
    public static void RefineAisle()
    {
        var scene=Level3MerchandisingTools.Open(); var inventory=Level3MerchandisingTools.Inventory(scene); var report=new StringBuilder("West bank width refined for the compound rig's west-side root corridor, rather than testing camera access alone.\n");
        foreach(var spec in Level3LayoutTools.Sections)
        {
            var section=inventory.Find(spec.Name);
            if(spec.x < -47)
            {
                var f=Furniture(section); var bounds=Level3LayoutTools.BoundsOf(f.gameObject); Fit(f,new Vector3(1.5f,bounds.size.y,.70f),new Vector3(spec.x,.62505f,spec.z));
                var body=section.Find("Display Body Collider").GetComponent<BoxCollider>(); var size=body.size;size.x=1.5f;body.size=size;
            }
            report.AppendLine(spec.id+" "+Level3LayoutTools.BoundsOf(Furniture(section).gameObject));
        }
        PositionUnits(inventory);ApplyTextMaterials(scene);BakeAndSave(scene);
        File.WriteAllText(Evidence+"/final-dimensions.txt",report.ToString());Audit();
    }
    public static void RefineDepth()
    {
        var scene=Level3MerchandisingTools.Open();var inventory=Level3MerchandisingTools.Inventory(scene);
        foreach(var spec in Level3LayoutTools.Sections)
        {
            var section=inventory.Find(spec.Name);var f=Furniture(section);var b=Level3LayoutTools.BoundsOf(f.gameObject);
            Fit(f,new Vector3(spec.x < -47?1.5f:1.6f,b.size.y,.65f),new Vector3(spec.x,.62505f,spec.z));
            var body=section.Find("Display Body Collider").GetComponent<BoxCollider>();var size=body.size;size.z=.65f;body.size=size;
        }
        PositionUnits(inventory);ApplyTextMaterials(scene);BakeAndSave(scene);
        var surface=scene.GetRootGameObjects().Single(g=>g.name=="supermarket").transform.Find("supermarket").GetComponent<Unity.AI.Navigation.NavMeshSurface>();
        var settings=UnityEngine.AI.NavMesh.GetSettingsByID(surface.agentTypeID);
        File.WriteAllText(Evidence+"/navigation-settings.txt","Agent radius="+settings.agentRadius+"; height="+settings.agentHeight+"; climb="+settings.agentClimb+"; voxel="+surface.voxelSize+"; agentType="+surface.agentTypeID);
        File.WriteAllLines(Evidence+"/final-dimensions.txt",Level3LayoutTools.Sections.Select(spec=>spec.id+" "+Level3LayoutTools.BoundsOf(Furniture(inventory.Find(spec.Name)).gameObject)));
        Audit();
    }
    [MenuItem("Tools/Level 3/Visual revision/Capture saved views")]
    public static void CaptureSaved() {Level3MerchandisingTools.Open();Capture("saved-reload");}
    public static void Capture(string prefix)
    {
        var player=Camera.main;
        if(player) Render(prefix+"-saved-player-eye",player.transform.position,player.transform.position+player.transform.forward*3,false);
        foreach(var spec in Level3LayoutTools.Sections.Where(s=>s.furniture=="Produce")) Render(prefix+"-"+spec.id+"-rear",new Vector3(spec.x+.1f,1.792f,spec.z+1.45f),new Vector3(spec.x,1.20f,spec.z),false);
        Render(prefix+"-overview",new Vector3(-46.6f,3.1f,1.5f),new Vector3(-46.6f,0,1.5f),true,1000,1800,10.5f);
        Render(prefix+"-north-aisle",new Vector3(-46.5f,1.792f,3.5f),new Vector3(-46.5f,1.4f,9),false);
        Render(prefix+"-south-aisle",new Vector3(-46.5f,1.792f,3.5f),new Vector3(-46.5f,1.4f,-4.3f),false);
        foreach(var spec in Level3LayoutTools.Sections) Render(prefix+"-"+spec.id,new Vector3(spec.x+.1f,1.792f,spec.z-1.45f),new Vector3(spec.x,1.25f,spec.z),false);
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















