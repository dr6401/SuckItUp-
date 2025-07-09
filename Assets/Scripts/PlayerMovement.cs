using System;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    private float moveSpeed;
    public float baseMoveSpeed = 1f;
    private float halvedBaseMoveSpeed;
    public float sprintMultiplier = 1.5f;
    public float jumpForce = 5f;
    public float mouseSensitivity = 0.5f; // Controls mouse sensitivity
    public float gravity = 9.81f;
    private bool canMove = true;
    private bool isRunning = false;
    public bool inputBlocked = false;

    public Transform cameraTransform; // Store camera reference
    private float verticalRotation = 0f;
    private float verticalVelocity = 0f;
    CharacterController characterController;
    [SerializeField] private WeaponHandler weaponHandler;


    private bool isCrouching = false;
    private bool cameraLowered = false;
    private float playerHeight = 0.75f;
    private Vector3 originalCameraTransform;

    Vector3 moveDirection = Vector3.zero;

    [SerializeField] private new Camera camera;

    void Start()
    {
        camera = Camera.main;

        halvedBaseMoveSpeed = baseMoveSpeed / 2;
        characterController = GetComponent<CharacterController>();
        // Get the camera (Make sure the camera is a child of the player)
        cameraTransform = Camera.main.transform;
        originalCameraTransform = cameraTransform.localPosition;
        weaponHandler = GetComponent<WeaponHandler>();

        // Lock the cursor so it feels like an FPS
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (!PlayerPrefs.HasKey("sensitivity"))
        {
            PlayerPrefs.SetFloat("sensitivity", mouseSensitivity);
            Debug.Log("Player didnt have sensitivity yet. Setting it to: " + PlayerPrefs.GetFloat("sensitivity"));
        }
        else
        {
            mouseSensitivity = PlayerPrefs.GetFloat("sensitivity");
            Debug.Log("Player already had defined sensitivity: " + PlayerPrefs.GetFloat("sensitivity"));
        }

    }


    void Update()
    {
        if (!inputBlocked)
        {
            ApplyGravity();
            Move();
            RotatePlayer();
            if (Input.GetKey(KeyCode.Space) && characterController.isGrounded) // Jump
            {
                if (!Physics.Raycast(transform.position - Vector3.down * 0.25f, transform.up, playerHeight * 1.4f))
                {
                    Jump();
                }
            }

            if (Input.GetKey(KeyCode.LeftControl) && characterController.isGrounded) // Crouch
            {
                isCrouching = true;
                Crouch();
            }

            if (!(Input.GetKey(KeyCode.LeftControl)) && isCrouching) // Stop Crouching
            {
                if (!Physics.Raycast(transform.position + Vector3.down * 0.3f, transform.up, playerHeight * 1.4f))// These floats are just fine-tuning, so we get the ray cast to align with the newly created player collider (collider when player is crouching)
                {
                    isCrouching = false;
                    DeCrouch();
                    characterController.Move(Vector3.zero); // Something to start physics quickly
                }
            }

            if (weaponHandler.isAiming)
            {
                moveSpeed = halvedBaseMoveSpeed;
            }
            else
            {
                moveSpeed = baseMoveSpeed;
            }
        }
    }

    void Move()
    {
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        isRunning = (Input.GetKey(KeyCode.LeftShift) &&
                     !weaponHandler.isAiming); // Enable sprint only if player isn't aiming

        float curSpeedX =
            canMove ? (isRunning ? moveSpeed * sprintMultiplier : moveSpeed) * Input.GetAxis("Vertical") : 0;
        float curSpeedZ =
            canMove ? (isRunning ? moveSpeed * sprintMultiplier : moveSpeed) * Input.GetAxis("Horizontal") : 0;

        moveDirection = (forward * curSpeedX) + (right * curSpeedZ);
        moveDirection.y = verticalVelocity;

        characterController.Move(moveDirection * Time.deltaTime);
    }

    void Jump()
    {
        verticalVelocity = jumpForce;
    }

    void Crouch()
    {
        characterController.height = 1;
        characterController.center = new Vector3(0, -playerHeight, 0);
        if (!cameraLowered)
        {
            cameraTransform.localPosition = originalCameraTransform + Vector3.down * 1.2f;
            cameraLowered = true;
        }
        //LowerCamera();
    }

    void DeCrouch()
    {
        characterController.height = 2;
        characterController.center = new Vector3(0, 0, 0);
        if (cameraLowered)
        {
            cameraTransform.localPosition = originalCameraTransform;
            cameraLowered = false;
        }
    }

void LowerCamera()
    {
        if (!cameraLowered)
        {
            cameraTransform.localPosition += Vector3.down * 0.05f;    
        }
        else
        {
            cameraLowered = true;
        }
    }

    void ApplyGravity()
    {
        if (characterController.isGrounded)
        {
            //verticalVelocity -= 0.1f;
        }
        else
        {
            verticalVelocity -= gravity * Time.deltaTime;
        }
    }

    void RotatePlayer()
    {
        // Get mouse input
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Rotate player left/right
        transform.Rotate(Vector3.up * mouseX);

        // Rotate camera up/down (clamping to avoid flipping)
        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -80f, 80f);
        cameraTransform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
    }

    public void SetSensitivity(float sensitivity)
    {
        mouseSensitivity = sensitivity;
        PlayerPrefs.SetFloat("sensitivity", sensitivity);
        
    }
    
    private void OnDrawGizmos()
{
    if (camera == null)
    {
        camera = Camera.main;
    }
    
    Gizmos.color = Color.red;

    Vector3 shootOrigin = transform.position + Vector3.down * 0.25f;
    Vector3 shootDirection = transform.up;

    Gizmos.DrawRay(shootOrigin, shootDirection * playerHeight * 1.4f);
    }
}
