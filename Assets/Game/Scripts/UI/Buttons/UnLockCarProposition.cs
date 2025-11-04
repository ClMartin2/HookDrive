using UnityEngine;

public class UnLockCarProposition : CustomButton
{
    public CarData carData;

    protected override void OnClick()
    {
        base.OnClick();

        GameManager.Instance.UnlockSkin(carData);
        GameEvents.Play?.Invoke();
    }
}
