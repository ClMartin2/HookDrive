using UnityEngine;

public class CarControl : MonoBehaviour
{
    [SerializeField] private float horizontalTorque = 100;

    [Header("Car Properties")]
    public float motorTorque = 2000f;
    public float brakeTorque = 2000f;
    public float maxSpeed = 20f;
    public float steeringRange = 30f;
    public float steeringRangeAtMaxSpeed = 10f;
    public float centreOfGravityOffset = -1f;

    private WheelControl[] wheels;
    private Rigidbody rb;

    private IA_Player carControls; // Reference to the new input system

    void Awake()
    {
        carControls = new IA_Player(); // Initialize Input Actions
    }
    void OnEnable()
    {
        carControls.Enable();
    }

    void OnDisable()
    {
        carControls.Disable();
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Adjust center of mass to improve stability and prevent rolling
        Vector3 centerOfMass = rb.centerOfMass;
        centerOfMass.y += centreOfGravityOffset;
        rb.centerOfMass = centerOfMass;

        // Get all wheel components attached to the car
        wheels = GetComponentsInChildren<WheelControl>();
    }

    // FixedUpdate is called at a fixed time interval
    void FixedUpdate()
    {
        // Read the Vector2 input from the new Input System
        Vector2 inputVector = carControls.Car.Movement.ReadValue<Vector2>();

        // Get player input for acceleration and steering
        float vInput = inputVector.y; // Forward/backward input
        float hInput = inputVector.x; // Steering input

        //Rotation test !!!!!!
        //Vector3 rotation = transform.right * hInput * horizontalTorque;
        //rb.AddTorque(rotation, ForceMode.Acceleration);

        // Calculate current speed along the car's forward axis
        float forwardSpeed = Vector3.Dot(transform.forward, rb.linearVelocity);
        float speedFactor = Mathf.InverseLerp(0, maxSpeed, Mathf.Abs(forwardSpeed)); // Normalized speed factor

        Debug.Log("Forward speed : " + forwardSpeed);
        Debug.Log("Speed Factor : " + speedFactor);

        // Reduce motor torque and steering at high speeds for better handling
        float currentMotorTorque = Mathf.Lerp(motorTorque, 0, speedFactor);

        Debug.Log("Current MotorTorque : " + currentMotorTorque);

        // Determine if the player is accelerating or trying to reverse
        bool isAccelerating = Mathf.Sign(vInput) == Mathf.Sign(forwardSpeed);

        foreach (var wheel in wheels)
        {
            if (isAccelerating)
            {
                // Apply torque to motorized wheels
                if (wheel.motorized)
                {
                    wheel.WheelCollider.motorTorque = vInput * currentMotorTorque;
                }
                // Release brakes when accelerating
                wheel.WheelCollider.brakeTorque = 0f;
            }
            else
            {
                // Apply brakes when reversing direction
                wheel.WheelCollider.motorTorque = 0f;
                wheel.WheelCollider.brakeTorque = Mathf.Abs(vInput) * brakeTorque;
            }
        }
    }
}
