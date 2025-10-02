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
    [SerializeField] private ControlButton[] btnsHook;

    [Header("Hook Settigs")]
    [SerializeField] private float hookLength = 10;
    [SerializeField] private float hookStrength = 1000;
    [SerializeField] private float hookOffsetAngle = 20f;
    [SerializeField, Tooltip("In Seconds")] private float hookCooldown = 0.5f;
    [SerializeField] private LayerMask ignoreLayers;
    [field: SerializeField] public Transform hookStartPoint { get; private set; }

    [SerializeField] private Transform iconHook;
    [SerializeField] private float speedIconHook = 200f;
    [SerializeField] private float offsetScreenBorder = 50f; // marge pour éviter que ça colle au bord
    [SerializeField] private int frameToCalculateIconHookPosition = 0;
    [SerializeField] private float smoothTime = 0.05f;

    public static Player Instance;

    public Vector3 hookPoint { get; private set; }
    public bool isGrappling { get; private set; }
    public bool attachedToHook { get; private set; }

    private RaycastHit hit;
    private float counterHookCooldown = 0;
    private bool canHook = true;
    private CarControl carControl;
    private int frameCounter;
    private Vector3 velocityIconHook;
    private bool firstTouchIconHook;

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
            foreach (var btnHook in btnsHook)
            {
                btnHook.onPointerDown += HookStart;
                btnHook.onPointerUp += HookEnd;
            }
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
        if (!canHook)
            return;
        Vector3 direction = Quaternion.AngleAxis(hookOffsetAngle, Vector3.right) * rb.transform.forward;

        Debug.DrawRay(hookStartPoint.position, direction * hookLength, Color.red, 5f);

        if (Physics.Raycast(hookStartPoint.position, direction, out hit, hookLength, ~ignoreLayers))
        {
            attachedToHook = true;
            hookPoint = hit.point;
        }
        else
        {
            hookPoint = hookStartPoint.position + direction * hookLength;
        }

        isGrappling = true;
    }

    private void Update()
    {
        frameCounter++;

        Vector3 direction = Quaternion.AngleAxis(hookOffsetAngle, Vector3.right) * rb.transform.forward;

        if (frameCounter >= frameToCalculateIconHookPosition )
        {
            Vector3 iconHookPosition = Vector3.zero;
            float localSmoothTime = smoothTime;

            if (!isGrappling || !attachedToHook)
            {
                RaycastHit hitIcon = new RaycastHit();
                bool hookHasTarget = Physics.Raycast(hookStartPoint.position, direction, out hitIcon, hookLength, ~ignoreLayers);
                iconHookPosition = hookHasTarget ? hitIcon.point : hookStartPoint.position + direction * hookLength;

                iconHook.gameObject.SetActive(hookHasTarget);
                frameCounter = 0;

                if (!firstTouchIconHook && hookHasTarget)
                {
                    localSmoothTime = 0;
                }

                firstTouchIconHook = hookHasTarget;
            }
            else
            {
                iconHookPosition = hit.point;
                localSmoothTime = 0;
            }

            SetIconHookPosition(iconHookPosition,localSmoothTime);
        }

        if (!canHook)
        {
            counterHookCooldown += Time.deltaTime;

            if (counterHookCooldown >= hookCooldown)
            {
                canHook = true;
                hookInput.action.performed += Hook_performed;
                hookInput.action.canceled += Hook_canceled;
            }
        }
    }

    private void SetIconHookPosition(Vector3 iconHookPosition, float localSmoothTime)
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(iconHookPosition);

        float clampedX = Mathf.Clamp(screenPos.x, offsetScreenBorder, Screen.width - offsetScreenBorder);
        float clampedY = Mathf.Clamp(screenPos.y, offsetScreenBorder, Screen.height - offsetScreenBorder);

        Vector3 clampedPos = new Vector3(clampedX, clampedY, screenPos.z);
        iconHook.position = Vector3.SmoothDamp(iconHook.position, clampedPos, ref velocityIconHook, localSmoothTime);
    }

    private void FixedUpdate()
    {
        if (attachedToHook && canHook)
        {
            Vector3 forceDirection = (hit.point - rb.transform.position).normalized;
            rb.AddForce(forceDirection * hookStrength, ForceMode.Acceleration);
        }
    }
}
