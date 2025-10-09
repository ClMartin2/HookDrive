using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Hud : CustomScreen
{
    [SerializeField] private TextMeshProUGUI[] txtsLevelName;
    [SerializeField] private RectTransform[] controlPanels;
    [SerializeField] private HorizontalLayoutGroup[] horizontalLayoutGroups;

    [Header("Hook button")]
    [SerializeField] private GameObject hookButtonHorizontal;
    [SerializeField] private GameObject hookButtonVertical;

    [Header("Settings Button Control")]
    [SerializeField] private float widthPanelButtonControlHorizontal = 750;
    [SerializeField] private float widthPanelButtonControlVertical = 500;
    [SerializeField] private float spacingHorizontal = 70;
    [SerializeField] private float spacingVertical = 50;

    [Header("Top Layout")]
    [SerializeField] private GameObject layoutTopVertical;
    [SerializeField] private GameObject layoutTopHorizontal;

    [Header("Debug")]
    [SerializeField] private bool test;

    private ControlButton[] controlsButton;
    private bool activateControlButton = true;

    private void Awake()
    {
        controlsButton = GetComponentsInChildren<ControlButton>();

        if (!GameManager.isMobile())
        {
            ActivateVerticalElement(false);
            ActivateControlButtons(false);
        }
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
            controlButton.gameObject.SetActive(localActivate);
        }
    }

    private void Update()
    {
        if (GameManager.isMobile() && activateControlButton)
        {
            bool isVertical = Screen.orientation == ScreenOrientation.Portrait || Screen.orientation == ScreenOrientation.PortraitUpsideDown || test == true;
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

        layoutTopVertical.SetActive(isVertical);
        layoutTopHorizontal.SetActive(!isVertical);
    }
}
