using System;
using System.Collections;
using System.Numerics;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    private float moveSpeed;
    public float baseMoveSpeed = 1f;
    private float halvedBaseMoveSpeed;
    public float sprintMultiplier = 1.5f;
    public float jumpForce = 5f;
    public float mouseSensitivity = 0.5f; // Controls mouse sensitivity
    private int isMouseInverted = 1;
    public float gravity = 9.81f;
    private bool canMove = true;
    private bool isRunning = false;
    public bool inputBlocked = false;
    [SerializeField] private SphereCollider playerFeetColliderHitBox;
    private PlayerControls controls;

    private float verticalRotation = 0f;
    private float verticalVelocity = 0f;
    CharacterController characterController;
    [SerializeField] private WeaponHandler weaponHandler;

    [Header("Sliding")] // Sliding
    [SerializeField] private float slideSpeedMultiplier = 5f;
    private float slideDecay = 2f;
    private bool isSliding = false;
    private Vector3 slideDirection;
    private Vector3 horizontalMoveSpeed;
    [SerializeField] private GameObject slideVFX;

    private bool isCrouching = false;
    private bool cameraLowered = false;
    private float playerHeight = 0.75f;
    private Vector3 originalCameraTransform;

    Vector3 moveDirection = Vector3.zero;
    private Vector2 moveInput;
    private Vector2 lookDelta;
    
    // AUGMENT STUFF
    private bool isZooming = false;
    private bool currentZoomiesSpeed;
    private float beforeZoomingMovementSpeed;
    private float sizeScale = 1f;
    private bool isDustRunnerEnabled = false;
    private float dustRunnerSpeedMultiplier = 1f;

    [Header("Camera")]
    public Transform cameraTransform;
    [SerializeField] private Camera camera;


    private void Awake()
    {
        controls = SettingsManager.controls;
    }

    void Start()
    {
        camera = Camera.main;

        halvedBaseMoveSpeed = baseMoveSpeed / 2;
        characterController = GetComponent<CharacterController>();
        // Get the camera (Make sure the camera is a child of the player)
        cameraTransform = Camera.main.transform;
        originalCameraTransform = cameraTransform.localPosition;
        weaponHandler = GetComponent<WeaponHandler>();
        if (SettingsManager.Instance.isMouseInverted)
        {
            isMouseInverted = -1;
        }
        else isMouseInverted = 1;

        // Lock the cursor so it feels like an FPS
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (!PlayerPrefs.HasKey("Sensitivity"))
        {
            PlayerPrefs.SetFloat("Sensitivity", mouseSensitivity);
            Debug.Log("Player didn't have sensitivity yet. Setting it to: " + PlayerPrefs.GetFloat("Sensitivity"));
        }
        else
        {
            mouseSensitivity = PlayerPrefs.GetFloat("Sensitivity");
            Debug.Log("Player already had defined sensitivity: " + PlayerPrefs.GetFloat("Sensitivity"));
        }

    }


    void Update()
    {
        if (!inputBlocked)
        {
            ApplyGravity();
            //if (isZooming) Zoom();
            Move();
            RotatePlayer();
            if (controls.Player.Jump.IsPressed() && characterController.isGrounded) // Jump
            {
                if (!Physics.Raycast(transform.position - Vector3.down * 0.25f, transform.up, playerHeight * 1.4f, ~(1 << LayerMask.NameToLayer("PlayerHeadHitBox"))))
                {
                    Jump();
                }
            }
            
            horizontalMoveSpeed = new Vector3(moveDirection.x, moveDirection.y * 0.2f, moveDirection.z);
            if (controls.Player.Crouch.IsPressed() && characterController.isGrounded) // Crouch or Slide
            {
                if (!isSliding && horizontalMoveSpeed.sqrMagnitude >
                    baseMoveSpeed * baseMoveSpeed + 1f)
                {
                    StartSlide();
                }
                else if (!isSliding)
                {
                    //isCrouching = true;
                    Crouch();   
                }
            }

            if (!controls.Player.Crouch.IsPressed() && (isCrouching || isSliding) && characterController.isGrounded && horizontalMoveSpeed.sqrMagnitude <
                baseMoveSpeed * baseMoveSpeed + 1f) // Stop Crouching/Sliding
            {
                //Debug.Log("Conditions for isUncrouching/StoppingSlide were met");
                if (!Physics.Raycast(transform.position + Vector3.down * 0.3f, transform.up, playerHeight * 1.4f, ~(1 << LayerMask.NameToLayer("PlayerHeadHitBox"))))// These floats are just fine-tuning, so we get the ray cast to align with the newly created player collider (collider when player is crouching)
                {
                    isCrouching = false;
                    isSliding = false;
                    DeCrouch();
                    characterController.Move(Vector3.zero); // Something to start physics quickly
                }
            }

            if (isSliding)
            {
                HandleSlide();
            }

            if (weaponHandler.isAiming && !isSliding)
            {
                moveSpeed = halvedBaseMoveSpeed;
            }
            else if (isCrouching && !isSliding && characterController.isGrounded)
            {
                moveSpeed = halvedBaseMoveSpeed;
            }
            else if (!isSliding)
            {
                moveSpeed = baseMoveSpeed;
            }
        }
        //Debug.Log("baseMoveSpeed * baseMoveSpeed + 1f: " + baseMoveSpeed * baseMoveSpeed + 1f + ", horizontalMoveSpeed.sqrMagnitude: " + horizontalMoveSpeed.sqrMagnitude);
        //Debug.Log("isCrouching: " + isCrouching);
        //Debug.Log("isSliding: " + isSliding);
        //Debug.Log("horizontalMoveSpeed.y.sqrMagnitude: " + horizontalMoveSpeed.y);
        //Debug.Log("MoveSpeed: " + moveSpeed);
        //Debug.Log($"Vertical velocity: {verticalVelocity}");
    }

    void Move()
    {
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);
        moveInput = controls.Player.Move.ReadValue<Vector2>();

        isRunning = controls.Player.Sprint.IsPressed() && !weaponHandler.isAiming; // Enable sprint only if player isn't aiming

        float curSpeedX =
            canMove ? (isRunning ? moveSpeed * sprintMultiplier : moveSpeed) * moveInput.x : 0;
        float curSpeedZ =
            canMove ? (isRunning ? moveSpeed * sprintMultiplier : moveSpeed) * moveInput.y : 0;

        if (characterController.isGrounded)
        {
            moveDirection = (forward * curSpeedZ) + (right * curSpeedX);    
        }
        else if (isSliding)
        {
            moveDirection += (forward * (curSpeedZ * (5f * Time.deltaTime)));
        }
        else
        {
            moveDirection = (forward * curSpeedZ) + (right * curSpeedX);    
        }
        moveDirection.y = verticalVelocity;

        float currentDustRunnerMultiplierValue = 1f;
        if (isDustRunnerEnabled && weaponHandler.isAlreadySucking)
        {
            currentDustRunnerMultiplierValue = dustRunnerSpeedMultiplier;
            //Debug.Log("DUST RUNNING");
        }
        characterController.Move(moveDirection * (currentDustRunnerMultiplierValue * Time.deltaTime));
    }

    void Jump()
    {
        verticalVelocity = jumpForce;
        if (isSliding) verticalVelocity *= 1.2f; //bigger jump after jumping from slide - B-hopping
    }

    void StartSlide()
    {
        isCrouching = false;
        isSliding = true;
        slideDirection = new Vector3(moveDirection.x, 0, moveDirection.z).normalized;
        moveSpeed *= slideSpeedMultiplier;
        StartCoroutine(SpawnSlideVFX());
        //Crouch();
    }

    void HandleSlide()
    {
        characterController.Move(slideDirection * (moveSpeed * Time.deltaTime));

        moveSpeed = Mathf.Lerp(moveSpeed, 0, slideDecay * Time.deltaTime);
        
        characterController.height = 1;
        characterController.center = new Vector3(0, -playerHeight, 0);
        if (!cameraLowered)
        {
            cameraTransform.localPosition = originalCameraTransform + Vector3.down * 1.2f;
            cameraLowered = true;
        }

        if (moveSpeed < 1f)
        {
            isSliding = false;
            isCrouching = true;
            Crouch();
            Debug.Log("You were sliding a bit to slow, so you stopped sliding and started crouching");
        }
    }

    private IEnumerator SpawnSlideVFX()
    {
        slideVFX.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        slideVFX.SetActive(false);
    }

    void Crouch()
    {
        isCrouching = true;
        characterController.height = 1 * sizeScale;
        characterController.center = new Vector3(0, -playerHeight, 0);
        if (!cameraLowered)
        {
            cameraTransform.localPosition = originalCameraTransform + Vector3.down * (1.2f);
            cameraLowered = true;
        }
    }

    void DeCrouch()
    {
        characterController.height = 2 * sizeScale;
        characterController.center = new Vector3(0, 0, 0);
        if (cameraLowered)
        {
            cameraTransform.localPosition = originalCameraTransform * sizeScale;
            cameraLowered = false;
        }
    }
    

    void ApplyGravity()
    {
        if (characterController.isGrounded)
        {
            verticalVelocity -= 0.1f;

            verticalVelocity = Mathf.Max(verticalVelocity, -1f);
        }
        else
        {
            verticalVelocity -= gravity * Time.deltaTime;
        }
    }

    public void AddVerticalVelocity()
    {
        Debug.Log($"Boutta reduce verticalVelocity by {Mathf.Abs(verticalVelocity)}");
        verticalVelocity -= Mathf.Abs(verticalVelocity) * 0.75f; // decrease vertical speed by 0.75 of speed upwards
    }

    void RotatePlayer()
    {
        lookDelta = controls.Player.Look.ReadValue<Vector2>();
        // Get mouse input
        float mouseX = lookDelta.x * mouseSensitivity * 0.05f; // Add the 0.1f because the ReadValue<Vector2> returns hella big numbers
        float mouseY = lookDelta.y * mouseSensitivity * isMouseInverted * 0.05f; // Add the 0.1f because the ReadValue<Vector2> returns hella big numbers

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
        PlayerPrefs.SetFloat("Sensitivity", sensitivity);
    }

    public void InvertSensitivity(int isInverted)
    {
        isMouseInverted = isInverted;
        Debug.Log("isInverted: " + isInverted);
    }

    // AUGMENT FUNCTIONS
    public void UpdateHalvedMovementSpeed()
    {
        Debug.Log("previous halved moveSpeed: " + halvedBaseMoveSpeed);
        halvedBaseMoveSpeed = baseMoveSpeed / 2;
        Debug.Log("updated halved moveSpeed: " + halvedBaseMoveSpeed);
    }
    
    public void ApplySteadyAim()
    {
        halvedBaseMoveSpeed = baseMoveSpeed;
        Debug.Log("Halved move speed: " + halvedBaseMoveSpeed + " set to base move speed (" + baseMoveSpeed + ")");
    }

    public void ApplyFeatherless()
    {
        gravity *= 0.5f;
    }

    public void ApplySwiftness()
    {
        baseMoveSpeed *= 1.25f;
        UpdateHalvedMovementSpeed();
    }

    public void ApplyZoomies()
    {
        isZooming = true;
        beforeZoomingMovementSpeed = baseMoveSpeed;
    }

    public void ApplySpringstep(float jumpIncrease)
    {
        jumpForce *= jumpIncrease;
    }
    public void ApplyColossalCleaner(float scale)
    {
        characterController.height *= scale;
        characterController.radius *= scale * 0.8f;
        sizeScale *= scale;

        cameraTransform.localPosition = new Vector3(
            cameraTransform.localPosition.x,
            0.917f * characterController.height,
            cameraTransform.localPosition.z
        );
        playerFeetColliderHitBox.radius *= scale * 1.2f; // Make the hit box a bit bigger for safety
        playerFeetColliderHitBox.center = new Vector3(0, -playerFeetColliderHitBox.radius * 0.8f, 0); // * 0.5f for fine tunning
    }

    public void ApplyDustRunner(float speedMultiplier)
    {
        isDustRunnerEnabled = true;
        dustRunnerSpeedMultiplier *= speedMultiplier;
    }
    
    /*private void OnDrawGizmos()
{
    if (camera == null)
    {
        camera = Camera.main;
    }
    
    Gizmos.color = Color.red;

    Vector3 shootOrigin = transform.position + Vector3.down * 0.25f;
    Vector3 shootDirection = transform.up;

    Gizmos.DrawRay(shootOrigin, shootDirection * playerHeight * 1.4f);
    }*/

    private void OnEnable()
    {
        controls.Player.Enable();
        SettingsManager.OnMouseInvertedFromSettingsManager += InvertSensitivity;
        GameEvents.OnSensitivityChanged += SetSensitivity;
    }

    private void OnDisable()
    {
        controls.Player.Disable();
        SettingsManager.OnMouseInvertedFromSettingsManager -= InvertSensitivity;
        GameEvents.OnSensitivityChanged -= SetSensitivity;
    }
}
