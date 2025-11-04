using UnityEngine;
using UnityEngine.UI;

public class BtnSelectShop : OptionButton
{
    [SerializeField] private Sprite imageAD;
    [SerializeField] private Sprite imageSelect;
    [SerializeField] private Image imageBtn;
    [SerializeField] private GameObject txtBtn;

    public CarData carData;

    override protected void OnEnable()
    {
        base.OnEnable();

        if (GameSaveController.Instance == null)
            return;

        bool carUnlock = GameSaveController.Instance.IsCarUnlocked(carData.name);

        imageBtn.sprite = carUnlock ? imageSelect : imageAD;
        imageBtn.preserveAspect = carUnlock ? false : true;
        txtBtn.SetActive(carUnlock);
    }

    protected override void OnClick()
    {
        GameManager.Instance.UnlockSkin(carData);   
    }
}
