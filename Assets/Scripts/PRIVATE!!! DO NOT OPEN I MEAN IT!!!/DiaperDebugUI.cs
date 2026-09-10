using UnityEngine;

public class DiaperDebugUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private DiaperState diaperState;
    [SerializeField] private BladderBowelSystem bladderBowel;
    
    [Header("UI Settings")]
    [SerializeField] private bool showDebugUI = true;
    [SerializeField] private int fontSize = 14;
    
    private GUIStyle textStyle;
    
    private void Start()
    {
        if (diaperState == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                diaperState = player.GetComponent<DiaperState>();
                bladderBowel = player.GetComponent<BladderBowelSystem>();
            }
        }
    }
    
    private void OnGUI()
    {
        if (!showDebugUI || diaperState == null || bladderBowel == null) return;
        
        if (textStyle == null)
        {
            textStyle = new GUIStyle(GUI.skin.label);
            textStyle.fontSize = fontSize;
            textStyle.normal.textColor = Color.white;
        }
        
        int yPos = 120;
        int lineHeight = fontSize + 5;
        
        if (bladderBowel.IsHavingAccident)
        {
            GUIStyle warningStyle = new GUIStyle(textStyle);
            warningStyle.fontSize = fontSize + 4;
            warningStyle.normal.textColor = Color.red;
            GUI.Label(new Rect(10, yPos, 400, lineHeight * 2), 
                "⚠️ HAVING ACCIDENT! PLAYER FROZEN! ⚠️", 
                warningStyle);
            yPos += lineHeight * 2;
        }
        
        GUI.Label(new Rect(10, yPos, 400, lineHeight), "=== DIAPER SYSTEM DEBUG ===", textStyle);
        yPos += lineHeight + 5;
        
        GUIStyle bladderStyle = new GUIStyle(textStyle);
        bladderStyle.normal.textColor = GetUrgencyColor(bladderBowel.BladderPercentage);
        GUI.Label(new Rect(10, yPos, 400, lineHeight), 
            $"Bladder: {bladderBowel.BladderPercentage:F0}% {(bladderBowel.IsBladderFull ? "[FULL!]" : "")}", 
            bladderStyle);
        yPos += lineHeight;
        
        GUIStyle bowelStyle = new GUIStyle(textStyle);
        bowelStyle.normal.textColor = GetUrgencyColor(bladderBowel.BowelPercentage);
        GUI.Label(new Rect(10, yPos, 400, lineHeight), 
            $"Bowel: {bladderBowel.BowelPercentage:F0}% {(bladderBowel.IsBowelFull ? "[FULL!]" : "")}", 
            bowelStyle);
        yPos += lineHeight + 5;
        
        GUI.Label(new Rect(10, yPos, 400, lineHeight), 
            $"Diaper Wetness: {diaperState.WetnessPercentage:F0}% {(diaperState.IsWetLeaking ? "[LEAKING]" : "")}", 
            textStyle);
        yPos += lineHeight;
        
        GUI.Label(new Rect(10, yPos, 400, lineHeight), 
            $"Diaper Mess: {diaperState.MessPercentage:F0}% {(diaperState.IsMessLeaking ? "[LEAKING]" : "")}", 
            textStyle);
        yPos += lineHeight;
        
        GUI.Label(new Rect(10, yPos, 400, lineHeight), 
            $"Condition: {diaperState.CurrentCondition}", 
            textStyle);
        yPos += lineHeight;
        
        GUI.Label(new Rect(10, yPos, 400, lineHeight), 
            $"Smell Intensity: {diaperState.GetSmellIntensity():F2}", 
            textStyle);
        yPos += lineHeight;
        
        GUI.Label(new Rect(10, yPos, 400, lineHeight), 
            $"Speed Penalty: {diaperState.GetMovementSpeedMultiplier() * 100f:F0}%", 
            textStyle);
        yPos += lineHeight + 5;
        
        GUI.Label(new Rect(10, yPos, 400, lineHeight), 
            "[P] - Use Diaper | [C] - Change Diaper (Not Implemented)", 
            textStyle);
    }
    
    private Color GetUrgencyColor(float percentage)
    {
        if (percentage >= 90f)
            return Color.red;
        else if (percentage >= 60f)
            return Color.yellow;
        else
            return Color.green;
    }
}
