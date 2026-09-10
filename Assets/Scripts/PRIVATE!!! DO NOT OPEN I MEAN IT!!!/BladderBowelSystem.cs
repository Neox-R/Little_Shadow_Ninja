using UnityEngine;
using UnityEngine.InputSystem;

public class BladderBowelSystem : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private DiaperState diaperState;
    
    [Header("Capacity Settings")]
    [SerializeField] private float maxBladderCapacity = 100f;
    [SerializeField] private float maxBowelCapacity = 100f;
    
    [Header("Fill Rates (per second)")]
    [SerializeField] private float bladderFillRate = 1f;
    [SerializeField] private float bowelFillRate = 0.5f;
    
    [Header("Release Amounts")]
    [SerializeField] private float bladderReleaseAmount = 25f;
    [SerializeField] private float bowelReleaseAmount = 30f;
    
    [Header("Current Levels")]
    [SerializeField] private float currentBladderLevel = 0f;
    [SerializeField] private float currentBowelLevel = 0f;
    
    [Header("Accident Settings")]
    [SerializeField] private float accidentDuration = 3f;
    [SerializeField] private bool freezePlayerDuringAccident = true;
    
    public float CurrentBladderLevel => currentBladderLevel;
    public float CurrentBowelLevel => currentBowelLevel;
    public float BladderPercentage => (currentBladderLevel / maxBladderCapacity) * 100f;
    public float BowelPercentage => (currentBowelLevel / maxBowelCapacity) * 100f;
    
    public bool IsBladderFull => currentBladderLevel >= maxBladderCapacity;
    public bool IsBowelFull => currentBowelLevel >= maxBowelCapacity;
    public bool IsHavingAccident => isHavingAccident;
    
    private float automaticReleaseTimer = 0f;
    private const float AUTOMATIC_RELEASE_DELAY = 2f;
    private bool isHavingAccident = false;
    private float accidentTimer = 0f;
    
    private void Awake()
    {
        if (diaperState == null)
        {
            diaperState = GetComponent<DiaperState>();
        }
    }
    
    private void Update()
    {
        if (!isHavingAccident)
        {
            FillOverTime();
            CheckAutomaticRelease();
        }
        else
        {
            HandleAccident();
        }
    }
    
    private void FillOverTime()
    {
        currentBladderLevel = Mathf.Clamp(currentBladderLevel + bladderFillRate * Time.deltaTime, 0f, maxBladderCapacity);
        currentBowelLevel = Mathf.Clamp(currentBowelLevel + bowelFillRate * Time.deltaTime, 0f, maxBowelCapacity);
    }
    
    private void CheckAutomaticRelease()
    {
        if (IsBladderFull || IsBowelFull)
        {
            automaticReleaseTimer += Time.deltaTime;
            
            if (automaticReleaseTimer >= AUTOMATIC_RELEASE_DELAY)
            {
                StartAccident();
                automaticReleaseTimer = 0f;
            }
        }
        else
        {
            automaticReleaseTimer = 0f;
        }
    }
    
    private void StartAccident()
    {
        isHavingAccident = true;
        accidentTimer = 0f;
        
        Debug.Log("[BladderBowel] ⚠️ ACCIDENT! Player can't hold it anymore and stops moving!");
    }
    
    private void HandleAccident()
    {
        accidentTimer += Time.deltaTime;
        
        float releaseRate = Time.deltaTime / accidentDuration;
        
        if (IsBladderFull)
        {
            float amountToRelease = currentBladderLevel * releaseRate;
            currentBladderLevel -= amountToRelease;
            diaperState.AddWetness(amountToRelease);
        }
        
        if (IsBowelFull)
        {
            float amountToRelease = currentBowelLevel * releaseRate;
            currentBowelLevel -= amountToRelease;
            diaperState.AddMess(amountToRelease);
        }
        
        if (accidentTimer >= accidentDuration)
        {
            currentBladderLevel = Mathf.Max(0f, currentBladderLevel);
            currentBowelLevel = Mathf.Max(0f, currentBowelLevel);
            
            isHavingAccident = false;
            Debug.Log("[BladderBowel] Accident finished. Player can move again.");
        }
    }
    
    public void OnPottyInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            UseDiaper();
        }
    }
    
    public void UseDiaper()
    {
        if (isHavingAccident)
        {
            Debug.Log("[BladderBowel] Can't use diaper voluntarily during an accident!");
            return;
        }
        
        bool didSomething = false;
        
        if (currentBladderLevel > 0f)
        {
            ReleaseBladder(forced: false);
            didSomething = true;
        }
        
        if (currentBowelLevel > 0f)
        {
            ReleaseBowel(forced: false);
            didSomething = true;
        }
        
        if (!didSomething)
        {
            Debug.Log("[BladderBowel] Nothing to release!");
        }
    }
    
    private void ReleaseBladder(bool forced)
    {
        float amountToRelease = forced ? currentBladderLevel : Mathf.Min(bladderReleaseAmount, currentBladderLevel);
        
        currentBladderLevel -= amountToRelease;
        currentBladderLevel = Mathf.Max(0f, currentBladderLevel);
        
        diaperState.AddWetness(amountToRelease);
        
        if (forced)
        {
            Debug.Log($"[BladderBowel] Bladder automatically released! ({amountToRelease:F1} wetness added)");
        }
        else
        {
            Debug.Log($"[BladderBowel] Bladder voluntarily released ({amountToRelease:F1} wetness added)");
        }
    }
    
    private void ReleaseBowel(bool forced)
    {
        float amountToRelease = forced ? currentBowelLevel : Mathf.Min(bowelReleaseAmount, currentBowelLevel);
        
        currentBowelLevel -= amountToRelease;
        currentBowelLevel = Mathf.Max(0f, currentBowelLevel);
        
        diaperState.AddMess(amountToRelease);
        
        if (forced)
        {
            Debug.Log($"[BladderBowel] Bowel automatically released! ({amountToRelease:F1} mess added)");
        }
        else
        {
            Debug.Log($"[BladderBowel] Bowel voluntarily released ({amountToRelease:F1} mess added)");
        }
    }
    
    public void AddBladderContent(float amount)
    {
        currentBladderLevel = Mathf.Clamp(currentBladderLevel + amount, 0f, maxBladderCapacity);
        Debug.Log($"[BladderBowel] Added {amount:F1} to bladder (now at {BladderPercentage:F0}%)");
    }
    
    public void AddBowelContent(float amount)
    {
        currentBowelLevel = Mathf.Clamp(currentBowelLevel + amount, 0f, maxBowelCapacity);
        Debug.Log($"[BladderBowel] Added {amount:F1} to bowel (now at {BowelPercentage:F0}%)");
    }
}
