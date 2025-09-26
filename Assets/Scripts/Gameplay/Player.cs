using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;

    [Header("Car")]
    [SerializeField] private GameObject carModel;
    [SerializeField] private Transform carParent;

    [Header("Inputs")]
    [SerializeField] private InputActionReference hookInput;
    [SerializeField] private ControlButton btnHook;

    [Header("Hook Settigs")]
    [SerializeField] private float hookLength = 10;
    [SerializeField] private float hookStrength = 1000;
    [SerializeField, Tooltip("In Seconds")] private float hookCooldown = 0.5f;
    [SerializeField] private LayerMask ignoreLayers;
    [field: SerializeField] public Transform hookStartPoint { get; private set; }

    public static Player Instance;

    public Vector3 hookPoint { get; private set; }
    public bool isGrappling { get; private set; }
    public bool attachedToHook { get; private set; }

    private RaycastHit hit;
    private float counterHookCooldown = 0;
    private bool canHook = true;
    private CarControl carControl;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        carControl = GetComponent<CarControl>();

        hookInput.action.Enable();
        hookInput.action.performed += Hook_performed;
        hookInput.action.canceled += Hook_canceled;

        if (GameManager.isMobile())
        {
            btnHook.onPointerDown += HookStart;
            btnHook.onPointerUp += HookEnd;
        }

        carControl.Init();

        Instantiate(carModel, carParent);
    }

    public void Restart()
    {
        carControl.Restart();
        ResetHook();
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
        carControl.Deactivate();
    }

    public void Activate()
    {
        gameObject.SetActive(true);
        carControl.Activate();
    }

    private void ResetHook()
    {
        if (canHook)
        {
            counterHookCooldown = 0;
            canHook = false;
        }

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
        ResetHook();
        hookInput.action.performed -= Hook_performed;
        hookInput.action.canceled -= Hook_canceled;
        isGrappling = false;
    }

    private void HookStart()
    {
        Debug.DrawRay(hookStartPoint.position, rb.transform.forward * hookLength,Color.red,5f);

        if (!canHook)
            return;

        if (Physics.Raycast(hookStartPoint.position, rb.transform.forward, out hit, hookLength, ~ignoreLayers)) {
            attachedToHook = true;
            hookPoint = hit.point;
        }
        else
        {
            hookPoint = hookStartPoint.position + rb.transform.forward * hookLength;
        }

        isGrappling = true;
    }

    private void FixedUpdate()
    {
        if (attachedToHook && canHook)
        {
            Vector3 direction = (hit.point - rb.transform.position).normalized;
            rb.AddForce(direction * hookStrength,ForceMode.Acceleration);
        }

        if (!canHook)
        {
            counterHookCooldown += Time.deltaTime;

            if(counterHookCooldown >= hookCooldown)
            {
                canHook = true;
                hookInput.action.performed += Hook_performed;
                hookInput.action.canceled += Hook_canceled;
            }
        }
    }
}
