using UnityEngine;

[System.Serializable]
public class DialogueResponse
{
    [TextArea(2, 10)]
    public string responseText;
    public DialogueNode nextNode;

    [Header("Quest")]
    [SerializeField] private QuestInfoSO questInfoForPoint;

    [Header("Config")]
    [SerializeField] private bool startPoint = false;
    [SerializeField] private bool finishPoint = false;

    private string questId;
    private QuestState currentQuestState;



    private void OnEnable()
    {
        GameEventsManager.Instance.questEvents.onQuestStateChange += QuestStateChange;

    }

    private void OnDisable()
    {
        GameEventsManager.Instance.questEvents.onQuestStateChange -= QuestStateChange;

    }

    public void OnClick()
    {
        if (!startPoint && !finishPoint) { Debug.Log("Not a quest respone"); return; }
        questId = questInfoForPoint.id;

        if (currentQuestState.Equals(QuestState.CAN_START) && startPoint)
        {
            Debug.Log("Quest start " +  questId);
            GameEventsManager.Instance.questEvents.StartQuest(questId);
        }
        else if (currentQuestState.Equals(QuestState.CAN_FINISH) && finishPoint)
        {
            Debug.Log("Quest start " + questId);
            GameEventsManager.Instance.questEvents.FinishQuest(questId);
        }else
        {
            Debug.Log("Not Right state");
            return;
        }
    }


    private void QuestStateChange(QuestClass quest)
    {
        if (quest.info.id.Equals(questId))
        {
            currentQuestState = quest.state;
            Debug.Log("Quest with id: " + questId + "updated to: " + currentQuestState);
        }
    }
}

