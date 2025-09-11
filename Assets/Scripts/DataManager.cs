using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public Transform player;          // Player transform
    public Transform[] targets;       // Assign 3 targets in the Inspector
    public float interactionRange = 10f;

    private void Update()
    {
        if (player == null || targets.Length == 0) return;

        for (int i = 0; i < targets.Length; i++)
        {
            if (targets[i] == null) continue;

            float dist = Vector3.Distance(player.position, targets[i].position);

            // Use switch to handle each target differently
            if (i == 0)
            {
                if (dist <= interactionRange)
                    Debug.Log("✅ Player close to Target 1 → maybe pick up item");
                break;
            }
            else if (i == 1)
            {
                if (dist <= interactionRange)
                    Debug.Log("✅ Player close to Target 2 → maybe open a door");
            }
            else if (i == 2)
            {
                if (dist <= interactionRange)
                    Debug.Log("✅ Player close to Target 3 → maybe talk to NPC");
            }
            else
            {
                Debug.Log("Target not handled!");
            }
        }
    }
}
