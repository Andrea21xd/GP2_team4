using UnityEngine;
using UnityEngine.SceneManagement;

public class FORTESTSCENECHANGE : MonoBehaviour
{
    private bool playerIsNear = false;
    public int sceneName;
    public GameObject duck;

    public Vector3 playerPos;
    public VectorValue playerMemory;
  



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
            playerPos = duck.transform.position;
            playerMemory.initialValue = playerPos;
            SceneManager.LoadScene(sceneName); 
        }
    }
}
