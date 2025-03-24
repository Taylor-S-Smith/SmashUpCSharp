namespace SmashUp.Backend.Services;

internal class EventManager
{
    public event EventHandler<EventArgs> StartOfTurn = delegate { };
    public event EventHandler<EventArgs> EndOfTurn = delegate { };

    public void TriggerStartOfTurn()
    {
        StartOfTurn.Invoke(this, EventArgs.Empty);
    }
    public void TriggerEndOfTurn()
    {
        StartOfTurn.Invoke(this, EventArgs.Empty);
    }

}
