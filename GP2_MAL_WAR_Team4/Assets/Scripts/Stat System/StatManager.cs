using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatManager : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private int startingCharisma = 1;
    [SerializeField] private int startingSpeed = 1;
    [SerializeField] private int startingLuck = 1;

    private int currentCharisma;
    private int currentSpeed;
    private int currentLuck;

    private void Awake()
    {
        currentCharisma = startingCharisma;
        currentSpeed = startingSpeed;
        currentLuck = startingLuck;
    }

    private void OnEnable()
    {
        GameEventsManager.Instance.statEvents.onCharismaGained += OnCharismaGained;
        GameEventsManager.Instance.statEvents.onSpeedGained += OnSpeedGained;
        GameEventsManager.Instance.statEvents.onLuckGained += OnLuckGained;
    }

    private void OnDisable()
    {
        GameEventsManager.Instance.statEvents.onCharismaGained -= OnCharismaGained;
        GameEventsManager.Instance.statEvents.onSpeedGained -= OnSpeedGained;
        GameEventsManager.Instance.statEvents.onLuckGained -= OnLuckGained;
    }

    private void Start()
    {
        // Notify listeners of the initial stat values
        GameEventsManager.Instance.statEvents.CharismaChanged(currentCharisma);
        GameEventsManager.Instance.statEvents.SpeedChanged(currentSpeed);
        GameEventsManager.Instance.statEvents.LuckChanged(currentLuck);
    }

    private void OnCharismaGained(int charisma)
    {
        currentCharisma += charisma;
        GameEventsManager.Instance.statEvents.CharismaChanged(currentCharisma);
    }

    private void OnSpeedGained(int speed)
    {
        currentSpeed += speed;
        GameEventsManager.Instance.statEvents.SpeedChanged(currentSpeed);
    }

    private void OnLuckGained(int luck)
    {
        currentLuck += luck;
        GameEventsManager.Instance.statEvents.LuckChanged(currentLuck);
    }
}


