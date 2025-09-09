using System;
using UnityEngine;

public class GrabableObject : MonoBehaviour
{
    private Rigidbody objectRigidbody;
    private Transform objectGrabPointTransform;

    private void Awake()
    {
        objectRigidbody =  GetComponent<Rigidbody>();
    }

    public void Grab(Transform grabPointTransform)
    {
        this.objectGrabPointTransform = grabPointTransform;
        this.objectRigidbody.useGravity = false;
        this.objectRigidbody.isKinematic = true;
        this.objectRigidbody.linearDamping = 5f;
    }

    public void Drop()
    {
        this.objectGrabPointTransform = null;
        this.objectRigidbody.useGravity = true;
        this.objectRigidbody.isKinematic = false;
        this.objectRigidbody.linearDamping = 0f;
    }

    public void FixedUpdate()
    {
        if (objectGrabPointTransform != null)
        {
            float lerpSpeed = 10f;
            Vector3 newPosition = Vector3.Lerp(transform.position, objectGrabPointTransform.position, lerpSpeed * Time.deltaTime);
            objectRigidbody.MovePosition(newPosition);
        }
    }
}
