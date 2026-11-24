using System;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
[InitializeOnLoad]
public static class Level3CheckoutTools
{
 static Level3CheckoutTools() { EditorApplication.update += Poll; }
 static void Poll() {
 if(EditorApplication.isCompiling || EditorApplication.isUpdating || EditorApplication.isPlayingOrWillChangePlaymode ||
 !File.Exists("Library/Level3Checkout.install")) return;
 File.Delete("Library/Level3Checkout.install");
 try { Install(); } catch(Exception e) { File.WriteAllText("Library/Level3Checkout-install.txt",e.ToString()); }
 }
 static void Install() {
 var scene=SceneManager.GetSceneByPath("Assets/Scenes/Scene-level-3.unity");
 if(!scene.isLoaded || scene.isDirty) throw new Exception("Requires clean loaded Level 3; no unsaved state overwritten.");
 var all=scene.GetRootGameObjects().SelectMany(r=>r.GetComponentsInChildren<Transform>(true)).ToArray();
 var teleport=all.Select(t=>t.GetComponent<VRCheckoutTeleport>()).First(c=>c!=null);
 var counter=all.Single(t=>t.name=="Checkout_Counter:Checkout_Counter" && t.gameObject.activeInHierarchy);
 var renderers=counter.GetComponentsInChildren<Renderer>();
 if(renderers.Length==0) throw new Exception("Counter bounds unavailable");
 Bounds bounds=renderers[0].bounds; foreach(var r in renderers.Skip(1)) bounds.Encapsulate(r.bounds);
 var cashier=all.Single(t=>t.name=="NPCs charge money" && t.gameObject.activeInHierarchy);
 // Cashier faces the customer side. Stand beyond the rendered counter's front edge.
 Vector3 direction=cashier.forward; direction.y=0; direction.Normalize();
 Vector3 stand=bounds.center + direction*(bounds.extents.z+0.8f);
 if(!Physics.Raycast(new Vector3(stand.x,bounds.max.y+2,stand.z), Vector3.down,out var floor,5))
 throw new Exception("No floor under candidate checkout position");
 stand.y=floor.point.y;
 var blockers=Physics.OverlapCapsule(stand+Vector3.up*0.25f,stand+Vector3.up*0.9f,0.22f,
 ~0, QueryTriggerInteraction.Ignore).Where(c=>!c.transform.IsChildOf(teleport.xrOrigin)).ToArray();
 if(blockers.Length>0) throw new Exception("Candidate blocked: "+string.Join(",",blockers.Select(c=>c.name)));
 var eye=stand+Vector3.up*1.0f;
 Undo.RecordObjects(new UnityEngine.Object[]{teleport,teleport.poiChargeMoney,teleport.checkoutAnchor},"Configure Level 3 checkout");
 teleport.poiChargeMoney.SetPositionAndRotation(eye,Quaternion.LookRotation(-direction));
 teleport.checkoutAnchor.SetPositionAndRotation(eye,Quaternion.LookRotation(-direction));
 teleport.scrollUiSampleToHide=all.Single(t=>t.name=="ShoppingListUI").gameObject;
 var providers=teleport.xrOrigin.GetComponentsInChildren<Behaviour>(true);
 teleport.movementScripts=providers.Where(p=>p.GetType().Name.Contains("MoveProvider")).ToArray();
 teleport.turnScripts=providers.Where(p=>p.GetType().Name.Contains("TurnProvider")).ToArray();
 teleport.otherControlScripts=new Behaviour[0];
 if(teleport.movementScripts.Length==0 || teleport.turnScripts.Length==0) throw new Exception("Missing locomotion providers");
 EditorSceneManager.MarkSceneDirty(scene);
 EditorSceneManager.SaveScene(scene);
 File.WriteAllText("Library/Level3Checkout-install.txt","PASS counter="+bounds+" floor="+floor.point+" eye="+eye+" facing="+(-direction)+
 " move="+teleport.movementScripts.Length+" turn="+teleport.turnScripts.Length);
 }
}

