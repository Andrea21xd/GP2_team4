using UnityEngine;
using System.Collections.Generic;

public class StateMachine : MonoBehaviour
{
    public List<State> states = new List<State>();
    public State currentState = null;

    public virtual void SwitchState<T>() where T : State
    {
        foreach (State s in states)
        {
            if (s.GetType() == typeof(T))
            {
                currentState?.exitState();
                currentState = s;
                currentState.enterState();
                return;
            }
        }
        Debug.LogWarning("State does not exist");
    }
    public void switchState<T>() where T : State
    {
        SwitchState<T>();
    }

    public void UpdateStateMachine()
    {
        currentState?.updateState();
    }

    public void FixedUpdateStateMachine()
    {
        currentState?.fixedUpdateState();
    }

}
