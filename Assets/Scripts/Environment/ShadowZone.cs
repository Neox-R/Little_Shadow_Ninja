using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class ShadowZone : MonoBehaviour
{
    [Header("Shadow Settings")]
    [SerializeField] private Color shadowColor = new Color(0, 0, 0, 0.5f);
    [SerializeField] private bool showDebugGizmo = true;
    
    private BoxCollider shadowCollider;

    private void Awake()
    {
        shadowCollider = GetComponent<BoxCollider>();
        shadowCollider.isTrigger = true;
        gameObject.tag = "Shadow";
    }

    private void OnDrawGizmos()
    {
        if (showDebugGizmo)
        {
            Gizmos.color = shadowColor;
            Gizmos.DrawCube(transform.position, transform.localScale);
        }
    }
}
