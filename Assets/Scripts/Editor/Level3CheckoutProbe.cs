using System.Linq;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
[InitializeOnLoad]
public static class Level3CheckoutProbe {
 static Level3CheckoutProbe() { EditorApplication.delayCall += Inspect; }
 static void Inspect() {
 var s=SceneManager.GetSceneByPath("Assets/Scenes/Scene-level-3.unity");
 if(!s.isLoaded || EditorApplication.isPlayingOrWillChangePlaymode) return;
 var b=new StringBuilder("dirty="+s.isDirty+"\n");
 foreach(var r in s.GetRootGameObjects()) foreach(var t in r.GetComponentsInChildren<Transform>(true)) {
 if(t.name=="NPCs charge money" || t.name=="PoiChargeMoney" || t.name=="CheckoutAnchor" || t.name.ToLower().Contains("checkout") || t.name.ToLower().Contains("cashier"))
 b.AppendLine(t.name+" active="+t.gameObject.activeInHierarchy+" pos="+t.position+" forward="+t.forward);
 }
 foreach(var r in s.GetRootGameObjects()) foreach(var c in r.GetComponentsInChildren<Behaviour>(true))
 if(c.GetType().Name.Contains("MoveProvider") || c.GetType().Name.Contains("TurnProvider"))
 b.AppendLine("PROVIDER "+c.GetType().FullName+" path="+c.name+" id="+GlobalObjectId.GetGlobalObjectIdSlow(c)+" enabled="+c.enabled);
 File.WriteAllText("Library/Level3Checkout-probe.txt", b.ToString());
 }
}

