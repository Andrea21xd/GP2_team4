using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraScript : MonoBehaviour
{
    // transform.position  = Quaternion.Euler(cameraview) * transform.position;
    // Vector3 = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    // mousePos.z = 0;
    // [SerializeField] private Vector3 cameraview = new Vector3(0, 0);

    public Transform target; // The object the camera will orbit around
    public float sensitivity = 2.0f; // Mouse sensitivity
    public float distance = 5.0f; // Default distance from the target
    public float zoomSpeed = 2.0f; // Speed of zooming in/out
    public float maxDistance = 13.0f; // Maximum zoom distance
    public float minDistance = 2.0f; // Minimum zoom distance
    public float maxVerticalLimit = 40f; // Maximum vertical rotation limit
    public float minVerticalLimit = 40; // Minimum vertical rotation limit

    private float rotationX = 0f;
    private float rotationY = 0f;

    private void Start()
    {
        if (target == null)
        {
            Debug.LogError("No target assigned");
            return;
        }

        Vector3 angles = transform.eulerAngles;
        rotationX = angles.x;
        rotationY = angles.y;

        LockCursor();
    }

    private void Update()
    {
        if (target == null) return;

        RoteraKameran();
        ZoomaKameran();
    }


    void RoteraKameran()
    {

        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

        rotationY += mouseX;
        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, minVerticalLimit, maxVerticalLimit);

        Quaternion rotation = Quaternion.Euler(rotationX, rotationY, 0);
        Vector3 position = target.position - (rotation * Vector3.forward * distance);

        transform.rotation = rotation;
        transform.position = position;

    }


    void ZoomaKameran()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
        distance = Mathf.Clamp(distance - scroll, minDistance, maxDistance);
    }

    public Quaternion PlanarRotation => Quaternion.Euler(0, rotationY, 0);

    public Quaternion GetPlanarRotation()
    {
        return Quaternion.Euler(0, rotationY, 0);
    }


    public void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
    }
}       