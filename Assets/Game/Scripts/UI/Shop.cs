using System;
using UnityEngine;
using UnityEngine.UI;

public class Shop : CustomScreen
{
    [SerializeField] private Button btnCloseShop;
    [SerializeField] private RectTransform containerShop;
    [SerializeField] private Vector2 sizeVerticalContainerShop;

    private Vector2 sizeHorizontalContainerShop;

    public static Shop Instance;

    private void Awake()
    {
        if (Instance != null)
            Destroy(gameObject);
        else
            Instance = this;

        btnCloseShop.onClick.AddListener(Hide);
        GameEvents.ChangeOrientation += ChangeOrientation;
        sizeHorizontalContainerShop = containerShop.sizeDelta;
    }

    private void Start()
    {
        Hide();
    }
    private void ChangeOrientation()
    {
        if (!GameManager.isMobile())
            return;

        bool isVertical = Screen.orientation == ScreenOrientation.Portrait || Screen.orientation == ScreenOrientation.PortraitUpsideDown;

        if (isVertical)
        {
            containerShop.sizeDelta = sizeVerticalContainerShop;
        }
        else
        {
            containerShop.sizeDelta = sizeHorizontalContainerShop;
        }
    }
}
