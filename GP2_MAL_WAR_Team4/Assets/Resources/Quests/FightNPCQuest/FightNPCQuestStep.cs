using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
public class FightNPCQuestStep : QuestStep
{
    [SerializeField] int EnemyId;

    private void Start()
    {
        if (!PlayerPrefs.HasKey("HasFought"))
        {
            PlayerPrefs.SetInt("HasFought", 1);
            PlayerPrefs.Save();
        }

        string NPCstate = PlayerPrefs.GetString("Dead");
        bool hasFought = PlayerPrefs.GetInt("HasFought") == 0;
        bool hasWon = false;

        if (NPCstate == "isDead" && EnemyId == PlayerPrefs.GetInt("EnemyId"))
        {
            hasWon = true;
        }

        if (hasWon)
        {
            FinishQuestStep();
            string flag = "Enemy" + EnemyId + "Alive";
            PlayerPrefs.SetInt(flag, 0);

            if (EnemyId == 2)
            {
                PlayerPrefs.SetInt("Chapter", 2);
            }
        }
        else if (hasFought)
        {
            RestartQuest();
            Debug.Log("QuestRestart");
        }
    }

    protected override void SetQuestStepState(string state)
    {

    }
}

