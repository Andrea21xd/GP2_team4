using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
public class VisitPointsStep : QuestStep
{
    public void Start()
    {
        FinishQuestStep();
    }

    protected override void SetQuestStepState(string state)
    {
        // no state is needed for this quest step
    }
}
