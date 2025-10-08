using UnityEngine;

public class FinishZone : MonoBehaviour
{
    public bool levelFinished { get; private set; } = false;

    private void OnTriggerEnter(Collider other)
    {
        levelFinished = true;
        GameEvents.EndScene?.Invoke();
    }
}
