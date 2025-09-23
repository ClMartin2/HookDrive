using UnityEngine;

public class FinishZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        GameEvents.EndScene?.Invoke();
    }
}
