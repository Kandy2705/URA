using UnityEngine;

public class PushableObject : MonoBehaviour
{
    private Rigidbody objectRigidBody;
    private Transform objectPushPointTransform;

    private void Awake()
    {
        this.objectRigidBody = this.GetComponent<Rigidbody>();
    }

    public void Hold(Transform pushPointTransform)
    {
        this.objectPushPointTransform = pushPointTransform;
        this.objectRigidBody.linearDamping = 5f;
    }

    public void Release()
    {
        this.objectPushPointTransform = null;
        this.objectRigidBody.linearDamping = 0f;
    }
    
    public void FixedUpdate()
    {
        if (objectPushPointTransform != null)
        {
            float lerpSpeed = 2f;
            Vector3 targetPos = objectPushPointTransform.position;
            Vector3 newPosition = Vector3.Lerp(transform.position, targetPos, lerpSpeed * Time.deltaTime);
            
            if (newPosition.y != 0.01705062f)
            {
                newPosition.y = 0.01705062f;
            }

            objectRigidBody.MovePosition(newPosition);
        }
    }

}
