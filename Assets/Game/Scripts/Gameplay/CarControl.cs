using System.Collections;
using UnityEngine;

public class CarControl : MonoBehaviour
{
    [Header("Inputs")]
    [SerializeField] private ControlButton btnLeftRotate;
    [SerializeField] private ControlButton btnRightRotate;
    [SerializeField] private ControlButton btnForward;
    [SerializeField] private ControlButton btnBackward;

    [Header("Car Properties")]
    [SerializeField] private float horizontalTorque = 100;
    [SerializeField] private float motorTorque = 2000f;
    [SerializeField] private float brakeTorque = 2000f;
    [SerializeField] private float maxForwardSpeed = 20f;
    [SerializeField] private float maxReverseSpeed = 20f;
    [SerializeField] private float centreOfGravityOffset = -1f;


    public WheelControl[] wheels { get; private set; }

    private Rigidbody rb;
    private IA_Player carControls;
    private Quaternion startRotation;
    private float maxSpeed;

    private float vInput = 0;
    private float hInput = 0;

    public void Activate()
    {
        carControls.Enable();
    }

    public void Deactivate()
    {
        carControls.Disable();
    }

    public void Restart()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        foreach (var wheel in wheels)
        {
            wheel.wheelCollider.motorTorque = 0f;
            wheel.wheelCollider.brakeTorque = 0f;
            wheel.wheelCollider.steerAngle = 0f;

            // Désactive temporairement le WheelCollider
            wheel.wheelCollider.enabled = false;
        }

        StartCoroutine(ReenableWheels());
        transform.rotation = startRotation;
    }

    public void Init()
    {
        carControls = new IA_Player();

        rb = GetComponent<Rigidbody>();

        // Adjust center of mass to improve stability and prevent rolling
        Vector3 centerOfMass = rb.centerOfMass;
        centerOfMass.y += centreOfGravityOffset;
        rb.centerOfMass = centerOfMass;

        // Get all wheel components attached to the car
        wheels = GetComponentsInChildren<WheelControl>();

        foreach (var wheel in wheels)
        {
            wheel.Init();
        }

        startRotation = transform.rotation;
    }

    private void FixedUpdate()
    {
        if (GameManager.isMobile())
        {
            if (btnLeftRotate.IsPressed)
                hInput = -1;
            else if (btnRightRotate.IsPressed)
                hInput = 1;
            else
                hInput = 0;

            if (btnForward.IsPressed)
                vInput = 1;
            else if (btnBackward.IsPressed)
                vInput = -1;
            else
                vInput = 0;
        }
        else
        {
            Vector2 inputVector = carControls.Car.Movement.ReadValue<Vector2>();

            vInput = inputVector.y;
            hInput = inputVector.x;
        }

        if(!GameManager.Instance.gameplayStart && (hInput != 0 || vInput != 0))
        {
            GameEvents.GameplayStart?.Invoke();
        }

        Vector3 localTorque = new Vector3(hInput * horizontalTorque, 0, 0);
        rb.AddRelativeTorque(localTorque, ForceMode.Force);

        // Calculate current speed along the car's forward axis
        float forwardSpeed = Vector3.Dot(transform.forward, rb.linearVelocity);

        maxSpeed = vInput > 0 ? maxForwardSpeed : maxReverseSpeed;

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
        else if (vInput == 0f && Mathf.Abs(rb.linearVelocity.z) <= 0.1f)
        {
            desiredBrake = 1000f;
            desiredMotor = 0f;
        }

        SetWheelTorque(desiredMotor, desiredBrake);
    }

    private IEnumerator ReenableWheels()
    {
        yield return new WaitForFixedUpdate();
        foreach (var wheel in wheels)
        {
            wheel.wheelCollider.enabled = true;
        }
    }

    private void SetWheelTorque(float desiredMotor, float desiredBrake)
    {
        foreach (var wheel in wheels)
        {
            if (wheel.motorized)
                wheel.wheelCollider.motorTorque = desiredMotor;

            wheel.wheelCollider.brakeTorque = desiredBrake;
        }
    }
}
