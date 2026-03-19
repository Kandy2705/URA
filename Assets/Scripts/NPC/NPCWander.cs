using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
public class NPCWander : MonoBehaviour
{
    public float wanderSpeed = 1.5f;
    public float wanderRadius = 10f;
    public float waitTimeMin = 2f;
    public float waitTimeMax = 5f;

    private NavMeshAgent agent;
    private NPCAnimator animator;
    private bool isWaiting = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<NPCAnimator>();
        
        agent.speed = wanderSpeed;
        agent.stoppingDistance = 0.5f;
        
        StartCoroutine(WanderLoop());
    }

    IEnumerator WanderLoop()
    {
        while (true)
        {
            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance && !isWaiting)
            {
                StartCoroutine(PickNewDestination());
            }
            float normalizedSpeed = agent.velocity.magnitude / agent.speed;
            animator.SetWalk(normalizedSpeed);

            yield return new WaitForSeconds(0.1f);
        }
    }

    IEnumerator PickNewDestination()
    {
        isWaiting = true;
        
        agent.isStopped = true;
        float actualWaitTime = Random.Range(waitTimeMin, waitTimeMax);
        yield return new WaitForSeconds(actualWaitTime);
        
        Vector3 newDest = GetRandomPoint(transform.position, wanderRadius);
        
        agent.SetDestination(newDest);
        agent.isStopped = false;
        
        isWaiting = false;
    }
    
    private Vector3 GetRandomPoint(Vector3 center, float radius)
    {
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection += center;
        
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, radius, NavMesh.AllAreas))
        {
            return hit.position;
        }
        
        return center;
    }
}