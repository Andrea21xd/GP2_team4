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

    public CharacterMovement playerMovement;
    public CameraScript camera;
    public GameObject duck;

    public Vector3 playerPos;
    public VectorValue playerMemory;

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

        for (int i = 1; i <= 9; i++)
        {
            string key = $"Enemy{i}Alive";

            if (!PlayerPrefs.HasKey(key))
            {
                PlayerPrefs.SetInt(key, 1);
            }
        }

        if (!PlayerPrefs.HasKey("Chapter"))
        {
            PlayerPrefs.SetInt("Chapter", 1);
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
        response.InitializeQuestState();

        if (response == null)
        {
            HideDialogue();
            return;
        }

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
                playerPos = duck.transform.position;
                playerMemory.initialValue = playerPos;

                camera.UnlockCursor();
                PlayerPrefs.SetInt("EnemyId", response.EnemyId);
                PlayerPrefs.SetInt("HasFought", 1); 
                PlayerPrefs.Save();
                SceneManager.LoadScene(2);
            }
        }
    }

    
    public void HideDialogue()
    {
        camera.startCamera = true;
        DialogueParent.SetActive(false);
        playerMovement.PlayerMovementAllowed = true;
        camera.LockCursor();
    }

    private void ShowDialogue()
    {
        camera.startCamera = false;
        DialogueParent.SetActive(true);
        playerMovement.PlayerMovementAllowed = false;
        camera.UnlockCursor();
    }

    public bool IsDialogueActive()
    {
        return DialogueParent.activeSelf;
    }
}
