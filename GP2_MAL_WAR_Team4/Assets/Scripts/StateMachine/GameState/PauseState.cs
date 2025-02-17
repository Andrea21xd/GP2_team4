using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseState : GameState
{
    public UIMenu UIMenu;
    public CameraScript camera;

    public override void enterState()
    {
        base.enterState();
        int currentScene = SceneManager.GetActiveScene().buildIndex;

        switch (currentScene)
        {
            case 0:
                break;
            case 1:
                camera.LockCursor();
                UIMenu.ClosePause();
                break;
            case 2:
                UIMenu.ClosePause();
                break;
        }

        Time.timeScale = 0f;
        Debug.Log("Pause State");

        camera.startCamera = false;
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
        //Time.timeScale = 1f;
        //UIMenu.ClosePause();
        //camera.LockCursor();
        myStateMachine.SwitchState<PlayingState>();
    }

    public override void fixedUpdateState()
    {
        base.fixedUpdateState();
    }
    
    public override void exitState()
    {
        base.exitState();
        camera.startCamera = true;
        Time.timeScale = 1f;


    }
}
