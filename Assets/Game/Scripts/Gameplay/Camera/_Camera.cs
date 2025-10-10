using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class _Camera : MonoBehaviour
{
    [SerializeField] private float endDistanceZoom = 8f;
    [SerializeField] private float timeToZoom = 0.45f;
    [SerializeField] private AnimationCurve curveZoom;
    [SerializeField] private float yVerticalRotationCam = -43.264f;
    [SerializeField] private float distanceVerticalCamera = 30;

    private float yHorizontalRotationCam;
    private float distanceHorizontalCamera;

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
    }

    private void Start()
    {
        startDistance = cinemachinePositionComposer.CameraDistance;
        distanceHorizontalCamera = startDistance;
        yHorizontalRotationCam = cinemachinePositionComposer.transform.rotation.eulerAngles.y;

        GameEvents.ChangeOrientation += OrientationChange;
        OrientationChange();
    }

    private void OrientationChange()
    {
        if (GameManager.isMobile())
        {
            bool isVertical = Screen.orientation == ScreenOrientation.Portrait || Screen.orientation == ScreenOrientation.PortraitUpsideDown;
            Vector3 cameraRotation = cinemachinePositionComposer.transform.rotation.eulerAngles;
            float newYRotation = isVertical ? yVerticalRotationCam : yHorizontalRotationCam;
            float newDistance = isVertical ? distanceVerticalCamera : distanceHorizontalCamera;
            startDistance = newDistance;

            cinemachinePositionComposer.CameraDistance = newDistance;
            cinemachinePositionComposer.transform.rotation = Quaternion.Euler(cameraRotation.x, newYRotation, cameraRotation.z);
        }
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
            cinemachinePositionComposer.CameraDistance = Mathf.LerpUnclamped(startDistance, endDistanceZoom, delta);

            yield return null;
        }

        cinemachinePositionComposer.CameraDistance = endDistanceZoom;

        yield return null;
    }
}
