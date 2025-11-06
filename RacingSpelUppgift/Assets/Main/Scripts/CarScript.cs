using UnityEngine;
using UnityEngine.InputSystem;

public class CarScript : MonoBehaviour
{
    [SerializeField] float acceleration = 10f;
    [SerializeField] Rigidbody carPhysics;

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

    public void Update()
    {
        velocity = carPhysics.linearVelocity;
        speed = velocity.magnitude;
        if (speed >= 0.5f)
        {
            moving = true;
        }
    }
}