using UnityEngine;
using UnityEngine.InputSystem;

public class LocalLevelManager : MonoBehaviour
{
    [SerializeField] private Transform startPlayerPoint;
    [SerializeField] private InputActionReference restartInput;
    [SerializeField] private GameObject ftuePC;
    [SerializeField] private GameObject ftueMobile;
    [SerializeField] private FinishZone finishZone;

    private Player player;

    private void Start()
    {
        player = Player.Instance;
        GameEvents.OnRestartRequested += RestartLevel;
        StartLevel();

        restartInput.action.Enable();
        restartInput.action.performed += Restart_performed;

        GameEvents.StartWorld?.Invoke();

        ActivateFTUE();
    }

    private void ActivateFTUE()
    {
        bool mobile = GameManager.isMobile();

        ftueMobile.SetActive(mobile);
        ftuePC.SetActive(!mobile);
    }

    private void Restart_performed(InputAction.CallbackContext obj)
    {
        RestartLevel();
    }

    public void StartLevel()
    {
        player.Activate();
        player.transform.position = startPlayerPoint.position;
        player.Restart();
    }

    private void RestartLevel()
    {
        if (!finishZone.levelFinished)
            StartLevel();
    }

    private void OnDestroy()
    {
        GameEvents.OnRestartRequested -= RestartLevel;
        restartInput.action.performed -= Restart_performed;
    }
}
