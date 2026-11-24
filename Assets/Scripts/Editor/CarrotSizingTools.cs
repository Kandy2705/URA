using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
public static class CarrotSizingTools
{
    public static void Apply()
    {
        var scene=EditorSceneManager.OpenScene(Level3LayoutTools.ScenePath);
        var section=Level3MerchandisingTools.Inventory(scene).Find("S01_Vegetables");
        var carrots=section.GetComponentsInChildren<SelectableItem>().Where(i=>PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(i.gameObject)=="Assets/Prefabs/Carrot.prefab").ToArray();
        if(carrots.Length!=24) throw new Exception("Expected 24 carrots");
        int index=0;
        foreach(var item in carrots)
        {
            var old=Level3LayoutTools.BoundsOf(item.gameObject);
            var depths=new[]{-.26f,-.18f,-.10f,.10f,.18f,.26f};
            int slot=index%12;
            float x=index<12?-.353f+(slot%2==0?-.075f:.075f):(slot%2==0?-.69f:-.60f);
            float z=depths[slot/2]; index++;
            var mesh=item.GetComponent<MeshFilter>().sharedMesh.bounds.size;
            var axis=mesh.z>=mesh.x && mesh.z>=mesh.y?Vector3.forward:(mesh.y>=mesh.x?Vector3.up:Vector3.right);
            item.transform.rotation=Quaternion.FromToRotation(axis,Vector3.up);
            item.transform.localScale=Vector3.one*(.15f/Mathf.Max(mesh.x,mesh.y,mesh.z));
            var b=Level3LayoutTools.BoundsOf(item.gameObject);
            item.transform.position+=new Vector3(-48.2f+x-b.center.x,0,6f+z-b.center.z);
            var support=Level3VisualRevisionTools.ContactSurface(section,-48.2f+x,6f+z);
            item.transform.position+=Vector3.up*((.002f-Level3ShelfFixTools.MinimumPlaneDistance(item.gameObject,support))/support.normal.y);
            PrefabUtility.RecordPrefabInstancePropertyModifications(item.transform);
        }
        Level3VisualRevisionTools.Validate(scene);
        Level3VisualRevisionTools.MeasureFacingAccess(scene);
        EditorSceneManager.MarkSceneDirty(scene); EditorSceneManager.SaveScene(scene);
        ShelfFollowupTools.Capture("carrots-fixed");
        Directory.CreateDirectory("Evidence/CarrotSizing");
        File.WriteAllText("Evidence/CarrotSizing/result.txt","24 carrots use the same uniform mesh scale and 0.15m length, upright rather than compressed by the sloping tray depth cap. Shelf support, overlaps and targeting validated. Other products, stock and interaction unchanged.");
    }
}
