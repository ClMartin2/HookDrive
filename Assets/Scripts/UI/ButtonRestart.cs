public class ButtonRestart : CustomButton
{
    protected override void OnClick()
    {
        base.OnClick();
        GameEvents.OnRestartRequested?.Invoke();
    }
}
