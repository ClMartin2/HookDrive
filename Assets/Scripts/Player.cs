using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;

    [Header("Inputs")]
    [SerializeField] private InputActionReference move;

    [Header("Speed")]
    [SerializeField] private float verticalSpeed;
    [SerializeField] private float maxVerticalSpeed;
    [SerializeField] private float horizontalSpeed;

    private Vector2 moveDirection = Vector2.zero;

    private void Update()
    {
        moveDirection = move.action.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        //rb.AddForce(Vector3.right * moveDirection.y * verticalSpeed * Time.fixedDeltaTime,ForceMode.Force);
        Vector3 velocity = Vector3.right * moveDirection.y * verticalSpeed;
        rb.linearVelocity = new Vector3(velocity.x, rb.linearVelocity.y, velocity.z);
    }
}
