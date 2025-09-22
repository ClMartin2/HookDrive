using UnityEngine;

public class Hud : CustomScreen
{
    private ControlButton[] controlsButton;

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
