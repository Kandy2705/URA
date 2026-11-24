using System;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
public static class ShelfVisualSizing
{
    public static void Apply()
    {
        var scene=EditorSceneManager.OpenScene(Level3LayoutTools.ScenePath);
        var inventory=Level3MerchandisingTools.Inventory(scene);
        var report=new StringBuilder(); int changed=0;
        foreach(var spec in Level3LayoutTools.Sections.Where(s=>s.id=="S02"))
        {
            var section=inventory.Find(spec.Name);
            var units=section.GetComponentsInChildren<SelectableItem>();
            var furniture=Level3LayoutTools.BoundsOf(Level3VisualRevisionTools.Furniture(section).gameObject);
            var walls=Level3VisualRevisionTools.Furniture(section).GetComponentsInChildren<Collider>();
            foreach(var item in units.OrderBy(i=>Level3LayoutTools.BoundsOf(i.gameObject).size.sqrMagnitude))
            {
                var position=item.transform.position; var scale=item.transform.localScale;
                var original=Level3LayoutTools.BoundsOf(item.gameObject);
                bool accepted=false;
                foreach(float factor in new[]{1.35f,1.25f,1.15f,1.08f})
                {
                    foreach(float dx in new[]{0f,.012f,-.012f,.025f,-.025f})
                    {
                        item.transform.localScale=scale*factor; item.transform.position=position;
                        var b=Level3LayoutTools.BoundsOf(item.gameObject);
                        item.transform.position+=new Vector3(original.center.x+dx-b.center.x,0,original.center.z-b.center.z);
                        var support=Level3VisualRevisionTools.ContactSurface(section,original.center.x+dx,original.center.z,original.min.y+.035f);
                        item.transform.position+=Vector3.up*((.002f-Level3ShelfFixTools.MinimumPlaneDistance(item.gameObject,support))/support.normal.y);
                        b=Level3LayoutTools.BoundsOf(item.gameObject);
                        if(b.min.x<furniture.min.x+.015f || b.max.x>furniture.max.x-.015f || b.min.z<furniture.min.z+.015f || b.max.z>furniture.max.z-.015f || Mathf.Abs(b.min.y-original.min.y)>.025f) continue;
                        var padded=b; padded.Expand(.002f);
                        if(units.Any(n=>n!=item && padded.Intersects(Level3LayoutTools.BoundsOf(n.gameObject)))) continue;
                        Physics.SyncTransforms(); var collider=item.GetComponent<Collider>();
                        if(walls.Any(w=>Physics.ComputePenetration(collider,collider.transform.position,collider.transform.rotation,w,w.transform.position,w.transform.rotation,out _,out float depth) && depth>.002f)) continue;
                        accepted=true; changed++; report.AppendLine(spec.id+" "+item.name+" scale x"+factor); break;
                    }
                    if(accepted) break;
                }
                if(!accepted) { item.transform.position=position; item.transform.localScale=scale; }
                else PrefabUtility.RecordPrefabInstancePropertyModifications(item.transform);
            }
        }
        if(changed==0) throw new Exception("No enlargement fits existing bins; scene not saved.");
        Level3VisualRevisionTools.Validate(scene);
        Level3VisualRevisionTools.MeasureFacingAccess(scene);
        EditorSceneManager.MarkSceneDirty(scene); EditorSceneManager.SaveScene(scene);
        Directory.CreateDirectory("Evidence/ShelfSizing");
        File.WriteAllText("Evidence/ShelfSizing/result.txt","Enlarged "+changed+" units. Original stock, player collision and grab preserved.\n"+report);
        ShelfFollowupTools.Capture("larger");
    }
}

