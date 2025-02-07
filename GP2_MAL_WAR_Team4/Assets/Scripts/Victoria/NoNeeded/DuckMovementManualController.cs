using UnityEngine;

public class DuckMovementManualController : MonoBehaviour
{
    private float speed;
    private float rotationSpeed;
    private InputsScript input;
    private Vector3 moveDirection;
    private const float isometricAngle = 45f;

    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask obstacleLayer;

    void Start()
    {
        input = GetComponent<InputsScript>();
        speed = 4f;
        rotationSpeed = 180f;
    }

    void Update()
    {
        HandleInput();
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void HandleInput()
    {
        float horizontal = input.move.x;
        float vertical = input.move.y;


        Vector3 isometricInput = new Vector3(horizontal, 0, vertical);
        Quaternion isometricRotation = Quaternion.Euler(0, isometricAngle, 0);
        moveDirection = isometricRotation * isometricInput;


    }

    private void HandleMovement()
    {
        if (moveDirection.magnitude > 0)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            float distanceToTravel = speed * Time.deltaTime;
            RaycastHit hit;

            if (Physics.Raycast(transform.position, transform.forward, out hit, distanceToTravel, groundLayer | obstacleLayer))
            {
                distanceToTravel = hit.distance; 
            }
            else
            {
                transform.Translate(transform.forward * distanceToTravel, Space.World);
            }
        }
    }
}