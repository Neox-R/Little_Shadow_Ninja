using UnityEngine;
using System;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;
    
    [Header("Stun Settings")]
    [SerializeField] private float stunResistance = 1f;
    
    [Header("Hitstun")]
    [SerializeField] private float lightHitstunDuration = 0.3f;
    [SerializeField] private float heavyHitstunDuration = 0.6f;
    
    [Header("Knockback")]
    [SerializeField] private float knockbackForce = 5f;
    [SerializeField] private bool canBeLaunched = true;
    
    private EnemyPatrol enemyPatrol;
    private Rigidbody rb;
    
    private bool isDead = false;
    private bool isStunned = false;
    private bool isInHitstun = false;
    private float stunTimer = 0f;
    private float hitstunTimer = 0f;
    
    public float MaxHealth => maxHealth;
    public float CurrentHealth => currentHealth;
    public float HealthPercentage => (currentHealth / maxHealth) * 100f;
    public bool IsDead => isDead;
    public bool IsStunned => isStunned;
    public bool IsInHitstun => isInHitstun;
    
    public event Action<float, float> OnHealthChanged;
    public event Action<Transform> OnDeath;
    public event Action OnStunned;
    public event Action OnStunEnd;
    
    private void Awake()
    {
        currentHealth = maxHealth;
        enemyPatrol = GetComponent<EnemyPatrol>();
        rb = GetComponent<Rigidbody>();
        
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true;
        }
    }
    
    private void Update()
    {
        if (isStunned)
        {
            stunTimer -= Time.deltaTime;
            if (stunTimer <= 0f)
            {
                EndStun();
            }
        }
        
        if (isInHitstun)
        {
            hitstunTimer -= Time.deltaTime;
            if (hitstunTimer <= 0f)
            {
                isInHitstun = false;
            }
        }
    }
    
    public void TakeDamage(float damage, Vector3 attackerPosition, bool isHeavyAttack = false)
    {
        if (isDead) return;
        
        currentHealth -= damage;
        currentHealth = Mathf.Max(0f, currentHealth);
        
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        
        ApplyHitstun(isHeavyAttack);
        ApplyKnockback(attackerPosition, isHeavyAttack);
        
        Debug.Log($"[EnemyHealth] {gameObject.name} took {damage:F1} damage! Health: {currentHealth:F1}/{maxHealth:F1}");
        
        if (currentHealth <= 0f && !isDead)
        {
            Die(attackerPosition);
        }
    }
    
    private void ApplyHitstun(bool isHeavy)
    {
        isInHitstun = true;
        hitstunTimer = isHeavy ? heavyHitstunDuration : lightHitstunDuration;
        
        if (enemyPatrol != null)
        {
            enemyPatrol.enabled = false;
        }
    }
    
    private void ApplyKnockback(Vector3 attackerPosition, bool isHeavy)
    {
        Vector3 knockbackDirection = (transform.position - attackerPosition).normalized;
        knockbackDirection.y = 0;
        
        float force = knockbackForce * (isHeavy ? 2f : 1f);
        
        if (rb != null && !rb.isKinematic)
        {
            rb.AddForce(knockbackDirection * force, ForceMode.Impulse);
        }
    }
    
    public void Stun(float duration)
    {
        if (isDead) return;
        
        isStunned = true;
        stunTimer = duration / stunResistance;
        
        if (enemyPatrol != null)
        {
            enemyPatrol.enabled = false;
        }
        
        OnStunned?.Invoke();
        Debug.Log($"[EnemyHealth] {gameObject.name} stunned for {stunTimer:F1}s!");
    }
    
    private void EndStun()
    {
        isStunned = false;
        
        if (enemyPatrol != null && !isDead)
        {
            enemyPatrol.enabled = true;
        }
        
        OnStunEnd?.Invoke();
        Debug.Log($"[EnemyHealth] {gameObject.name} stun ended");
    }
    
    public void Launch(Vector3 direction, float force)
    {
        if (!canBeLaunched || isDead) return;
        
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.AddForce((direction + Vector3.up) * force, ForceMode.Impulse);
            Debug.Log($"[EnemyHealth] {gameObject.name} LAUNCHED!");
        }
    }
    
    private void Die(Vector3 attackerPosition)
    {
        isDead = true;
        
        if (enemyPatrol != null)
        {
            enemyPatrol.enabled = false;
        }
        
        OnDeath?.Invoke(transform);
        
        Debug.Log($"[EnemyHealth] {gameObject.name} defeated!");
        
        Destroy(gameObject, 3f);
    }
    
    public void SetMaxHealth(float newMaxHealth, bool healToFull = false)
    {
        maxHealth = newMaxHealth;
        
        if (healToFull)
        {
            currentHealth = maxHealth;
        }
        else
        {
            currentHealth = Mathf.Min(currentHealth, maxHealth);
        }
        
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }
}
