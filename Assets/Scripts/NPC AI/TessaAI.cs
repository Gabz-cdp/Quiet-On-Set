using UnityEngine;
using UnityEngine.AI;

public class TessaAI : MonoBehaviour
{
    private NavMeshAgent agent;
    public Transform[] patrolPoints;
    private int currentPoint = 0;

    public Animator animator;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (agent.remainingDistance != 0)
        {
            animator.SetBool("isMoving", true);
        }
        else if(!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            currentPoint++;

            if (currentPoint >= patrolPoints.Length)
            {
                currentPoint = 0;
            }

            agent.SetDestination(patrolPoints[currentPoint].position);
        }
    }

}
