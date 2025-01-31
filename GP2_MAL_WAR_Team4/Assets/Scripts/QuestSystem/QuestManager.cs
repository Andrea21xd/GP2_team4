using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class QuestManager : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private bool loadQuestState = false;

    private Dictionary<string, QuestClass> questMap;

    //quest start requirements

    private int currentCharisma;
    private int currentSpeed;
    private int currentLuck;

    private void Awake()
    {
        questMap = CreateQuestMap();
    }

    private void Update()
    {
        foreach(QuestClass quest in questMap.Values)
        {
            if(quest.state == QuestState.REQUIREMENTS_NOT_MET && CheckRequirementsMet(quest))
            {
                ChangeQuestState(quest.info.id, QuestState.CAN_START);
            }
        }
    }

    private void OnEnable()
    {
        GameEventsManager.Instance.questEvents.onStartQuest += StartQuest;
        GameEventsManager.Instance.questEvents.onAdvanceQuest += AdvanceQuest;
        GameEventsManager.Instance.questEvents.onFinishQuest += FinishQuest;

        GameEventsManager.Instance.questEvents.onQuestStepStateChange += QuestStepStateChange;

        GameEventsManager.Instance.statEvents.onCharismaChanged += PlayerCharismaChange;
        GameEventsManager.Instance.statEvents.onSpeedChanged += PlayerSpeedChange;
        GameEventsManager.Instance.statEvents.onLuckChanged += PlayerLuckChange;
    }

    private void OnDisable()
    {
        GameEventsManager.Instance.questEvents.onStartQuest -= StartQuest;
        GameEventsManager.Instance.questEvents.onAdvanceQuest -= AdvanceQuest;
        GameEventsManager.Instance.questEvents.onFinishQuest -= FinishQuest;

        GameEventsManager.Instance.questEvents.onQuestStepStateChange -= QuestStepStateChange;

        GameEventsManager.Instance.statEvents.onCharismaChanged-= PlayerCharismaChange;
        GameEventsManager.Instance.statEvents.onSpeedChanged -= PlayerSpeedChange;
        GameEventsManager.Instance.statEvents.onLuckChanged -= PlayerLuckChange;
    }

    private void PlayerCharismaChange(int charisma)
    {
        currentCharisma = charisma;
    }

    private void PlayerSpeedChange(int speed)
    {
        currentSpeed = speed;
    }

    private void PlayerLuckChange(int luck)
    {
        currentLuck = luck;
    }

    private bool CheckRequirementsMet(QuestClass quest)
    {
        bool meetsRequirements = true;

        if(currentCharisma < quest.info.charisma)
        {
            meetsRequirements = false;
        }

        if (currentSpeed < quest.info.speed)
        {
            meetsRequirements = false;
        }

        if (currentLuck < quest.info.luck)
        {
            meetsRequirements = false;
        }

        foreach (QuestInfoSO prerequisiteQuestInfo in quest.info.questPrerequisites)
        {
            if (GetQuestById(prerequisiteQuestInfo.id).state != QuestState.FINISHED)
            {
                meetsRequirements = false;
                break;
            }
        }

        return meetsRequirements;
    }

    private void Start()
    {
        //broadcast initial state of all quests
        foreach (QuestClass quest in questMap.Values)
        {
            if(quest.state == QuestState.IN_PROGRESS)
            {
                quest.InstantiateCurrentQuestStep(this.transform);
            }
            GameEventsManager.Instance.questEvents.QuestStateChange(quest);
        }
    }

    private void ChangeQuestState(string id, QuestState state)
    {
        QuestClass quest = GetQuestById(id);
        quest.state = state;
        GameEventsManager.Instance.questEvents.QuestStateChange(quest);
    }

    private void StartQuest(string id)
    {
        QuestClass quest = GetQuestById(id);
        quest.InstantiateCurrentQuestStep(this.transform);
        ChangeQuestState(quest.info.id, QuestState.IN_PROGRESS);
    }

    private void AdvanceQuest(string id)
    {
        QuestClass quest = GetQuestById(id);
        quest.MoveToNextStep();

        if (quest.CurrentStepExists())
        {
            quest.InstantiateCurrentQuestStep(this.transform);
        } else
        {
            ChangeQuestState(quest.info.id, QuestState.CAN_FINISH);
        }
    }

    private void FinishQuest(string id)
    {
        QuestClass quest = GetQuestById(id);
        ClaimRewards(quest);
        ChangeQuestState(quest.info.id, QuestState.FINISHED);
    }

    private void ClaimRewards(QuestClass quest)
    {
        GameEventsManager.Instance.statEvents.CharismaGained(quest.info.charismaReward);
        GameEventsManager.Instance.statEvents.SpeedGained(quest.info.speedReward);
        GameEventsManager.Instance.statEvents.LuckGained(quest.info.luckReward);
    }

    private void QuestStepStateChange(string id, int stepIndex, QuestStepState questStepState)
    {
        QuestClass quest = GetQuestById(id);
        quest.StoreQuestStepState(questStepState, stepIndex);
        ChangeQuestState(id, quest.state);
    }


    private Dictionary<string, QuestClass> CreateQuestMap()
    {
        QuestInfoSO[] allQuests = Resources.LoadAll<QuestInfoSO>("Quests");

        Dictionary<string, QuestClass> idToQuestMap = new Dictionary<string, QuestClass>();
        foreach(QuestInfoSO questInfo in allQuests)
        {
            if (idToQuestMap.ContainsKey(questInfo.id))
            {
                Debug.LogWarning("Duplicate ID: " + questInfo.id);
            }
            idToQuestMap.Add(questInfo.id, LoadQuest(questInfo));
        }
        return idToQuestMap;
    }

    private QuestClass GetQuestById(string id)
    {
        QuestClass quest = questMap[id];
        if (quest == null)
        {
            Debug.LogError("ID not found in Quest Map: " + id);
        }
        return quest;
    }

    private void SaveQuest(QuestClass quest)
    {
        try
        {
            QuestData questData = quest.GetQuestData();
            string serializedData = JsonUtility.ToJson(questData);
            PlayerPrefs.SetString(quest.info.id, serializedData);
        }
        catch (System.Exception e) 
        {
            Debug.Log("Failed to save quest" + e);
        }
    }

    private QuestClass LoadQuest(QuestInfoSO questInfo)
    {
        QuestClass quest = null;
        try
        {
            if (PlayerPrefs.HasKey(questInfo.id) && loadQuestState)
            {
                string serializedData = PlayerPrefs.GetString(questInfo.id);
                QuestData questData = JsonUtility.FromJson<QuestData>(serializedData);
                quest = new QuestClass(questInfo, questData.state, questData.questStepIndex, questData.questStepStates);
            }
            else
            {
                quest = new QuestClass(questInfo);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to load quest with id " + quest.info.id + ": " + e);
        }
        return quest;
    }

}
