using Coffee.UIExtensions;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WorldClearedScreen : CustomScreen
{
    [SerializeField] private Image background;
    [SerializeField] private string[] nameWorlds;
    [SerializeField] private UIParticle[] uIParticles;
    [SerializeField] private Color[] color;
    [SerializeField] private UIScreenShake uiScreenShake;

    [Header ("Medal")]
    [SerializeField] private Sprite BronzeMedal;
    [SerializeField] private Sprite SilverMedal;
    [SerializeField] private Sprite GoldMedal;

    [Header("Stars")]
    [SerializeField] private Star[] stars;
    [SerializeField] private float delayActivateStarAnimation = 0.25f;
    [SerializeField] private float delayBetweenStarAnimation = 0.1f;

    [Header ("Text")]
    [SerializeField] private TextMeshProUGUI txtLevelCleared;
    [SerializeField] private AnimScriptScale animScriptScaleTxtWorld;

    [Header ("Trophies")]
    [SerializeField] private Image[] trophies;
    [SerializeField] private AnimScriptScale[] animScriptScaleTrophies;

    [Header("Debug")]
    [SerializeField] private bool test;
    [SerializeField] private Medal medalTest;
    [SerializeField] private string levelNameTest = "W-1 L5";

    private void Start()
    {
        animScriptScaleTxtWorld = txtLevelCleared.GetComponent<AnimScriptScale>();
        Hide();
    }

    private void OnEnable()
    {
        if (!test)
            return;

        SetWorldClearedScreen(medalTest, levelNameTest);
        Show();
    }

    public override void Show()
    {
        base.Show();

        foreach (UIParticle uIParticle in uIParticles)
        {
            uIParticle.Play();
        }

        foreach (var animScriptScaleTrophie in animScriptScaleTrophies)
        {
            animScriptScaleTrophie.Scale();
        }

        SoundManager.Instance.SetMusicInUI(true);
        SoundManager.Instance.PlaySoundSFX(SoundManager.Cheering.audioClip, SoundManager.Cheering.volume);
        SoundManager.Instance.PlaySoundSFX(SoundManager.Confetti.audioClip, SoundManager.Confetti.volume);
    }

    public override void Hide()
    {
        base.Hide();

        if (SoundManager.Instance != null)
            SoundManager.Instance.SetMusicInUI(false);
    }

    public void SetWorldClearedScreen(Medal localActualMedal, string levelName)
    {
        switch (localActualMedal)
        {
            case Medal.bronze:
                SetObjects(BronzeMedal, color[0], color[1], 1);
                break;
            case Medal.silver:
                SetObjects(SilverMedal, color[2], color[3],2);
                break;
            case Medal.gold:
                SetObjects(GoldMedal, color[4], color[5], 3);
                break;
        }

        SetTxtWorld(levelName);
    }

    private void SetObjects(Sprite medal, Color color1, Color color2, int numberOfStars)
    {
        SetSpriteTrophy(medal);
        //SetColorBackground(color1, color2);
        StartCoroutine(SetStars(numberOfStars));
    }

    private void SetTxtWorld(string nameLastLevel)
    {
        switch (nameLastLevel)
        {
            case "W-1 L5":
                txtLevelCleared.text = nameWorlds[0] + "\n" + "CLEARED";
                break;
            case "W-2 L5":
                txtLevelCleared.text = nameWorlds[1] + "\n" + "CLEARED";
                break;
            case "W-3 L5":
                txtLevelCleared.text = nameWorlds[2] + "\n" + "CLEARED";
                break;
            case "W-4 L5":
                txtLevelCleared.text = nameWorlds[3] + "\n" + "CLEARED";
                break;
            case "W-5 L5":
                txtLevelCleared.text = nameWorlds[4] + "\n" + "CLEARED";
                break;
        }

        animScriptScaleTxtWorld.Scale();
    }

    private void SetSpriteTrophy(Sprite sprite)
    {
        foreach (Image trophie in trophies)
        {
            trophie.sprite = sprite;
        }
    }

    private void SetColorBackground(Color color, Color color1)
    {
        background.material.SetColor("_ColorA", color);
        background.material.SetColor("_ColorB", color1);
    }

    private IEnumerator SetStars(int numberOfStar)
    {
        for (int i = 0; i < stars.Length; i++)
        {
            Star star = stars[i];
            star.Deactivate();
        }

        yield return new WaitForSeconds(delayActivateStarAnimation);

        for (int i = 0; i < numberOfStar; i++)
        {
            Star star = stars[i];
            star.FillStar();
            SoundManager.Instance.PlaySoundSFX(SoundManager.WinStar.audioClip, SoundManager.WinStar.volume);
            uiScreenShake.Shake();
            yield return new WaitUntil(() => star.animScriptScale.endAnim);
            yield return new WaitForSeconds(delayBetweenStarAnimation);
        }
    }
}

public enum Medal
{
    none,
    bronze,
    silver,
    gold
}
