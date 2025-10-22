using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WorldClearedScreen : CustomScreen
{
    [SerializeField] private Image[] trophies;
    [SerializeField] private Star[] stars;
    [SerializeField] private TextMeshProUGUI txtLevelCleared;
    [SerializeField] private Image background;
    [SerializeField] private string[] nameWorlds;

    [SerializeField] private Sprite BronzeMedal;
    [SerializeField] private Sprite SilverMedal;
    [SerializeField] private Sprite GoldMedal;

    [SerializeField] private Color[] color;

    private AnimScriptScale animScriptScaleTxtWorld;

    private void Start()
    {
        animScriptScaleTxtWorld = txtLevelCleared.GetComponent<AnimScriptScale>();
        Hide();
    }

    public override void Show()
    {
        base.Show();

        //Faire toutes les anims puis passé au monde suivant direct 
        //screenshake
        //animé les etoiles après qu'elle ait apparu
        //mettre une animation de lettre qui bouge 
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
        SetColorBackground(color1, color2);
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

        for (int i = 0; i < numberOfStar; i++)
        {
            Star star = stars[i];
            star.FillStar();
            yield return new WaitUntil(() => star.animScriptScale.endAnim);
        }
    }
}

public enum Medal
{
    bronze,
    silver,
    gold
}
