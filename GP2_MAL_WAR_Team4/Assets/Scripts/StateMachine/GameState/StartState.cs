using UnityEngine;

public class StartState : GameState
{
    public override void enterState()
    {
        base.enterState();
        
    }

    public override void updateState()
    {
        base.updateState();
        myStateMachine.SwitchState<PlayingState>();
    }

    public override void exitState()
    {
        base.exitState();
    }
}
