using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private ParticleSystem particleFinish;

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

    [Header("EndZone Settings")]
    [SerializeField] private float diviserVelocity = 1;
    [SerializeField] private float diviserAngularVelocity = 1;

    public static Player Instance;

    public Vector3 hookPointPosition { get; private set; }
    public bool isGrappling { get; private set; }
    public bool attachedToHook { get; private set; }

    private float counterHookCooldown = 0;
    private bool canHook = true;
    private CarControl carControl;
    private bool stopUpdate = false;
    private HookDetection hookDetection;
    private HookPoint hookPoint;
    private HookPoint lastHookPoint;

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

        GameObject newCar = Instantiate(carData.carModel, carParent);
        newCar.transform.position += carData.offsetPositionCar;

        int layer = LayerMask.NameToLayer(carLayerName);
        SetLayerRecursively(newCar, layer);
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
    }

    public void Restart()
    {
        carControl.Restart();
        hookDetection.Restart();
        ResetHook();
        particleFinish.gameObject.SetActive(false);
        particleFinish.Stop();

        hookInput.action.performed += Hook_performed;
        hookInput.action.canceled += Hook_canceled;
    }

    public void Pause()
    {
        carControl.Deactivate();
        hookInput.action.Disable();
        rb.useGravity = false;
        particleFinish.gameObject.SetActive(false);
        particleFinish.Stop();
    }

    public void EndScene()
    {
        Pause();
        rb.linearVelocity /= diviserVelocity;
        rb.angularVelocity /= diviserAngularVelocity;

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

    public void Activate()
    {
        gameObject.SetActive(true);
        carControl.Activate();
        hookInput.action.Enable();
        rb.useGravity = true;
        stopUpdate = false;
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
        }

        hookInput.action.performed -= Hook_performed;
        hookInput.action.canceled -= Hook_canceled;
    }

    private void HookStart()
    {
        if (!canHook)
            return;

        if (!GameManager.Instance.gameplayStart)
        {
            GameEvents.GameplayStart?.Invoke();
        }

        if (hookPoint == null)
            return;
        else
        {
            isGrappling = true;
            attachedToHook = true;
            hookPointPosition = hookPoint.transform.position;
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, rb.linearVelocity.y / hookStartVelocityDivider, rb.linearVelocity.z / hookStartVelocityDivider);
        }
    }

    private void Update()
    {
        if (stopUpdate)
            return;

        hookPoint = GetClosestHookPoint(transform, hookDetection.hookPoints);

        if (lastHookPoint != null && hookPoint != lastHookPoint)
            lastHookPoint.Unlock();

        if (hookPoint != null)
        {
            hookPoint.Lock();
            lastHookPoint = hookPoint;
        }

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

    private HookPoint GetClosestHookPoint(Transform player, List<HookPoint> hookPoints)
    {
        HookPoint closest = null;
        float closestDistanceSqr = Mathf.Infinity;

        if (hookPoints.Count > 1)
        {
            foreach (HookPoint hookPoint in hookPoints)
            {
                if (hookPoint == null)
                    continue;

                Vector3 toHook = hookPoint.transform.position - player.position;

                if (Vector3.Dot(player.forward, toHook.normalized) < 0f)
                    continue;

                float sqrDistance = (hookPoint.transform.position - player.position).sqrMagnitude;

                if (sqrDistance < closestDistanceSqr)
                {
                    closestDistanceSqr = sqrDistance;
                    closest = hookPoint;
                }
            }
        }
        else if (hookPoints.Count == 1)
            closest = hookPoints[0];

        return hookPoints.Count > 0 ? closest : null;
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
}
