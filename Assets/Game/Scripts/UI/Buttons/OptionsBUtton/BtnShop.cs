using UnityEngine;

public class BtnShop : OptionButton
{
    [SerializeField] private float durationScaleLoop = 0.5f;
    [SerializeField] private float durationScaleClick = 0.1f;

    private bool Click = true;

    private void Update()
    {
        if (!GameManager.Instance.gameplayStart && !animScriptScale.loop && Shop.Instance.hide)
        {
            animScriptScale.loop = true;
            animScriptScale.duration = durationScaleLoop;
            animScriptScale.Scale();
        }
        else if (GameManager.Instance.gameplayStart)
        {
            animScriptScale.loop = false;
        }
    }

    protected override void OnClick()
    {
        animScriptScale.Reset();
        animScriptScale.duration = durationScaleClick;
        animScriptScale.loop = false;

        base.OnClick();
        Shop.Instance.Show();
    }
}
