using UnityEngine;

public static class GameConstants
{
    public const string PLAYER_TAG = "Player";
    public const string ENEMY_TAG = "Enemy";
    public const string SHADOW_TAG = "Shadow";
    
    public const string PLAYER_LAYER = "Player";
    public const string ENEMY_LAYER = "Enemy";
    public const string COVER_LAYER = "Cover";
    public const string GROUND_LAYER = "Ground";
    public const string SHADOW_LAYER = "Shadow";
    
    public const float DEFAULT_DETECTION_RANGE = 10f;
    public const float DEFAULT_FOV = 90f;
    public const float DEFAULT_PATROL_SPEED = 2f;
    public const float DEFAULT_CHASE_SPEED = 5f;
}
