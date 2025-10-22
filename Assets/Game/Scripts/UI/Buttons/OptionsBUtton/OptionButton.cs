using UnityEngine;

public class OptionButton : CustomButton
{
    private AnimScriptScale animScriptScale;

    private void OnEnable()
    {
        animScriptScale = GetComponent<AnimScriptScale>();
    }

    protected override void OnClick()
    {
        base.OnClick();
        animScriptScale.Scale();
    }
}
