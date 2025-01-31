using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;
using UnityEngine.SceneManagement;
using Unity.Collections;


public enum BattleState
{
    START,
    PLAYERTURN,
    ENEMYTURN,
    WON,
    LOST
}
public class BattleSystem : MonoBehaviour
{
    public GameObject playerPrefab;
    public GameObject enemyPrefab;

    private int randomChance;
    private int hitChanceAmount;
    [HideInInspector]
    public int evationOnButtonChance;
    private int heavyChanceAmount;

    [HideInInspector]
    public Unit playerUnit;
    [HideInInspector]
    public Unit enemyUnit;

    public TMP_Text dialogue;
    public TMP_Text critAttack;

    public BattleHUD playerHUD;
    public BattleHUD enemyHUD;

    public Transform playerBattleStation;
    public Transform enemyBattleStation;

    private bool isPlayerFaster = false;
    private bool isHeavyAttack = false;
    private bool executeHeavyAttack = false;
    public BattleInterface battleInterface;

    public BattleState state;

    [Header("=== Scene Loader Name ===")]
    public int sceneLoader;

    //**Script needs refactoring** 
    void Start()
    {
        state = BattleState.START;
        StartCoroutine(SetupBattle());
        battleInterface = FindObjectOfType<BattleInterface>();

    }


    IEnumerator SetupBattle()
    {
        GameObject myPlayerGO = Instantiate(playerPrefab, playerBattleStation);
        playerUnit = myPlayerGO.GetComponent<Unit>();

        GameObject enemyGo = Instantiate(enemyPrefab, enemyBattleStation);
        enemyUnit = enemyGo.GetComponent<Unit>();

        dialogue.text = " A wild " + enemyUnit.unitName + " approaches...";

        playerHUD.SetHUD(playerUnit);
        enemyHUD.SetHUD(enemyUnit);

        yield return new WaitForSeconds(3f);

        if (playerUnit.speed >= enemyUnit.speed)
        {
            isPlayerFaster = true;
            state = BattleState.PLAYERTURN;
            PlayerTurn();

        }
        else if(playerUnit.speed < enemyUnit.speed)
        {
            isPlayerFaster = false;
            state = BattleState.ENEMYTURN;
            StartCoroutine(EnemyTurn());
        }
    }


    public IEnumerator PlayerAttack()
    {
        if (isPlayerFaster || RandomChance(enemyUnit, playerUnit))
        {
            int damage = 0;
            if (CritChance(playerUnit))
            {
                critAttack.gameObject.SetActive(true);
                damage = playerUnit.critDamage;
            }
            else
            {
                damage = playerUnit.damage;
            }
            Debug.Log("player Damage" + damage);

            yield return ExecutePlayerAttack(damage);
        }
        else
        {
            dialogue.text = " I Ducked Up (miiiiiiiiiss)";
            yield return new WaitForSeconds(3f);
            critAttack.gameObject.SetActive(false);
            state = BattleState.ENEMYTURN;
            StartCoroutine(EnemyTurn());
        }
        if (isHeavyAttack)
        {

            executeHeavyAttack = true;
        }
    }
    IEnumerator ExecutePlayerAttack(int damage)
    {
        dialogue.text = " Duck Attacking ";
        bool isDead = enemyUnit.TakeDamage(damage);
        yield return new WaitForSeconds(3f);

        enemyHUD.SetHP(enemyUnit.currentHP);
        Debug.Log("enemy HP" + enemyUnit.currentHP);
        dialogue.text = " I Ducked him Up (hiiiiiiiiiit) ";
        critAttack.gameObject.SetActive(false);
        yield return new WaitForSeconds(2f);

        if (isDead)
        {
            state = BattleState.WON;
            EndBattle();
        }
        else
        {
            dialogue.text = enemyUnit.unitName + "'s turn...";
            yield return new WaitForSeconds(2f);
            state = BattleState.ENEMYTURN;
            StartCoroutine(EnemyTurn());
        }
    }

    void EndBattle()
    {
        if (state == BattleState.WON)
        {
            dialogue.text = "You won the battle!";
            SceneManager.LoadScene(1);
        }
        else if (state == BattleState.LOST)
        {
            dialogue.text = "You were defeated";
            SceneManager.LoadScene(1);
        }
    }
    bool RandomChance(Unit unitOne, Unit unitTwo)
    {

        hitChanceAmount = 100 - (unitOne.speed - unitTwo.speed) * 10;
        randomChance = Random.Range(0, 100);

        if (randomChance <= hitChanceAmount)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    bool CritChance(Unit unit)
    {
        randomChance = Random.Range(0, 100);

        if (randomChance <= unit.critAttackChance)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    void HeavyAttackChance()
    {
        if (!isHeavyAttack)
        {
            heavyChanceAmount = Random.Range(0, 100);
            int notHeavyChance = Random.Range(0, 100);


            if (notHeavyChance <= heavyChanceAmount)
            {
                isHeavyAttack = true;
            }
            else
            {
                isHeavyAttack = false;
            }
        }
    }

    
    IEnumerator EnemyTurn()
    {
        yield return new WaitForSeconds(2f);

        if (!isHeavyAttack)
        {

            HeavyAttackChance();
        }
        Debug.Log("executeHeavyAttack" + executeHeavyAttack);

        if (executeHeavyAttack)
        {
            if (ChanceOfEvasion(evationOnButtonChance))
            {
                int damage = 0;
                if (CritChance(enemyUnit))
                {
                    critAttack.gameObject.SetActive(true);
                    damage = enemyUnit.critDamage;
                }
                else
                {
                    damage = enemyUnit.damage;
                }

                Debug.Log("enemy Heavy Damage" + damage * 3);
                yield return ExecuteEnemyAttack(damage * 3);
                executeHeavyAttack = false;
                isHeavyAttack = false;

            }
            else
            {
                dialogue.text = "Enemy Landing The Attack!!";
                yield return new WaitForSeconds(2f);

                dialogue.text = "Duck Has Evaded the Attack!!";
                yield return new WaitForSeconds(3f);

                state = BattleState.PLAYERTURN;
                PlayerTurn();
                executeHeavyAttack = false;
                isHeavyAttack = false;
            }
        }
        else
        {
            Debug.Log("isHeavyAttack" + isHeavyAttack);

            if (isHeavyAttack)
            {
                dialogue.text = "Incoming Heavy Attack!!";
                yield return new WaitForSeconds(3f);
                state = BattleState.PLAYERTURN;
                PlayerTurn();
            }
            else
            {
                if (!isPlayerFaster || RandomChance(playerUnit, enemyUnit))
                {
                    int damage = 0;
                    if (CritChance(enemyUnit))
                    {
                        critAttack.gameObject.SetActive(true);
                        damage = enemyUnit.critDamage;
                    }
                    else
                    {
                        damage = enemyUnit.damage;
                    }
                    Debug.Log("enemy Damage" + damage);

                    yield return ExecuteEnemyAttack(damage);
                }
                else
                {
                    dialogue.text = " He Ducked Up (miiiiiiiiiiss)";
                    critAttack.gameObject.SetActive(false);
                    yield return new WaitForSeconds(3f);
                    state = BattleState.PLAYERTURN;
                    PlayerTurn();
                }
            }
        }
    }

    IEnumerator ExecuteEnemyAttack(int damage)
    {
        dialogue.text = enemyUnit.unitName + " Attacks!";
        bool isDead = playerUnit.TakeDamage(damage);
        yield return new WaitForSeconds(2f);

        playerHUD.SetHP(playerUnit.currentHP);
        dialogue.text = " He Ducked me Up (hiiiiiiiiiiit) ";
        critAttack.gameObject.SetActive(false);

        Debug.Log("Player HP" + playerUnit.currentHP);
        yield return new WaitForSeconds(2f);

        if (isDead)
        {
            state = BattleState.LOST;
            EndBattle();
        }
        else
        {
            yield return new WaitForSeconds(1f);
            state = BattleState.PLAYERTURN;
            PlayerTurn();
        }
    }

    void PlayerTurn()
    {
        dialogue.text = "Make your move Duck:";
        battleInterface.ActivateButtons();
    }

    bool ChanceOfEvasion(int evationOnButtonChance)
    {
        hitChanceAmount = Random.Range(0, 100);
        //100 - (unitOne.speed - unitTwo.speed) * 10
        Debug.Log("unit.evasionChance" + evationOnButtonChance);
        Debug.Log("hitChanceAmount" + hitChanceAmount);

        if (evationOnButtonChance <= hitChanceAmount)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public IEnumerator PlayerEvation()
    {
        if (isHeavyAttack)
        {
        executeHeavyAttack = true;
        dialogue.text = " Ready For Evasion ";
        yield return new WaitForSeconds(3f);
        state = BattleState.ENEMYTURN;
        StartCoroutine(EnemyTurn());
        }else
        {
            dialogue.text = " You Have Evaded Nothing Genius ";
            yield return new WaitForSeconds(3f);
            state = BattleState.ENEMYTURN;
            StartCoroutine(EnemyTurn());
        }
    }
}
