using System;
using System.Collections.Generic;
using UnityEngine;

public class Menu : CustomScreen
{
    [SerializeField] PanelWorldSelection[] allPanelWorldSelection;
    [SerializeField] RectTransform selectWorld;

    [SerializeField] private Vector2 sizeVerticalPanelWorldSelection;
    [SerializeField] private Vector2 sizeVerticalSelectWorld;

    private Vector2 sizeHorizontalPanelWorldSelection;
    private Vector2 sizeHorizontalSelectWorld;

    private void Awake()
    {
        GameEvents.ChangeOrientation += OrientationChange;

        foreach (PanelWorldSelection panelWorldSelection in allPanelWorldSelection) {
            sizeHorizontalPanelWorldSelection = panelWorldSelection.GetComponent<RectTransform>().sizeDelta;
        }

        sizeHorizontalSelectWorld = selectWorld.sizeDelta;
    }

    private void OrientationChange()
    {
        bool isVertical = Screen.orientation == ScreenOrientation.Portrait || Screen.orientation == ScreenOrientation.PortraitUpsideDown;

        if (isVertical) {
            SetSizePanelWorldSelection(sizeVerticalPanelWorldSelection);
            SetSizeAndAnchorSelectWorld(sizeVerticalSelectWorld, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));
        }
        else
        {
            SetSizePanelWorldSelection(sizeHorizontalPanelWorldSelection);
            SetSizeAndAnchorSelectWorld(sizeHorizontalSelectWorld, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f));

        }
    }

    private void SetSizePanelWorldSelection(Vector2 size)
    {
        foreach (PanelWorldSelection panelWorldSelection in allPanelWorldSelection)
        {
            panelWorldSelection.GetComponent<RectTransform>().sizeDelta = size;
        }
    }
    private void SetSizeAndAnchorSelectWorld(Vector2 size, Vector2 anchorMax, Vector2 anchorMin, Vector2 pivot)
    {
        selectWorld.sizeDelta = size;
        selectWorld.anchorMax = anchorMax;
        selectWorld.anchorMin = anchorMin;
        selectWorld.pivot = pivot;
    }

    public override void Show()
    {
        base.Show();

        SoundManager.Instance.SetMusicInUI(true);
    }

    public override void Hide()
    {
        base.Hide();

        SoundManager.Instance.SetMusicInUI(false);
    }

    public void SetLock(Dictionary<WorldData, bool> unlocksWorldData)
    {
        foreach (var panelWorldSelection in allPanelWorldSelection)
        {
            WorldData worldData = panelWorldSelection.worldData;

            if (worldData == null || !unlocksWorldData.ContainsKey(worldData))
                continue;

            bool isUnlcok = unlocksWorldData[worldData];

            if (isUnlcok)
                panelWorldSelection.UnLock();
            else
                panelWorldSelection.Lock();
        }
    }
}
