using TMPro;
using UnityEngine;

public class Hud : CustomScreen
{
    [SerializeField] private TextMeshProUGUI txtLevelName;

    private ControlButton[] controlsButton;

    public void UpdateLevelName(string lvlName)
    {
        txtLevelName.text = lvlName;
    }

    private void Awake()
    {
        if (!GameManager.isMobile())
        {
            controlsButton = GetComponentsInChildren<ControlButton>();

            foreach (var controlButton in controlsButton)
            {
                controlButton.gameObject.SetActive(false);
            }
        }
    }
}
