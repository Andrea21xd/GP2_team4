using UnityEngine;

public class DuckMovementController : MonoBehaviour
{

    private InputsScript input;
    private CharacterController controller;

    private float speed;
    private float rotationSmoothTime;


    private float targetRotation;
    private float rotationVelocity;
    private const float gravity = -9.8f;
    private const float isometricAngle = 45f;

    void Start()
    {
        input = GetComponent<InputsScript>();
        controller = GetComponent<CharacterController>();
        speed = 6f;
        rotationSmoothTime = 0.1f;
    }

    void Update()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        if (input.move != Vector2.zero)
        {
            targetRotation = Mathf.Atan2(input.move.x, input.move.y) * Mathf.Rad2Deg;
            float smoothRotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref rotationVelocity, rotationSmoothTime);
            transform.rotation = Quaternion.Euler(0.0f, smoothRotation, 0.0f);

            Vector3 targetDirection = Quaternion.Euler(0.0f, targetRotation, 0.0f) * Vector3.forward;
            Vector3 velocity = targetDirection.normalized * speed + new Vector3(0f, gravity, 0f);

            controller.Move(velocity * Time.deltaTime);
        }
    }

    private void HandleIsometricMovement()
    {
        if (input.move != Vector2.zero)
        {
            Vector3 isometricInput = new Vector3(input.move.x, 0, input.move.y);
            Quaternion isometricRotation = Quaternion.Euler(0, isometricAngle, 0);
            Vector3 targetDirection = isometricRotation * isometricInput;

            targetRotation = Mathf.Atan2(targetDirection.x, targetDirection.z) * Mathf.Rad2Deg;
            float smoothRotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref rotationVelocity, rotationSmoothTime);
            transform.rotation = Quaternion.Euler(0.0f, smoothRotation, 0.0f);

            Vector3 velocity = targetDirection.normalized * speed + new Vector3(0f, gravity, 0f);
            controller.Move(velocity * Time.deltaTime);
        }
    }
}
