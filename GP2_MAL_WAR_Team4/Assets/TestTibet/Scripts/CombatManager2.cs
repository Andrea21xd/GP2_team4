using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// A comprehensive example for turn-based combat with Attack/Defense sliders,
/// heavy attack mechanics, Fumble Points, and ultimate attacks.
/// 
/// IMPORTANT: 
/// 1) All UI references must be properly assigned in the Inspector.
/// 2) You may need to adapt some logic to match your exact scene/camera/player setup.
/// 3) All logs, prompts, and method names are in English.
/// </summary>
public class CombatManager2 : MonoBehaviour
{
    [Header("=== Enemy Spawn ===")]
    public GameObject enemyPrefab;      // Assign this in the Inspector (a valid enemy prefab)
    public Transform enemySpawnPoint;   // A Transform object where the enemy will appear
    private GameObject currentEnemy;    // Stores the spawned enemy reference

    [Header("=== Player & Enemy Data ===")]
    public int playerMaxHealth = 10;
    public int playerCurrentHealth = 10;
    public int enemyMaxHealth = 10;
    public int enemyCurrentHealth = 10;

    [Header("=== Sliders & UI Elements ===")]
    public Slider playerHealthSlider;       // Visual representation of Player HP
    public Slider enemyHealthSlider;        // Visual representation of Enemy HP
    public Slider attackSlider;             // Attack slider (0 -> 1)
    public Slider defenseSlider;            // Defense slider (0 -> 1)
    public Slider fumblePointSlider;        // Fumble point slider (0 -> 3 ideally)
    public Text fumblePointText;           // Shows the numeric count of Fumble Points
    public Text messageText;               // Displays prompts and turn messages
    public Button attackModeButton;         // Enables Attack slider
    public Button defenseModeButton;        // Enables Defense slider
    public Button ultimateButton;           // Becomes active if Fumble Points >= 3

    [Header("=== Timing & Speeds ===")]
    public float sliderSpeed = 1f;          // Speed for Attack/Defense sliders filling from 0 -> 1

    [Header("=== Fumble Points ===")]
    public int maxFumblePoints = 3;         // Maximum Fumble Points (3)
    private int currentFumblePoints = 0;    // Current Fumble Points

    // Internal flags
    private bool isPlayerTurn = true;
    private bool isSliderActive = false;    // If the current slider (attack or defense) is moving
    private bool isAwaitingSpace = false;   // We show a prompt, waiting for SPACE to proceed

    // Which slider is active?
    private bool isAttackSlider = false;
    private bool isDefenseSlider = false;

    // Enemy heavy attack state
    // 0 = no heavy attack in progress
    // 1 = heavy attack "preparing" (this turn no dmg, next turn is the actual heavy)
    private int enemyHeavyState = 0;

    // Remember if the player used a special defense that modifies the heavy attack's second turn
    private bool defenseUsedOnHeavyTurn = false;

    // Placeholder for damage that will be applied after we press SPACE
    private int pendingDamageToEnemy = 0;
    private int pendingDamageToPlayer = 0;

    private bool isUltimateAvailable = false;  // Becomes true when player has 3 Fumble Points
    private bool enemyStunned = false;  // If true, enemy will skip its next turn
    private bool enemyAttackBlocked = false;  // Becomes true if player did Perfect Defense (90-100 range)



    void Start()
    {
        Debug.Log("CombatManager2 Start() called.");

        // 🔹 ENABLE MOUSE CURSOR AUTOMATICALLY
        Cursor.lockState = CursorLockMode.None;  // Unlock cursor
        Cursor.visible = true;                   // Make cursor visible

        // Ensure an enemy prefab is assigned
        if (enemyPrefab != null && enemySpawnPoint != null)
        {
            currentEnemy = Instantiate(enemyPrefab, enemySpawnPoint.position, enemySpawnPoint.rotation);
            Debug.Log("Enemy spawned successfully.");
        }
        else
        {
            Debug.LogError("ERROR: Enemy Prefab or Spawn Point is not assigned in the Inspector!");
        }

        // Initialize health
        playerCurrentHealth = playerMaxHealth;
        enemyCurrentHealth = enemyMaxHealth;

        // Set up UI
        if (playerHealthSlider) playerHealthSlider.maxValue = playerMaxHealth;
        if (enemyHealthSlider) enemyHealthSlider.maxValue = enemyMaxHealth;
        UpdateHealthSliders();

        currentFumblePoints = 0;
        UpdateFumbleUI();

        if (ultimateButton) ultimateButton.gameObject.SetActive(false);

        if (attackSlider) attackSlider.gameObject.SetActive(false);
        if (defenseSlider) defenseSlider.gameObject.SetActive(false);

        isPlayerTurn = true;
        messageText.text = "It is your turn. Choose an action (Attack or Defense).";
        Debug.Log("Player turn starts.");

        if (attackModeButton) attackModeButton.interactable = true;
        if (defenseModeButton) defenseModeButton.interactable = true;
    }

    void Update()
    {
        if (isPlayerTurn && !isSliderActive && !isAwaitingSpace && !isUltimateAvailable)
        {
            if (Input.GetKeyDown(KeyCode.J))
            {
                OnClick_AttackMode();
            }
            else if (Input.GetKeyDown(KeyCode.K))
            {
                OnClick_DefenseMode();
            }
        }

        // If we are showing a prompt and waiting for space
        if (isAwaitingSpace && Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("SPACE pressed - proceeding from prompt.");
            isAwaitingSpace = false;
            messageText.text = "";
            ProceedAfterSpace();
        }

        // Slider logic (Attack or Defense) if active
        if (isSliderActive)
        {
            // Fill from 0 -> 1
            float delta = sliderSpeed * Time.deltaTime;
            if (isAttackSlider && attackSlider != null)
            {
                attackSlider.value += delta;
                if (attackSlider.value >= 1f)
                {
                    // Fail - did not press space in time
                    attackSlider.value = 1f;
                    StopAttackSlider(failed: true);
                }
                else if (Input.GetKeyDown(KeyCode.Space))
                {
                    // Player stops it
                    StopAttackSlider(failed: false);
                }
            }
            else if (isDefenseSlider && defenseSlider != null)
            {
                defenseSlider.value += delta;
                if (defenseSlider.value >= 1f)
                {
                    defenseSlider.value = 1f;
                    StopDefenseSlider(failed: true);
                }
                else if (Input.GetKeyDown(KeyCode.Space))
                {
                    StopDefenseSlider(failed: false);
                }
            }
        }
    }

    // -----------------------------------------------------------------------------------
    // BUTTON HANDLERS
    // -----------------------------------------------------------------------------------

    /// <summary>
    /// Called when the player clicks the Attack Mode button.
    /// </summary>
    public void OnClick_AttackMode()
    {
        if (!isPlayerTurn || isSliderActive || isAwaitingSpace) return;
        Debug.Log("Player chose ATTACK mode.");

        // Disable the other mode button
        if (attackModeButton) attackModeButton.interactable = false;
        if (defenseModeButton) defenseModeButton.interactable = false;

        // Show Attack slider
        isAttackSlider = true;
        isDefenseSlider = false;
        isSliderActive = true;

        if (attackSlider)
        {
            attackSlider.gameObject.SetActive(true);
            attackSlider.value = 0f;
        }
        if (defenseSlider) defenseSlider.gameObject.SetActive(false);

        messageText.text = "Attack slider is filling up. Press SPACE before it reaches 1!";
    }

    /// <summary>
    /// Called when the player clicks the Defense Mode button.
    /// </summary>
    public void OnClick_DefenseMode()
    {
        if (!isPlayerTurn || isSliderActive || isAwaitingSpace) return;
        Debug.Log("Player chose DEFENSE mode.");

        // Disable the other mode button
        if (attackModeButton) attackModeButton.interactable = false;
        if (defenseModeButton) defenseModeButton.interactable = false;

        // Show Defense slider
        isDefenseSlider = true;
        isAttackSlider = false;
        isSliderActive = true;

        if (defenseSlider)
        {
            defenseSlider.gameObject.SetActive(true);
            defenseSlider.value = 0f;
        }
        if (attackSlider) attackSlider.gameObject.SetActive(false);

        messageText.text = "Defense slider is filling up. Press SPACE before it reaches 1!";
    }

    /// <summary>
    /// Called when the player clicks the Ultimate button (only enabled if Fumble Points >= 3).
    /// </summary>
    public void OnClick_Ultimate()
    {
        Debug.Log("Player clicked ULTIMATE button.");

        // Enemy gets stunned
        enemyStunned = true;

        // 🔹 Reset Fumble Points
        currentFumblePoints = 0;
        UpdateFumbleUI();  // Refresh the Fumble UI

        // 🔹 Disable Ultimate Button
        if (ultimateButton) ultimateButton.gameObject.SetActive(false);
        isUltimateAvailable = false;

        // 🔹 Re-enable Attack & Defense buttons
        if (attackModeButton) attackModeButton.interactable = true;
        if (defenseModeButton) defenseModeButton.interactable = true;

        // Prompt player to press SPACE
        messageText.text = "ULTIMATE attack ready! Press SPACE to deal 5 damage.";
        pendingDamageToEnemy = 5;
        isAwaitingSpace = true;
    }

    // -----------------------------------------------------------------------------------
    // ATTACK SLIDER LOGIC
    // -----------------------------------------------------------------------------------

    private void StopAttackSlider(bool failed)
    {
        Debug.Log("StopAttackSlider called. failed=" + failed);
        isSliderActive = false;
        isAttackSlider = false;

        if (attackSlider) attackSlider.gameObject.SetActive(false);

        int damage = 0;
        if (failed)
        {
            // No damage
            damage = 0;
            messageText.text = "You failed to press SPACE in time! 0 damage dealt.";
        }
        else
        {
            float val = (attackSlider != null) ? attackSlider.value : 0f;
            Debug.Log("Attack slider final value = " + val.ToString("F2"));

            // Ranges (0..0.35 or 0.65..1 => 1 dmg, 0.36..0.44 or 0.56..0.64 => 2 dmg, 0.45..0.55 => 3 dmg +1 fumble)
            if ((val >= 0f && val <= 0.35f) || (val >= 0.65f && val <= 1f))
            {
                damage = 1;
                messageText.text = "You dealt 1 damage!";
            }
            else if ((val > 0.35f && val < 0.45f) || (val > 0.55f && val < 0.65f))
            {
                damage = 2;
                messageText.text = "You dealt 2 damage!";
            }
            else
            {
                damage = 3;
                messageText.text = "CRITICAL HIT! You dealt 3 damage and gained 1 Fumble Point!";
                currentFumblePoints = Mathf.Min(currentFumblePoints + 1, maxFumblePoints);
                UpdateFumbleUI();
            }
        }

        // Prepare to apply damage after SPACE
        pendingDamageToEnemy = damage;
        isAwaitingSpace = true;

        // ⚡ Enemy WILL attack after player, even if the attack failed
        if (failed)
        {
            Debug.Log("Player failed attack! Enemy will now take its turn.");
            isPlayerTurn = false;
            StartEnemyTurn();
        }
    }

    // -----------------------------------------------------------------------------------
    // DEFENSE SLIDER LOGIC
    // -----------------------------------------------------------------------------------

    private void StopDefenseSlider(bool failed)
    {
        Debug.Log("StopDefenseSlider called. failed=" + failed);
        isSliderActive = false;
        isDefenseSlider = false;

        if (defenseSlider) defenseSlider.gameObject.SetActive(false);

        float val = (defenseSlider != null) ? defenseSlider.value : 0f;
        Debug.Log("Defense slider final value = " + val.ToString("F2"));

        if (failed)
        {
            lastDefenseOutcome = DefenseOutcome.Fail;
            messageText.text = "Defense failed! Enemy attack probability is now worse.";
        }
        else
        {
            if (val <= 0.70f)
            {
                lastDefenseOutcome = DefenseOutcome.Normal;
                messageText.text = "Defense was weak. Enemy attack probability remains normal.";
            }
            else if (val <= 0.89f)
            {
                lastDefenseOutcome = DefenseOutcome.Medium;
                messageText.text = "Defense was medium! Enemy attack probability is altered.";
            }
            else
            {
                // 🔹 Player achieved Perfect Defense (90 - 100 range)
                lastDefenseOutcome = DefenseOutcome.High;
                enemyAttackBlocked = true;  // Prevent enemy from attacking next turn

                messageText.text = "Perfect Defense! You restored 20% HP, gained +1 Fumble Point, and blocked all enemy attacks next turn.";
                RecoverHealth(0.2f);
                currentFumblePoints = Mathf.Min(currentFumblePoints + 1, maxFumblePoints);
                UpdateFumbleUI();
            }
        }

        isAwaitingSpace = true;
    }

    private enum DefenseOutcome
    {
        Normal,
        Medium,
        High,
        Fail
    }
    private DefenseOutcome lastDefenseOutcome = DefenseOutcome.Normal;

    // -----------------------------------------------------------------------------------
    // SPACE CONFIRMATION FLOW
    // -----------------------------------------------------------------------------------

    private void ProceedAfterSpace()
    {
        Debug.Log("ProceedAfterSpace() called.");

        // 1) If we have pending damage to enemy, apply it first
        if (pendingDamageToEnemy > 0)
        {
            ApplyAttackDamage();

            if (enemyStunned)
            {
                Debug.Log("Ultimate was used. Fumble Points reset, returning to player turn.");
                UpdateFumbleUI();
            }

            return;
        }

        // 2) If we have pending damage to player, apply it
        if (pendingDamageToPlayer > 0)
        {
            ApplyEnemyDamage();
            return;
        }

        // 3) If enemy was stunned, they SKIP their turn
        if (enemyStunned)
        {
            Debug.Log("Enemy is stunned! Skipping enemy turn.");
            enemyStunned = false;
            StartPlayerTurn();
            return;
        }

        // 4) If none of the above, check whose turn it should be
        if (isPlayerTurn)
        {
            Debug.Log("Player action finished. Switching to enemy turn.");
            StartEnemyTurn();
        }
        else
        {
            Debug.Log("Enemy action finished. Switching to player turn.");
            StartPlayerTurn();
        }
    }

    // -----------------------------------------------------------------------------------
    // APPLY DAMAGE
    // -----------------------------------------------------------------------------------

    private void ApplyAttackDamage()
    {
        Debug.Log($"ApplyAttackDamage: {pendingDamageToEnemy} dmg to enemy.");
        enemyCurrentHealth -= pendingDamageToEnemy;
        if (enemyCurrentHealth < 0) enemyCurrentHealth = 0;
        pendingDamageToEnemy = 0;

        UpdateHealthSliders();

        // Check if enemy is dead
        if (enemyCurrentHealth <= 0)
        {
            Debug.Log("Enemy died. Ending combat with victory for player.");
            EndCombat(playerWon: true);
            return;
        }

        // If this was an Ultimate attack, enemy is stunned → Skip enemy turn
        if (enemyStunned)
        {
            Debug.Log("Enemy was stunned by Ultimate attack. Player gets another turn.");
            StartPlayerTurn();  // Skip enemy turn and return to player
            return;
        }

        // Otherwise, enemy gets its normal turn
        isPlayerTurn = false;
        StartEnemyTurn();
    }

    private void ApplyEnemyDamage()
    {
        Debug.Log($"ApplyEnemyDamage: {pendingDamageToPlayer} dmg to player.");
        playerCurrentHealth -= pendingDamageToPlayer;
        if (playerCurrentHealth < 0) playerCurrentHealth = 0;
        pendingDamageToPlayer = 0;

        UpdateHealthSliders();

        // Check if player is dead
        if (playerCurrentHealth <= 0)
        {
            Debug.Log("Player died. Ending combat with defeat.");
            EndCombat(playerWon: false);
            return;
        }

        // ⚡ Immediately switch back to player's turn after enemy attack
        isPlayerTurn = true;
        StartPlayerTurn();
    }

    // -----------------------------------------------------------------------------------
    // TURN HANDLERS
    // -----------------------------------------------------------------------------------

    private void StartPlayerTurn()
    {
        Debug.Log("StartPlayerTurn() called.");
        isPlayerTurn = true;
        isAwaitingSpace = false;
        isSliderActive = false;
        defenseUsedOnHeavyTurn = false;

        // Reactivate the mode buttons
        if (attackModeButton) attackModeButton.interactable = true;
        if (defenseModeButton) defenseModeButton.interactable = true;

        // If we have 3 fumble points, show ultimate button
        if (currentFumblePoints >= 3 && ultimateButton != null)
        {
            ultimateButton.gameObject.SetActive(true);
        }
        else if (ultimateButton != null)
        {
            ultimateButton.gameObject.SetActive(false);
        }

        messageText.text = "Your turn. Choose Attack or Defense, or use Ultimate if available.";
    }

    private void StartEnemyTurn()
    {
        Debug.Log("StartEnemyTurn() called.");
        isPlayerTurn = false;
        isAwaitingSpace = false;

        // 🔹 If Perfect Defense was used last turn, the enemy skips this attack
        if (enemyAttackBlocked)
        {
            Debug.Log("Enemy is completely blocked due to Perfect Defense! Skipping attack.");
            enemyAttackBlocked = false;  // Reset block state
            messageText.text = "Enemy is stunned and cannot attack this turn! Press SPACE to continue.";
            isAwaitingSpace = true;
            return;
        }

        int dmg = 0;
        string desc = "";

        if (enemyHeavyState == 1)
        {
            Debug.Log("Enemy is executing the 2nd turn of its Heavy Attack!");
            enemyHeavyState = 0;  // Reset Heavy Attack state

            float rand = Random.value;
            if (rand < 0.80f) dmg = 3;
            else dmg = 1;

            desc = $"Enemy executes its Heavy Attack and deals {dmg} damage!";
        }
        else
        {
            float rand = Random.value;
            if (rand < 0.20f) dmg = 0;
            else if (rand < 0.40f) dmg = 2;
            else if (rand < 0.60f)
            {
                enemyHeavyState = 1;
                dmg = 0;
                desc = "Enemy is preparing a HEAVY ATTACK for next turn!";
            }
            else dmg = 1;

            if (enemyHeavyState == 0) desc = $"Enemy attacks and deals {dmg} damage!";
        }

        pendingDamageToPlayer = dmg;
        messageText.text = desc + " Press SPACE to continue.";
        isAwaitingSpace = true;
    }

    // -----------------------------------------------------------------------------------
    // ENEMY DAMAGE CALCULATION
    // -----------------------------------------------------------------------------------

    /// <summary>
    /// If secondHeavy == false, normal distribution:
    ///    - 20% -> 0
    ///    - 20% -> 2
    ///    - 20% -> start heavy (represented by returning -999)
    ///    - 40% -> 1
    /// 
    /// If secondHeavy == true, base distribution: 
    ///    - 80% -> 3
    ///    - 20% -> 1
    /// 
    /// DefenseOutcome modifies these distributions.
    /// </summary>
    private int CalculateEnemyDamage(bool secondHeavy)
    {
        Debug.Log($"CalculateEnemyDamage called. secondHeavy={secondHeavy}, lastDefenseOutcome={lastDefenseOutcome}");
        float rand = Random.value;
        int damage = 0;

        if (!secondHeavy)
        {
            // Normal turn
            // Check defense outcome
            switch (lastDefenseOutcome)
            {
                case DefenseOutcome.Normal:
                    // 20% -> 0, 20% -> 2, 20% -> heavy, 40% -> 1
                    if (rand < 0.20f) damage = 0;
                    else if (rand < 0.40f) damage = 2;
                    else if (rand < 0.60f) return -999; // means "heavy preparation"
                    else damage = 1;
                    break;

                case DefenseOutcome.Medium:
                    // 0.71..0.89 => new distribution:
                    //   40% -> 0, 15% -> 2, 0% -> heavy, 45% -> 1
                    //   If second turn heavy was about to happen, we do partial logic, but here it's normal turn
                    if (rand < 0.40f) damage = 0;
                    else if (rand < 0.55f) damage = 2;
                    else if (rand < 0.55f) return -999; // 0% => won't happen
                    else damage = 1;
                    break;

                case DefenseOutcome.High:
                    // 0.90..1 => full block => enemy deals 0
                    // plus player recovers HP, +1 fumble (already done)
                    damage = 0;
                    break;

                case DefenseOutcome.Fail:
                    // fail => 5% -> 0, 30% -> 2, 0% -> heavy, 65% -> 1
                    if (rand < 0.05f) damage = 0;
                    else if (rand < 0.35f) damage = 2;
                    else damage = 1;
                    break;
            }
        }
        else
        {
            // second turn of heavy
            // base distribution: 80% -> 3, 20% -> 1
            // but if lastDefenseOutcome==Medium => 65%->3, 35%->0
            // if High => 0 dmg
            // if Fail => 80%->3, 20%->1

            switch (lastDefenseOutcome)
            {
                case DefenseOutcome.Normal:
                    if (rand < 0.80f) damage = 3;
                    else damage = 1;
                    break;

                case DefenseOutcome.Medium:
                    if (rand < 0.65f) damage = 3;
                    else damage = 0;
                    break;

                case DefenseOutcome.High:
                    damage = 0;
                    break;

                case DefenseOutcome.Fail:
                    // fail on heavy => 80%->3, 20%->1
                    if (rand < 0.80f) damage = 3;
                    else damage = 1;
                    break;
            }
        }

        return damage;
    }

    // -----------------------------------------------------------------------------------
    // MISC UTILITY
    // -----------------------------------------------------------------------------------

    private void RecoverHealth(float percent)
    {
        Debug.Log($"RecoverHealth {percent * 100}% of max HP.");
        int amount = Mathf.RoundToInt(playerMaxHealth * percent);
        int oldHP = playerCurrentHealth;
        playerCurrentHealth += amount;
        if (playerCurrentHealth > playerMaxHealth)
            playerCurrentHealth = playerMaxHealth;

        Debug.Log($"Player HP recovered from {oldHP} to {playerCurrentHealth}.");
        UpdateHealthSliders();
    }

    private void UpdateHealthSliders()
    {
        if (playerHealthSlider) playerHealthSlider.value = playerCurrentHealth;
        if (enemyHealthSlider) enemyHealthSlider.value = enemyCurrentHealth;
    }

    private void UpdateFumbleUI()
    {
        if (fumblePointText)
            fumblePointText.text = $"Fumble Points: {currentFumblePoints}/{maxFumblePoints}";

        if (fumblePointSlider)
        {
            fumblePointSlider.maxValue = maxFumblePoints;
            fumblePointSlider.value = currentFumblePoints;
        }

        // If player has 3 Fumble Points, enable Ultimate and disable normal attacks
        if (currentFumblePoints >= 3)
        {
            isUltimateAvailable = true;
            if (ultimateButton) ultimateButton.gameObject.SetActive(true);

            if (attackModeButton) attackModeButton.interactable = false;
            if (defenseModeButton) defenseModeButton.interactable = false;

            Debug.Log("Ultimate available! Attack & Defense sliders are disabled.");
        }
        else
        {
            // 🔹 Reset Ultimate if Fumble Points are 0
            isUltimateAvailable = false;
            if (ultimateButton) ultimateButton.gameObject.SetActive(false);

            // 🔹 Re-enable Attack & Defense
            if (attackModeButton) attackModeButton.interactable = true;
            if (defenseModeButton) defenseModeButton.interactable = true;

            Debug.Log("Ultimate disabled. Returning to normal combat mode.");
        }
    }

    // -----------------------------------------------------------------------------------
    // ENDING COMBAT
    // -----------------------------------------------------------------------------------

    private void EndCombat(bool playerWon)
    {
        Debug.Log("EndCombat called. playerWon=" + playerWon);

        // 🔹 HIDE MOUSE CURSOR WHEN LEAVING BATTLEMAP
        Cursor.lockState = CursorLockMode.Locked;  // Lock cursor
        Cursor.visible = false;                    // Hide cursor

        // Reset Fumble Points
        currentFumblePoints = 0;
        UpdateFumbleUI();

        // Return to OpenWorldMap
        StartCoroutine(ReturnToOpenWorld(playerWon));
    }

    private IEnumerator ReturnToOpenWorld(bool playerWon)
    {
        // Wait 2 seconds, then load the open world
        yield return new WaitForSeconds(2f);

        if (playerWon)
        {
            Debug.Log("Loading OpenWorldMap... Enemy is destroyed in that scene.");
        }
        else
        {
            Debug.Log("Player lost. Loading OpenWorldMap... Enemy resets to full HP or similar logic.");
        }
        SceneManager.LoadScene("OpenWorldMap");
    }
}
