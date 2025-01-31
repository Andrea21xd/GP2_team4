using UnityEngine;
using Button = UnityEngine.UI.Button;
using UnityEngine.SceneManagement;


public class BattleInterface : MonoBehaviour
{
    public Button attackButton;
    public Button evasionButton;
    public Button runButton;
    public BattleSystem battleSystem;

    void Start()

    {
        battleSystem = FindObjectOfType<BattleSystem>();
    }

    public void OnAttackButton()
    {
        if (battleSystem.state != BattleState.PLAYERTURN)
            return;
        battleSystem.evationOnButtonChance = battleSystem.playerUnit.evasionChance;

        battleSystem.StartCoroutine(battleSystem.PlayerAttack());
        DeactivateButtons();
    }
    public void OnEvationButton()
    {
        if (battleSystem.state != BattleState.PLAYERTURN)
            return;
        battleSystem.evationOnButtonChance = battleSystem.playerUnit.evasionChance * 10;

        battleSystem.StartCoroutine(battleSystem.PlayerEvation());
        DeactivateButtons();
    }
    public void OnRunAway()
    {
        Debug.Log("Changing to: " + battleSystem.sceneLoader);

        SceneManager.LoadScene(battleSystem.sceneLoader);
    }

    public void DeactivateButtons()
    {
        attackButton.interactable = false;
        evasionButton.interactable = false;
        runButton.interactable = false;
    }
   public void ActivateButtons()
    {
        attackButton.interactable = true;
        evasionButton.interactable = true;
        runButton.interactable = true;
    }
}
