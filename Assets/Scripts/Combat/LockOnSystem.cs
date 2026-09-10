using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Linq;

public class LockOnSystem : MonoBehaviour
{
    [Header("Lock-On Settings")]
    [SerializeField] private float lockOnRange = 15f;
    [SerializeField] private float lockOnSwitchRange = 20f;
    [SerializeField] private float lockOnBreakDistance = 25f;
    [SerializeField] private LayerMask enemyLayer;
    
    [Header("Camera Settings")]
    [SerializeField] private float lockedCameraDistance = 5f;
    [SerializeField] private float lockedCameraHeight = 2f;
    [SerializeField] private float cameraRotationSpeed = 8f;
    
    [Header("Target Switching")]
    [SerializeField] private float switchCooldown = 0.3f;
    
    private Transform currentTarget;
    private List<Transform> availableTargets = new List<Transform>();
    private bool isLockedOn = false;
    private float switchCooldownTimer = 0f;
    
    public Transform CurrentTarget => currentTarget;
    public bool IsLockedOn => isLockedOn;
    
    private void Update()
    {
        if (switchCooldownTimer > 0f)
        {
            switchCooldownTimer -= Time.deltaTime;
        }
        
        if (isLockedOn)
        {
            UpdateLockOn();
        }
    }
    
    public void OnLockOn(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            ToggleLockOn();
        }
    }
    
    public void OnSwitchTargetLeft(InputAction.CallbackContext context)
    {
        if (context.performed && isLockedOn && switchCooldownTimer <= 0f)
        {
            SwitchTarget(-1);
        }
    }
    
    public void OnSwitchTargetRight(InputAction.CallbackContext context)
    {
        if (context.performed && isLockedOn && switchCooldownTimer <= 0f)
        {
            SwitchTarget(1);
        }
    }
    
    private void ToggleLockOn()
    {
        if (isLockedOn)
        {
            ReleaseLockOn();
        }
        else
        {
            AcquireLockOn();
        }
    }
    
    private void AcquireLockOn()
    {
        FindAvailableTargets();
        
        if (availableTargets.Count == 0)
        {
            Debug.Log("[LockOn] No targets in range!");
            return;
        }
        
        currentTarget = GetClosestTarget();
        
        if (currentTarget != null)
        {
            isLockedOn = true;
            Debug.Log($"[LockOn] Locked onto {currentTarget.name}");
        }
    }
    
    private void ReleaseLockOn()
    {
        currentTarget = null;
        isLockedOn = false;
        availableTargets.Clear();
        Debug.Log("[LockOn] Lock released");
    }
    
    private void UpdateLockOn()
    {
        if (currentTarget == null)
        {
            ReleaseLockOn();
            return;
        }
        
        float distanceToTarget = Vector3.Distance(transform.position, currentTarget.position);
        
        if (distanceToTarget > lockOnBreakDistance)
        {
            Debug.Log("[LockOn] Target too far, breaking lock");
            ReleaseLockOn();
            return;
        }
        
        FaceTarget();
    }
    
    private void FaceTarget()
    {
        if (currentTarget == null) return;
        
        Vector3 directionToTarget = (currentTarget.position - transform.position).normalized;
        directionToTarget.y = 0;
        
        if (directionToTarget != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, cameraRotationSpeed * Time.deltaTime);
        }
    }
    
    private void SwitchTarget(int direction)
    {
        FindAvailableTargets();
        
        if (availableTargets.Count <= 1) return;
        
        int currentIndex = availableTargets.IndexOf(currentTarget);
        if (currentIndex == -1)
        {
            currentTarget = availableTargets[0];
        }
        else
        {
            int newIndex = (currentIndex + direction + availableTargets.Count) % availableTargets.Count;
            currentTarget = availableTargets[newIndex];
        }
        
        switchCooldownTimer = switchCooldown;
        Debug.Log($"[LockOn] Switched to {currentTarget.name}");
    }
    
    private void FindAvailableTargets()
    {
        availableTargets.Clear();
        
        Collider[] enemyColliders = Physics.OverlapSphere(transform.position, lockOnSwitchRange, enemyLayer);
        
        foreach (Collider col in enemyColliders)
        {
            EnemyHealth enemyHealth = col.GetComponent<EnemyHealth>();
            if (enemyHealth != null && !enemyHealth.IsDead)
            {
                availableTargets.Add(col.transform);
            }
        }
        
        availableTargets = availableTargets.OrderBy(t => Vector3.Distance(transform.position, t.position)).ToList();
    }
    
    private Transform GetClosestTarget()
    {
        if (availableTargets.Count == 0) return null;
        
        Transform closest = availableTargets[0];
        float closestDistance = Vector3.Distance(transform.position, closest.position);
        
        foreach (Transform target in availableTargets)
        {
            float distance = Vector3.Distance(transform.position, target.position);
            if (distance < closestDistance)
            {
                closest = target;
                closestDistance = distance;
            }
        }
        
        return closest;
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, lockOnRange);
        
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, lockOnSwitchRange);
        
        if (isLockedOn && currentTarget != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, currentTarget.position);
        }
    }
}
