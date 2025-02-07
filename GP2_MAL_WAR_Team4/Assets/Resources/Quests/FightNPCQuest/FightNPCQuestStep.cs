using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
public class FightNPCQuestStep : QuestStep
{
    public void Start()
    {
        FinishQuestStep();
    }

    public void Update()
    {
        //TO DO: Get info about battle

        bool hadFight = true;
        bool hasWon = true;

        if (hasWon)
        {
            FinishQuestStep();
        }
        else
        {
            RestartQuest();
        }
    }

    protected override void SetQuestStepState(string state)
    {

    }
}

