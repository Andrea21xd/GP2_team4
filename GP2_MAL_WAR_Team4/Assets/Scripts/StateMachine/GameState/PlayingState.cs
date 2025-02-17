using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayingState : GameState
{
    public UIMenu UIMenu;
    public CameraScript camera;

    public override void enterState()
    {
        base.enterState();
        Time.timeScale = 1f;
        Debug.Log("Playing State");

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


    }

    public override void updateState()
    {
        base.updateState();
        if (InputsScript.instance.MenuOpenCloseInput)
        {
            SwitchingSceneUI();
        }
        if (PlayerPrefs.GetInt("EndGame") == 1)
        {
            myStateMachine.SwitchState<EndState>();
        }
    }
    public void SwitchingSceneUI()
    {
        //Time.timeScale = 0f;
        //UIMenu.OpenPause();
        //camera.UnlockCursor();
        myStateMachine.SwitchState<PauseState>();
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
