using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stats : MonoBehaviour
{
    [SerializeField] private int health;
    [SerializeField] private int speed;
    [SerializeField] private int luck;
    [SerializeField] private int baseDamage;

    public Stats(int health,  int speed, int luck, int baseDamage)
    {
        this.health = health;
        this.speed = speed;
        this.luck = luck;
        this.baseDamage = baseDamage;
    }

    public int GetHealth()
    {
        return health;
    }

    public int GetSpeed()
    {
        return speed;
    }

    public int GetLuck()
    {
        return luck;
    }

    public int GetBaseDamage()
    {
        return baseDamage;
    }
}
