using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Hud : CustomScreen
{
    [SerializeField] private TextMeshProUGUI[] txtsLevelName;
    [SerializeField] private RectTransform[] controlPanels;
    [SerializeField] private HorizontalLayoutGroup[] horizontalLayoutGroups;
    [SerializeField] private Button[] optionsButton;

    [Header("Hook button")]
    [SerializeField] private GameObject hookButtonHorizontal;
    [SerializeField] private GameObject hookButtonVertical;

    [Header("Shop button")]
    [SerializeField] private GameObject shopButtonHorizontal;
    [SerializeField] private GameObject shopButtonVertical;

    [Header("Settings Button Control")]
    [SerializeField] private float widthPanelButtonControlHorizontal = 750;
    [SerializeField] private float widthPanelButtonControlVertical = 500;
    [SerializeField] private float spacingHorizontal = 70;
    [SerializeField] private float spacingVertical = 50;

    [Header("Top Layout")]
    [SerializeField] private GameObject layoutTopVertical;
    [SerializeField] private GameObject layoutTopHorizontal;

    [Header("Debug")]
    [SerializeField] private bool vertical;
    [SerializeField] private bool horizontal;

    private ControlButton[] controlsButton;
    private bool activateControlButton = true;
    private bool isVertical = false;

    private void Awake()
    {
        controlsButton = GetComponentsInChildren<ControlButton>(true);

        if (!GameManager.isMobile())
        {
            ActivateVerticalElement(false);
            ActivateControlButtons(false);
        }

        GameEvents.ChangeOrientation += OrientationChange;
        OrientationChange();
    }

    public void UpdateLevelName(string lvlName)
    {
        foreach (TextMeshProUGUI txtLevelName in txtsLevelName)
        {
            txtLevelName.text = lvlName;
        }
    }

    public void ActivateControlButtons(bool Activate)
    {
        activateControlButton = Activate;
        bool localActivate = GameManager.isMobile() == true ? Activate : false;

        foreach (var controlButton in controlsButton)
        {
            if (localActivate == false)
                controlButton.gameObject.SetActive(localActivate);
            else
            {
                if (controlButton.CompareTag("Hook"))
                {
                    hookButtonVertical.SetActive(isVertical);
                    hookButtonHorizontal.SetActive(!isVertical);
                }
                else
                    controlButton.gameObject.SetActive(localActivate);
            }

        }
    }

    public void ActivateOptionButtons(bool activate)
    {
        foreach (var optionButton in optionsButton)
        {
            optionButton.interactable = activate;
        }
    }

    private void OrientationChange()
    {
        if (GameManager.isMobile() && activateControlButton)
        {
#if UNITY_EDITOR

            isVertical = Screen.orientation == ScreenOrientation.Portrait || Screen.orientation == ScreenOrientation.PortraitUpsideDown || vertical == true;

            if (horizontal == true)
                isVertical = false;
#else
           isVertical = Screen.orientation == ScreenOrientation.Portrait || Screen.orientation == ScreenOrientation.PortraitUpsideDown;
#endif

            ActivateVerticalElement(isVertical);

            foreach (RectTransform controlPanel in controlPanels)
            {
                Vector2 size = controlPanel.sizeDelta;
                size.x = isVertical ? widthPanelButtonControlVertical : widthPanelButtonControlHorizontal;
                controlPanel.sizeDelta = size;
            }

            foreach (HorizontalLayoutGroup horizontalLayoutGroup in horizontalLayoutGroups)
            {
                horizontalLayoutGroup.spacing = isVertical ? spacingVertical : spacingHorizontal;
            }

        }
    }

    private void ActivateVerticalElement(bool isVertical)
    {
        hookButtonVertical.SetActive(isVertical);
        hookButtonHorizontal.SetActive(!isVertical);

        shopButtonVertical.SetActive(isVertical);
        shopButtonHorizontal.SetActive(!isVertical);

        layoutTopVertical.SetActive(isVertical);
        layoutTopHorizontal.SetActive(!isVertical);
    }
}
