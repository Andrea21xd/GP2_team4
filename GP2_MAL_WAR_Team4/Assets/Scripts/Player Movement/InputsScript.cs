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
}
