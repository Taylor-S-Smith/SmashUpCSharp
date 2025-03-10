namespace SmashUpBackend.Models.Cards;

internal enum TriggerType
{
    onplay
}

internal class CardTrigger(TriggerType type, Action effect)
{
    public TriggerType Type { get; set; } = type;

    private event Action Effects = effect;

    public void Trigger()
    {
        Effects?.Invoke();
    }

    public void Add(Action effect)
    {
        Effects += effect;
    }
}
