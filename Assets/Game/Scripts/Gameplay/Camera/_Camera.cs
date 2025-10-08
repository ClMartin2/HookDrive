using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class _Camera : MonoBehaviour
{
    [SerializeField] private float endDistanceZoom = 8f;
    [SerializeField] private float timeToZoom = 0.45f;
    [SerializeField] private AnimationCurve curveZoom;

    public static _Camera Instance;

    private CinemachinePositionComposer cinemachinePositionComposer;
    private float startDistance;
    private float counterZoom;
    private Coroutine coroutineZoom;

    public void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        cinemachinePositionComposer = GetComponent<CinemachinePositionComposer>();
        startDistance = cinemachinePositionComposer.CameraDistance;
    }

    public void Zoom()
    {
        counterZoom = 0;
        coroutineZoom = StartCoroutine(_Zoom());
    }

    public void DeZoom()
    {
        cinemachinePositionComposer.CameraDistance = startDistance;

        if(coroutineZoom != null)
        {
            StopCoroutine(coroutineZoom);
            coroutineZoom = null;
        }
    }

    private IEnumerator _Zoom()
    {
        while (counterZoom < timeToZoom)
        {
            counterZoom += Time.deltaTime;
            float delta = curveZoom.Evaluate(counterZoom / timeToZoom);
            cinemachinePositionComposer.CameraDistance = Mathf.Lerp(startDistance, endDistanceZoom, delta);

            yield return null;
        }

        cinemachinePositionComposer.CameraDistance = endDistanceZoom;

        yield return null;
    }
}
