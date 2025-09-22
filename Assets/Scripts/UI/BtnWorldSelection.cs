using UnityEngine;

public class BtnWorldSelection : CustomButton
{
    [SerializeField] WorldData worldData;

    protected override void OnClick()
    {
        base.OnClick();
        GameEvents.LoadWorld?.Invoke(worldData);
    }
}
