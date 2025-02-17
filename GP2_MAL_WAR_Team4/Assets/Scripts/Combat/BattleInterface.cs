using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Button = UnityEngine.UI.Button;


public class BattleInterface : MonoBehaviour 
{
    public Button attackButton;
    public Button evasionButton;
    public Button runButton;
    public Button specialButton;
    public BattleSystem battleSystem;
    public TMP_Text dialogue;
    public TMP_Text damageText;


    void Start()

    {
        battleSystem = FindFirstObjectByType<BattleSystem>();
    }

    public void OnAttackButton()
    {
        if (battleSystem.state != BattleState.PLAYERTURN)
            return;
        battleSystem.playerAnimator.Play("Quack");

        battleSystem.evasionOnButtonChance = battleSystem.playerUnit.evasionChance;

        battleSystem.StartCoroutine(battleSystem.PlayerAttack());
        DeactivateButtons();
    }
    public void OnEvasionButton()
    {    
        if (battleSystem.state != BattleState.PLAYERTURN)
            return;
        battleSystem.playerAnimator.Play("Waddle");

        battleSystem.evasionOnButtonChance = battleSystem.playerUnit.evasionChance * 10;

        battleSystem.StartCoroutine(battleSystem.PlayerEvasion());
        DeactivateButtons();
    }
    public void OnRunAway()
    {
        Debug.Log("Changing to: " + battleSystem.sceneLoader);
        battleSystem.playerAnimator.Play("Run");
        StartCoroutine(ChangeScene());
    }
    public void OnSpecialAttack()
    {
        battleSystem.StartCoroutine(battleSystem.ExecutePlayerAttack(999));
        DeactivateButtons();
    }

    public void DeactivateButtons()
    {
        attackButton.interactable = false;
        evasionButton.interactable = false;
        runButton.interactable = false;
        specialButton.interactable = false;
    }
    public void ActivateButtons()
    {
        attackButton.interactable = true;
        evasionButton.interactable = true; 
        runButton.interactable = true;
    }
    public void SetDialogue(string text)
    {
        dialogue.text = text;
    }

    public void SetDamageDialogue(bool active, int damage = 0)
    {
        damageText.text = "Damage:" + damage;
        damageText.gameObject.SetActive(active);
    }

    public void HasFinisher(int enemyCurrentHP, int enemyMaxHP)
    {
        if (((float)enemyMaxHP * 0.5f) >= (float)enemyCurrentHP)
        {
            specialButton.interactable = true;        
        }
    }
    IEnumerator ChangeScene()
    {
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene(battleSystem.sceneLoader);
    } 
}
