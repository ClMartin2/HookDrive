using TMPro;
using UnityEngine;

public class StartLoadingScreen : CustomScreen
{
    [field: SerializeField] public TextMeshProUGUI txtLoading { get; private set; }

    public override void Show()
    {
        base.Show();

        SoundManager.Instance.SetMusicInUI(true);
    }

    public override void Hide()
    {
        base.Hide();

        if (SoundManager.Instance != null)
            SoundManager.Instance.SetMusicInUI(false);
    }
}
