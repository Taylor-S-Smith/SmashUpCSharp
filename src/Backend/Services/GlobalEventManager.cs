using SmashUp.Backend.GameObjects;

namespace SmashUp.Backend.Services;

internal class GlobalEventManager
{
    

    public event Action<Battle, ActivePlayer> StartOfTurn = delegate { };
    public event Action<Battle, ActivePlayer> EndOfTurn = delegate { };

    public void TriggerStartOfTurn(Battle battle, ActivePlayer activePlayer)
    {
        StartOfTurn.Invoke(battle, activePlayer);
    }
    public void TriggerEndOfTurn(Battle battle, ActivePlayer activePlayer)
    {
        EndOfTurn.Invoke(battle, activePlayer);
    }

}
