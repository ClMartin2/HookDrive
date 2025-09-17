using System.Collections;
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

    private bool attachedToHook = false;
    private RaycastHit hit;

    private void Awake()
    {
        hookInput.action.Enable();
        hookInput.action.performed += Hook_performed;
        hookInput.action.canceled += Hook_canceled; ;
    }

    private void Hook_canceled(InputAction.CallbackContext obj)
    {
        Debug.Log("Canceled");
        attachedToHook = false;
    }

    private void Hook_performed(InputAction.CallbackContext obj)
    {
        Debug.DrawRay(rb.transform.position, rb.transform.forward * hookLength,Color.red,5f);

        if (Physics.Raycast(rb.transform.position, rb.transform.forward, out hit, hookLength)) {
            Debug.Log("hit");
            attachedToHook = true;
        }
    }

    private void FixedUpdate()
    {
        if (attachedToHook)
        {
            Vector3 direction = (hit.point - rb.transform.position).normalized;

            rb.AddForce(direction * hookStrength,ForceMode.Acceleration);
            Debug.Log("Attached to Hook"); 
        }
    }
}
