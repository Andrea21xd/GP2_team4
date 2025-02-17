using UnityEngine;

public class GameEventsManager : MonoBehaviour
{
    public static GameEventsManager Instance {  get; private set; }

    public QuestEvents questEvents;
    public StatEvents statEvents;
    public MiscEvents miscEvents;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Found more than one GameEventManager in the scene");
        }
        Instance = this;

        miscEvents = new MiscEvents();
        questEvents = new QuestEvents();
        statEvents = new StatEvents();
    }
}
