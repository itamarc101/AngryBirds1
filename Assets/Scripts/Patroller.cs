using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Patroller : MonoBehaviour
{
    public enum PathStyle
        {
            StraightLine,
            Circular
        }
        
    [SerializeField] private PathStyle pathStyle = PathStyle.StraightLine;

    [Tooltip("The distance the NPC moves left and right from the starting point")]
    [SerializeField] private float moveDistance = 10f;
    [Tooltip("The radius for circular movement")]
    [SerializeField] private float circleRadius = 5f;

    [SerializeField] private bool initialDirectionIsRight = true;

    private NavMeshAgent navMeshAgent;
    private Animator animator;
    private float rotationSpeed = 5f;

    private Vector3 startPoint;
    private Vector3 leftEndpoint;
    private Vector3 rightEndpoint;
    private float timeElapsed;

    // private bool hasSetInitialDestination = false;

    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        startPoint = transform.position;
        leftEndpoint = startPoint - new Vector3(moveDistance, 0f, 0f);
        rightEndpoint = startPoint + new Vector3(moveDistance, 0f, 0f);
    }

    private void Update()
    {
        switch (pathStyle)
        {
            case PathStyle.StraightLine:
                PatrolStraightLine();
                break;
            case PathStyle.Circular:
                PatrolCircular();
                break;

        }

        UpdateAnimations();
        FaceDestination();
    }

    private void PatrolStraightLine()
    {
        animator.SetFloat("WalkingSpeed", navMeshAgent.velocity.magnitude);
        if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance < 0.1f)
        {
            initialDirectionIsRight = !initialDirectionIsRight;
            navMeshAgent.SetDestination(initialDirectionIsRight ? rightEndpoint : leftEndpoint);
        }
    }

    private void PatrolCircular()
    {
        animator.SetFloat("WalkingSpeed", navMeshAgent.velocity.magnitude);
        timeElapsed += Time.deltaTime;
        float angle = timeElapsed * Mathf.PI * 2 / circleRadius;
        Vector3 circularPosition = startPoint + new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * circleRadius;
        navMeshAgent.SetDestination(circularPosition);
    }

    private void UpdateAnimations()
    {
        animator.SetFloat("WalkingSpeed", navMeshAgent.velocity.magnitude);
        animator.SetBool("isWalking", navMeshAgent.velocity.magnitude > 0.1f);
    }

    // private void SetInitialDestination()
    // {
    //     navMeshAgent.SetDestination(initialDirectionIsRight ? rightEndpoint : leftEndpoint);
    //     hasSetInitialDestination = true;
    // }

    // private void Patrol()
    // {
    //     // Check if the NPC has reached the destination
    //     if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance < 0.1f)
    //     {
    //         // Switch direction
    //         initialDirectionIsRight = !initialDirectionIsRight;

    //         // Set the new destination
    //         navMeshAgent.SetDestination(initialDirectionIsRight ? rightEndpoint : leftEndpoint);
    //     }
        
    //     // Check if the NPC has exceeded the move distance from the starting point
    //     float distanceFromStartPoint = Vector3.Distance(transform.position, startPoint);
    //     if (distanceFromStartPoint > moveDistance)
    //     {
    //         // Reset the destination to the starting point
    //         navMeshAgent.SetDestination(startPoint);
    //     }
    // }

    // private void UpdateAnimations()
    // {
    //     // Agent is moving, play "Walk" animation
    //     animator.SetBool("isWalking", true);
    // }

    private void FaceDestination()
    {
        if (navMeshAgent.velocity.magnitude > 0.1f)
        {
            Vector3 directionToDestination = (navMeshAgent.destination - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(directionToDestination.x, 0, directionToDestination.z), Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
        }
    }
}
