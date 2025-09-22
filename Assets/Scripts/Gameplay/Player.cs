using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;

    [Header("Inputs")]
    [SerializeField] private InputActionReference hookInput;

    [Header("Hook Settigs")]
    [SerializeField] private float hookLength = 10;
    [SerializeField] private float hookStrength = 1000;
    [SerializeField,Tooltip("In Seconds")] private float hookCooldown = 0.5f;

    public static Player Instance;

    private bool attachedToHook = false;
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

        carControl.Init();

        Deactivate();
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
        if (attachedToHook)
        {
            counterHookCooldown = 0;
            canHook = false;
        }

        attachedToHook = false;
    }

    private void Hook_canceled(InputAction.CallbackContext obj)
    {
        Debug.Log("Canceled");
        ResetHook();
    }

    private void Hook_performed(InputAction.CallbackContext obj)
    {
        Debug.DrawRay(rb.transform.position, rb.transform.forward * hookLength,Color.red,5f);

        if (Physics.Raycast(rb.transform.position, rb.transform.forward, out hit, hookLength)) {
            attachedToHook = true;
        }
    }

    private void FixedUpdate()
    {
        if (attachedToHook && canHook)
        {
            Debug.Log("Start hook");
            Vector3 direction = (hit.point - rb.transform.position).normalized;
            rb.AddForce(direction * hookStrength,ForceMode.Acceleration);
        }

        if (!canHook)
        {
            counterHookCooldown += Time.deltaTime;

            if(counterHookCooldown >= hookCooldown)
                canHook = true;
        }
    }
}
