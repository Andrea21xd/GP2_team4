using System;

public class MiscEvents
{
    public event Action onInteractionPressed;
    public void InteractionPressed()
    {
        onInteractionPressed?.Invoke();
    }
}
