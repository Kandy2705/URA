using UnityEngine;
using Unity.XR.CoreUtils;
using UnityEngine.XR.Interaction.Toolkit.Locomotion;

// The simulator translates tracked device poses directly instead of moving the XR Origin.
// Project that Editor-only translation through XRI's existing body collision manipulator.
[DefaultExecutionOrder(30000), DisallowMultipleComponent]
public sealed class SimulatorBodyCollision : MonoBehaviour
{
    public XROrigin xrOrigin;
    public XRBodyTransformer bodyTransformer;
    public Behaviour simulator;
    CharacterController bodyCollider;
    CharacterController[] ownColliders;
    void Awake() => IgnoreOwnCapsules();
    void LateUpdate()
    {
        IgnoreOwnCapsules();
#if UNITY_EDITOR
        ConstrainSimulatorMotion();
#endif
    }
    void IgnoreOwnCapsules()
    {
        if(!xrOrigin) return;
        if(!bodyCollider)
        {
            bodyCollider=xrOrigin.Origin.GetComponent<CharacterController>();
            ownColliders=xrOrigin.GetComponentsInChildren<CharacterController>(true);
        }
        if(!bodyCollider || !bodyCollider.enabled) return;
        foreach(var other in ownColliders)
            if(other && other!=bodyCollider && other.enabled && !Physics.GetIgnoreCollision(bodyCollider,other))
                Physics.IgnoreCollision(bodyCollider,other,true);
    }
#if UNITY_EDITOR
    Vector3 lastTrackedPosition;
    bool initialized;
    public void ResetTracking()
    {
        if(!xrOrigin || !xrOrigin.Camera) { initialized=false; return; }
        lastTrackedPosition=xrOrigin.Camera.transform.localPosition;
        initialized=true;
    }
    void OnEnable() => initialized=false;
    void ConstrainSimulatorMotion()
    {
        if(!xrOrigin || !xrOrigin.Camera || !simulator || !simulator.isActiveAndEnabled ||
           !bodyTransformer || bodyTransformer.constrainedBodyManipulator==null)
        { initialized=false; return; }
        if(UnityEngine.XR.XRSettings.isDeviceActive) { initialized=false; return; }
        var camera=xrOrigin.Camera.transform;
        var trackingSpace=camera.parent;
        if(!trackingSpace) { initialized=false; return; }
        var desired=camera.localPosition;
        if(!initialized) { ResetTracking(); return; }
        var motion=trackingSpace.TransformVector(desired-lastTrackedPosition); motion.y=0;
        // Explicit scene teleports/repositions are not keyboard walking.
        if(motion.sqrMagnitude>2.25f) { ResetTracking(); return; }
        if(motion.sqrMagnitude<.00000001f) return;
        // Accumulate sub-millimetre samples instead of letting them bypass collision.
        lastTrackedPosition=desired;
        var origin=xrOrigin.Origin.transform;
        var controller=origin.GetComponent<CharacterController>();
        if(!controller) { ResetTracking(); return; }
        bool canMove=controller.enabled;
        bool grounded=controller.isGrounded;
        // Consume only tracked-device translation, not an Origin move/turn/teleport.
        // Keeping that offset in tracking space lets the Origin follow accepted walking.
        trackingSpace.position-=motion;
        if(canMove)
        {
            Physics.SyncTransforms();
            bodyTransformer.constrainedBodyManipulator.MoveBody(motion+(grounded?Vector3.down*.002f:Vector3.zero));
        }
        // Disabled body capsules indicate a checkout movement lock: keep hands active,
        // but do not accept simulator walking while movement is locked.
    }
#endif
}
