using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CapsuleCollider))]
public class CollectItems : QuestStep
{
    private bool playerNearby = false;

    public void OnEnable()
    {
        GameEventsManager.Instance.miscEvents.onInteractionPressed += InteractPressed;
    }

    public void OnDisable()
    {
        GameEventsManager.Instance.miscEvents.onInteractionPressed -= InteractPressed;
    }

    private void OnTriggerEnter(Collider otherCollider)
    {
        if (otherCollider.CompareTag("Player"))
        {
            playerNearby = true;
        }
    }

    private void OnTriggerExit(Collider otherCollider)
    {
        if (otherCollider.CompareTag("Player"))
        {
            playerNearby = false;
        }
    }

    public void InteractPressed()
    {
        if (playerNearby)
        {
            FinishQuestStep();
        }
    }

    protected override void SetQuestStepState(string state)
    {
        // No state is needed for this quest step
    }
}
