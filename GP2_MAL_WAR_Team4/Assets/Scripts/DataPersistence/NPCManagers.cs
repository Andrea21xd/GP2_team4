using UnityEngine;

public class NPCManagers : MonoBehaviour
{
    [Header("Chapter 1 NPC")]
    public GameObject NPC1Chapter1;
    public GameObject NPC2Chapter1;
    public GameObject NPC3Chapter1;
    public GameObject NPC4Chapter1;

    [Header("Chapter 2 NPC")]
    public GameObject NPC1Chapter2;
    public GameObject NPC2Chapter2;

    void Start()
    {
        int chapter = PlayerPrefs.GetInt("Chapter");

        if (chapter == 1)
        {
            NPC1Chapter1.SetActive(true);
            NPC2Chapter1.SetActive(true);
            NPC3Chapter1.SetActive(true);
            NPC4Chapter1.SetActive(true);

            NPC1Chapter2.SetActive(false);
            NPC2Chapter2.SetActive(false);
        }else if (chapter == 2)
        {
            NPC1Chapter1.SetActive(false);
            NPC2Chapter1.SetActive(false);
            NPC3Chapter1.SetActive(false);
            NPC4Chapter1.SetActive(false);

            NPC1Chapter2.SetActive(true);
            NPC2Chapter2.SetActive(true);
        }
    }

    private void Update()
    {
        int chapter = PlayerPrefs.GetInt("Chapter");
        if (chapter == 2)
        {
            NPC1Chapter1.SetActive(false);
            NPC2Chapter1.SetActive(false);
            NPC3Chapter1.SetActive(false);
            NPC4Chapter1.SetActive(false);

            NPC1Chapter2.SetActive(true);
            NPC2Chapter2.SetActive(true);
        }
    }
}
