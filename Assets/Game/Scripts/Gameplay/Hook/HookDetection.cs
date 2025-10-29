using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookDetection : MonoBehaviour
{
    [SerializeField] private Collider _collider;
    [SerializeField] private Transform playerTransform;

    [HideInInspector]
    public List<HookPoint> hookPoints { get; private set; } = new();

    private void Update()
    {
        transform.position = playerTransform.position;
    }

    public void Restart()
    {
        hookPoints.Clear();
        _collider.enabled = false;
        StartCoroutine(delayReactivation());    
    }

    private IEnumerator delayReactivation()
    {
        yield return new WaitForEndOfFrame();
        _collider.enabled = true;
        yield return null;
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
