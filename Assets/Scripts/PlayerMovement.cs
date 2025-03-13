using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float sprintMultiplier = 1.5f;
    public float jumpForce = 5f;
    public float drag = 2f;
    public float mouseSensitivity = 2f; // Controls mouse sensitivity
    public float gravity = 9.81f;
    public float maxGravity = 50f;
    public float gravityAcceleration = 1.5f;
    private float currentGravity;
    private bool canMove = true;
    private bool isRunning = false;
    private float movementDirectionY;
    float rotationX;

    private Rigidbody rb;
    private bool isGrounded;
    private Transform cameraTransform; // Store camera reference
    private float verticalRotation = 0f;
    CharacterController characterController;

    Vector3 moveDirection = Vector3.zero;
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        rb = GetComponent<Rigidbody>();
        rb.linearDamping = drag;
        rb.freezeRotation = true; // Prevents physics-based rotation

        // Get the camera (Make sure the camera is a child of the player)
        cameraTransform = Camera.main.transform;

        // Lock the cursor so it feels like an FPS
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        ApplyGravity();
        Move();
        RotatePlayer();
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
        }
        Debug.Log("Is player grounded:" + isGrounded);
    }

    void Move()
    {
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right= transform.TransformDirection(Vector3.right);

        isRunning = Input.GetKey(KeyCode.LeftShift);

        float curSpeedX = canMove ? (isRunning ? moveSpeed * sprintMultiplier : moveSpeed) * Input.GetAxis("Vertical") : 0;
        float curSpeedZ = canMove ? (isRunning ? moveSpeed * sprintMultiplier : moveSpeed) * Input.GetAxis("Horizontal") : 0;

        movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedZ);

        characterController.Move(moveDirection * Time.deltaTime);
    }

    void Jump()
    {
        if (characterController.isGrounded)
        {
            moveDirection.y = jumpForce;
            isGrounded = false;
        }
    }

    void ApplyGravity()
    {
        if (characterController.isGrounded)
        {
            currentGravity = 0.1f;
        }
        else
        {
            currentGravity += gravity * Time.deltaTime;
        }

        moveDirection.y = -currentGravity;
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

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Ground"))
        {
            isGrounded = true;
        }

    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
}
