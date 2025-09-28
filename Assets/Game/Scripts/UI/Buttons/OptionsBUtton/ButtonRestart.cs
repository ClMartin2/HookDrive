public class ButtonRestart : OptionButton
{
    protected override void OnClick()
    {
        base.OnClick();
        GameEvents.OnRestartRequested?.Invoke();
    }
}
