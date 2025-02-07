using UnityEngine;

public class PauseState : GameState
{
    public UIMenu UIMenu;
    public CameraScript camera;

    public override void enterState()
    {
        base.enterState();
        Time.timeScale = 0f;

        camera.UnlockCursor();
        UIMenu.OpenPause();
    }

    public override void updateState()
    {
        base.updateState();
        if (InputsScript.instance.MenuOpenCloseInput)
        {
            SwitchingSceneUI();
        }
    }
    
    public void SwitchingSceneUI()
    {
        myStateMachine.SwitchState<PlayingState>();
    }

    public override void fixedUpdateState()
    {
        base.fixedUpdateState();
    }
    
    public override void exitState()
    {
        base.exitState();
        Time.timeScale = 1f;
        camera.LockCursor();
        UIMenu.ClosePause();
    }
}
