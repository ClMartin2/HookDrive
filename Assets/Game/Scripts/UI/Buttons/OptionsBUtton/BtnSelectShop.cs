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
        // Vérifie si la reward est validée
        if (withReward)
        {
            // Action si la reward est donnée
            GameEvents.SelectShop?.Invoke(carData);
            GameSaveController.Instance.UnlockCar(carData.name);
        }
        else
        {
            Debug.Log("Rewarded break annulée ou sans récompense.");
        }

        GameManager.Instance.Pause(false);
    }
}
