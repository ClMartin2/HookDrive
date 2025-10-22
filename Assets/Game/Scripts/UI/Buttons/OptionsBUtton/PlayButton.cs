using UnityEngine;

public class PlayButton : OptionButton
{
    protected override void OnClick()
    {
        base.OnClick();
        GameEvents.Play?.Invoke();
    }
}
