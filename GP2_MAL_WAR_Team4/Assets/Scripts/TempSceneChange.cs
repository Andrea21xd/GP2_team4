using UnityEngine;
using UnityEngine.SceneManagement;

public class TempSceneChange : MonoBehaviour
{
    [Header("=== Scene Loader Name ===")]
    public int sceneLoader;

    private bool playerIsNear;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsNear = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        playerIsNear = false;
    }

    public void Update()
    {
        if (playerIsNear && Input.GetKeyUp(KeyCode.E))
        {
            ChangeScene();
        }
    }


    public void ChangeScene()
    {
        SceneManager.LoadScene(sceneLoader);
    }
}
