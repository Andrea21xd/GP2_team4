using UnityEngine;

public class EndState : GameState
{
    public GameObject winPanel;


    public override void enterState()
    {
        base.enterState();
        if(winPanel == null) { return; }
        winPanel.SetActive(true);
    }

    public override void updateState()
    {
        base.updateState();
    }

    public override void exitState()
    {
        base.exitState();
        if (winPanel == null) { return; }
        winPanel.SetActive(false);
    }
}
