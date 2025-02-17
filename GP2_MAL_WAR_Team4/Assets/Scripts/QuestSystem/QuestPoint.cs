using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
public class QuestPoint : MonoBehaviour
{
    [Header("Quest")]
    [SerializeField] private QuestInfoSO questInfoForPoint;

    [Header("Config")]
    [SerializeField] private bool startPoint = true;
    [SerializeField] private bool finishPoint = true;

    private bool playerIsNear = false;

    private string questId;
    private QuestState currentQuestState;

    private void Awake()
    {
        questId = questInfoForPoint.id;
    }

    private void OnEnable()
    {
        GameEventsManager.Instance.questEvents.onQuestStateChange += QuestStateChange;
        GameEventsManager.Instance.miscEvents.onInteractionPressed += SubmitPressed;
    }

    private void OnDisable()
    {
        GameEventsManager.Instance.questEvents.onQuestStateChange -= QuestStateChange;
        GameEventsManager.Instance.miscEvents.onInteractionPressed -= SubmitPressed;
    }

    private void SubmitPressed()
    {
        if (!playerIsNear)
        {
            return;
        }
        if (currentQuestState.Equals(QuestState.CAN_START) && startPoint) 
        {
            GameEventsManager.Instance.questEvents.StartQuest(questId);

        } else if (currentQuestState.Equals(QuestState.CAN_FINISH) && finishPoint)
        {

            GameEventsManager.Instance.questEvents.FinishQuest(questId);

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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsNear = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsNear = false;
        }
    }

    public bool GetStartPoint()
    {
        return startPoint;
    }

    public bool GetFinishPoint()
    {
        return finishPoint;
    }

    public QuestState GetCurrentQuestState()
    {
        return currentQuestState;
    }
}
