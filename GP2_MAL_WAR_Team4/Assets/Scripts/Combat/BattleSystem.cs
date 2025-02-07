using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;


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
    public GameObject enemyPrefab1;
    public GameObject enemyPrefab2;
    private int enemyID;

    [HideInInspector]
    public Animator playerAnimator;
    [HideInInspector]
    public Animator enemyAnimator;

    private int randomChance;
    private int hitChanceAmount;
    [HideInInspector]
    public int evasionOnButtonChance;
    private int heavyChanceAmount;

    [HideInInspector]
    public Unit playerUnit;
    [HideInInspector]
    public Unit enemyUnit;

    public TMP_Text critAttackText;

    public BattleHUD playerHUD;
    public BattleHUD enemyHUD;

    public Transform playerBattleStation;
    public Transform enemyBattleStation;

    private bool isPlayerFaster = false;
    private bool isHeavyAttack = false;
    private bool executeHeavyAttack = false;
    [HideInInspector]    
    public bool playerSpeedBuffActive = false;
    [HideInInspector]
    public int savedPlayerSpeed;
    public BattleInterface battleInterface;

    public BattleState state;

    [Header("=== Scene Loader Name ===")]
    public int sceneLoader;

    //**Script needs refactoring** 
    void Start()
    {
        state = BattleState.START;
        StartCoroutine(SetupBattle());
        battleInterface = FindFirstObjectByType<BattleInterface>();
    }

    IEnumerator SetupBattle()
    {
        GameObject myPlayerGO = Instantiate(playerPrefab, playerBattleStation);
        playerUnit = myPlayerGO.GetComponent<Unit>();
        playerAnimator = myPlayerGO.GetComponent<Animator>();
        //playerUnit.currentHP = PlayerPrefs.GetInt("currentHealth");
        //playerUnit.speed = PlayerPrefs.GetInt("currentSpeed");
        //playerUnit.luck = PlayerPrefs.GetInt("currentLuck");
        //playerUnit.damage = PlayerPrefs.GetInt("currentBaseDamage");
        enemyID = PlayerPrefs.GetInt("EnemyId");

        Debug.Log("damage" + playerUnit.damage);
        Debug.Log("currentHP" + playerUnit.currentHP);

        if (enemyID == 2)
        {
            GameObject enemyGo = Instantiate(enemyPrefab2, enemyBattleStation);
            enemyUnit = enemyGo.GetComponent<Unit>();
            enemyAnimator = enemyGo.GetComponent<Animator>();
        }
        else
        {
            GameObject enemyGo = Instantiate(enemyPrefab1, enemyBattleStation);
            enemyUnit = enemyGo.GetComponent<Unit>();
            enemyAnimator = enemyGo.GetComponent<Animator>();
        }




        battleInterface.SetDialogue(" A wild " + enemyUnit.unitName + " approaches...");

        playerHUD.SetHUD(playerUnit);
        enemyHUD.SetHUD(enemyUnit);

        yield return new WaitForSeconds(3f);

        if (playerUnit.speed >= enemyUnit.speed)
        {
            isPlayerFaster = true;
            state = BattleState.PLAYERTURN;
            PlayerTurn();

        }
        else if (playerUnit.speed < enemyUnit.speed)
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
                critAttackText.gameObject.SetActive(true);
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
            battleInterface.SetDialogue("I Ducked Up (miiiiiiiiiss)");

            yield return new WaitForSeconds(3f);
            critAttackText.gameObject.SetActive(false);
            state = BattleState.ENEMYTURN;
            StartCoroutine(EnemyTurn());
        }
        if (isHeavyAttack)
        {

            executeHeavyAttack = true;
        }
    }
    public IEnumerator ExecutePlayerAttack(int damage)
    {

        battleInterface.SetDialogue("Duck Attacking");
        bool isDead = enemyUnit.TakeDamage(damage);
        yield return new WaitForSeconds(3f);

        battleInterface.SetDamageDialogue(true, damage);
        enemyHUD.UpdateStats(enemyUnit);
        Debug.Log("enemy HP" + enemyUnit.currentHP);


        battleInterface.SetDialogue("I Ducked him Up (hiiiiiiiiiit)");

        critAttackText.gameObject.SetActive(false);
        yield return new WaitForSeconds(3f);
        battleInterface.SetDamageDialogue(false, damage);

        if (isDead)
        {
            state = BattleState.WON;
            EndBattle();
        }
        else
        {
            battleInterface.SetDialogue("it's " + enemyUnit.unitName + "'s turn...");

            yield return new WaitForSeconds(3f);
            state = BattleState.ENEMYTURN;
            StartCoroutine(EnemyTurn());
        }
    }

    void EndBattle()
    {
        if (state == BattleState.WON)
        {
            battleInterface.SetDialogue("You won the battle");

            SceneManager.LoadScene(1);
        }
        else if (state == BattleState.LOST)
        {
            battleInterface.SetDialogue("You were defeated");

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

    bool HeavyChance(Unit unit)
    {
        randomChance = Random.Range(0, 100);

        if (randomChance <= unit.heavyAttackChance)
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
            heavyChanceAmount = Random.Range(0, 20);
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
            //HeavyChance(enemyUnit);
        }
        Debug.Log("executeHeavyAttack" + executeHeavyAttack);

        if (executeHeavyAttack)
        {
            if (ChanceOfEvasion(evasionOnButtonChance))
            {
                int damage = 0;
                if (CritChance(enemyUnit))
                {
                    critAttackText.gameObject.SetActive(true);
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
                enemyAnimator.Play("Attack");
                battleInterface.SetDialogue("Enemy Landing The Attack!!");

                yield return new WaitForSeconds(3f);

                battleInterface.SetDialogue("Duck Has Evaded the Attack!!");
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
                battleInterface.SetDialogue("Incoming Heavy Attack!!");
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
                        critAttackText.gameObject.SetActive(true);
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
                    battleInterface.SetDialogue("He Ducked Up (miiiiiiiiiiss)");

                    critAttackText.gameObject.SetActive(false);
                    yield return new WaitForSeconds(3f);
                    state = BattleState.PLAYERTURN;
                    PlayerTurn();
                }
            }
        }
    }

    IEnumerator ExecuteEnemyAttack(int damage)
    {
        if (playerSpeedBuffActive)
        {
            playerSpeedBuffActive = false;
        }
        enemyAnimator.Play("Attack");
        battleInterface.SetDialogue("The " + enemyUnit.unitName + " Attacks!");

        bool isDead = playerUnit.TakeDamage(damage);
        yield return new WaitForSeconds(3f);

        battleInterface.SetDamageDialogue(true, damage);
        playerHUD.UpdateStats(playerUnit);
        battleInterface.SetDialogue("He Ducked me Up (hiiiiiiiiiiit)");
        critAttackText.gameObject.SetActive(false);

        Debug.Log("Player HP" + playerUnit.currentHP);
        yield return new WaitForSeconds(3f);
        battleInterface.SetDamageDialogue(false, damage);

        if (isDead)
        {
            state = BattleState.LOST;
            EndBattle();
        }
        else
        {
            yield return new WaitForSeconds(2f);
            state = BattleState.PLAYERTURN;
            PlayerTurn();
        }
    }

    void PlayerTurn()
    {
        savedPlayerSpeed=playerUnit.speed;


        battleInterface.SetDialogue("Make your move Duck:");
        if (playerSpeedBuffActive)
        {
            playerSpeedBuffActive = false;
        }
        else
        {
            playerUnit.speed = savedPlayerSpeed;
        }
        battleInterface.ActivateButtons();
        battleInterface.HasFinisher(enemyUnit.currentHP, enemyUnit.currentHP);
    }

    bool ChanceOfEvasion(int evasionOnButtonChance)
    {
        hitChanceAmount = Random.Range(0, 80);
        //100 - (unitOne.speed - unitTwo.speed) * 10
        Debug.Log("unit.evasionChance" + evasionOnButtonChance);
        Debug.Log("hitChanceAmount" + hitChanceAmount);

        if (evasionOnButtonChance <= hitChanceAmount)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public IEnumerator PlayerEvasion()
    {

        if (isHeavyAttack)
        {
            executeHeavyAttack = true;
            battleInterface.SetDialogue("Ready For Evasion");

            yield return new WaitForSeconds(3f);
            state = BattleState.ENEMYTURN;
            StartCoroutine(EnemyTurn());
        }
        else 
        { 
        battleInterface.SetDialogue("Speed has temporarily been doubled!");
        savedPlayerSpeed = playerUnit.speed;
        playerUnit.speed *= 2;
        playerSpeedBuffActive = true;

        // if (playerSpeedBuffActive) RemovePlayerSpeedBuff();

        yield return new WaitForSeconds(3f);
        state = BattleState.ENEMYTURN;
        StartCoroutine(EnemyTurn());
        playerHUD.UpdateStats(playerUnit);                  
        }

        //if (isHeavyAttack)
        //{
        //    executeHeavyAttack = true;
        //    battleInterface.SetDialogue("Ready For Evasion");

        //    yield return new WaitForSeconds(3f);
        //    state = BattleState.ENEMYTURN;
        //    StartCoroutine(EnemyTurn());
        //}
        //else
        //{
        //    battleInterface.SetDialogue("You Have Evaded Nothing Genius");

        //    yield return new WaitForSeconds(3f);
        //    state = BattleState.ENEMYTURN;
        //    StartCoroutine(EnemyTurn());
        //}
    }

    void RemovePlayerSpeedBuff()
    {
        playerUnit.speed = (playerUnit.speed)/2;
        playerSpeedBuffActive = false;
    }
}
