using UnityEngine;
using System.Collections.Generic;

public class AlertManager : MonoBehaviour
{
    public static AlertManager Instance { get; private set; }
    
    [Header("Alert Settings")]
    [SerializeField] private float globalAlertDecayRate = 5f;
    [SerializeField] private float alertPropagationRadius = 20f;
    
    [Header("Current State")]
    [SerializeField] private float globalAlertLevel;
    
    private List<EnemyAwareness> allEnemies = new List<EnemyAwareness>();
    
    public float GlobalAlertLevel => globalAlertLevel;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        RegisterAllEnemies();
    }

    private void Update()
    {
        UpdateGlobalAlert();
    }

    private void RegisterAllEnemies()
    {
        EnemyAwareness[] enemies = FindObjectsByType<EnemyAwareness>(FindObjectsSortMode.None);
        foreach (var enemy in enemies)
        {
            RegisterEnemy(enemy);
        }
    }

    public void RegisterEnemy(EnemyAwareness enemy)
    {
        if (!allEnemies.Contains(enemy))
        {
            allEnemies.Add(enemy);
            enemy.OnStateChanged += HandleEnemyStateChange;
        }
    }

    public void UnregisterEnemy(EnemyAwareness enemy)
    {
        if (allEnemies.Contains(enemy))
        {
            allEnemies.Remove(enemy);
            enemy.OnStateChanged -= HandleEnemyStateChange;
        }
    }

    private void HandleEnemyStateChange(AwarenessState newState)
    {
        if (newState == AwarenessState.Alert)
        {
            PropagateAlert();
        }
    }

    private void PropagateAlert()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;
        
        foreach (var enemy in allEnemies)
        {
            if (enemy.CurrentState != AwarenessState.Alert)
            {
                float distance = Vector3.Distance(enemy.transform.position, player.transform.position);
                if (distance <= alertPropagationRadius)
                {
                    enemy.IncreaseAwareness(50f);
                }
            }
        }
        
        globalAlertLevel = 100f;
    }

    private void UpdateGlobalAlert()
    {
        int alertEnemies = 0;
        foreach (var enemy in allEnemies)
        {
            if (enemy.CurrentState == AwarenessState.Alert)
            {
                alertEnemies++;
            }
        }
        
        if (alertEnemies == 0)
        {
            globalAlertLevel = Mathf.Max(0f, globalAlertLevel - globalAlertDecayRate * Time.deltaTime);
        }
    }
}
