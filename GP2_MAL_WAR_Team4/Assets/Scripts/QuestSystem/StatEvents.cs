using System;

public class StatEvents
{
    public event Action<int> onCharismaGained;
    public void CharismaGained(int charisma)
    {
        onCharismaGained?.Invoke(charisma);
    }

    public event Action<int> onCharismaChanged;
    public void CharismaChanged(int charisma)
    {
        onCharismaChanged?.Invoke(charisma);
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
}
