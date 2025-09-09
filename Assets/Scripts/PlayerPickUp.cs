using System;
using UnityEngine;

public class PlayerPickUp : MonoBehaviour
{
    [SerializeField] private Transform playerCameraTransform;
    [SerializeField] private LayerMask pickUpLayerMask;
    [SerializeField] private Transform grabPointTransform;

    [SerializeField]
    [Range(2f, 5f)]
    private float pickUpDis;
    
    private GrabableObject grabableObject;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (grabableObject == null)
            {
                if (Physics.Raycast(playerCameraTransform.position,
                        playerCameraTransform.forward,
                        out RaycastHit rayCastHit, pickUpDis))
                {
                    if (rayCastHit.transform.TryGetComponent(out grabableObject))
                    {
                        grabableObject.Grab(grabPointTransform);
                    }
                }
            }
            else
            {
                grabableObject.Drop();
                grabableObject = null;
            }
        }
    }
}
