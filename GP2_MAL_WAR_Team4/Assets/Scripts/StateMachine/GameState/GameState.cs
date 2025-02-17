using UnityEngine;

public class GameState : State
{

    protected GameManager myStateMachine;

    public void SetParentStateMachine()
    {
        myStateMachine = GameManager.Instance;
    }

    public virtual void Start()
    {
        SetParentStateMachine();
    }

    public override void enterState()
    {
        base.enterState();
    }

    public override void updateState()
    {
        base.updateState();
    }

    public override void fixedUpdateState()
    {
        base.fixedUpdateState();
    }

    public override void exitState()
    {
        base.exitState();
    }
}
