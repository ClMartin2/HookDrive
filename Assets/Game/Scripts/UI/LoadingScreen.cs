using UnityEngine;
using UnityEngine.UI;

public class LoadingScreen : CustomScreen
{
    [SerializeField] private Image backgroundImage;

    public void UpdateBackgroundImage(float percentage)
    {
        backgroundImage.fillAmount = percentage;
    }

    public override void Show()
    {
        base.Show();
        UpdateBackgroundImage(1);
    }
}
