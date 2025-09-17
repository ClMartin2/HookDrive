using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float smoothTime = 0.0001f;

    [ContextMenu("Reset To player position")]
    private void ResetToPlayerPosition()
    {
        transform.position = player.position;
    }

    private void FixedUpdate()
    {
        Vector3 velocity = Vector3.zero;
        transform.position = Vector3.SmoothDamp(transform.position, player.position, ref velocity, smoothTime);
    }
}
