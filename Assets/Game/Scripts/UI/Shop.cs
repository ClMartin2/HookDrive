using System;
using UnityEngine;
using UnityEngine.UI;

public class Shop : CustomScreen
{
    [SerializeField] private Button[] btnsCloseShop;
    [SerializeField] private RectTransform containerShop;
    [SerializeField] private Vector2 sizeVerticalContainerShop;

    public static Shop Instance;

    public bool EndScene = false;

    private Vector2 sizeHorizontalContainerShop;

    private void Awake()
    {
        if (Instance != null)
            Destroy(gameObject);
        else
            Instance = this;

        foreach (Button btnCloseShop in btnsCloseShop)
        {
            btnCloseShop.onClick.AddListener(HideBtnCloseShop);
        }

        GameEvents.ChangeOrientation += ChangeOrientation;
        sizeHorizontalContainerShop = containerShop.sizeDelta;
    }

    public override void Hide()
    {
        base.Hide();

        GameEvents.HideShop?.Invoke(EndScene);

        foreach (Button btnCloseShop in btnsCloseShop)
        {
            btnCloseShop.gameObject.SetActive(false);
        }
    }

    public override void Show()
    {
        base.Show();

        GameEvents.ShowShop?.Invoke();

        foreach (Button btnCloseShop in btnsCloseShop)
        {
            btnCloseShop.gameObject.SetActive(true);
        }
    }

    private void HideBtnCloseShop()
    {
        EndScene = false;
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
