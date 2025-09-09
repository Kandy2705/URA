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
    }

    public void Drop()
    {
        this.objectGrabPointTransform = null;
        this.objectRigidbody.useGravity = true;
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
