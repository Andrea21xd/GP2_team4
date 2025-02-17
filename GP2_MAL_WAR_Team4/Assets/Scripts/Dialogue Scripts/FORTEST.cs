using UnityEngine;

public class FORTEST : MonoBehaviour
{
    private bool playerIsNear = false;
    public Actor actor;


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
            actor.neverTalked = false;
        }
    }
}
