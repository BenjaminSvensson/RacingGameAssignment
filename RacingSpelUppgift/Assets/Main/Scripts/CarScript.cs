using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class CarScript : MonoBehaviour
{
    [SerializeField] float acceleration = 10f;
    [SerializeField] Rigidbody carPhysics;
    [SerializeField] Transform carVisual;
    [SerializeField] TMP_Text speedText;

    private Vector3 velocity;
    public float speed;
    public bool moving = false;

    Vector2 moveInput;

    void Awake()
    {
        var playerInput = GetComponent<PlayerInput>();
        playerInput.actions["Move"].performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        playerInput.actions["Move"].canceled += ctx => moveInput = Vector2.zero;
    }

    void FixedUpdate()
    {
        Vector3 force = new Vector3(moveInput.x, 0, moveInput.y) * acceleration;
        carPhysics.AddForce(force, ForceMode.Force);
    }

    void Update()
    {
        velocity = carPhysics.linearVelocity;
        speed = velocity.magnitude;
        moving = speed >= 0.5f;

        speedText.text = "Speed: " + speed.ToString("F1") + " km/h";

        if (moving)
        {
            Vector3 flatVel = new Vector3(velocity.x, 0f, velocity.z);
            if (flatVel.sqrMagnitude > 0.01f)
            {
                FaceSurface(); //chatgpt man
            }

        }
    }
    public void FaceSurface()
    {
        // Check if grounded with a raycast
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 0.4f))
        {
            Vector3 surfaceNormal = hit.normal;
            Vector3 forward = Vector3.ProjectOnPlane(velocity, surfaceNormal).normalized;

            if (forward.sqrMagnitude > 0.001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(forward, surfaceNormal);
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    targetRotation,
                    Time.deltaTime * 10f
                );
            }
        }
        else
        {
            if (velocity.sqrMagnitude > 0.001f)
            {
                // Forward points along velocity, "up" points opposite to gravity
                Vector3 forward = velocity.normalized;
                Vector3 up = -Physics.gravity.normalized;

                Quaternion targetRotation = Quaternion.LookRotation(forward, up);

                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    targetRotation,
                    Time.deltaTime * 2f // slower tilt for dramatic nosedive
                );
            }
        }
    }
}



