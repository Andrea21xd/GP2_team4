using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    private InputsScript input;
    private CharacterController controller;
    public Animator Duck;

    private float speed;
    private float rotationSmoothTime;
    private float rotationSpeed;
    private float targetRotation;
    private Quaternion targetRotation1;
    private float rotationVelocity;
    private const float gravity = -9.8f;

    [HideInInspector]
    public bool PlayerMovementAllowed = true;

    public VectorValue playerMemory;
    public VectorValue playerStartMemory;
    
    CameraScript CameraScript;


    private void Awake()
    {    
        CameraScript = Camera.main.GetComponent<CameraScript>();
    }

    void Start()
    {
        if (PlayerPrefs.GetInt("Start") == 0)
        {
            transform.position = playerStartMemory.initialValue;
            PlayerPrefs.SetInt("Start", 1);
        }
        else
        {
            transform.position = playerMemory.initialValue;
        }
        Duck = GetComponent<Animator>();
        input = GetComponent<InputsScript>();
        controller = GetComponent<CharacterController>();
        speed = 6f;
        rotationSmoothTime = 0.1f;
        rotationSpeed = 500f;
        Duck.SetBool("Idle", true);
    }



    private void Update()
    {
        if (PlayerMovementAllowed)
        {
            HanteraRörelse();
        }
    }


    private void HanteraRörelse()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        float moveAmount = Mathf.Abs(h) + Mathf.Abs(v);

        var moveInput = (new Vector3(h, 0, v)).normalized;

        var moveDir = CameraScript.PlanarRotation * moveInput;


        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation1,
        rotationSpeed * Time.deltaTime);
     
        Vector3 gravity2 = new Vector3(0f, gravity, 0f);
        
        if (moveAmount > 0)
        {
            
            controller.Move(moveDir * speed * Time.deltaTime);
            controller.Move(gravity2 * Time.deltaTime);
            // transform.position += moveDir * speed * Time.deltaTime;
            targetRotation1 = Quaternion.LookRotation(moveDir);
        }

        Duck.SetBool("Idle", false);
        Duck.SetBool("Running", true);

        if (input.move == Vector2.zero)
        {
            Duck.SetBool("Running", false);
            Duck.SetBool("Idle", true);
            // Debug.Log("Is idle"); 
        }
    }


    private void daylightsavings() 
    {
        if (input.move != Vector2.zero)
        {
            targetRotation = Mathf.Atan2(input.move.x, input.move.y) * Mathf.Rad2Deg;
            float smoothRotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref rotationVelocity, rotationSmoothTime);
            transform.rotation = Quaternion.Euler(0.0f, smoothRotation, 0.0f);
            Vector3 targetDirection = Quaternion.Euler(0.0f, targetRotation, 0.0f) * Vector3.forward;
            Vector3 velocity = targetDirection.normalized * speed + new Vector3(0f, gravity, 0f);
            controller.Move(velocity * Time.deltaTime);


            // Debug.Log("Is moving");

            Duck.SetBool("Idle", false);
            Duck.SetBool("Running", true);

        }

        if (input.move == Vector2.zero)
        {
            Duck.SetBool("Running", false);
            Duck.SetBool("Idle", true);
            // Debug.Log("Is idle"); 
        }
    }
}
