using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    private float moveSpeed;
    public float baseMoveSpeed = 1f;
    private float halvedBaseMoveSpeed;
    public float sprintMultiplier = 1.5f;
    public float jumpForce = 5f;
    public float mouseSensitivity = 2f; // Controls mouse sensitivity
    public float gravity = 9.81f;
    public float maxGravity = 50f;
    private bool canMove = true;
    private bool isRunning = false;
    public bool inputBlocked = false;

    //private bool isGrounded = true;
    public Transform cameraTransform; // Store camera reference
    private float verticalRotation = 0f;
    private float verticalVelocity = 0f;
    CharacterController characterController;
    [SerializeField] private WeaponHandler weaponHandler;

    Vector3 moveDirection = Vector3.zero;
    void Start()
    {
        halvedBaseMoveSpeed = baseMoveSpeed / 2;
        characterController = GetComponent<CharacterController>();
        // Get the camera (Make sure the camera is a child of the player)
        cameraTransform = Camera.main.transform;
        weaponHandler = GetComponent<WeaponHandler>();

        // Lock the cursor so it feels like an FPS
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

    }

    void Update()
    {
        if (!inputBlocked)
        {
            ApplyGravity();
            Move();
            RotatePlayer();
            if (Input.GetKeyDown(KeyCode.Space) && characterController.isGrounded)
            {
                Jump();
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
        Vector3 right= transform.TransformDirection(Vector3.right);

        isRunning = (Input.GetKey(KeyCode.LeftShift) && !weaponHandler.isAiming); // Enable sprint only if player isn't aiming

        float curSpeedX = canMove ? (isRunning ? moveSpeed * sprintMultiplier : moveSpeed) * Input.GetAxis("Vertical") : 0;
        float curSpeedZ = canMove ? (isRunning ? moveSpeed * sprintMultiplier : moveSpeed) * Input.GetAxis("Horizontal") : 0;

        moveDirection = (forward * curSpeedX) + (right * curSpeedZ);
        moveDirection.y = verticalVelocity;

        characterController.Move(moveDirection * Time.deltaTime);
    }

    void Jump()
    {
         verticalVelocity = jumpForce;
         //isGrounded = false;
         //Debug.Log("Jumped");
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
}
