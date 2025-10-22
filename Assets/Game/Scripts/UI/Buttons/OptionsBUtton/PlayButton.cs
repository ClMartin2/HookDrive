using UnityEngine;

public class PlayButton : CustomButton
{
    protected override void OnClick()
    {
        base.OnClick();
        GameEvents.Play?.Invoke();
    }
}
