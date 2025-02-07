using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatManager : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private int startingHealth = 10;
    [SerializeField] private int startingSpeed = 5;
    [SerializeField] private int startingLuck = 5;
    [SerializeField] private int startingBaseDamage = 2;
    
    private int currentHealth;
    private int currentSpeed;
    private int currentLuck;
    private int currentBaseDamage;


    private void Awake()
    {
        currentHealth = startingHealth;
        currentSpeed = startingSpeed;
        currentLuck = startingLuck;
        currentBaseDamage = startingBaseDamage;
    }

    private void OnEnable()
    {
        GameEventsManager.Instance.statEvents.onHealthGained += OnHealthGained;
        GameEventsManager.Instance.statEvents.onSpeedGained += OnSpeedGained;
        GameEventsManager.Instance.statEvents.onLuckGained += OnLuckGained;
        GameEventsManager.Instance.statEvents.onBaseDamageGained += OnBaseDamageGained;
    }

    private void OnDisable()
    {
        GameEventsManager.Instance.statEvents.onHealthGained -= OnHealthGained;
        GameEventsManager.Instance.statEvents.onSpeedGained -= OnSpeedGained;
        GameEventsManager.Instance.statEvents.onLuckGained -= OnLuckGained;
        GameEventsManager.Instance.statEvents.onBaseDamageGained -= OnBaseDamageGained;
    }

    private void Start()
    {
        // Notify listeners of the initial stat values
        GameEventsManager.Instance.statEvents.HealthChanged(currentHealth);
        GameEventsManager.Instance.statEvents.SpeedChanged(currentSpeed);
        GameEventsManager.Instance.statEvents.LuckChanged(currentLuck);
        GameEventsManager.Instance.statEvents.LuckChanged(currentBaseDamage);
        PlayerPrefs.SetInt("currentHealth", currentHealth);
        PlayerPrefs.SetInt("currentSpeed", currentSpeed);
        PlayerPrefs.SetInt("currentLuck", currentLuck);
        PlayerPrefs.SetInt("currentBaseDamage", currentBaseDamage);
        PlayerPrefs.Save();
    }

    public void OnHealthGained(int health)
    {
        currentHealth += health;
        GameEventsManager.Instance.statEvents.HealthChanged(currentHealth);
        PlayerPrefs.SetInt("currentHealth", currentHealth);
        PlayerPrefs.Save();
    }

    public void OnSpeedGained(int speed)
    {
        currentSpeed += speed;
        GameEventsManager.Instance.statEvents.SpeedChanged(currentSpeed);
        PlayerPrefs.SetInt("currentSpeed", currentSpeed);
        PlayerPrefs.Save();

    }

    public void OnLuckGained(int luck)
    {
        currentLuck += luck;
        GameEventsManager.Instance.statEvents.LuckChanged(currentLuck);
        PlayerPrefs.SetInt("currentLuck", currentLuck);
        PlayerPrefs.Save();

    }

    public void OnBaseDamageGained(int baseDamage)
    {
        currentBaseDamage += baseDamage;
        GameEventsManager.Instance.statEvents.BaseDamageChanged(currentBaseDamage);
        PlayerPrefs.SetInt("currentBaseDamage", currentBaseDamage);
        PlayerPrefs.Save();
    }

    
    public int GetHealth()
    {
        Debug.Log("GetHealth" +  currentHealth);
        return currentHealth;
    }

    public int GetSpeed()
    {
        return currentSpeed;
    }

    public int GetLuck()
    {
        return currentLuck;
    }

    public int GetBaseDamage()
    {
        return currentBaseDamage;
    }
}


