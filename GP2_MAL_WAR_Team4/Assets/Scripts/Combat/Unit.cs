using UnityEngine;

public class Unit : MonoBehaviour
{
    public string unitName;

    public int damage;
    [HideInInspector]
    public int critAttackChance;
    [HideInInspector]
    public int evasionChance;
    [HideInInspector]
    public int critDamage;
    public int critModifire;
    public int maxHP;
    public int currentHP;
    public int luck;
    public int speed;
    public int charisma;


    void Start()
    {
        critAttackChance = 10 * luck;
        critDamage = critModifire * damage;
        evasionChance = (speed + (luck / 2));
    }


    public bool TakeDamage(int damage)
    {
        currentHP -= damage;

        if (currentHP <= 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
