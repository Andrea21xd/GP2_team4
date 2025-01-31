using UnityEngine;

public class DontDestroy : MonoBehaviour
{
    /// <summary>
    /// A script that be put on parent objects that should be saved. If there is more object in another scene (same object) but the array is
    /// full then they will be deleted. So there arent any dublicates
    /// </summary>

    private static GameObject[] persistentObjects = new GameObject[3];  // create a array with objects that should be saved. 
    public int objectIndex;

    void Awake()
    {
        if(persistentObjects[objectIndex] == null)
        {
            persistentObjects[objectIndex] = gameObject;
            DontDestroyOnLoad(gameObject);
        }
        else if (persistentObjects[objectIndex] != gameObject)
        {
            Destroy(gameObject);
        }
    }
}
