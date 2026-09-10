using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ComboSystem : MonoBehaviour
{
    [Header("Combo Settings")]
    [SerializeField] private float comboWindowTime = 1.0f;
    [SerializeField] private int maxComboCount = 5;
    [SerializeField] private float comboTimeoutExtension = 0.5f;
    
    [Header("Combo Modifiers")]
    [SerializeField] private float comboDamageMultiplier = 1.15f;
    [SerializeField] private float heavyFinisherMultiplier = 2.0f;
    
    [Header("Launcher Settings")]
    [SerializeField] private int launcherComboIndex = 3;
    [SerializeField] private float launcherForce = 10f;
    
    private PlayerCombat playerCombat;
    private StyleRankSystem styleRank;
    
    private int currentComboCount = 0;
    private float comboTimer = 0f;
    private bool canQueueNext = false;
    private bool queuedAttack = false;
    private bool queuedHeavy = false;
    
    private List<string> comboSequence = new List<string>();
    
    public int CurrentComboCount => currentComboCount;
    public bool IsComboActive => comboTimer > 0f;
    public bool CanQueueNextAttack() => canQueueNext;
    
    private void Awake()
    {
        playerCombat = GetComponent<PlayerCombat>();
        styleRank = GetComponent<StyleRankSystem>();
    }
    
    private void Update()
    {
        if (comboTimer > 0f)
        {
            comboTimer -= Time.deltaTime;
            
            if (comboTimer <= 0f)
            {
                ResetCombo();
            }
        }
    }
    
    public void ExecuteLightAttack()
    {
        if (canQueueNext)
        {
            queuedAttack = true;
            queuedHeavy = false;
            return;
        }
        
        currentComboCount++;
        comboSequence.Add("L");
        RefreshComboTimer();
        
        float damage = CalculateDamage(20f, false);
        float duration = 0.4f;
        
        StartCoroutine(ExecuteComboAttack(damage, duration, false));
    }
    
    public void ExecuteHeavyAttack()
    {
        if (canQueueNext)
        {
            queuedAttack = true;
            queuedHeavy = true;
            return;
        }
        
        currentComboCount++;
        comboSequence.Add("H");
        RefreshComboTimer();
        
        bool isLauncher = currentComboCount == launcherComboIndex;
        bool isFinisher = currentComboCount >= maxComboCount;
        
        float baseDamage = 40f;
        if (isFinisher) baseDamage *= heavyFinisherMultiplier;
        
        float damage = CalculateDamage(baseDamage, true);
        float duration = isFinisher ? 1.2f : 0.8f;
        
        StartCoroutine(ExecuteComboAttack(damage, duration, true, isLauncher, isFinisher));
    }
    
    private IEnumerator ExecuteComboAttack(float damage, float duration, bool isHeavy, bool isLauncher = false, bool isFinisher = false)
    {
        canQueueNext = false;
        
        yield return playerCombat.ExecuteAttack(damage, duration, isHeavy);
        
        if (isLauncher)
        {
            Debug.Log("[Combo] LAUNCHER! Enemy airborne!");
        }
        
        if (isFinisher)
        {
            Debug.Log("[Combo] FINISHER! Combo complete!");
            ResetCombo();
            yield break;
        }
        
        if (currentComboCount < maxComboCount)
        {
            canQueueNext = true;
            yield return new WaitForSeconds(0.3f);
            canQueueNext = false;
            
            if (queuedAttack)
            {
                queuedAttack = false;
                if (queuedHeavy)
                {
                    ExecuteHeavyAttack();
                }
                else
                {
                    ExecuteLightAttack();
                }
            }
        }
        else
        {
            ResetCombo();
        }
    }
    
    private float CalculateDamage(float baseDamage, bool isHeavy)
    {
        float damage = baseDamage;
        
        if (currentComboCount > 1)
        {
            float comboBonus = Mathf.Pow(comboDamageMultiplier, currentComboCount - 1);
            damage *= comboBonus;
        }
        
        if (styleRank != null)
        {
            styleRank.AddComboHit(isHeavy);
        }
        
        return damage;
    }
    
    private void RefreshComboTimer()
    {
        comboTimer = comboWindowTime + (currentComboCount * comboTimeoutExtension);
    }
    
    public void ResetCombo()
    {
        if (currentComboCount > 0)
        {
            Debug.Log($"[Combo] Combo ended at {currentComboCount} hits. Sequence: {string.Join("-", comboSequence)}");
        }
        
        currentComboCount = 0;
        comboTimer = 0f;
        canQueueNext = false;
        queuedAttack = false;
        comboSequence.Clear();
    }
    
    public void ExtendCombo(float extraTime)
    {
        comboTimer += extraTime;
    }
}
