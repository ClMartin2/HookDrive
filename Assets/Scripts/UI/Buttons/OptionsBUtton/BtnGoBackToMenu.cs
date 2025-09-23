using UnityEngine;

public class BtnGoBackToMenu : OptionButton
{
    protected override void OnClick()
    {
        base.OnClick();

        GameEvents.GoBackToMenu?.Invoke();
    }
}
