using SmashUp.Backend.Models;

namespace SmashUp.Backend.Services;

internal class GlobalEventManager
{
    

    public event Action<EventArgs> StartOfTurn = delegate { };
    public event Action<EventArgs> EndOfTurn = delegate { };

    public void TriggerStartOfTurn()
    {
        StartOfTurn.Invoke(EventArgs.Empty);
    }
    public void TriggerEndOfTurn()
    {
        StartOfTurn.Invoke(EventArgs.Empty);
    }

}
