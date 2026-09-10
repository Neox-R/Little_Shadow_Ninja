using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 3f;
    [SerializeField] private float sprintSpeed = 6f;
    [SerializeField] private float crouchSpeed = 1.5f;
    [SerializeField] private float rotationSpeed = 10f;
    
    [Header("Crouch Settings")]
    [SerializeField] private float standingHeight = 2f;
    [SerializeField] private float crouchingHeight = 1f;
    [SerializeField] private float crouchTransitionSpeed = 10f;
    
    private CharacterController characterController;
    private PlayerInput playerInput;
    private BladderBowelSystem bladderBowelSystem;
    private Vector2 moveInput;
    private bool isCrouching;
    private bool isSprinting;
    private float currentSpeed;
    private float targetHeight;
    
    public bool IsCrouching => isCrouching;
    public bool IsSprinting => isSprinting;
    public float CurrentSpeed => characterController.velocity.magnitude;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();
        bladderBowelSystem = GetComponent<BladderBowelSystem>();
        targetHeight = standingHeight;
    }

    private void Update()
    {
        if (bladderBowelSystem != null && bladderBowelSystem.IsHavingAccident)
        {
            HandleCrouching();
            return;
        }
        
        HandleMovement();
        HandleCrouching();
    }

    private void HandleMovement()
    {
        if (moveInput.magnitude > 0.1f)
        {
            Vector3 inputDirection = new Vector3(moveInput.x, 0, moveInput.y);
            
            Vector3 cameraForward = Camera.main.transform.forward;
            Vector3 cameraRight = Camera.main.transform.right;
            cameraForward.y = 0;
            cameraRight.y = 0;
            cameraForward.Normalize();
            cameraRight.Normalize();
            
            Vector3 moveDirection = (cameraForward * inputDirection.z + cameraRight * inputDirection.x).normalized;
            
            currentSpeed = isCrouching ? crouchSpeed : (isSprinting ? sprintSpeed : walkSpeed);
            characterController.Move(moveDirection * currentSpeed * Time.deltaTime);
            
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
        
        characterController.Move(Vector3.down * 9.81f * Time.deltaTime);
    }

    private void HandleCrouching()
    {
        targetHeight = isCrouching ? crouchingHeight : standingHeight;
        characterController.height = Mathf.Lerp(characterController.height, targetHeight, crouchTransitionSpeed * Time.deltaTime);
        
        Vector3 center = characterController.center;
        center.y = characterController.height / 2f;
        characterController.center = center;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isCrouching = !isCrouching;
            isSprinting = false;
        }
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        isSprinting = context.ReadValueAsButton();
        if (isSprinting) isCrouching = false;
    }
}
