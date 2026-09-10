using UnityEngine;

public class CombatDebugUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerCombat playerCombat;
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private ComboSystem comboSystem;
    [SerializeField] private LockOnSystem lockOnSystem;
    [SerializeField] private StyleRankSystem styleRank;
    
    [Header("UI Settings")]
    [SerializeField] private bool showDebugUI = true;
    [SerializeField] private int fontSize = 14;
    
    private GUIStyle textStyle;
    private GUIStyle rankStyle;
    
    private void Start()
    {
        if (playerCombat == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerCombat = player.GetComponent<PlayerCombat>();
                playerHealth = player.GetComponent<PlayerHealth>();
                comboSystem = player.GetComponent<ComboSystem>();
                lockOnSystem = player.GetComponent<LockOnSystem>();
                styleRank = player.GetComponent<StyleRankSystem>();
            }
        }
    }
    
    private void OnGUI()
    {
        if (!showDebugUI) return;
        if (playerCombat == null) return;
        
        if (textStyle == null)
        {
            textStyle = new GUIStyle(GUI.skin.label);
            textStyle.fontSize = fontSize;
            textStyle.normal.textColor = Color.white;
            
            rankStyle = new GUIStyle(textStyle);
            rankStyle.fontSize = fontSize + 8;
            rankStyle.fontStyle = FontStyle.Bold;
        }
        
        int yPos = 10;
        int lineHeight = fontSize + 5;
        int xPos = Screen.width - 310;
        
        GUI.Label(new Rect(xPos, yPos, 300, lineHeight), "=== COMBAT DEBUG ===", textStyle);
        yPos += lineHeight + 5;
        
        if (playerHealth != null)
        {
            GUIStyle healthStyle = new GUIStyle(textStyle);
            healthStyle.normal.textColor = GetHealthColor(playerHealth.HealthPercentage);
            GUI.Label(new Rect(xPos, yPos, 300, lineHeight), 
                $"Health: {playerHealth.CurrentHealth:F0}/{playerHealth.MaxHealth:F0}", 
                healthStyle);
            yPos += lineHeight;
        }
        
        if (styleRank != null)
        {
            rankStyle.normal.textColor = GetRankColor(styleRank.CurrentRank);
            GUI.Label(new Rect(xPos, yPos, 300, lineHeight), 
                $"Style Rank: {styleRank.CurrentRank}", 
                rankStyle);
            yPos += lineHeight;
            
            GUI.Label(new Rect(xPos, yPos, 300, lineHeight), 
                $"Style Points: {styleRank.StylePoints:F0}", 
                textStyle);
            yPos += lineHeight + 5;
        }
        
        if (comboSystem != null && comboSystem.IsComboActive)
        {
            GUIStyle comboStyle = new GUIStyle(textStyle);
            comboStyle.normal.textColor = Color.yellow;
            comboStyle.fontStyle = FontStyle.Bold;
            GUI.Label(new Rect(xPos, yPos, 300, lineHeight), 
                $"COMBO: {comboSystem.CurrentComboCount} HITS!", 
                comboStyle);
            yPos += lineHeight;
        }
        
        if (lockOnSystem != null && lockOnSystem.IsLockedOn && lockOnSystem.CurrentTarget != null)
        {
            GUIStyle lockStyle = new GUIStyle(textStyle);
            lockStyle.normal.textColor = Color.cyan;
            GUI.Label(new Rect(xPos, yPos, 300, lineHeight), 
                $"🎯 Locked: {lockOnSystem.CurrentTarget.name}", 
                lockStyle);
            yPos += lineHeight;
        }
        
        GUI.Label(new Rect(xPos, yPos, 300, lineHeight), 
            $"In Combat: {(playerCombat.IsInCombat ? "YES" : "NO")}", 
            textStyle);
        yPos += lineHeight;
        
        yPos += 5;
        GUI.Label(new Rect(xPos, yPos, 300, lineHeight), 
            "Controls:", 
            textStyle);
        yPos += lineHeight;
        GUI.Label(new Rect(xPos, yPos, 300, lineHeight), 
            "Mouse L - Light Attack", 
            textStyle);
        yPos += lineHeight;
        GUI.Label(new Rect(xPos, yPos, 300, lineHeight), 
            "Mouse R - Heavy Attack", 
            textStyle);
        yPos += lineHeight;
        GUI.Label(new Rect(xPos, yPos, 300, lineHeight), 
            "Space - Dodge", 
            textStyle);
        yPos += lineHeight;
        GUI.Label(new Rect(xPos, yPos, 300, lineHeight), 
            "Q - Parry", 
            textStyle);
        yPos += lineHeight;
        GUI.Label(new Rect(xPos, yPos, 300, lineHeight), 
            "Tab - Lock-On", 
            textStyle);
    }
    
    private Color GetHealthColor(float percentage)
    {
        if (percentage >= 60f)
            return Color.green;
        else if (percentage >= 30f)
            return Color.yellow;
        else
            return Color.red;
    }
    
    private Color GetRankColor(string rank)
    {
        return rank switch
        {
            "D" => Color.gray,
            "C" => Color.white,
            "B" => Color.cyan,
            "A" => Color.green,
            "S" => Color.yellow,
            "SS" => new Color(1f, 0.5f, 0f),
            "SSS" => Color.red,
            _ => Color.white
        };
    }
}
