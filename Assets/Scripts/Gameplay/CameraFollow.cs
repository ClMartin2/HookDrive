using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float smoothTime = 0.0001f;

    private Vector3 velocity = Vector3.zero;

    [ContextMenu("Reset To player position")]
    private void ResetToPlayerPosition()
    {
        transform.position = player.position;
    }

    private void FixedUpdate()
    {
        if (!player.gameObject.activeSelf)
            return;

        Vector3 playerPosition = player.position;
        Vector3 targetPosition = Vector3.SmoothDamp(transform.position, playerPosition, ref velocity, smoothTime);

        transform.position = new Vector3(targetPosition.x, playerPosition.y, targetPosition.z);
    }
}
