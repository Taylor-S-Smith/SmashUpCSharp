namespace Backend.Services;

internal class EventManager
{
    public event EventHandler<EventArgs> StartOfTurn = delegate { };

    public void TriggerStartOfTurn()
    {
        StartOfTurn.Invoke(this, EventArgs.Empty);
    }

}
