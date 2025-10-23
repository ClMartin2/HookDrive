using UnityEngine;

public class BtnSelectShop : OptionButton
{
    public CarData carData;

    protected override void OnClick()
    {
        base.OnClick();
        GameEvents.SelectShop?.Invoke(carData);
    }
}
