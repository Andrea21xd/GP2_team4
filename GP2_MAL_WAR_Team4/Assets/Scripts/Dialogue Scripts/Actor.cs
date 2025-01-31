using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Actor : MonoBehaviour
{
    public string Name;
    public Dialogue Dialogue;
    public DialogueManager DialogueManager;

    private bool playerIsNear = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsNear = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        DialogueManager.HideDialogue();
        playerIsNear = false;
    }

    public void Update()
    {
        if (playerIsNear && Input.GetKeyUp(KeyCode.E))
        {
            SpeakTo();
        }
    }



    // Trigger dialogue for this actor
    public void SpeakTo()
    {
        DialogueManager.Instance.StartDialogue(Name, Dialogue.RootNode);
    }
}
