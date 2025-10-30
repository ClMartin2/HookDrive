using System;
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

        bool carUnlock = GameSaveController.Instance.IsCarUnlocked(carData.name);

        imageBtn.sprite = carUnlock ? imageSelect : imageAD;
        imageBtn.preserveAspect = carUnlock ? false : true;
        txtBtn.SetActive(carUnlock);
    }

    protected override void OnClick()
    {
        base.OnClick();

        if (!GameSaveController.Instance.IsCarUnlocked(carData.name))
        {
            GameManager.Instance.Pause(true);
            PokiUnitySDK.Instance.rewardedBreakCallBack = OnRewardedBreakCompleted;
            PokiUnitySDK.Instance.rewardedBreak();
            PokiUnitySDK.Instance.gameplayStop();
        }
        else
        {
            GameEvents.SelectShop?.Invoke(carData);
        }

    }

    private void OnRewardedBreakCompleted(bool withReward)
    {
        // V�rifie si la reward est valid�e
        if (withReward)
        {
            // Action si la reward est donn�e
            GameEvents.SelectShop?.Invoke(carData);
            GameSaveController.Instance.UnlockCar(carData.name);
        }
        else
        {
            Debug.Log("Rewarded break annul�e ou sans r�compense.");
        }

        GameManager.Instance.Pause(false);
    }
}
