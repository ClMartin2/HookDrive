using UnityEngine;

public class BtnShop : OptionButton
{
    protected override void OnClick()
    {
        base.OnClick();
        Shop.Instance.Show();
    }
}
