using UnityEngine;
using System.Collections;

public class EnemyVision : MonoBehaviour
{
    [Header("Vision Settings")]
    [SerializeField] private float visionRange = 10f;
    [SerializeField] private float fieldOfView = 90f;
    [SerializeField] private float detectionCheckDelay = 0.2f;
    [SerializeField] private LayerMask coverLayer;
    
    [Header("Detection")]
    [SerializeField] private float baseDetectionRate = 20f;
    [SerializeField] private Transform eyePosition;
    
    private EnemyAwareness awareness;
    private SphereCollider detectionCollider;
    private Coroutine detectPlayerCoroutine;
    private GameObject player;
    private PlayerVisibility playerVisibility;

    private void Awake()
    {
        awareness = GetComponent<EnemyAwareness>();
        
        detectionCollider = gameObject.AddComponent<SphereCollider>();
        detectionCollider.isTrigger = true;
        detectionCollider.radius = visionRange;
        
        if (eyePosition == null)
        {
            GameObject eyeObj = new GameObject("EyePosition");
            eyeObj.transform.SetParent(transform);
            eyeObj.transform.localPosition = new Vector3(0, 1.6f, 0);
            eyePosition = eyeObj.transform;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.gameObject;
            playerVisibility = player.GetComponent<PlayerVisibility>();
            detectPlayerCoroutine = StartCoroutine(DetectPlayer());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (detectPlayerCoroutine != null)
            {
                StopCoroutine(detectPlayerCoroutine);
            }
            player = null;
            playerVisibility = null;
        }
    }

    private IEnumerator DetectPlayer()
    {
        while (player != null)
        {
            yield return new WaitForSeconds(detectionCheckDelay);
            
            if (CanSeePlayer())
            {
                float detectionAmount = baseDetectionRate * playerVisibility.CurrentVisibility * detectionCheckDelay;
                awareness.IncreaseAwareness(detectionAmount);
            }
        }
    }

    private bool CanSeePlayer()
    {
        if (player == null) return false;
        
        Vector3[] boundingPoints = GetBoundingPoints(player.GetComponent<Collider>().bounds);
        int pointsVisible = 0;
        
        foreach (Vector3 point in boundingPoints)
        {
            if (IsPointVisible(point))
            {
                pointsVisible++;
            }
        }
        
        return pointsVisible > 0;
    }

    private bool IsPointVisible(Vector3 point)
    {
        Vector3 directionToPoint = point - eyePosition.position;
        float distanceToPoint = directionToPoint.magnitude;
        
        if (distanceToPoint > visionRange) return false;
        
        float angleToPoint = Vector3.Angle(transform.forward, directionToPoint);
        if (angleToPoint > fieldOfView / 2f) return false;
        
        RaycastHit[] hits = Physics.RaycastAll(eyePosition.position, directionToPoint.normalized, distanceToPoint);
        
        foreach (RaycastHit hit in hits)
        {
            if (((1 << hit.collider.gameObject.layer) & coverLayer) != 0)
            {
                return false;
            }
        }
        
        return true;
    }

    private Vector3[] GetBoundingPoints(Bounds bounds)
    {
        return new Vector3[]
        {
            bounds.min,
            bounds.max,
            new Vector3(bounds.min.x, bounds.min.y, bounds.max.z),
            new Vector3(bounds.min.x, bounds.max.y, bounds.min.z),
            new Vector3(bounds.max.x, bounds.min.y, bounds.min.z),
            new Vector3(bounds.min.x, bounds.max.y, bounds.max.z),
            new Vector3(bounds.max.x, bounds.min.y, bounds.max.z),
            new Vector3(bounds.max.x, bounds.max.y, bounds.min.z)
        };
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, visionRange);
        
        Vector3 leftBoundary = Quaternion.Euler(0, -fieldOfView / 2f, 0) * transform.forward * visionRange;
        Vector3 rightBoundary = Quaternion.Euler(0, fieldOfView / 2f, 0) * transform.forward * visionRange;
        
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + leftBoundary);
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary);
    }
}
