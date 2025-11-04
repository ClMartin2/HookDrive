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
        if (!GameSaveController.Instance.IsCarUnlocked(carData.name))
        {
#if UNITY_WEBGL && !UNITY_EDITOR
        if (PokiUnitySDK.Instance != null)
        {
            GameManager.Instance.Pause(true);
            PokiUnitySDK.Instance.rewardedBreakCallBack = OnRewardedBreakCompleted;
            PokiUnitySDK.Instance.rewardedBreak();
        }
        else
        {
            Debug.LogWarning("Poki SDK not ready, simulating reward.");
            OnRewardedBreakCompleted(true);
        }
#else
            OnRewardedBreakCompleted(true); // Simulation en Editor
#endif
        }
        else
        {
            GameEvents.SelectShop?.Invoke(carData);
        }

    }

    private void OnRewardedBreakCompleted(bool withReward)
    {
        GameManager.Instance.Pause(false);

        if (withReward)
        {
            GameSaveController.Instance.UnlockCar(carData.name);
            GameEvents.SelectShop?.Invoke(carData);
        }
        else
        {
            Debug.Log("Rewarded break annulée ou sans récompense.");
        }

    }
}
