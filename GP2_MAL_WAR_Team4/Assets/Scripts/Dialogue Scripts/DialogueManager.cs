using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    // UI references
    public GameObject DialogueParent; 
    public TextMeshProUGUI DialogTitleText, DialogBodyText; 
    public GameObject responseButtonPrefab; 
    public Transform responseButtonContainer;
    public CameraScript camera;

    private void Awake()
    {
        // Singleton pattern to ensure only one instance of DialogueManager
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        
        HideDialogue();
    }


    // Starts the dialogue with given title and dialogue node
    public void StartDialogue(string title, DialogueNode node)
    {
        ShowDialogue();

        DialogTitleText.text = title;
        DialogBodyText.text = node.dialogueText;

        // Remove any existing response buttons
        foreach (Transform child in responseButtonContainer)
        {
            Destroy(child.gameObject);
        }

        // Create and setup response buttons based on current dialogue node
        foreach (DialogueResponse response in node.responses)
        {
            GameObject buttonObj = Instantiate(responseButtonPrefab, responseButtonContainer);
            buttonObj.GetComponentInChildren<TextMeshProUGUI>().text = response.responseText;

            // Setup button to trigger SelectResponse when clicked
            buttonObj.GetComponent<Button>().onClick.AddListener(() => SelectResponse(response, title));
        }
    }


    // Handles response selection and triggers next dialogue node
    public void SelectResponse(DialogueResponse response, string title)
    {
        // Check if there's a follow-up node
        if (!response.nextNode.IsLastNode())
        {
            StartDialogue(title, response.nextNode); // Start next dialogue
            response.OnClick();
        }
        else
        {
            // If no follow-up node, end the dialogue
            HideDialogue();
            if (response.battleNPC)
            {
                camera.UnlockCursor();
                PlayerPrefs.SetInt("EnemyId", response.EnemyId);
                PlayerPrefs.Save();
                SceneManager.LoadScene(2);
            }
        }
    }

    
    public void HideDialogue()
    {
        DialogueParent.SetActive(false);
        camera.LockCursor();
    }

    private void ShowDialogue()
    {
        DialogueParent.SetActive(true);
        camera.UnlockCursor();
    }

    public bool IsDialogueActive()
    {
        return DialogueParent.activeSelf;
    }
}
