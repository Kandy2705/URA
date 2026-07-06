using UnityEngine;
using UnityEngine.SceneManagement;

public static class NpcSceneResolver
{
    public const string DefaultNpcObjectName = "NPC";

    public static Transform FindNpcTransform(string objectName = DefaultNpcObjectName, Transform overrideTransform = null)
    {
        if (overrideTransform != null)
            return overrideTransform;

        if (string.IsNullOrWhiteSpace(objectName))
            objectName = DefaultNpcObjectName;

        GameObject activeObject = GameObject.Find(objectName);
        if (activeObject != null)
            return activeObject.transform;

        Scene activeScene = SceneManager.GetActiveScene();
        Transform[] transforms = Resources.FindObjectsOfTypeAll<Transform>();
        foreach (Transform transform in transforms)
        {
            if (transform == null)
                continue;

            GameObject gameObject = transform.gameObject;
            if (gameObject.name != objectName)
                continue;

            if (!gameObject.scene.IsValid() || gameObject.scene != activeScene)
                continue;

            if ((gameObject.hideFlags & HideFlags.HideInHierarchy) != 0)
                continue;

            return transform;
        }

        return null;
    }

    public static Animator FindNpcAnimator(string objectName = DefaultNpcObjectName, Transform overrideTransform = null)
    {
        Transform npcTransform = FindNpcTransform(objectName, overrideTransform);
        if (npcTransform == null)
            return null;

        Animator animator = npcTransform.GetComponent<Animator>();
        if (animator != null)
            return animator;

        return npcTransform.GetComponentInChildren<Animator>(true);
    }
}