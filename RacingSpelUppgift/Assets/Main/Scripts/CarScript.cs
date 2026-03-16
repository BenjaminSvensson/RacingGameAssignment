using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class CarScript : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float acceleration = 10f;
    [SerializeField] float rotationSpeed = 10f;
    [SerializeField] Vector3 visualOffset = new Vector3(0, -0.5f, 0); // Adjust this to align car body with wheels

    [Header("References")]
    [SerializeField] Rigidbody carPhysics; // The Sphere Rigidbody
    [SerializeField] Transform carVisual;  // The Car Mesh (NOT a child of the sphere)
    [SerializeField] TMP_Text speedText;
    [SerializeField] Transform cameraTransform;

    private Vector3 velocity;
    public float speed;
    public bool moving = false;

    Vector2 moveInput;

    void Awake()
    {
        var playerInput = GetComponent<PlayerInput>();
        playerInput.actions["Move"].performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        playerInput.actions["Move"].canceled += ctx => moveInput = Vector2.zero;

        // Ensure the visual is not a child of the sphere to prevent rolling
        if (carVisual.parent == transform)
        {
            carVisual.parent = null;
        }
    }

    void FixedUpdate()
    {
        // --- PHYSICS (Moves the Sphere) ---
        velocity = carPhysics.linearVelocity;
        speed = velocity.magnitude;
        moving = speed >= 0.5f;

        // Calculate camera relative direction
        Vector3 camForward = cameraTransform.forward;
        camForward.y = 0;
        camForward.Normalize();

        Vector3 camRight = cameraTransform.right;
        camRight.y = 0;
        camRight.Normalize();

        Vector3 moveDir = camForward * moveInput.y + camRight * moveInput.x;

        // Apply force to sphere
        if (moveInput.sqrMagnitude > 0.1f)
        {
            carPhysics.AddForce(moveDir * acceleration, ForceMode.Force);
        }
    }

    void Update()
    {
        // Update Text
        if (speedText != null)
            speedText.text = "Speed: " + speed.ToString("F1") + " km/h";

        // --- VISUALS (Moves the Car Model) ---

        // 1. Follow the Sphere smoothly
        carVisual.position = Vector3.Lerp(carVisual.position, transform.position + visualOffset, Time.deltaTime * 20f);

        // 2. Rotate the Visual
        if (moving)
        {
            FaceDirection();
        }
    }

    public void FaceDirection()
    {
        Vector3 moveDir = velocity;
        moveDir.y = 0; // Keep rotation flat initially

        // --- THE FIX FOR REVERSING ---
        // Check the angle between where the car acts like it's facing vs where it's moving.
        // If Dot is negative, we are moving backwards relative to the car's front.
        if (Vector3.Dot(carVisual.forward, moveDir.normalized) < 0)
        {
            // We are reversing, so look in the OPPOSITE direction of movement 
            // (Look Front even though moving Back)
            moveDir = -moveDir;
        }

        if (moveDir.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDir, Vector3.up);

            // Optional: Add Surface alignment (Raycast)
            if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 2f))
            {
                // Re-calculate alignment based on slope normal
                targetRotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(moveDir, hit.normal), hit.normal);
            }

            carVisual.rotation = Quaternion.Slerp(
                carVisual.rotation,
                targetRotation,
                Time.deltaTime * rotationSpeed
            );
        }
    }
}