using UnityEngine;
using UnityEngine.AI;

public class MoveTo : MonoBehaviour
{
    [Header("Patrol Settings")]
    [SerializeField] private string waypointTag = "Waypoint";
    [SerializeField] private float patrolSpeed = 3.5f;
    [SerializeField] private float patrolTurnSpeed = 120f;

    [Header("Alert Settings")]
    public Transform alertTarget;
    [SerializeField] private float alertSpeed = 7.0f;
    [SerializeField] private float alertTurnSpeed = 360f;
    [SerializeField] private float attackRange = 2.0f;

    private NavMeshAgent agent;
    private Transform[] waypoints;
    private int currentWaypointIndex = -1;
    private bool isAlerted = false;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        InitializeWaypoints();
        SetPatrolState();
    }

    private void Update()
    {
        if (agent == null) return;

        if (isAlerted)
        {
            HandleAlertMovement();
        }
        else
        {
            HandlePatrolMovement();
        }
    }

    private void InitializeWaypoints()
    {
        GameObject[] waypointObjects = GameObject.FindGameObjectsWithTag(waypointTag);
        waypoints = new Transform[waypointObjects.Length];

        for (int i = 0; i < waypointObjects.Length; i++)
        {
            waypoints[i] = waypointObjects[i].transform;
        }
    }

    private void HandleAlertMovement()
    {
        if (alertTarget == null) return;

        float distanceToTarget = Vector3.Distance(transform.position, alertTarget.position);

        if (distanceToTarget <= attackRange)
        {
            agent.isStopped = true;
            FaceTarget();
        }
        else
        {
            agent.isStopped = false;
            agent.SetDestination(alertTarget.position);
        }
    }

    private void FaceTarget()
    {
        Vector3 direction = (alertTarget.position - transform.position).normalized;
        direction.y = 0;

        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    private void HandlePatrolMovement()
    {
        if (waypoints == null || waypoints.Length == 0) return;

        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            GoToNextWaypoint();
        }
    }

    private void GoToNextWaypoint()
    {
        if (waypoints == null || waypoints.Length == 0) return;

        int nextIndex = currentWaypointIndex;

        if (waypoints.Length > 1)
        {
            while (nextIndex == currentWaypointIndex)
            {
                nextIndex = Random.Range(0, waypoints.Length);
            }
        }
        else
        {
            nextIndex = 0;
        }

        currentWaypointIndex = nextIndex;
        agent.SetDestination(waypoints[currentWaypointIndex].position);
    }

    public void SetPatrolState()
    {
        if (agent == null) return;

        isAlerted = false;
        agent.isStopped = false;
        agent.speed = patrolSpeed;
        agent.angularSpeed = patrolTurnSpeed;

        GoToNextWaypoint();
    }

    public void SetAlertState()
    {
        if (agent == null) return;

        isAlerted = true;
        agent.isStopped = false;
        agent.speed = alertSpeed;
        agent.angularSpeed = alertTurnSpeed;
    }
}
