using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalToBattle : MonoBehaviour
{
    private bool canEnter = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canEnter = true;
            Debug.Log("Savaş bölgesine geçmek için F tuşuna bas.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canEnter = false;
        }
    }

    private void Update()
    {
        if (canEnter && Input.GetKeyDown(KeyCode.F))
        {
            // BattleMap sahnesine geçiş
            SceneManager.LoadScene("BattleMap");
        }
    }
}
