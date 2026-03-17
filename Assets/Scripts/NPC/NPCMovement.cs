using UnityEngine;

public class NPCMovement : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float rotationSpeed = 10f;
    private Vector3 moveDirection;
    private NPCAnimator animator;

    void Start()
    {
        animator = GetComponent<NPCAnimator>();
    }

    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        moveDirection = new Vector3(horizontal, 0, vertical).normalized;

        if (moveDirection.magnitude >= 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            
            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
            
            animator.SetWalk(1f);
        }
        else
        {
            animator.SetWalk(0f);
        }
    }
}
