using UnityEngine;
using System;

public enum AwarenessState
{
    Idle,
    Suspicious,
    Alert
}

public class EnemyAwareness : MonoBehaviour
{
    [Header("Awareness Settings")]
    [SerializeField] private float suspicionDecayRate = 1f;
    [SerializeField] private float suspicionThreshold = 30f;
    [SerializeField] private float alertThreshold = 100f;
    [SerializeField] private float alertCooldownTime = 10f;
    
    [Header("Current State")]
    [SerializeField] private AwarenessState currentState = AwarenessState.Idle;
    [SerializeField] private float awarenessLevel = 0f;
    
    private float alertCooldownTimer;
    
    public AwarenessState CurrentState => currentState;
    public float AwarenessLevel => awarenessLevel;
    
    public event Action<AwarenessState> OnStateChanged;

    private void Update()
    {
        UpdateAwarenessDecay();
        UpdateState();
    }

    public void IncreaseAwareness(float amount)
    {
        awarenessLevel = Mathf.Clamp(awarenessLevel + amount, 0f, alertThreshold);
    }

    private void UpdateAwarenessDecay()
    {
        if (currentState != AwarenessState.Alert)
        {
            awarenessLevel = Mathf.Max(0f, awarenessLevel - suspicionDecayRate * Time.deltaTime);
        }
        else
        {
            alertCooldownTimer -= Time.deltaTime;
            if (alertCooldownTimer <= 0f)
            {
                awarenessLevel = suspicionThreshold - 1f;
            }
        }
    }

    private void UpdateState()
    {
        AwarenessState previousState = currentState;
        
        if (awarenessLevel >= alertThreshold)
        {
            currentState = AwarenessState.Alert;
            alertCooldownTimer = alertCooldownTime;
        }
        else if (awarenessLevel >= suspicionThreshold)
        {
            currentState = AwarenessState.Suspicious;
        }
        else
        {
            currentState = AwarenessState.Idle;
        }
        
        if (previousState != currentState)
        {
            OnStateChanged?.Invoke(currentState);
            Debug.Log($"{gameObject.name} state changed to: {currentState}");
        }
    }

    public void ForceAlert()
    {
        awarenessLevel = alertThreshold;
        alertCooldownTimer = alertCooldownTime;
    }
}
