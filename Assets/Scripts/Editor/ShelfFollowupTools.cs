using System;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit.Locomotion;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEditor.TestTools.TestRunner.Api;

[InitializeOnLoad]
public static class ShelfFollowupTools
{
    public const string Output="Evidence/ShelfFollowup";
    const string Request="Library/ShelfFollowup.request";
    static TestRunnerApi runner;
    static ShelfFollowupTools()
    {
        EditorApplication.update+=Poll;
        if(SessionState.GetBool("ShelfFollowup.Testing",false)) RegisterRunner();
    }
    static void Poll()
    {
        if(EditorApplication.isCompiling || EditorApplication.isUpdating || !File.Exists(Request)) return;
        var action=File.ReadAllText(Request).Trim(); File.Delete(Request);
        Directory.CreateDirectory(Output);
        try { if(action=="probe") Probe(); else if(action=="capture") Capture("before"); else if(action=="install") Install(); else if(action=="tests") Tests(); }
        catch(Exception e) { File.WriteAllText(Output+"/error.txt",e.ToString()); }
    }
    public static void Probe()
    {
        Directory.CreateDirectory(Output);
        var scene=SceneManager.GetSceneByPath(Level3LayoutTools.ScenePath);
        if(!scene.isLoaded) throw new Exception("Level 3 must be loaded; no scene switch performed.");
        var b=new StringBuilder("Playing="+EditorApplication.isPlaying+" Dirty="+scene.isDirty+"\n");
        foreach(var root in scene.GetRootGameObjects())
        {
            foreach(var c in root.GetComponentsInChildren<CharacterController>(true))
                b.AppendLine("CC "+Level3LayoutTools.PathOf(c.transform)+" active="+(c.enabled && c.gameObject.activeInHierarchy)+" pos="+c.transform.position+" scale="+c.transform.lossyScale+" radius="+c.radius+" height="+c.height+" center="+c.center+" step="+c.stepOffset+" skin="+c.skinWidth+" detect="+c.detectCollisions+" layer="+c.gameObject.layer+" bounds="+c.bounds);
            foreach(var t in root.GetComponentsInChildren<XRBodyTransformer>(true))
                b.AppendLine("BODY "+Level3LayoutTools.PathOf(t.transform)+" active="+t.isActiveAndEnabled+" useCC="+t.useCharacterControllerIfExists+" origin="+t.xrOrigin?.Origin.name+" manipulator="+t.constrainedBodyManipulator);
        }
        var inventory=Level3MerchandisingTools.Inventory(scene);
        foreach(var spec in Level3LayoutTools.Sections)
        {
            var section=inventory.Find(spec.Name); var furniture=Level3VisualRevisionTools.Furniture(section);
            b.AppendLine("SECTION "+spec.Name+" geometry="+Level3LayoutTools.BoundsOf(furniture.gameObject)+" goods="+section.GetComponentsInChildren<SelectableItem>().Length);
            foreach(var c in section.GetComponentsInChildren<Collider>(true).Where(c=>c.GetComponentInParent<SelectableItem>()==null))
                b.AppendLine("BLOCKER "+Level3LayoutTools.PathOf(c.transform)+" type="+c.GetType().Name+" enabled="+c.enabled+" trigger="+c.isTrigger+" layer="+c.gameObject.layer+" bounds="+c.bounds);
        }
        File.WriteAllText(Output+"/probe.txt",b.ToString());
    }
    public static void Capture(string prefix)
    {
        Directory.CreateDirectory(Output);
        foreach(var spec in Level3LayoutTools.Sections.Where(s=>s.furniture=="Produce"))
        {
            Level3MerchandisingTools.Render("../ShelfFollowup/"+prefix+"-"+spec.id,new Vector3(spec.x,1.85f,spec.z-.95f),new Vector3(spec.x,1.05f,spec.z),false);
            Level3MerchandisingTools.Render("../ShelfFollowup/"+prefix+"-"+spec.id+"-rear",new Vector3(spec.x,1.85f,spec.z+.95f),new Vector3(spec.x,1.05f,spec.z),false);
        }
    }
    static Scene CleanScene()
    {
        if(EditorApplication.isPlayingOrWillChangePlaymode) throw new Exception("Stop Play Mode first.");
        var scene=SceneManager.GetSceneByPath(Level3LayoutTools.ScenePath);
        if(!scene.isLoaded || scene.isDirty) throw new Exception("Requires loaded, saved Level 3; unsaved scene was not overwritten.");
        return scene;
    }
    public static void Install()
    {
        var scene=CleanScene(); Directory.CreateDirectory(Output);
        if(!File.Exists("Library/ShelfFollowup-before.unity")) File.Copy(Level3LayoutTools.ScenePath,"Library/ShelfFollowup-before.unity");
        if(!File.Exists(Output+"/before-S02.png")) Capture("before");
        var inventory=Level3MerchandisingTools.Inventory(scene);
        foreach(var spec in Level3LayoutTools.Sections)
        {
            var section=inventory.Find(spec.Name); var bounds=Level3LayoutTools.BoundsOf(Level3VisualRevisionTools.Furniture(section).gameObject);
            var barrier=section.Find("Player Body Barrier");
            if(!barrier) { barrier=new GameObject("Player Body Barrier").transform; barrier.SetParent(section,true); }
            barrier.gameObject.layer=2; barrier.position=bounds.center; barrier.rotation=Quaternion.identity; barrier.localScale=Vector3.one;
            var box=barrier.GetComponent<BoxCollider>(); if(!box) box=barrier.gameObject.AddComponent<BoxCollider>();
            box.center=Vector3.zero; box.size=bounds.size; box.isTrigger=false; box.enabled=true;
            // Body layer already used by the rig. Food, rays and hand targets use Default.
            box.includeLayers=1<<2; box.excludeLayers=~(1<<2); box.layerOverridePriority=10;
            var modifier=barrier.GetComponent<Unity.AI.Navigation.NavMeshModifier>(); if(!modifier) modifier=barrier.gameObject.AddComponent<Unity.AI.Navigation.NavMeshModifier>();
            modifier.ignoreFromBuild=true;
            if(spec.furniture=="Produce")
            {
                FillProduce(section,spec,scene);
                foreach(var side in new[]{-1,1}) foreach(var depth in new[]{-1,1})
                {
                    string name=(depth>0?"Approach Back ":"Approach ")+(side<0?"Left":"Right");
                    var marker=section.Find(name); if(!marker) { marker=new GameObject(name).transform; marker.SetParent(section,true); }
                    marker.position=new Vector3(bounds.center.x+side*.55f,.62505f,bounds.center.z+depth*.85f);
                    var hits=Physics.OverlapCapsule(marker.position+Vector3.up*.25f,marker.position+Vector3.up*.95f,.22f,~0,QueryTriggerInteraction.Ignore)
                        .Where(c=>!c.transform.root.name.StartsWith("XR Origin")).ToArray();
                    if(hits.Length>0) throw new Exception("Produce side approach blocked: "+name+" "+spec.id+" "+string.Join(",",hits.Select(c=>Level3LayoutTools.PathOf(c.transform))));
                }
            }
        }
        var body=scene.GetRootGameObjects().SelectMany(r=>r.GetComponentsInChildren<XRBodyTransformer>(true)).Single(t=>t.isActiveAndEnabled);
        body.useCharacterControllerIfExists=true; PrefabUtility.RecordPrefabInstancePropertyModifications(body);
        var simulatorCollision=body.xrOrigin.GetComponent<SimulatorBodyCollision>();
        if(!simulatorCollision) simulatorCollision=body.xrOrigin.gameObject.AddComponent<SimulatorBodyCollision>();
        simulatorCollision.xrOrigin=body.xrOrigin; simulatorCollision.bodyTransformer=body;
        simulatorCollision.simulator=scene.GetRootGameObjects().SelectMany(r=>r.GetComponentsInChildren<Behaviour>(true)).Single(b=>b.GetType().Name=="XRDeviceSimulator" && b.isActiveAndEnabled);
        foreach(var controller in body.xrOrigin.GetComponentsInChildren<CharacterController>(true))
        {
            controller.gameObject.layer=2; controller.stepOffset=.12f;
            controller.skinWidth=.02f; controller.minMoveDistance=0;
            PrefabUtility.RecordPrefabInstancePropertyModifications(controller);
            PrefabUtility.RecordPrefabInstancePropertyModifications(controller.gameObject);
        }
        EditorSceneManager.MarkSceneDirty(scene); EditorSceneManager.SaveScene(scene); AssetDatabase.SaveAssets();
        Probe(); Capture("after");
        File.WriteAllText(Output+"/validation.txt",Level3VisualRevisionTools.Validate(scene)+"\n"+Level3VisualRevisionTools.MeasureFacingAccess(scene)+"\n"+Level3LayoutTools.MeasureAccess(scene,false));
        File.WriteAllText(Output+"/installed.txt","12 body-only shelf barriers. Produce S01/S02: 36 -> 72 units each, six identities x24. Total 300. Shared prefab links/stock owners retained. Player layer 2; food Default excluded from barriers; near/far target masks unchanged.\n");
    }
    public static void InstallBatch()
    {
        EditorSceneManager.OpenScene(Level3LayoutTools.ScenePath);
        Install();
    }
    static void FillProduce(Transform section,Level3LayoutTools.Section spec,Scene scene)
    {
        if(section.Find("Packed Produce Layout")) return;
        int rowIndex=0;
        foreach(Transform row in section.Find("Products"))
        {
            var units=row.GetComponentsInChildren<SelectableItem>().ToList();
            var owner=units.Single(i=>i.GetComponent<Level3StockFacing>().IsStockOwner);
            var source=AssetDatabase.LoadAssetAtPath<GameObject>(PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(owner.gameObject));
            while(units.Count<24)
            {
                var go=(GameObject)PrefabUtility.InstantiatePrefab(source,scene); go.transform.SetParent(row,false);
                go.transform.localScale=owner.transform.localScale; go.name=owner.name+" - Refill "+units.Count;
                ShelfGrabTools.Configure(go,false);
                var item=go.GetComponent<SelectableItem>(); item.stockQuantity=1;
                var facing=go.AddComponent<Level3StockFacing>(); facing.stockOwner=owner;
                PrefabUtility.RecordPrefabInstancePropertyModifications(item);
                units.Add(item);
            }
            if(units.Count!=24) throw new Exception("Unexpected produce quantity "+row.name);
            owner.stockQuantity=24; PrefabUtility.RecordPrefabInstancePropertyModifications(owner);
            for(int i=0;i<units.Count;i++)
            {
                var item=units[i]; float x,z;
                var fourDepths=new[]{-.26f,-.20f,-.14f,-.08f,.08f,.14f,.20f,.26f};
                var threeDepths=new[]{-.26f,-.18f,-.10f,.10f,.18f,.26f};
                if(rowIndex==0 && i<16) { x=-.353f+(i%2==0?-.075f:.075f); z=fourDepths[i/2]; }
                else if(rowIndex==0) { x=-.665f; z=fourDepths[i-16]; }
                else if(rowIndex==1) { x=(i%4-1.5f)*.08f; z=threeDepths[i/4]; }
                else if(i<18) { x=.34f+(i%3-1)*.08f; z=threeDepths[i/3]; }
                else { x=.665f; z=threeDepths[i-18]; }
                var rotation=rowIndex==0 && spec.id=="S01"?UprightProduceRotation(item):Quaternion.identity;
                item.transform.rotation=rotation;
                var b=Level3LayoutTools.BoundsOf(item.gameObject);
                float size=rowIndex==0?(spec.id=="S01"?.144f:.1344f):(rowIndex==1?.075f:.11f);
                item.transform.localScale*=size/Mathf.Max(b.size.x,b.size.y,b.size.z);
                var contact=Level3VisualRevisionTools.ContactSurface(section,spec.x+x,spec.z+z);
                item.transform.rotation=Quaternion.FromToRotation(Vector3.up,contact.normal)*rotation;
                b=Level3LayoutTools.BoundsOf(item.gameObject);
                float capX=rowIndex==0?.14f:(rowIndex==1?.075f:.073f);
                float capZ=rowIndex==0?.05f:(rowIndex==1?.075f:.073f);
                item.transform.localScale*=Mathf.Min(1,capX/b.size.x,capZ/b.size.z);
                b=Level3LayoutTools.BoundsOf(item.gameObject);
                item.transform.position+=new Vector3(spec.x+x,0,spec.z+z)-new Vector3(b.center.x,0,b.center.z);
                item.transform.position+=Vector3.up*((.002f-Level3ShelfFixTools.MinimumPlaneDistance(item.gameObject,contact))/contact.normal.y);
                PrefabUtility.RecordPrefabInstancePropertyModifications(item.transform);
            }
            rowIndex++;
        }
    }
    static Quaternion UprightProduceRotation(SelectableItem item)
    {
        item.transform.rotation=Quaternion.identity;
        var s=Level3LayoutTools.BoundsOf(item.gameObject).size;
        var axis=s.z>=s.x && s.z>=s.y?Vector3.forward:(s.y>=s.x?Vector3.up:Vector3.right);
        return Quaternion.FromToRotation(axis,Vector3.up);
    }
    static void RegisterRunner()
    {
        runner=ScriptableObject.CreateInstance<TestRunnerApi>(); runner.RegisterCallbacks(new Callbacks());
    }
    public static void OpenForReview()
    {
        for(int i=0;i<SceneManager.sceneCount;i++) if(SceneManager.GetSceneAt(i).isDirty) throw new Exception("Unsaved scene left intact; open Level 3 manually.");
        EditorSceneManager.OpenScene(Level3LayoutTools.ScenePath);
    }
    public static void Tests()
    {
        CleanScene(); SessionState.SetBool("ShelfFollowup.Testing",true); RegisterRunner();
        runner.Execute(new ExecutionSettings(new Filter{testMode=TestMode.EditMode,groupNames=new[]{"^ShelfFollowupTests\\.","^ShelfGrabTests\\.","^Level3MerchandisingTests\\.","^Level3PhysicalStockTests\\.","^Level3LayoutTests\\.","^Level3LocomotionTests\\."}}));
    }
    class Callbacks:ICallbacks
    {
        public void RunStarted(ITestAdaptor test) {}
        public void TestStarted(ITestAdaptor test) {}
        public void TestFinished(ITestResultAdaptor result) {}
        public void RunFinished(ITestResultAdaptor result)
        {
            if(!SessionState.GetBool("ShelfFollowup.Testing",false)) return;
            SessionState.SetBool("ShelfFollowup.Testing",false);
            TestRunnerApi.SaveResultToFile(result,Output+"/tests.xml");
            File.WriteAllText(Output+"/test-summary.txt",result.TestStatus+" passed="+result.PassCount+" failed="+result.FailCount);
            EditorApplication.delayCall+=()=> { if(!EditorApplication.isPlayingOrWillChangePlaymode && !SceneManager.GetActiveScene().isDirty) EditorSceneManager.OpenScene(Level3LayoutTools.ScenePath); };
        }
    }
}
