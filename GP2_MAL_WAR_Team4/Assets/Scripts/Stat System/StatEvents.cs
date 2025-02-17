using System;

public class StatEvents
{
    public event Action<int> onHealthGained;
    public void HealthGained(int health)
    {
        onHealthGained?.Invoke(health);
    }

    public event Action<int> onHealthChanged;
    public void HealthChanged(int health)
    {
        onHealthChanged?.Invoke(health);
    }

    public event Action<int> onSpeedGained;
    public void SpeedGained(int speed)
    {
        onSpeedGained?.Invoke(speed);
    }

    public event Action<int> onSpeedChanged;
    public void SpeedChanged(int speed)
    {
        onSpeedChanged?.Invoke(speed);
    }

    public event Action<int> onLuckGained;
    public void LuckGained(int luck)
    {
        onLuckGained?.Invoke(luck);
    }

    public event Action<int> onLuckChanged;
    public void LuckChanged(int luck)
    {
        onLuckChanged?.Invoke(luck);
    }

    public event Action<int> onBaseDamageGained;
    public void BaseDamageGained(int baseDamage)
    {
        onBaseDamageGained?.Invoke(baseDamage);
    }

    public event Action<int> onBaseDamageChanged;
    public void BaseDamageChanged(int baseDamage)
    {
        onBaseDamageChanged?.Invoke(baseDamage);
    }
}
