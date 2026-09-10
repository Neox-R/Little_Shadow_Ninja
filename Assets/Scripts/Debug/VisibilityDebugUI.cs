using UnityEngine;

public class VisibilityDebugUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerVisibility playerVisibility;
    [SerializeField] private AlertManager alertManager;
    
    [Header("UI Settings")]
    [SerializeField] private bool showDebugUI = true;
    [SerializeField] private int fontSize = 20;
    
    private GUIStyle textStyle;

    private void Awake()
    {
        if (playerVisibility == null)
        {
            playerVisibility = FindFirstObjectByType<PlayerVisibility>();
        }
        
        if (alertManager == null)
        {
            alertManager = FindFirstObjectByType<AlertManager>();
        }
    }

    private void OnGUI()
    {
        if (!showDebugUI) return;
        
        if (textStyle == null)
        {
            textStyle = new GUIStyle(GUI.skin.label);
            textStyle.fontSize = fontSize;
            textStyle.normal.textColor = Color.white;
            textStyle.alignment = TextAnchor.UpperLeft;
        }
        
        float yPos = 10f;
        float lineHeight = fontSize + 5f;
        
        GUI.Label(new Rect(10, yPos, 400, lineHeight), "=== STEALTH SYSTEM DEBUG ===", textStyle);
        yPos += lineHeight * 1.5f;
        
        if (playerVisibility != null)
        {
            Color visColor = GetVisibilityColor(playerVisibility.CurrentVisibility);
            GUIStyle visStyle = new GUIStyle(textStyle);
            visStyle.normal.textColor = visColor;
            
            GUI.Label(new Rect(10, yPos, 400, lineHeight), $"Visibility: {playerVisibility.CurrentVisibility:F2}", visStyle);
            yPos += lineHeight;
            
            GUI.Label(new Rect(10, yPos, 400, lineHeight), $"In Shadow: {playerVisibility.IsInShadow}", textStyle);
            yPos += lineHeight;
        }
        
        yPos += lineHeight * 0.5f;
        
        if (alertManager != null)
        {
            Color alertColor = GetAlertColor(alertManager.GlobalAlertLevel);
            GUIStyle alertStyle = new GUIStyle(textStyle);
            alertStyle.normal.textColor = alertColor;
            
            GUI.Label(new Rect(10, yPos, 400, lineHeight), $"Global Alert: {alertManager.GlobalAlertLevel:F0}%", alertStyle);
            yPos += lineHeight;
        }
        
        EnemyAwareness[] enemies = FindObjectsByType<EnemyAwareness>(FindObjectsSortMode.None);
        int idleCount = 0;
        int suspiciousCount = 0;
        int alertCount = 0;
        
        foreach (var enemy in enemies)
        {
            switch (enemy.CurrentState)
            {
                case AwarenessState.Idle: idleCount++; break;
                case AwarenessState.Suspicious: suspiciousCount++; break;
                case AwarenessState.Alert: alertCount++; break;
            }
        }
        
        yPos += lineHeight * 0.5f;
        
        GUI.Label(new Rect(10, yPos, 400, lineHeight), $"Enemies:", textStyle);
        yPos += lineHeight;
        
        GUIStyle idleStyle = new GUIStyle(textStyle);
        idleStyle.normal.textColor = Color.green;
        GUI.Label(new Rect(30, yPos, 400, lineHeight), $"Idle: {idleCount}", idleStyle);
        yPos += lineHeight;
        
        GUIStyle susStyle = new GUIStyle(textStyle);
        susStyle.normal.textColor = Color.yellow;
        GUI.Label(new Rect(30, yPos, 400, lineHeight), $"Suspicious: {suspiciousCount}", susStyle);
        yPos += lineHeight;
        
        GUIStyle alertedStyle = new GUIStyle(textStyle);
        alertedStyle.normal.textColor = Color.red;
        GUI.Label(new Rect(30, yPos, 400, lineHeight), $"Alert: {alertCount}", alertedStyle);
    }

    private Color GetVisibilityColor(float visibility)
    {
        if (visibility < 0.3f) return Color.green;
        if (visibility < 0.7f) return Color.yellow;
        return Color.red;
    }

    private Color GetAlertColor(float alertLevel)
    {
        if (alertLevel < 30f) return Color.green;
        if (alertLevel < 70f) return Color.yellow;
        return Color.red;
    }
}
