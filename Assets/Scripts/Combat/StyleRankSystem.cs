using UnityEngine;
using System;
using System.Collections.Generic;

public class StyleRankSystem : MonoBehaviour
{
    [Header("Style Ranks")]
    [SerializeField] private List<string> ranks = new List<string> { "D", "C", "B", "A", "S", "SS", "SSS" };
    
    [Header("Style Points")]
    [SerializeField] private float currentStylePoints = 0f;
    [SerializeField] private float maxStylePoints = 5000f;
    [SerializeField] private float styleDecayRate = 50f;
    [SerializeField] private float styleDecayDelay = 2f;
    
    [Header("Point Values")]
    [SerializeField] private float lightAttackPoints = 10f;
    [SerializeField] private float heavyAttackPoints = 20f;
    [SerializeField] private float comboMultiplier = 1.5f;
    [SerializeField] private float dodgePoints = 30f;
    [SerializeField] private float parryPoints = 50f;
    [SerializeField] private float perfectDodgePoints = 100f;
    [SerializeField] private float killPoints = 200f;
    
    [Header("Variety Bonus")]
    [SerializeField] private float varietyBonusMultiplier = 1.5f;
    [SerializeField] private int varietyMoveThreshold = 3;
    
    private ComboSystem comboSystem;
    private DodgeSystem dodgeSystem;
    private ParrySystem parrySystem;
    
    private int currentRankIndex = 0;
    private float styleDecayTimer = 0f;
    private Queue<string> recentMoves = new Queue<string>();
    private int maxRecentMoves = 5;
    
    public string CurrentRank => ranks[Mathf.Clamp(currentRankIndex, 0, ranks.Count - 1)];
    public float StylePoints => currentStylePoints;
    public float StylePercentage => (currentStylePoints / maxStylePoints) * 100f;
    
    public event Action<string> OnRankChanged;
    public event Action<float> OnStylePointsChanged;
    
    private void Awake()
    {
        comboSystem = GetComponent<ComboSystem>();
        dodgeSystem = GetComponent<DodgeSystem>();
        parrySystem = GetComponent<ParrySystem>();
    }
    
    private void Update()
    {
        UpdateStyleDecay();
    }
    
    private void UpdateStyleDecay()
    {
        if (currentStylePoints > 0f)
        {
            styleDecayTimer += Time.deltaTime;
            
            if (styleDecayTimer >= styleDecayDelay)
            {
                currentStylePoints -= styleDecayRate * Time.deltaTime;
                currentStylePoints = Mathf.Max(0f, currentStylePoints);
                
                UpdateRank();
                OnStylePointsChanged?.Invoke(currentStylePoints);
            }
        }
    }
    
    public void AddComboHit(bool isHeavy)
    {
        float points = isHeavy ? heavyAttackPoints : lightAttackPoints;
        
        if (comboSystem != null && comboSystem.CurrentComboCount > 1)
        {
            points *= Mathf.Pow(comboMultiplier, comboSystem.CurrentComboCount - 1);
        }
        
        AddMove(isHeavy ? "Heavy" : "Light");
        AddStylePoints(points);
    }
    
    public void AddDodgePoints(bool isPerfect = false)
    {
        float points = isPerfect ? perfectDodgePoints : dodgePoints;
        AddMove(isPerfect ? "PerfectDodge" : "Dodge");
        AddStylePoints(points);
    }
    
    public void AddParryPoints()
    {
        AddMove("Parry");
        AddStylePoints(parryPoints);
    }
    
    public void AddKillPoints()
    {
        AddMove("Kill");
        AddStylePoints(killPoints);
    }
    
    private void AddMove(string moveName)
    {
        recentMoves.Enqueue(moveName);
        
        if (recentMoves.Count > maxRecentMoves)
        {
            recentMoves.Dequeue();
        }
    }
    
    private void AddStylePoints(float basePoints)
    {
        float points = basePoints;
        
        if (HasVariety())
        {
            points *= varietyBonusMultiplier;
            Debug.Log("[Style] VARIETY BONUS!");
        }
        
        currentStylePoints += points;
        currentStylePoints = Mathf.Min(currentStylePoints, maxStylePoints);
        
        styleDecayTimer = 0f;
        
        UpdateRank();
        OnStylePointsChanged?.Invoke(currentStylePoints);
        
        Debug.Log($"[Style] +{points:F0} points! Rank: {CurrentRank} ({currentStylePoints:F0}/{maxStylePoints:F0})");
    }
    
    private bool HasVariety()
    {
        if (recentMoves.Count < varietyMoveThreshold) return false;
        
        HashSet<string> uniqueMoves = new HashSet<string>(recentMoves);
        return uniqueMoves.Count >= varietyMoveThreshold;
    }
    
    private void UpdateRank()
    {
        float pointsPerRank = maxStylePoints / ranks.Count;
        int newRankIndex = Mathf.FloorToInt(currentStylePoints / pointsPerRank);
        newRankIndex = Mathf.Clamp(newRankIndex, 0, ranks.Count - 1);
        
        if (newRankIndex != currentRankIndex)
        {
            currentRankIndex = newRankIndex;
            OnRankChanged?.Invoke(CurrentRank);
            Debug.Log($"[Style] ★ RANK UP: {CurrentRank} ★");
        }
    }
    
    public void ResetStyle()
    {
        currentStylePoints = 0f;
        currentRankIndex = 0;
        recentMoves.Clear();
        styleDecayTimer = 0f;
        
        OnRankChanged?.Invoke(CurrentRank);
        OnStylePointsChanged?.Invoke(currentStylePoints);
    }
}
