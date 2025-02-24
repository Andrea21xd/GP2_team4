using System;

public class QuestEvents
{
    public event Action<string> onStartQuest;

    public void StartQuest(string id)
    {
        if (onStartQuest != null)
        {
            onStartQuest(id);
        }
    }

    public event Action<string> onAdvanceQuest;

    public void AdvanceQuest(string id)
    {
        if (onAdvanceQuest != null)
        {
            onAdvanceQuest(id);
        }
    }

    public event Action<string> onRestartQuest;

    public void RestartQuest(string id)
    {
        if (onRestartQuest != null)
        {
            onRestartQuest(id);
        }
    }


    public event Action<string> onFinishQuest;

    public void FinishQuest(string id)
    {
        if (onFinishQuest != null)
        {
            onFinishQuest(id);
        }
    }


    public event Action<QuestClass> onQuestStateChange;

    public void QuestStateChange(QuestClass quest)
    {
        if (onQuestStateChange != null)
        {
            onQuestStateChange(quest);
        }
    }

    public event Action<string, int, QuestStepState> onQuestStepStateChange;

    public void QuestStepStateChange(string id, int stepIndex, QuestStepState questStepState)
    {
        if (onQuestStepStateChange != null)
        {
            onQuestStepStateChange(id, stepIndex, questStepState);
        }
    }

    public QuestState GetQuestState(string questId)
    {
        return QuestManager.Instance.GetQuestById(questId).state;
    }
}
