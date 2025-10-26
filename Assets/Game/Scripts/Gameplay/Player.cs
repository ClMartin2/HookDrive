using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private ParticleSystem particleFinish;
    [SerializeField] private ParticleSystem particleSystemHookPoint;
    [SerializeField] private Transform groupParticleSystemHookPoint;
    [SerializeField] private float delayToActivateHookParticle = 0.05f;
    [field : SerializeField] public float maxSpeed { get; private set; } = 40;

    [Header("Car")]
    [SerializeField] private CarData carData;
    [SerializeField] private Transform carParent;
    [SerializeField] private string carLayerName = "Player";

    [Header("Inputs")]
    [SerializeField] private InputActionReference hookInput;
    [SerializeField] private ControlButton[] btnsHook;

    [Header("Hook Settigs")]
    [Space(2)]
    [SerializeField] private float hookStrength = 1000;
    [SerializeField] private float hookStartVelocityDivider = 1.2f;
    [SerializeField, Tooltip("In Seconds")] private float hookCooldown = 0.5f;
    [field: SerializeField] public Transform hookStartPoint { get; private set; }

    [SerializeField] private float shakeDurationBadHook;
    [SerializeField] private float ampltitudeGainBadHook;
    [SerializeField] private float frequencyGainBadHook;

    [Header("Land Settings")]
    [SerializeField] private float impactThresholdWheel = 8f;
    [SerializeField] private float impactThresholdCar = 30f;

    [SerializeField] private float shakeDurationLand;
    [SerializeField] private float ampltitudeGainLand;
    [SerializeField] private float frequencyGainLand;

    [Header("EndZone Settings")]
    [SerializeField] private float diviserVelocity = 1;


    public static Player Instance;

    public Vector3 hookPointPosition { get; private set; }
    public bool isGrappling { get; private set; }
    public bool attachedToHook { get; private set; }
    public float actualSpeed { get; private set; }

    private float counterHookCooldown = 0;
    private bool canHook = true;
    private CarControl carControl;
    private bool stopUpdate = false;
    private HookDetection hookDetection;
    private HookPoint hookPoint;
    private HookPoint lastHookPoint;
    private HookPoint attachedHookPoint;
    private _Camera _camera;
    private bool checkCollision = true;
    private GameObject currentCarModel;
    private bool canActivateSoundLanding = true;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        hookDetection = GetComponentInChildren<HookDetection>();
        carControl = GetComponent<CarControl>();

        hookInput.action.Enable();
        hookInput.action.performed += Hook_performed;
        hookInput.action.canceled += Hook_canceled;

        carControl.Init();

        UpdateCarModel(carData);
    }

    private void Start()
    {
        if (GameManager.isMobile())
        {
            foreach (var btnHook in btnsHook)
            {
                btnHook.onPointerDown += HookStart;
                btnHook.onPointerUp += HookEnd;
            }
        }

        _camera = _Camera.Instance;
    }

    private void Update()
    {
        actualSpeed = rb.linearVelocity.z;

        if (stopUpdate)
            return;

        foreach (WheelControl wheel in carControl.wheels)
        {
            if (!wheel.wheelCollider.isGrounded)
            {
                checkCollision = true;
            }

            if (wheel.wheelCollider.isGrounded)
            {
                Vector3 wheelVelocity = rb.GetPointVelocity(wheel.wheelCollider.transform.position);
                float impactSpeed = Mathf.Abs(wheelVelocity.y);

                //Collide(impactSpeed, impactThresholdWheel);

                checkCollision = false;

                break;
            }
        }

        hookPoint = GetClosestHookPoint(hookDetection.hookPoints, true);

        if (lastHookPoint != null && hookPoint != lastHookPoint)
        {
            lastHookPoint.Unlock();
        }

        if (hookPoint != null && !hookPoint.isLocked)
        {
            hookPoint.Lock();
        }

        lastHookPoint = hookPoint;

        if (!canHook)
        {
            counterHookCooldown += Time.deltaTime;

            if (counterHookCooldown >= hookCooldown)
            {
                hookInput.action.performed += Hook_performed;
                hookInput.action.canceled += Hook_canceled;

                counterHookCooldown = 0;
                canHook = true;
            }
        }
    }


    private void FixedUpdate()
    {
        if (stopUpdate)
            return;

        if (attachedToHook && canHook)
        {
            Vector3 forceDirection = (hookPointPosition - rb.transform.position).normalized;
            rb.AddForce(forceDirection * hookStrength, ForceMode.Acceleration);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!checkCollision)
            return;

        float impactSpeed = collision.relativeVelocity.magnitude;
        Collide(impactSpeed,impactThresholdCar);
        checkCollision = false;
    }

    private void OnCollisionExit(Collision collision)
    {
        checkCollision = true;
    }

    public void Restart()
    {
        carControl.Restart();
        hookDetection.Restart();
        ResetHook();
        particleFinish.gameObject.SetActive(false);
        particleFinish.Stop();

        checkCollision = true;
        hookInput.action.performed += Hook_performed;
        hookInput.action.canceled += Hook_canceled;

        foreach (ControlButton btnHook in btnsHook)
        {
            btnHook.OnPointerUp(null);
        }
    }

    public void Pause()
    {
        carControl.Deactivate();
        hookInput.action.Disable();
        rb.useGravity = false;
    }

    public void EndScene()
    {
        Pause();
        rb.linearVelocity /= diviserVelocity;

        particleFinish.transform.position = transform.position;
        particleFinish.gameObject.SetActive(true);
        particleFinish.Play();
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
        hookInput.action.Disable();
        rb.useGravity = false;
        carControl.Deactivate();
        stopUpdate = true;
        particleFinish.gameObject.SetActive(false);
        particleFinish.Stop();
    }

    public void DeactivateControl()
    {
        hookInput.action.Disable();
        carControl.Deactivate();
    }

    public void ActivateControl()
    {
        carControl.Activate();
        hookInput.action.Enable();
    }

    public void Activate()
    {
        gameObject.SetActive(true);
        carControl.Activate();
        hookInput.action.Enable();
        rb.useGravity = true;
        stopUpdate = false;
    }

    public void UpdateCarModel(CarData carData)
    {
        if(currentCarModel)
            Destroy(currentCarModel);

        currentCarModel = Instantiate(carData.carModel, carParent);
        currentCarModel.transform.position += carData.offsetPositionCar;

        int layer = LayerMask.NameToLayer(carLayerName);
        SetLayerRecursively(currentCarModel, layer);
    }

    private void Collide(float impactSpeed, float impactThreshold)
    {
        if (impactSpeed > impactThreshold)
        {
            _camera.Shake(shakeDurationLand, ampltitudeGainLand, frequencyGainLand);

            if (canActivateSoundLanding)
            {
                SoundManager.Instance.PlaySoundSFX(SoundManager.Landing.audioClip, SoundManager.Landing.volume);
                canActivateSoundLanding = false;
                StartCoroutine(SetCanActivateSoundLanding());
            }
        }
    }

    private IEnumerator SetCanActivateSoundLanding()
    {
        yield return new WaitForSeconds(0.25f);
        canActivateSoundLanding = true;
        yield return null;
    }

    private void SetLayerRecursively(GameObject obj, int layer)
    {
        obj.layer = layer;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, layer);
        }
    }

    private void ResetHook()
    {
        counterHookCooldown = 0;
        canHook = true;
        attachedToHook = false;
    }

    private void Hook_canceled(InputAction.CallbackContext obj)
    {
        HookEnd();
    }

    private void Hook_performed(InputAction.CallbackContext obj)
    {
        HookStart();
    }

    private void HookEnd()
    {
        if (canHook)
        {
            canHook = false;
            attachedToHook = false;
            isGrappling = false;

            if (attachedHookPoint)
            {
                attachedHookPoint.UnAttached();
                attachedHookPoint = null;
            }
        }

        hookInput.action.performed -= Hook_performed;
        hookInput.action.canceled -= Hook_canceled;
    }

    private void HookStart()
    {
        if (!canHook)
        {
            _camera.Shake(shakeDurationBadHook, ampltitudeGainBadHook, frequencyGainBadHook);
            return;
        }

        if (!GameManager.Instance.gameplayStart)
        {
            GameEvents.GameplayStart?.Invoke();
        }

        if (hookPoint == null)
        {
            _camera.Shake(shakeDurationBadHook, ampltitudeGainBadHook, frequencyGainBadHook);
            return;
        }
        else
        {
            SoundManager.Instance.PlaySoundSFX(SoundManager.HookStart.audioClip, SoundManager.HookStart.volume);
            attachedHookPoint = hookPoint;
            hookPoint.Attached();
            isGrappling = true;
            attachedToHook = true;
            hookPointPosition = hookPoint.transform.position;
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, rb.linearVelocity.y / hookStartVelocityDivider, rb.linearVelocity.z / hookStartVelocityDivider);
            StartCoroutine(DelayParticleSystemHook());
        }
    }

    private IEnumerator DelayParticleSystemHook()
    {
        yield return new WaitForSeconds(delayToActivateHookParticle);
        groupParticleSystemHookPoint.position = hookPointPosition;
        particleSystemHookPoint.Play();
        yield return null;
    }

    private HookPoint GetClosestHookPoint( List<HookPoint> hookPoints, bool CheckHookPointBehind)
    {
        HookPoint closest = null;
        float closestDistanceSqr = Mathf.Infinity;

        if (hookPoints.Count > 1)
        {
            foreach (HookPoint hookPoint in hookPoints)
            {
                if (hookPoint == null)
                    continue;

                if (CheckHookPointBehind)
                {
                    Vector3 toHook = hookPoint.transform.position - transform.position;

                    if (Vector3.Dot(Vector3.forward, toHook.normalized) < 0f)
                        continue;
                }

                float sqrDistance = (hookPoint.transform.position - transform.position).sqrMagnitude;

                if (sqrDistance < closestDistanceSqr)
                {
                    closestDistanceSqr = sqrDistance;
                    closest = hookPoint;
                }
            }

            if (closest == null)
            {
                return GetClosestHookPoint(hookPoints, false);
            }
        }
        else if (hookPoints.Count == 1)
            closest = hookPoints[0];

        return hookPoints.Count > 0 ? closest : null;
    }
}
