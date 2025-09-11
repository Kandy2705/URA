using UnityEngine;

public class PlayerPush : MonoBehaviour
{
    [SerializeField] private Transform playerCameraTransform;
    [SerializeField] private LayerMask holdLayerMask;
    [SerializeField] private Transform pushPointTransform;
    
    [SerializeField]
    [Range(2f, 5f)]
    private float pushDis;
    
    private PushableObject pushableObject;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (pushableObject == null)
            {
                if (Physics.Raycast(playerCameraTransform.position,
                        playerCameraTransform.forward,
                        out RaycastHit rayCastHit, pushDis))
                {
                    if (rayCastHit.transform.TryGetComponent(out pushableObject))
                    {
                        pushableObject.Hold(pushPointTransform);
                    }
                }
            }
            else
            {
                pushableObject.Release();
                pushableObject = null;
            }
        }
    }
}
