using UnityEngine;
using UnityEngine.InputSystem;

public class UnLockCarProposition : CustomButton
{
    [SerializeField] private InputActionReference watchAdInput;

    public CarData carData;

    protected override void Awake()
    {
        base.Awake();

        watchAdInput.action.performed += Action_performed;
    }

    private void Action_performed(InputAction.CallbackContext obj)
    {
        UnlockSkin();
    }

    private void OnEnable()
    {
        watchAdInput.action.Enable();
    }

    private void OnDisable()
    {
        watchAdInput.action.Disable();

    }

    protected override void OnClick()
    {
        base.OnClick();
        UnlockSkin();
    }

    private void UnlockSkin()
    {
        GameManager.Instance.UnlockSkin(carData);
        GameEvents.Play?.Invoke();
    }
}
