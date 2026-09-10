using UnityEngine;

[RequireComponent(typeof(EnemyAwareness))]
public class DiaperSmellDetection : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private EnemyAwareness awareness;
    
    [Header("Smell Detection Settings")]
    [SerializeField] private float smellDetectionRange = 5f;
    [SerializeField] private float throughWallsRange = 3f;
    [SerializeField] private float smellDetectionRate = 10f;
    [SerializeField] private float smellCheckInterval = 0.5f;
    
    [Header("Detection Layers")]
    [SerializeField] private LayerMask wallLayers;
    
    private GameObject player;
    private DiaperState playerDiaperState;
    private float smellCheckTimer = 0f;
    
    private void Awake()
    {
        if (awareness == null)
        {
            awareness = GetComponent<EnemyAwareness>();
        }
    }
    
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        
        if (player != null)
        {
            playerDiaperState = player.GetComponent<DiaperState>();
        }
    }
    
    private void Update()
    {
        if (player == null || playerDiaperState == null) return;
        
        smellCheckTimer += Time.deltaTime;
        
        if (smellCheckTimer >= smellCheckInterval)
        {
            CheckSmell();
            smellCheckTimer = 0f;
        }
    }
    
    private void CheckSmell()
    {
        float smellIntensity = playerDiaperState.GetSmellIntensity();
        
        if (smellIntensity <= 0f) return;
        
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        
        bool canSmellThroughWalls = distanceToPlayer <= throughWallsRange;
        bool isInNormalRange = distanceToPlayer <= smellDetectionRange;
        
        if (!isInNormalRange && !canSmellThroughWalls) return;
        
        bool hasLineOfSight = true;
        if (!canSmellThroughWalls)
        {
            Vector3 directionToPlayer = (player.transform.position - transform.position).normalized;
            RaycastHit hit;
            
            if (Physics.Raycast(transform.position, directionToPlayer, out hit, distanceToPlayer, wallLayers))
            {
                hasLineOfSight = false;
            }
        }
        
        if (canSmellThroughWalls || hasLineOfSight)
        {
            float distanceFactor = 1f - (distanceToPlayer / smellDetectionRange);
            distanceFactor = Mathf.Max(0f, distanceFactor);
            
            float detectionAmount = smellDetectionRate * smellIntensity * distanceFactor * smellCheckInterval;
            
            awareness.IncreaseAwareness(detectionAmount);
            
            if (detectionAmount > 1f)
            {
                Debug.Log($"[SmellDetection] Enemy detected smell! Intensity: {smellIntensity:F2}, Distance: {distanceToPlayer:F1}m, Awareness +{detectionAmount:F1}");
            }
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.5f, 1f, 0.5f, 0.3f);
        Gizmos.DrawWireSphere(transform.position, smellDetectionRange);
        
        Gizmos.color = new Color(1f, 0.5f, 0.5f, 0.5f);
        Gizmos.DrawWireSphere(transform.position, throughWallsRange);
    }
}
