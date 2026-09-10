using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyPatrol : MonoBehaviour
{
    [Header("Patrol Settings")]
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private float waypointWaitTime = 2f;
    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private float chaseSpeed = 5f;
    
    private NavMeshAgent navAgent;
    private EnemyAwareness awareness;
    private int currentWaypointIndex;
    private float waitTimer;
    private bool isWaiting;
    private Vector3 lastKnownPlayerPosition;
    private bool hasLastKnownPosition;

    private void Awake()
    {
        navAgent = GetComponent<NavMeshAgent>();
        awareness = GetComponent<EnemyAwareness>();
        navAgent.speed = patrolSpeed;
    }

    private void OnEnable()
    {
        if (awareness != null)
        {
            awareness.OnStateChanged += HandleStateChange;
        }
    }

    private void OnDisable()
    {
        if (awareness != null)
        {
            awareness.OnStateChanged -= HandleStateChange;
        }
    }

    private void Update()
    {
        switch (awareness.CurrentState)
        {
            case AwarenessState.Idle:
                Patrol();
                break;
            case AwarenessState.Suspicious:
                Investigate();
                break;
            case AwarenessState.Alert:
                Chase();
                break;
        }
    }

    private void Patrol()
    {
        if (waypoints.Length == 0) return;
        
        if (isWaiting)
        {
            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0f)
            {
                isWaiting = false;
                currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
                navAgent.SetDestination(waypoints[currentWaypointIndex].position);
            }
            return;
        }
        
        if (!navAgent.pathPending && navAgent.remainingDistance <= navAgent.stoppingDistance)
        {
            isWaiting = true;
            waitTimer = waypointWaitTime;
        }
    }

    private void Investigate()
    {
        if (hasLastKnownPosition)
        {
            navAgent.SetDestination(lastKnownPlayerPosition);
            
            if (!navAgent.pathPending && navAgent.remainingDistance <= navAgent.stoppingDistance)
            {
                hasLastKnownPosition = false;
            }
        }
    }

    private void Chase()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            lastKnownPlayerPosition = player.transform.position;
            hasLastKnownPosition = true;
            navAgent.SetDestination(player.transform.position);
        }
    }

    private void HandleStateChange(AwarenessState newState)
    {
        switch (newState)
        {
            case AwarenessState.Idle:
                navAgent.speed = patrolSpeed;
                hasLastKnownPosition = false;
                break;
            case AwarenessState.Suspicious:
                navAgent.speed = patrolSpeed;
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    lastKnownPlayerPosition = player.transform.position;
                    hasLastKnownPosition = true;
                }
                break;
            case AwarenessState.Alert:
                navAgent.speed = chaseSpeed;
                break;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (waypoints == null || waypoints.Length == 0) return;
        
        Gizmos.color = Color.cyan;
        for (int i = 0; i < waypoints.Length; i++)
        {
            if (waypoints[i] != null)
            {
                Gizmos.DrawWireSphere(waypoints[i].position, 0.5f);
                
                if (i < waypoints.Length - 1 && waypoints[i + 1] != null)
                {
                    Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
                }
                else if (i == waypoints.Length - 1 && waypoints[0] != null)
                {
                    Gizmos.DrawLine(waypoints[i].position, waypoints[0].position);
                }
            }
        }
    }
}
