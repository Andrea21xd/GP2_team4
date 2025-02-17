using UnityEngine;

public class HideCharacters : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private bool visibleInChapterOne = true;
    [SerializeField] private bool visibleInChapterTwo = true;

    void Start()
    {
        int chapter = PlayerPrefs.GetInt("Chapter");

        if (chapter == 2)
        {
            if (visibleInChapterTwo == true)
            {
                this.gameObject.SetActive(true);
            } else
            {
                this.gameObject.SetActive(false);
            }
        } else
        {
            if (visibleInChapterOne == true)
            {
                this.gameObject.SetActive(true);
            }
            else
            {
                this.gameObject.SetActive(false);
            }
        }
    }
}
