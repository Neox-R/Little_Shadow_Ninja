using UnityEngine;
using System;

public class DiaperState : MonoBehaviour
{
    [Header("Diaper Capacity Settings")]
    [SerializeField] private float maxWetnessCapacity = 100f;
    [SerializeField] private float maxMessCapacity = 100f;
    
    [Header("Leak Thresholds")]
    [SerializeField] private float wetLeakThreshold = 90f;
    [SerializeField] private float messLeakThreshold = 90f;
    
    [Header("Current State")]
    [SerializeField] private float currentWetness = 0f;
    [SerializeField] private float currentMess = 0f;
    
    public float CurrentWetness => currentWetness;
    public float CurrentMess => currentMess;
    public float WetnessPercentage => (currentWetness / maxWetnessCapacity) * 100f;
    public float MessPercentage => (currentMess / maxMessCapacity) * 100f;
    
    public bool IsWetLeaking => currentWetness >= wetLeakThreshold;
    public bool IsMessLeaking => currentMess >= messLeakThreshold;
    public bool IsLeaking => IsWetLeaking || IsMessLeaking;
    
    public bool IsWetFull => currentWetness >= maxWetnessCapacity;
    public bool IsMessFull => currentMess >= maxMessCapacity;
    public bool IsCompletelyFull => IsWetFull || IsMessFull;
    
    public DiaperCondition CurrentCondition => GetDiaperCondition();
    
    public event Action<float> OnWetnessChanged;
    public event Action<float> OnMessChanged;
    public event Action<DiaperCondition> OnConditionChanged;
    public event Action OnStartedLeaking;
    
    private DiaperCondition previousCondition;
    private bool wasLeaking = false;
    
    public enum DiaperCondition
    {
        Clean,
        SlightlyWet,
        Wet,
        Soaked,
        SlightlyMessy,
        Messy,
        VeryMessy,
        WetAndMessy,
        CompletelyFull
    }
    
    private void Start()
    {
        previousCondition = CurrentCondition;
    }
    
    public void AddWetness(float amount)
    {
        if (amount <= 0f) return;
        
        float oldWetness = currentWetness;
        currentWetness = Mathf.Clamp(currentWetness + amount, 0f, maxWetnessCapacity);
        
        if (currentWetness != oldWetness)
        {
            OnWetnessChanged?.Invoke(currentWetness);
            CheckConditionChange();
            CheckLeakingStatus();
        }
    }
    
    public void AddMess(float amount)
    {
        if (amount <= 0f) return;
        
        float oldMess = currentMess;
        currentMess = Mathf.Clamp(currentMess + amount, 0f, maxMessCapacity);
        
        if (currentMess != oldMess)
        {
            OnMessChanged?.Invoke(currentMess);
            CheckConditionChange();
            CheckLeakingStatus();
        }
    }
    
    public void ChangeDiaper()
    {
        currentWetness = 0f;
        currentMess = 0f;
        wasLeaking = false;
        
        OnWetnessChanged?.Invoke(currentWetness);
        OnMessChanged?.Invoke(currentMess);
        CheckConditionChange();
    }
    
    public float GetMovementSpeedMultiplier()
    {
        bool isOnlyWet = currentWetness > 0f && currentMess == 0f;
        bool isOnlyMessy = currentMess > 0f && currentWetness == 0f;
        
        if (IsCompletelyFull)
        {
            return 0.3f;
        }
        
        if (isOnlyWet && WetnessPercentage >= 50f)
        {
            return 0.5f;
        }
        
        if (isOnlyMessy && MessPercentage >= 50f)
        {
            return 0.5f;
        }
        
        if (currentWetness > 0f || currentMess > 0f)
        {
            float wetFactor = currentWetness / maxWetnessCapacity;
            float messFactor = currentMess / maxMessCapacity;
            float maxFactor = Mathf.Max(wetFactor, messFactor);
            
            return Mathf.Lerp(1.0f, 0.5f, maxFactor);
        }
        
        return 1.0f;
    }
    
    public float GetSmellIntensity()
    {
        if (!IsLeaking && currentMess < messLeakThreshold * 0.7f)
        {
            return 0f;
        }
        
        float messSmell = (currentMess / maxMessCapacity) * 2.0f;
        float wetSmell = (currentWetness / maxWetnessCapacity) * 0.5f;
        
        float totalSmell = messSmell + wetSmell;
        
        if (IsLeaking)
        {
            totalSmell *= 2.0f;
        }
        
        return Mathf.Clamp(totalSmell, 0f, 5f);
    }
    
    private DiaperCondition GetDiaperCondition()
    {
        float wetPercent = WetnessPercentage;
        float messPercent = MessPercentage;
        
        if (IsCompletelyFull)
        {
            return DiaperCondition.CompletelyFull;
        }
        
        if (wetPercent > 30f && messPercent > 30f)
        {
            return DiaperCondition.WetAndMessy;
        }
        
        if (messPercent >= 70f)
        {
            return DiaperCondition.VeryMessy;
        }
        else if (messPercent >= 40f)
        {
            return DiaperCondition.Messy;
        }
        else if (messPercent >= 10f)
        {
            return DiaperCondition.SlightlyMessy;
        }
        
        if (wetPercent >= 70f)
        {
            return DiaperCondition.Soaked;
        }
        else if (wetPercent >= 40f)
        {
            return DiaperCondition.Wet;
        }
        else if (wetPercent >= 10f)
        {
            return DiaperCondition.SlightlyWet;
        }
        
        return DiaperCondition.Clean;
    }
    
    private void CheckConditionChange()
    {
        DiaperCondition newCondition = CurrentCondition;
        if (newCondition != previousCondition)
        {
            OnConditionChanged?.Invoke(newCondition);
            previousCondition = newCondition;
        }
    }
    
    private void CheckLeakingStatus()
    {
        if (IsLeaking && !wasLeaking)
        {
            OnStartedLeaking?.Invoke();
            wasLeaking = true;
        }
        else if (!IsLeaking)
        {
            wasLeaking = false;
        }
    }
}
