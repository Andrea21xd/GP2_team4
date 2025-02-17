using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

    [Header("Config")]
    [SerializeField] private bool loadQuestState = true;

    private Dictionary<string, QuestClass> questMap;

    [SerializeField] private GameObject statChoiceUiPrefab;
    [SerializeField] private GameObject questUIPanelPrefab;

    private GameObject questObjectiveUIPanel;
    private Transform questObjectiveContainer;
    [SerializeField] private GameObject questObjectivePrefab;

    private Dictionary<string, GameObject> questObjectiveEntries = new Dictionary<string, GameObject>();

    //quest start requirements

    private int currentHealth;
    private int currentSpeed;
    private int currentLuck;
    private int currentBaseDamage;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        questMap = CreateQuestMap();

        if (questUIPanelPrefab != null)
        {
            questObjectiveUIPanel = Instantiate(questUIPanelPrefab);

            Transform canvasTransform = questObjectiveUIPanel.transform.Find("Canvas");
            Transform panelTransform = canvasTransform.Find("Panel");
            questObjectiveContainer = panelTransform.Find("QuestListContainer");
        }
    }

    private void Update()
    {
        foreach(QuestClass quest in questMap.Values)
        {
            if(quest.state == QuestState.REQUIREMENTS_NOT_MET && CheckRequirementsMet(quest))
            {
                ChangeQuestState(quest.info.id, QuestState.CAN_START);
            }
            if (quest.state == QuestState.IN_PROGRESS || quest.state == QuestState.CAN_FINISH)
            {
                DisplayQuestObjective(quest.info.id);
            }
        }
    }

    private void DisplayQuestObjective(string id)
    {
        QuestClass quest = GetQuestById(id);

        if (!questObjectiveEntries.ContainsKey(id))
        {
            GameObject newEntry = Instantiate(questObjectivePrefab, questObjectiveContainer);
            newEntry.GetComponent<TMP_Text>().text = quest.info.objective;
            questObjectiveEntries.Add(id, newEntry);
        }
    }

    private void OnEnable()
    {
        GameEventsManager.Instance.questEvents.onStartQuest += StartQuest;
        GameEventsManager.Instance.questEvents.onAdvanceQuest += AdvanceQuest;
        GameEventsManager.Instance.questEvents.onRestartQuest += RestartQuest;
        GameEventsManager.Instance.questEvents.onFinishQuest += FinishQuest;

        GameEventsManager.Instance.questEvents.onQuestStepStateChange += QuestStepStateChange;

        GameEventsManager.Instance.statEvents.onHealthChanged += PlayerHealthChange;
        GameEventsManager.Instance.statEvents.onSpeedChanged += PlayerSpeedChange;
        GameEventsManager.Instance.statEvents.onLuckChanged += PlayerLuckChange;
        GameEventsManager.Instance.statEvents.onBaseDamageChanged += PlayerBaseDamageChange;
    }

    private void OnDisable()
    {
        GameEventsManager.Instance.questEvents.onStartQuest -= StartQuest;
        GameEventsManager.Instance.questEvents.onAdvanceQuest -= AdvanceQuest;
        GameEventsManager.Instance.questEvents.onRestartQuest -= RestartQuest;
        GameEventsManager.Instance.questEvents.onFinishQuest -= FinishQuest;

        GameEventsManager.Instance.questEvents.onQuestStepStateChange -= QuestStepStateChange;

        GameEventsManager.Instance.statEvents.onHealthChanged-= PlayerHealthChange;
        GameEventsManager.Instance.statEvents.onSpeedChanged -= PlayerSpeedChange;
        GameEventsManager.Instance.statEvents.onLuckChanged -= PlayerLuckChange;
        GameEventsManager.Instance.statEvents.onBaseDamageChanged -= PlayerBaseDamageChange;
    }

    private void PlayerHealthChange(int health)
    {
        currentHealth = health;
    }

    private void PlayerSpeedChange(int speed)
    {
        currentSpeed = speed;
    }

    private void PlayerLuckChange(int luck)
    {
        currentLuck = luck;
    }

    private void PlayerBaseDamageChange(int baseDamage)
    {
        currentBaseDamage = baseDamage;
    }

    private bool CheckRequirementsMet(QuestClass quest)
    {
        bool meetsRequirements = true;

        if(currentHealth < quest.info.health)
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

        if (currentBaseDamage < quest.info.baseDamage)
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
        SaveQuest(quest);
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

        SaveQuest(quest);
    }

    private void RestartQuest(string id)
    {
        QuestClass quest = GetQuestById(id);
        quest.MoveToFirstStep();
        GameEventsManager.Instance.questEvents.QuestStateChange(quest);
    }

    private void FinishQuest(string id)
    {
        QuestClass quest = GetQuestById(id);
        ClaimRewards(quest);
        if (questObjectiveEntries.ContainsKey(id))
        {
            Destroy(questObjectiveEntries[id]);
            questObjectiveEntries.Remove(id);
        }
        ChangeQuestState(quest.info.id, QuestState.FINISHED);
        SaveQuest(quest);

    }

    private void ClaimRewards(QuestClass quest)
    {
        GameEventsManager.Instance.statEvents.HealthGained(quest.info.healthReward);
        GameEventsManager.Instance.statEvents.SpeedGained(quest.info.speedReward);
        GameEventsManager.Instance.statEvents.LuckGained(quest.info.luckReward);
        GameEventsManager.Instance.statEvents.BaseDamageGained(quest.info.baseDamageReward);

        if (quest.info.playerChoosesReward)
        {
            if (statChoiceUiPrefab != null)
            {
                Instantiate(statChoiceUiPrefab, transform.position, Quaternion.identity);
            }
        }
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
                continue;
            }
            idToQuestMap.Add(questInfo.id, LoadQuest(questInfo));
        }
        return idToQuestMap;
    }

    public QuestClass GetQuestById(string id)
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
            PlayerPrefs.Save();
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
