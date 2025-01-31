using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneTransition : MonoBehaviour
{
    public string sceneToLoad;
    public Vector3 playerPos;
    public VectorValue playerMemory;



    /// <summary>
    /// TODO
    /// Add to player movement script:
    ///     public VectorValue startingPos;
    /// 
    /// IN START
    ///     transform.position = startingPosition.initialValue;
    /// </summary>


    // If a UI button should change the scene for the player
    public void LoadScene()
    {
        playerMemory.initialValue = playerPos;
        SceneManager.LoadScene(sceneToLoad);
    }

    // If a collider trigger should change the scene for the player
    public void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") && !other.isTrigger)
        {
            playerMemory.initialValue = playerPos;
            SceneManager.LoadScene(sceneToLoad);
        }
    }

}
