using UnityEngine;

public class CarControl : MonoBehaviour
{
    [SerializeField] private float horizontalTorque = 100;

    [Header("Car Properties")]
    [SerializeField] private float motorTorque = 2000f;
    [SerializeField] private float brakeTorque = 2000f;
    [SerializeField] private float maxSpeed = 20f;
    [SerializeField] private float centreOfGravityOffset = -1f;

    private WheelControl[] wheels;
    private Rigidbody rb;

    private IA_Player carControls;

    void Awake()
    {
        carControls = new IA_Player();
    }

    void OnEnable()
    {
        carControls.Enable();
    }

    void OnDisable()
    {
        carControls.Disable();
    }

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

    void FixedUpdate()
    {
        Vector2 inputVector = carControls.Car.Movement.ReadValue<Vector2>();

        float vInput = inputVector.y;
        float hInput = inputVector.x; 

        Vector3 localTorque = new Vector3(hInput * horizontalTorque,0,0);
        rb.AddRelativeTorque(localTorque, ForceMode.Acceleration);

        // Calculate current speed along the car's forward axis
        float forwardSpeed = Vector3.Dot(transform.forward, rb.linearVelocity);
        float speedFactor = Mathf.InverseLerp(0, maxSpeed, Mathf.Abs(forwardSpeed)); // Normalized speed factor

        // Reduce motor torque and steering at high speeds for better handling
        float currentMotorTorque = Mathf.Lerp(motorTorque, 0, speedFactor);

        // Determine if the player is accelerating or trying to reverse
        float desiredBrake = 0f;
        float desiredMotor = 0f;

        if (vInput != 0f)
        {
            if (Mathf.Sign(vInput) != Mathf.Sign(forwardSpeed) && Mathf.Abs(forwardSpeed) > 0.1f)
            {
                // On veut changer de direction → frein
                desiredBrake = brakeTorque;
                desiredMotor = 0f;
            }
            else
            {
                // On accélère dans la même direction
                desiredMotor = vInput * currentMotorTorque;
                desiredBrake = 0f;
            }
        }

        foreach (var wheel in wheels)
        {
            if (wheel.motorized)
                wheel.WheelCollider.motorTorque = desiredMotor;

            wheel.WheelCollider.brakeTorque = desiredBrake;
        }
    }
}
