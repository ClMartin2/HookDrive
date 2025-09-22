using UnityEngine;

public class Hud : CustomScreen
{
    [SerializeField] private ControlButton[] controlsButton;

    private void Awake()
    {
        if (GameManager.isMobile())
        {
            foreach (var controlButton in controlsButton)
            {
                controlButton.gameObject.SetActive(false);
            }
        }
    }
}
