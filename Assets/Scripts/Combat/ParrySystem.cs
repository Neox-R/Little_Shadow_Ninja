using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class ParrySystem : MonoBehaviour
{
    [Header("Parry Settings")]
    [SerializeField] private float parryWindowDuration = 0.3f;
    [SerializeField] private float parryCooldown = 1.5f;
    [SerializeField] private float parryStunDuration = 2.0f;
    
    [Header("Counter Attack")]
    [SerializeField] private bool enableCounterAttack = true;
    [SerializeField] private float counterDamageMultiplier = 2.5f;
    [SerializeField] private float counterWindowDuration = 1.0f;
    
    private PlayerCombat playerCombat;
    private ComboSystem comboSystem;
    
    private bool isParrying = false;
    private bool canParry = true;
    private bool canCounter = false;
    private float parryCooldownTimer = 0f;
    private float parryWindowTimer = 0f;
    private float counterWindowTimer = 0f;
    
    private Transform lastParriedEnemy;
    
    public bool IsParrying => isParrying;
    public bool CanCounter => canCounter;
    
    private void Awake()
    {
        playerCombat = GetComponent<PlayerCombat>();
        comboSystem = GetComponent<ComboSystem>();
    }
    
    private void Update()
    {
        UpdateTimers();
    }
    
    private void UpdateTimers()
    {
        if (!canParry)
        {
            parryCooldownTimer -= Time.deltaTime;
            if (parryCooldownTimer <= 0f)
            {
                canParry = true;
            }
        }
        
        if (isParrying)
        {
            parryWindowTimer -= Time.deltaTime;
            if (parryWindowTimer <= 0f)
            {
                EndParry(false);
            }
        }
        
        if (canCounter)
        {
            counterWindowTimer -= Time.deltaTime;
            if (counterWindowTimer <= 0f)
            {
                canCounter = false;
                lastParriedEnemy = null;
            }
        }
    }
    
    public void OnParry(InputAction.CallbackContext context)
    {
        if (context.performed && CanParry())
        {
            StartParry();
        }
        else if (context.performed && canCounter && enableCounterAttack)
        {
            ExecuteCounter();
        }
    }
    
    private bool CanParry()
    {
        if (!canParry) return false;
        if (isParrying) return false;
        if (playerCombat != null && playerCombat.IsAttacking) return false;
        
        return true;
    }
    
    private void StartParry()
    {
        isParrying = true;
        canParry = false;
        parryWindowTimer = parryWindowDuration;
        
        Debug.Log("[Parry] Parry window active!");
    }
    
    private void EndParry(bool successful)
    {
        isParrying = false;
        parryCooldownTimer = parryCooldown;
        
        if (!successful)
        {
            Debug.Log("[Parry] Parry window missed");
        }
    }
    
    public bool TryParryAttack(Transform attacker, float incomingDamage)
    {
        if (!isParrying) return false;
        
        Debug.Log("[Parry] ⚔️ SUCCESSFUL PARRY! ⚔️");
        
        EnemyHealth enemyHealth = attacker.GetComponent<EnemyHealth>();
        if (enemyHealth != null)
        {
            enemyHealth.Stun(parryStunDuration);
        }
        
        if (enableCounterAttack)
        {
            canCounter = true;
            counterWindowTimer = counterWindowDuration;
            lastParriedEnemy = attacker;
            Debug.Log("[Parry] Counter attack available!");
        }
        
        if (comboSystem != null)
        {
            comboSystem.ExtendCombo(1.5f);
        }
        
        EndParry(true);
        
        return true;
    }
    
    private void ExecuteCounter()
    {
        if (lastParriedEnemy == null)
        {
            canCounter = false;
            return;
        }
        
        Debug.Log("[Parry] 💥 COUNTER ATTACK! 💥");
        
        EnemyHealth enemyHealth = lastParriedEnemy.GetComponent<EnemyHealth>();
        if (enemyHealth != null)
        {
            float counterDamage = 50f * counterDamageMultiplier;
            enemyHealth.TakeDamage(counterDamage, transform.position, true);
        }
        
        canCounter = false;
        lastParriedEnemy = null;
        
        if (comboSystem != null)
        {
            comboSystem.ResetCombo();
            comboSystem.ExecuteHeavyAttack();
        }
    }
}
