using UnityEngine;

[RequireComponent(typeof(DiaperState))]
[RequireComponent(typeof(BladderBowelSystem))]
public class PlayerDiaperIntegration : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private PlayerVisibility playerVisibility;
    [SerializeField] private DiaperState diaperState;
    [SerializeField] private BladderBowelSystem bladderBowel;
    
    [Header("Movement Penalty Settings")]
    [SerializeField] private bool applyMovementPenalty = true;
    
    [Header("Visibility Penalty Settings")]
    [SerializeField] private bool applyVisibilityPenalty = true;
    [SerializeField] private float smellVisibilityMultiplier = 1.5f;
    
    private float originalWalkSpeed;
    private float originalSprintSpeed;
    private float originalCrouchSpeed;
    
    private void Awake()
    {
        if (playerController == null)
        {
            playerController = GetComponent<PlayerController>();
        }
        
        if (playerVisibility == null)
        {
            playerVisibility = GetComponent<PlayerVisibility>();
        }
        
        if (diaperState == null)
        {
            diaperState = GetComponent<DiaperState>();
        }
        
        if (bladderBowel == null)
        {
            bladderBowel = GetComponent<BladderBowelSystem>();
        }
    }
    
    private void Start()
    {
        if (playerController != null)
        {
            CacheOriginalSpeeds();
        }
        
        if (diaperState != null)
        {
            diaperState.OnConditionChanged += HandleDiaperConditionChanged;
            diaperState.OnStartedLeaking += HandleStartedLeaking;
        }
    }
    
    private void OnDestroy()
    {
        if (diaperState != null)
        {
            diaperState.OnConditionChanged -= HandleDiaperConditionChanged;
            diaperState.OnStartedLeaking -= HandleStartedLeaking;
        }
    }
    
    private void Update()
    {
        if (applyMovementPenalty && playerController != null)
        {
            UpdateMovementSpeed();
        }
    }
    
    private void CacheOriginalSpeeds()
    {
        originalWalkSpeed = 3f;
        originalSprintSpeed = 6f;
        originalCrouchSpeed = 1.5f;
    }
    
    private void UpdateMovementSpeed()
    {
        float speedMultiplier = diaperState.GetMovementSpeedMultiplier();
    }
    
    public float GetVisibilityModifier()
    {
        if (!applyVisibilityPenalty) return 1f;
        
        float smellIntensity = diaperState.GetSmellIntensity();
        
        if (smellIntensity > 0f)
        {
            return 1f + (smellIntensity * 0.2f);
        }
        
        return 1f;
    }
    
    private void HandleDiaperConditionChanged(DiaperState.DiaperCondition newCondition)
    {
        Debug.Log($"[DiaperIntegration] Diaper condition changed to: {newCondition}");
    }
    
    private void HandleStartedLeaking()
    {
        Debug.Log("[DiaperIntegration] WARNING: Diaper started leaking! Enemies can smell you more easily!");
    }
}
