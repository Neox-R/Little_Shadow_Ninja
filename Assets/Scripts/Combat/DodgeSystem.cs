using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class DodgeSystem : MonoBehaviour
{
    [Header("Dodge Settings")]
    [SerializeField] private float dodgeDistance = 5f;
    [SerializeField] private float dodgeDuration = 0.4f;
    [SerializeField] private float dodgeCooldown = 0.8f;
    
    [Header("Invincibility Frames")]
    [SerializeField] private float iFramesDuration = 0.3f;
    
    [Header("Perfect Dodge")]
    [SerializeField] private float perfectDodgeWindow = 0.2f;
    [SerializeField] private float perfectDodgeSlowMoTime = 0.5f;
    [SerializeField] private float perfectDodgeSlowMoScale = 0.3f;
    
    [Header("Dodge Offset (Bayonetta-style)")]
    [SerializeField] private bool enableDodgeOffset = true;
    [SerializeField] private float dodgeOffsetWindow = 0.5f;
    
    private CharacterController characterController;
    private PlayerCombat playerCombat;
    private ComboSystem comboSystem;
    
    private bool isDodging = false;
    private bool isInvincible = false;
    private bool canDodge = true;
    private float dodgeCooldownTimer = 0f;
    private float dodgeOffsetTimer = 0f;
    
    public bool IsDodging => isDodging;
    public bool IsInvincible => isInvincible;
    
    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        playerCombat = GetComponent<PlayerCombat>();
        comboSystem = GetComponent<ComboSystem>();
    }
    
    private void Update()
    {
        UpdateCooldowns();
    }
    
    private void UpdateCooldowns()
    {
        if (!canDodge)
        {
            dodgeCooldownTimer -= Time.deltaTime;
            if (dodgeCooldownTimer <= 0f)
            {
                canDodge = true;
            }
        }
        
        if (dodgeOffsetTimer > 0f)
        {
            dodgeOffsetTimer -= Time.deltaTime;
        }
    }
    
    public void OnDodge(InputAction.CallbackContext context)
    {
        if (context.performed && CanDodge())
        {
            Vector2 moveInput = Vector2.zero;
            
            if (context.ReadValue<Vector2>() != Vector2.zero)
            {
                moveInput = context.ReadValue<Vector2>();
            }
            
            PerformDodge(moveInput);
        }
    }
    
    private bool CanDodge()
    {
        if (!canDodge) return false;
        if (isDodging) return false;
        
        return true;
    }
    
    private void PerformDodge(Vector2 inputDirection)
    {
        Vector3 dodgeDirection = CalculateDodgeDirection(inputDirection);
        
        bool isDodgeOffset = enableDodgeOffset && playerCombat.IsAttacking && dodgeOffsetTimer > 0f;
        
        if (isDodgeOffset)
        {
            Debug.Log("[Dodge] DODGE OFFSET! Combo maintained!");
            if (comboSystem != null)
            {
                comboSystem.ExtendCombo(1.0f);
            }
        }
        
        StartCoroutine(ExecuteDodge(dodgeDirection, isDodgeOffset));
    }
    
    private Vector3 CalculateDodgeDirection(Vector2 input)
    {
        if (input.magnitude < 0.1f)
        {
            return -transform.forward;
        }
        
        Vector3 cameraForward = Camera.main.transform.forward;
        Vector3 cameraRight = Camera.main.transform.right;
        cameraForward.y = 0;
        cameraRight.y = 0;
        cameraForward.Normalize();
        cameraRight.Normalize();
        
        return (cameraForward * input.y + cameraRight * input.x).normalized;
    }
    
    private IEnumerator ExecuteDodge(Vector3 direction, bool isDodgeOffset)
    {
        isDodging = true;
        canDodge = false;
        
        StartCoroutine(InvincibilityFrames());
        
        float elapsedTime = 0f;
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = startPosition + (direction * dodgeDistance);
        
        while (elapsedTime < dodgeDuration)
        {
            float t = elapsedTime / dodgeDuration;
            float curveT = Mathf.Sin(t * Mathf.PI);
            
            Vector3 newPosition = Vector3.Lerp(startPosition, targetPosition, curveT);
            Vector3 movement = newPosition - transform.position;
            
            characterController.Move(movement);
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        isDodging = false;
        dodgeCooldownTimer = dodgeCooldown;
        
        if (enableDodgeOffset)
        {
            dodgeOffsetTimer = dodgeOffsetWindow;
        }
        
        Debug.Log($"[Dodge] Dodge complete! {(isDodgeOffset ? "(Offset)" : "")}");
    }
    
    private IEnumerator InvincibilityFrames()
    {
        isInvincible = true;
        Debug.Log("[Dodge] Invincibility active!");
        
        yield return new WaitForSeconds(iFramesDuration);
        
        isInvincible = false;
    }
    
    public bool TryPerfectDodge(float timeUntilHit)
    {
        if (timeUntilHit <= perfectDodgeWindow && isDodging && isInvincible)
        {
            Debug.Log("[Dodge] ⭐ PERFECT DODGE! ⭐");
            StartCoroutine(PerfectDodgeSlowMotion());
            
            if (comboSystem != null)
            {
                comboSystem.ExtendCombo(2.0f);
            }
            
            return true;
        }
        
        return false;
    }
    
    private IEnumerator PerfectDodgeSlowMotion()
    {
        Time.timeScale = perfectDodgeSlowMoScale;
        yield return new WaitForSecondsRealtime(perfectDodgeSlowMoTime);
        Time.timeScale = 1f;
    }
}
