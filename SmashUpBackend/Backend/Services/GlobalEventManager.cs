using SmashUp.Backend.GameObjects;
using SmashUp.Backend.Models;

namespace SmashUp.Backend.Services;

internal class GlobalEventManager
{
    

    public event Action<ActivePlayer> StartOfTurn = delegate { };
    public event Action<ActivePlayer> EndOfTurn = delegate { };

    public void TriggerStartOfTurn(ActivePlayer activePlayer)
    {
        StartOfTurn.Invoke(activePlayer);
    }
    public void TriggerEndOfTurn(ActivePlayer activePlayer)
    {
        EndOfTurn.Invoke(activePlayer);
    }

}
