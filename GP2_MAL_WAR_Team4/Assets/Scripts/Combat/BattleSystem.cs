using System.Collections;
using System.Runtime.InteropServices.WindowsRuntime;
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
    public GameObject enemyPrefab3;
    public GameObject enemyPrefab4;
    public GameObject enemyPrefab5;
    public GameObject enemyPrefab6;
    private int enemyID;
    private GameObject enemyObject;

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
        PlayerPrefs.SetString("Dead", "isAlive");
        state = BattleState.START;
        StartCoroutine(SetupBattle());
        battleInterface = FindFirstObjectByType<BattleInterface>();
    }

    IEnumerator SetupBattle()
    {
        GameObject myPlayerGO = Instantiate(playerPrefab, playerBattleStation);
        playerUnit = myPlayerGO.GetComponent<Unit>();
        playerAnimator = myPlayerGO.GetComponent<Animator>();
        playerUnit.currentHP = PlayerPrefs.GetInt("currentHealth");
        playerUnit.speed = PlayerPrefs.GetInt("currentSpeed");
        savedPlayerSpeed = playerUnit.speed;
        Debug.Log("Player unit speed " + playerUnit.speed.ToString()); 
        playerUnit.luck = PlayerPrefs.GetInt("currentLuck");
        playerUnit.damage = PlayerPrefs.GetInt("currentBaseDamage");
        enemyID = PlayerPrefs.GetInt("EnemyId");

        Debug.Log("damage" + playerUnit.damage);
        Debug.Log("currentHP" + playerUnit.currentHP);

        switch (enemyID)
        {
            case 1:
                if (enemyPrefab1 == null) { break; }
                enemyObject = enemyPrefab1;
                break;
            case 2:
                if (enemyPrefab2 == null) { break; }
                enemyObject = enemyPrefab2;
                break;
            case 3:
                if (enemyPrefab3 == null) { break; }
                enemyObject = enemyPrefab3;
                break;
            case 4:
                if (enemyPrefab4 == null) { break; }
                enemyObject = enemyPrefab4;
                break;
            case 5:
                if (enemyPrefab5 == null) { break; }
                enemyObject = enemyPrefab5;
                break;
            case 6:
                if (enemyPrefab6 == null) { break; }
                enemyObject = enemyPrefab6;
                break;         
        }



        GameObject enemyGo = Instantiate(enemyObject, enemyBattleStation);
        enemyUnit = enemyGo.GetComponent<Unit>();
        enemyAnimator = enemyGo.GetComponent<Animator>();


        battleInterface.SetDialogue(/*" A wild " + */ enemyUnit.unitName + " approaches...");

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
            battleInterface.SetDialogue("I Ducked Up (miss)");

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


        battleInterface.SetDialogue("I Ducked him Up (hit)");

        critAttackText.gameObject.SetActive(false);
        yield return new WaitForSeconds(3f);
        battleInterface.SetDamageDialogue(false, damage);

        if (isDead)
        {
            enemyAnimator.Play("Dead");
            yield return new WaitForSeconds(2f);
            PlayerPrefs.SetString("Dead", "isDead");
            PlayerPrefs.SetInt("enemyKey", 0);
            PlayerPrefs.Save();
            if(enemyID == 2)
            {
                PlayerPrefs.SetInt("Chapter",2);
                PlayerPrefs.Save();
            }
            else if(enemyID == 3)
            {
                PlayerPrefs.SetInt("EndGame", 1);
                PlayerPrefs.Save();
            }


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
        
        if (playerSpeedBuffActive == true)
        {
            RemovePlayerSpeedBuff();
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

            PlayerPrefs.SetInt("HasFought", 0);
            PlayerPrefs.Save();

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
            heavyChanceAmount = Random.Range(0, 80);
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
                enemyAnimator.Play("ExitIdle");
                battleInterface.SetDialogue("Enemy landing the attack!");

                yield return new WaitForSeconds(3f);

                battleInterface.SetDialogue("Duck has evaded the attack!!");
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
                    battleInterface.SetDialogue("He ducked up (miss)");

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
        enemyAnimator.Play("ExitIdle");
        battleInterface.SetDialogue("The " + enemyUnit.unitName + " Attacks!");

        bool isDead = playerUnit.TakeDamage(damage);
        yield return new WaitForSeconds(3f);

        battleInterface.SetDamageDialogue(true, damage);
        playerHUD.SetHP(playerUnit.currentHP);
        battleInterface.SetDialogue("He ducked me up (hit)");
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
        playerUnit.speed = savedPlayerSpeed;
        CheckPlayerSpeedBuff();
        battleInterface.SetDialogue("Make your move Duck:");
        battleInterface.ActivateButtons();
        battleInterface.HasFinisher(enemyUnit.currentHP, enemyUnit.maxHP);
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
            battleInterface.SetDialogue("Ready for evasion");

            yield return new WaitForSeconds(3f);
            state = BattleState.ENEMYTURN;
            StartCoroutine(EnemyTurn());
        }
        else
        {
            if (playerSpeedBuffActive == true) RemovePlayerSpeedBuff();         
            battleInterface.SetDialogue("Speed has temporarily been doubled!");
            if (!playerSpeedBuffActive)
            {
                savedPlayerSpeed = playerUnit.speed;
                playerUnit.speed *= 2;
                playerSpeedBuffActive = true;
                playerHUD.UpdateStats(playerUnit);
            }

            yield return new WaitForSeconds(3f);
            state = BattleState.ENEMYTURN;
            StartCoroutine(EnemyTurn());
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
    public void RemovePlayerSpeedBuff()
    {
        Debug.Log("Old Speed: " + playerUnit.speed);
        playerUnit.speed = savedPlayerSpeed;
        Debug.Log("New Speed: " + playerUnit.speed);

        playerSpeedBuffActive = false;
        playerHUD.UpdateStats(playerUnit);
        Debug.Log("Player buff speed has been removed");

    }

    public void CheckPlayerSpeedBuff()
    {
        if (playerSpeedBuffActive == true)
        {
            Debug.Log("PlayerSpeedBuff is true");
        }
        else
        {
            Debug.Log("PlayerSpeedBuff is false");
        }
    }
}
