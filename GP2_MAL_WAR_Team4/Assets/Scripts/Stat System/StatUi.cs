using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatUi : MonoBehaviour
{
    [SerializeField] private Button speedButton;
    [SerializeField] private Button luckButton;

    private StatManager statManager;

    private void Start()
    {
        statManager = FindAnyObjectByType<StatManager>();

        speedButton.onClick.AddListener(() => IncreaseSpeed());
        luckButton.onClick.AddListener(() => IncreaseLuck());
    }

    private void IncreaseSpeed()
    {
        statManager.OnSpeedGained(1);
        Destroy(gameObject);
    }

    private void IncreaseLuck()
    {
        statManager.OnLuckGained(1);
        Destroy(gameObject);
    }
}
