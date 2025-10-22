using UnityEngine;

public class BtnRestartWorld : CustomButton
{
    protected override void OnClick()
    {
        base.OnClick();
        GameEvents.OnRestartWorld?.Invoke();
    }
}
