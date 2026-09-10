using UnityEngine;

public class PlayerVisibility : MonoBehaviour
{
    [Header("Visibility Settings")]
    [SerializeField] private float baseVisibility = 1f;
    [SerializeField] private float crouchVisibilityMultiplier = 0.5f;
    [SerializeField] private float shadowVisibilityMultiplier = 0.2f;
    [SerializeField] private float movementVisibilityBonus = 0.3f;
    
    [Header("Debug")]
    [SerializeField] private bool showDebugInfo = true;
    
    private PlayerController playerController;
    private bool isInShadow;
    private float currentVisibility;
    
    public float CurrentVisibility => currentVisibility;
    public bool IsInShadow => isInShadow;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
    }

    private void Update()
    {
        CalculateVisibility();
    }

    private void CalculateVisibility()
    {
        currentVisibility = baseVisibility;
        
        if (playerController.IsCrouching)
        {
            currentVisibility *= crouchVisibilityMultiplier;
        }
        
        if (isInShadow)
        {
            currentVisibility *= shadowVisibilityMultiplier;
        }
        
        if (playerController.CurrentSpeed > 0.1f)
        {
            currentVisibility += movementVisibilityBonus;
        }
        
        currentVisibility = Mathf.Clamp(currentVisibility, 0f, 2f);
        
        if (showDebugInfo)
        {
            Debug.Log($"Visibility: {currentVisibility:F2} | Shadow: {isInShadow} | Crouch: {playerController.IsCrouching}");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Shadow"))
        {
            isInShadow = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Shadow"))
        {
            isInShadow = false;
        }
    }
}
