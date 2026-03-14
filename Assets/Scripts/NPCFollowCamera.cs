using UnityEngine;

public class NPCFollowerSimple : MonoBehaviour
{
    public Transform playerCamera;

    public float followDistance = 5f;
    public float moveSpeed = 1.2f; 
    public float rotationSpeed = 3f;

    private Animator animator;

    void Start()
    {
        if (playerCamera == null)
        {
            playerCamera = Camera.main.transform;
        }

        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (playerCamera == null) return;

        Vector3 targetPos = playerCamera.position;
        targetPos.y = transform.position.y;

        float dist = Vector3.Distance(transform.position, targetPos);

        // chỉ đi khi còn xa
        if (dist > followDistance)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPos,
                moveSpeed * Time.deltaTime
            );

            if (animator != null)
                animator.SetFloat("Speed", 1f);
        }
        else
        {
            if (animator != null)
                animator.SetFloat("Speed", 0f);
        }

        RotateToPlayer();
    }

    void RotateToPlayer()
    {
        Vector3 lookDir = playerCamera.position - transform.position;
        lookDir.y = 0;

        if (lookDir != Vector3.zero)
        {
            Quaternion rot = Quaternion.LookRotation(lookDir);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                rot,
                rotationSpeed * Time.deltaTime
            );
        }
    }
}