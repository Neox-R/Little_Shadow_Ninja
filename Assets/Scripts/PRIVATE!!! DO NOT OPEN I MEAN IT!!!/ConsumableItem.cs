using UnityEngine;

public class ConsumableItem : MonoBehaviour
{
    [Header("Item Type")]
    [SerializeField] private ItemType type;
    
    [Header("Effects")]
    [SerializeField] private float bladderFillAmount = 0f;
    [SerializeField] private float bowelFillAmount = 0f;
    
    [Header("Visuals")]
    [SerializeField] private Color itemColor = Color.white;
    
    public enum ItemType
    {
        Drink,
        Food,
        Both
    }
    
    private void Start()
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = itemColor;
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            BladderBowelSystem bladderBowel = other.GetComponent<BladderBowelSystem>();
            
            if (bladderBowel != null)
            {
                Consume(bladderBowel);
            }
        }
    }
    
    private void Consume(BladderBowelSystem bladderBowel)
    {
        if (bladderFillAmount > 0f)
        {
            bladderBowel.AddBladderContent(bladderFillAmount);
        }
        
        if (bowelFillAmount > 0f)
        {
            bladderBowel.AddBowelContent(bowelFillAmount);
        }
        
        Debug.Log($"[Consumable] Player consumed {type}: +{bladderFillAmount} bladder, +{bowelFillAmount} bowel");
        
        Destroy(gameObject);
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = itemColor;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
}
