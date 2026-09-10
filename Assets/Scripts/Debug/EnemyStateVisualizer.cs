using UnityEngine;

[RequireComponent(typeof(EnemyAwareness))]
public class EnemyStateVisualizer : MonoBehaviour
{
    [Header("Visual Settings")]
    [SerializeField] private bool showStateIndicator = true;
    [SerializeField] private float indicatorHeight = 2.5f;
    [SerializeField] private float indicatorSize = 0.5f;
    
    [Header("State Colors")]
    [SerializeField] private Color idleColor = Color.green;
    [SerializeField] private Color suspiciousColor = Color.yellow;
    [SerializeField] private Color alertColor = Color.red;
    
    private EnemyAwareness awareness;
    private GameObject indicatorObject;
    private MeshRenderer indicatorRenderer;

    private void Awake()
    {
        awareness = GetComponent<EnemyAwareness>();
        CreateIndicator();
    }

    private void CreateIndicator()
    {
        indicatorObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        indicatorObject.name = "StateIndicator";
        indicatorObject.transform.SetParent(transform);
        indicatorObject.transform.localPosition = new Vector3(0, indicatorHeight, 0);
        indicatorObject.transform.localScale = Vector3.one * indicatorSize;
        
        Destroy(indicatorObject.GetComponent<Collider>());
        
        indicatorRenderer = indicatorObject.GetComponent<MeshRenderer>();
        
        Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        mat.SetFloat("_Surface", 1);
        mat.SetFloat("_Blend", 0);
        mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
        mat.SetFloat("_SrcBlend", (float)UnityEngine.Rendering.BlendMode.SrcAlpha);
        mat.SetFloat("_DstBlend", (float)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        mat.SetFloat("_ZWrite", 0);
        mat.renderQueue = 3000;
        
        indicatorRenderer.material = mat;
        
        indicatorObject.SetActive(showStateIndicator);
    }

    private void OnEnable()
    {
        if (awareness != null)
        {
            awareness.OnStateChanged += UpdateVisual;
        }
    }

    private void OnDisable()
    {
        if (awareness != null)
        {
            awareness.OnStateChanged -= UpdateVisual;
        }
    }

    private void Update()
    {
        UpdateVisual(awareness.CurrentState);
    }

    private void UpdateVisual(AwarenessState state)
    {
        if (indicatorRenderer == null) return;
        
        Color targetColor = state switch
        {
            AwarenessState.Idle => idleColor,
            AwarenessState.Suspicious => suspiciousColor,
            AwarenessState.Alert => alertColor,
            _ => Color.white
        };
        
        indicatorRenderer.material.color = targetColor;
    }

    private void OnDestroy()
    {
        if (indicatorObject != null)
        {
            Destroy(indicatorObject);
        }
    }
}
