using UnityEngine;

public class PlayingState : GameState
{
    public override void enterState()
    {
        base.enterState();
        Debug.Log("Playing State");

        Time.timeScale = 1f;
    }

    public override void updateState()
    {
        //base.updateState();
        //if (InputsScript.instance.MenuOpenCloseInput)
        //{
        //    myStateMachine.SwitchState<PauseState>();
        //}
    }

    public override void fixedUpdateState()
    {
        base.fixedUpdateState();
    }

    public override void exitState()
    {
        base.exitState();
        Time.timeScale = 0f;
    }
}
