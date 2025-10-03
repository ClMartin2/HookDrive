using System.Collections.Generic;
using UnityEngine;

public class HookDetection : MonoBehaviour
{
    [HideInInspector]
    public List<HookPoint> hookPoints { get; private set; } = new();

    public void Restart()
    {
        hookPoints.Clear();
    }

    private void OnTriggerEnter(Collider other)
    {
        hookPoints.Add(other.GetComponent<HookPoint>());
    }

    private void OnTriggerExit(Collider other)
    {
        hookPoints.Remove(other.GetComponent<HookPoint>());
    }
}
