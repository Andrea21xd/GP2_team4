using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform player; 
    [SerializeField] private Vector3 offset = new Vector3(0, 10, -10);
    [SerializeField] private float smoothSpeed = 0.125f;

    private void LateUpdate()
    {
        if (player == null) return; 

        Vector3 targetPosition = player.position + offset;

        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed);
    }
}
