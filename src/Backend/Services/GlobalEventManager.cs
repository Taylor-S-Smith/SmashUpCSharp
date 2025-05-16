using SmashUp.Backend.GameObjects;
using SmashUp.Backend.Models;

namespace SmashUp.Backend.Services;

internal class GlobalEventManager
{

    public event Action<Battle, ActivePlayer> StartOfTurn = delegate { };
    public event Action<Battle, ActivePlayer> EndOfTurn = delegate { };
    public event Action<Battle, PlayableCard> AfterAddCard = delegate { };
    public event Action<Battle, PlayableCard> AfterRemoveCard = delegate { };

    public void TriggerStartOfTurn(Battle battle, ActivePlayer activePlayer) => StartOfTurn.Invoke(battle, activePlayer);
    public void TriggerEndOfTurn(Battle battle, ActivePlayer activePlayer) => EndOfTurn.Invoke(battle, activePlayer);
    public void TriggerAfterAddCard(Battle battle, PlayableCard card) => AfterAddCard.Invoke(battle, card);
    public void TriggerAfterRemoveCard(Battle battle, PlayableCard card) => AfterRemoveCard.Invoke(battle, card);

}
