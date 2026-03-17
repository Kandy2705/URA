using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NPCFollowerNavMesh : MonoBehaviour
{
    public Transform playerCamera;

    public float followDistance = 5f;
    public float stoppingDistance = 2f;
    public float moveSpeed = 1.5f;
    public float rotationSpeed = 5f;

    private NavMeshAgent agent;
    private NPCAnimator animator;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<NPCAnimator>();

        if (playerCamera == null)
        {
            playerCamera = Camera.main.transform;
        }

        agent.speed = moveSpeed;
        agent.stoppingDistance = stoppingDistance;
    }

    void Update()
    {
        if (playerCamera == null) return;

        Vector3 targetPos = playerCamera.position;
        targetPos.y = transform.position.y;

        float dist = Vector3.Distance(transform.position, targetPos);

        if (dist > followDistance)
        {
            agent.isStopped = false;
            agent.SetDestination(targetPos);
        }
        else
        {
            agent.isStopped = true;
        }

        float normalizedSpeed = agent.velocity.magnitude / agent.speed;
        if (animator != null)
        {
            animator.SetWalk(normalizedSpeed);
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