using UnityEngine;

public class BtnRestartWorld : OptionButton
{
    protected override void OnClick()
    {
        base.OnClick();
        GameEvents.OnRestartWorld?.Invoke();
    }
}
