using UnityEngine;
using UnityEngine.InputSystem;

public class InputsScript : MonoBehaviour
{
    public Vector2 move;

#if ENABLE_INPUT_SYSTEM

    public void OnMove(InputValue value)
    {
        MoveInput(value.Get<Vector2>());
    }

    public void OnInteract(InputValue value)
    {
        InteractInput();
    }

#endif

    public void MoveInput(Vector2 newMoveDirection)
    {
        move = newMoveDirection;
    }

    public void InteractInput()
    {
        GameEventsManager.Instance.miscEvents.InteractionPressed();
    }


    //Adding of more inputs for outside of events
    public static InputsScript instance;

    public bool MenuOpenCloseInput { get; private set; }

    private PlayerInput _playerInput;

    private InputAction _menuOpenCloseAction;

    private void Awake()
    {
        if (instance == null) { instance = this; }

        _playerInput = GetComponent<PlayerInput>();
        if (_playerInput == null)
        {
            throw new System.NullReferenceException("Player input is null");

            // or
            //Debug.Log("Player input is null");
            //return;
        }

        SetupInputActions();
    }

    private void Update()
    {
        UpdateInputs();
    }

    private void SetupInputActions()
    {
        Debug.Log($"Menu action == null : {_menuOpenCloseAction == null}");
        Debug.Log($"Player input == null {_playerInput== null}");
        _menuOpenCloseAction = _playerInput.actions["MenuOpenClose"];
    }

    private void UpdateInputs()
    {
        MenuOpenCloseInput = _menuOpenCloseAction.WasPressedThisFrame();
    }
}
