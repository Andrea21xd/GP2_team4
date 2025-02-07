using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
public class VisitMultiplePointsStep : QuestStep
{
    [Header("Config")]
    [SerializeField] private string pointNumberString = "first";

    private bool playerNearby = false;

    public void OnEnable()
    {
        GameEventsManager.Instance.miscEvents.onInteractionPressed += InteractPressed;
    }

    public void OnDisable()
    {
        GameEventsManager.Instance.miscEvents.onInteractionPressed -= InteractPressed;
    }

    private void Start()
    {
        string status = "Visit the " + pointNumberString + " point.";
        ChangeState("", status);
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

    private void InteractPressed()
    {
        if (playerNearby)
        {
            string status = "The " + pointNumberString + " point has been visited.";
            ChangeState("", status);
            FinishQuestStep();
        }
    }

    protected override void SetQuestStepState(string state)
    {
        // no state is needed for this quest step
    }
}