using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Actor : MonoBehaviour
{
    [Header("NPC Info")]
    public string Name;
    public int EnemyId = 1;
    public bool repeatableInteraction;
    public bool BattleNPC;
    public bool triggerNPC;
    public Dialogue Dialogue;
    public Dialogue AlternativeDialogue;

    [Header("Objects")]
    public DialogueManager DialogueManager;
    public GameObject InteractionCollider;
    public GameObject NPC;
  
    [HideInInspector] 
    public bool neverTalked = false;
    private bool playerIsNear = false;
    private string NPCstate;

    
    // Enables and disables the interaction buttons
    private void OnEnable()
    {
        GameEventsManager.Instance.miscEvents.onInteractionPressed += SubmitPressed;
    }

    private void OnDisable()
    {
        GameEventsManager.Instance.miscEvents.onInteractionPressed -= SubmitPressed;
    }

    private void Awake()
    {
        // If the NPC doesnt have a dialogue the the interaction collider be off
        if (Dialogue == null) { InteractionCollider.SetActive(false); return; }

        //NPCstate = PlayerPrefs.GetString("Dead");

        //Debug.Log(NPCstate);
        //Debug.Log(EnemyId);

        //if (NPCstate == "isDead" && EnemyId == PlayerPrefs.GetInt("EnemyId") && BattleNPC)
        //{    
            //Destroy(NPC);
            //return;
        //}
    }


    // When player have entered the collider and checks the bool playerIsNear is true 
    private void OnTriggerEnter(Collider other)
    {
        if (Dialogue == null) { return; }

        if (other.CompareTag("Player"))
        {
            playerIsNear = true;

            // Checks if the player is a trigger npc
            if (triggerNPC)
            {
                // trigger the dialogue if the player enters the collider
                    SpeakTo();
            }

            if (repeatableInteraction)
            {
                InteractionCollider.SetActive(true);
            }
            else
            {
                // If the player have not talked to the npc set the collider on (failsafe or change the neverTalked value)
                if (!neverTalked)
                {
                    InteractionCollider.SetActive(true);
                }
            }
        }
    }

    // When player have exit the collider and checks the bool playerIsNear is false
    private void OnTriggerExit(Collider other)
    {
        DialogueManager.HideDialogue();
        playerIsNear = false;

        if (repeatableInteraction) { return; }
        else
        {
            // If the player have talked to the NPC then turn off the interactionCollider
            if (neverTalked)
            {
                InteractionCollider.SetActive(false);
            }
        }
    }

    private void SubmitPressed()
    {
        if (Dialogue == null) { return; }



        // Check when the player is close to the interaction and when the player interact starts the dialogue
        if (playerIsNear)
        {
            // If its a important NPC then always start the dialogue
            if (repeatableInteraction)
            {
                SpeakTo();
            }
            // If its not a important dialogue checks the neverTalked value
            if (!neverTalked)
            {
                SpeakTo();
                neverTalked = true;
            }
        }
    }
    

    // Trigger dialogue for this actor
    public void SpeakTo()
    {
        if (BattleNPC) 
        {
            string enemyKey = "Enemy" + EnemyId + "Alive";
            int isAlive = PlayerPrefs.GetInt("enemyKey");

            if (isAlive == 1 && EnemyId == PlayerPrefs.GetInt("EnemyId"))
            {
                DialogueManager.Instance.StartDialogue(Name, Dialogue.RootNode);
            } else
            {
                DialogueManager.Instance.StartDialogue(Name, AlternativeDialogue.RootNode);
            }
        } else
        {
            int chapter = PlayerPrefs.GetInt("Chapter");
            if (chapter == 2)
            {
                DialogueManager.Instance.StartDialogue(Name, AlternativeDialogue.RootNode);
            }
            else
            {
                DialogueManager.Instance.StartDialogue(Name, Dialogue.RootNode);
            }
        }

        //DialogueManager.Instance.StartDialogue(Name, Dialogue.RootNode);
    }
}
