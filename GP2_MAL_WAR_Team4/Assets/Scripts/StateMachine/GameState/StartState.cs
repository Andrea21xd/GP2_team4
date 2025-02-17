using UnityEngine;
using UnityEngine.SceneManagement;

public class StartState : GameState
{
    public override void enterState()
    {
        base.enterState();
        
    }




    public override void updateState()
    {
        base.updateState();
        int currentScene = SceneManager.GetActiveScene().buildIndex;
        if(currentScene == 0)
        {
            myStateMachine.SwitchState<PlayingState>();
        }
    }

    public override void exitState()
    {
        PlayerPrefs.SetInt("EndGame", 0);
        PlayerPrefs.SetInt("Chapter", 1);
        PlayerPrefs.SetInt("enemyKey", 1);
        PlayerPrefs.Save();
        base.exitState();
    }
}
