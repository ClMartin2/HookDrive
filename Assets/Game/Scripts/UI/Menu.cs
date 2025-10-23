using System.Collections.Generic;
using UnityEngine;

public class Menu : CustomScreen
{
    [SerializeField] PanelWorldSelection[] allPanelWorldSelection;

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
