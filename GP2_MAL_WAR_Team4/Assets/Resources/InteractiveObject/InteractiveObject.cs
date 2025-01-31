using UnityEngine;

public class InteractiveObject : MonoBehaviour
{
    [SerializeField] private GameObject interactiveMark;

    private void Start()
    {
        if (interactiveMark != null)
        {
            interactiveMark.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider otherCollider)
    {
        if (otherCollider.CompareTag("InteractiveObject"))
        {
            interactiveMark.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider otherCollider)
    {
        if (otherCollider.CompareTag("InteractiveObject"))
        {
            interactiveMark.SetActive(false);
        }
    }
}
