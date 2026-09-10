using UnityEngine;
using System;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;
    
    [Header("Regeneration")]
    [SerializeField] private bool enableRegen = true;
    [SerializeField] private float regenDelay = 5f;
    [SerializeField] private float regenRate = 5f;
    
    [Header("Damage Reduction")]
    [SerializeField] private float damageReduction = 0f;
    
    private DodgeSystem dodgeSystem;
    private ParrySystem parrySystem;
    
    private float regenTimer = 0f;
    private bool isDead = false;
    
    public float MaxHealth => maxHealth;
    public float CurrentHealth => currentHealth;
    public float HealthPercentage => (currentHealth / maxHealth) * 100f;
    public bool IsDead => isDead;
    
    public event Action<float, float> OnHealthChanged;
    public event Action OnDeath;
    public event Action OnRevive;
    
    private void Awake()
    {
        dodgeSystem = GetComponent<DodgeSystem>();
        parrySystem = GetComponent<ParrySystem>();
        
        currentHealth = maxHealth;
    }
    
    private void Update()
    {
        if (isDead) return;
        
        if (enableRegen && currentHealth < maxHealth)
        {
            regenTimer += Time.deltaTime;
            
            if (regenTimer >= regenDelay)
            {
                Heal(regenRate * Time.deltaTime);
            }
        }
    }
    
    public void TakeDamage(float damage, Transform attacker = null)
    {
        if (isDead) return;
        
        if (dodgeSystem != null && dodgeSystem.IsInvincible)
        {
            Debug.Log("[PlayerHealth] Damage negated by invincibility frames!");
            return;
        }
        
        if (parrySystem != null && parrySystem.TryParryAttack(attacker, damage))
        {
            Debug.Log("[PlayerHealth] Damage parried!");
            return;
        }
        
        float finalDamage = damage * (1f - damageReduction);
        currentHealth -= finalDamage;
        currentHealth = Mathf.Max(0f, currentHealth);
        
        regenTimer = 0f;
        
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        
        Debug.Log($"[PlayerHealth] Took {finalDamage:F1} damage! Health: {currentHealth:F1}/{maxHealth:F1}");
        
        if (currentHealth <= 0f && !isDead)
        {
            Die();
        }
    }
    
    public void Heal(float amount)
    {
        if (isDead) return;
        
        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
        
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
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
    
    private void Die()
    {
        isDead = true;
        Debug.Log("[PlayerHealth] Player died!");
        OnDeath?.Invoke();
    }
    
    public void Revive(float healthAmount)
    {
        isDead = false;
        currentHealth = healthAmount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
        
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        OnRevive?.Invoke();
        
        Debug.Log($"[PlayerHealth] Player revived with {currentHealth:F1} health!");
    }
}
