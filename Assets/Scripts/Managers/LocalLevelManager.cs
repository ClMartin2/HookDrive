using UnityEngine;

public class LocalLevelManager : MonoBehaviour
{
    [SerializeField] private Transform startPlayerPoint;

    private Player player;

    private void Start()
    {
        player = Player.Instance;
        GameEvents.OnRestartRequested += RestartLevel;
        StartLevel();
    }

    public void StartLevel()
    {
        player.Activate();
        player.transform.position = startPlayerPoint.position;
        player.Restart();
    }

    private void RestartLevel()
    {
        StartLevel();
    }

    private void OnDestroy()
    {
        GameEvents.OnRestartRequested -= RestartLevel;
    }
}
