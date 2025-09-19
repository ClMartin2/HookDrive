using UnityEngine;

public class WheelControl : MonoBehaviour
{
    [SerializeField] private Transform wheelModel;
    
    [HideInInspector] public WheelCollider wheelCollider { get; private set; }

    // Create properties for the CarControl script
    // (You should enable/disable these via the 
    // Editor Inspector window)
    public bool steerable;
    public bool motorized;

    Vector3 position;
    Quaternion rotation;

    // Start is called before the first frame update
    public void Init()
    {
        wheelCollider = GetComponent<WheelCollider>();
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        // Get the Wheel collider's world pose values and
        // use them to set the wheel model's position and rotation
        wheelCollider.GetWorldPose(out position, out rotation);
        wheelModel.transform.position = position;
        wheelModel.transform.rotation = rotation;
    }
}
