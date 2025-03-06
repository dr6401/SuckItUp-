using UnityEngine;

public class VacuumMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float sprintMultiplier = 1.5f;
    public float jumpForce = 5f;
    public float drag = 2f;
    public float mouseSensitivity = 2f; // Controls mouse sensitivity

    private Rigidbody rb;
    private bool isGrounded;
    private Transform cameraTransform; // Store camera reference
    private float verticalRotation = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.drag = drag;
        rb.freezeRotation = true; // Prevents physics-based rotation

        // Get the camera (Make sure the camera is a child of the player)
        cameraTransform = Camera.main.transform;

        // Lock the cursor so it feels like an FPS
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        Move();
        RotatePlayer();
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
        }
    }

    void Move()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 moveDirection = transform.right * moveX + transform.forward * moveZ; // Move relative to player direction
        float speed = moveSpeed * (Input.GetKey(KeyCode.LeftShift) ? sprintMultiplier : 1f);

        rb.AddForce(moveDirection.normalized * speed * 10f, ForceMode.Acceleration);
    }

    void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
        isGrounded = false;
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

    private void OnTriggerEnter(Collider other)
    {
        isGrounded = true;
    }
    private void OnTriggerExit(Collider other)
    {
        isGrounded = false;
    }
}
