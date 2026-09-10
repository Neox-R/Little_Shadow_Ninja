using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

[RequireComponent(typeof(PlayerController))]
public class PlayerCombat : MonoBehaviour
{
    [Header("Combat State")]
    [SerializeField] private bool combatEnabled = true;
    [SerializeField] private float combatRange = 2.5f;
    
    [Header("Attack Settings")]
    [SerializeField] private float lightAttackDamage = 20f;
    [SerializeField] private float heavyAttackDamage = 40f;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackConeAngle = 60f;
    
    [Header("Attack Timing")]
    [SerializeField] private float lightAttackDuration = 0.4f;
    [SerializeField] private float heavyAttackDuration = 0.8f;
    [SerializeField] private float attackCooldown = 0.2f;
    
    [Header("References")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private LayerMask enemyLayer;
    
    private PlayerController playerController;
    private ComboSystem comboSystem;
    private LockOnSystem lockOnSystem;
    private DodgeSystem dodgeSystem;
    private ParrySystem parrySystem;
    
    private bool isAttacking = false;
    private bool canAttack = true;
    private float attackCooldownTimer = 0f;
    
    public bool IsAttacking => isAttacking;
    public bool IsInCombat { get; private set; }
    public bool CombatEnabled => combatEnabled;
    
    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        comboSystem = GetComponent<ComboSystem>();
        lockOnSystem = GetComponent<LockOnSystem>();
        dodgeSystem = GetComponent<DodgeSystem>();
        parrySystem = GetComponent<ParrySystem>();
        
        if (attackPoint == null)
        {
            GameObject attackPointObj = new GameObject("AttackPoint");
            attackPointObj.transform.SetParent(transform);
            attackPointObj.transform.localPosition = Vector3.forward;
            attackPoint = attackPointObj.transform;
        }
    }
    
    private void Update()
    {
        if (!combatEnabled) return;
        
        UpdateCooldown();
        CheckCombatState();
    }
    
    private void UpdateCooldown()
    {
        if (!canAttack)
        {
            attackCooldownTimer -= Time.deltaTime;
            if (attackCooldownTimer <= 0f)
            {
                canAttack = true;
            }
        }
    }
    
    private void CheckCombatState()
    {
        Collider[] enemies = Physics.OverlapSphere(transform.position, combatRange, enemyLayer);
        IsInCombat = enemies.Length > 0;
    }
    
    public void OnLightAttack(InputAction.CallbackContext context)
    {
        if (context.performed && CanPerformAttack())
        {
            PerformLightAttack();
        }
    }
    
    public void OnHeavyAttack(InputAction.CallbackContext context)
    {
        if (context.performed && CanPerformAttack())
        {
            PerformHeavyAttack();
        }
    }
    
    private bool CanPerformAttack()
    {
        if (!combatEnabled) return false;
        if (isAttacking) return comboSystem != null && comboSystem.CanQueueNextAttack();
        if (!canAttack) return false;
        if (dodgeSystem != null && dodgeSystem.IsDodging) return false;
        if (parrySystem != null && parrySystem.IsParrying) return false;
        
        return true;
    }
    
    private void PerformLightAttack()
    {
        if (comboSystem != null)
        {
            comboSystem.ExecuteLightAttack();
        }
        else
        {
            StartCoroutine(ExecuteAttack(lightAttackDamage, lightAttackDuration, false));
        }
    }
    
    private void PerformHeavyAttack()
    {
        if (comboSystem != null)
        {
            comboSystem.ExecuteHeavyAttack();
        }
        else
        {
            StartCoroutine(ExecuteAttack(heavyAttackDamage, heavyAttackDuration, true));
        }
    }
    
    public IEnumerator ExecuteAttack(float damage, float duration, bool isHeavy)
    {
        isAttacking = true;
        canAttack = false;
        
        FaceTarget();
        
        yield return new WaitForSeconds(duration * 0.3f);
        
        DealDamage(damage, isHeavy);
        
        yield return new WaitForSeconds(duration * 0.7f);
        
        isAttacking = false;
        attackCooldownTimer = attackCooldown;
        
        Debug.Log($"[Combat] {(isHeavy ? "Heavy" : "Light")} attack executed! Damage: {damage}");
    }
    
    private void DealDamage(float damage, bool isHeavy)
    {
        Vector3 attackPosition = attackPoint.position;
        Vector3 attackDirection = transform.forward;
        
        Collider[] hitEnemies = Physics.OverlapSphere(attackPosition, attackRange, enemyLayer);
        
        foreach (Collider enemyCollider in hitEnemies)
        {
            Vector3 directionToEnemy = (enemyCollider.transform.position - transform.position).normalized;
            float angleToEnemy = Vector3.Angle(attackDirection, directionToEnemy);
            
            if (angleToEnemy <= attackConeAngle / 2f)
            {
                EnemyHealth enemyHealth = enemyCollider.GetComponent<EnemyHealth>();
                if (enemyHealth != null)
                {
                    enemyHealth.TakeDamage(damage, transform.position, isHeavy);
                }
            }
        }
    }
    
    private void FaceTarget()
    {
        if (lockOnSystem != null && lockOnSystem.CurrentTarget != null)
        {
            Vector3 directionToTarget = (lockOnSystem.CurrentTarget.position - transform.position).normalized;
            directionToTarget.y = 0;
            if (directionToTarget != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(directionToTarget);
            }
        }
    }
    
    public void EnableCombat()
    {
        combatEnabled = true;
    }
    
    public void DisableCombat()
    {
        combatEnabled = false;
        isAttacking = false;
    }
    
    private void OnDrawGizmosSelected()
    {
        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
            
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, combatRange);
        }
    }
}
