using System.Formats.Asn1;
using SmashUp.Backend.GameObjects;
using SmashUp.Backend.Models;
using static SmashUp.Backend.Models.PlayableCard;

namespace SmashUp.Backend.Services;

internal class GlobalEventManager
{

    public event Action<Battle, ActivePlayer> StartOfTurn = delegate { };
    public event Action<Battle, ActivePlayer> EndOfTurn = delegate { };

    public event Action<Battle, PlayableCard> CardEnteredBattlefield = delegate { };
    public event Action<Battle, PlayableCard, BaseSlot> CardEnteredBase = delegate { };
    public event Action<Battle, PlayableCard, BaseSlot> CardExitedBase = delegate { };
    public event Action<Battle, PlayableCard> CardExitedBattlefield = delegate { };
    public event Action<Battle, PlayableCard> BeforeDestroyCard = delegate { };
    public event Action<Battle, PlayableCard> OnDestroyCard = delegate { };

    public event Action<Battle, PlayableCard, Tag> AfterAddTag = delegate { };
    public event Action<Battle, PlayableCard, Tag> AfterRemoveTag = delegate { };
    public event Action<Battle, PlayableCard, Player, Player> ControllerChange = delegate { };

    public event Action<Battle, BaseSlot> BeforeBaseScores = delegate { };
    public event Action<Battle, BaseSlot> AfterBaseScores = delegate { };

    public void TriggerStartOfTurn(Battle battle, ActivePlayer activePlayer) => StartOfTurn.Invoke(battle, activePlayer);
    public void TriggerEndOfTurn(Battle battle, ActivePlayer activePlayer) => EndOfTurn.Invoke(battle, activePlayer);

    public void TriggerCardEnteredBattlefield(Battle battle, PlayableCard card) => CardEnteredBattlefield.Invoke(battle, card);
    public void TriggerCardEnteredBase(Battle battle, PlayableCard card, BaseSlot baseSlot) => CardEnteredBase.Invoke(battle, card, baseSlot);
    public void TriggerCardExitedBattlefield(Battle battle, PlayableCard card) => CardExitedBattlefield.Invoke(battle, card);
    public void TriggerCardExitedBase(Battle battle, PlayableCard card, BaseSlot baseSlot) => CardExitedBase.Invoke(battle, card, baseSlot);
    public void TriggerBeforeDestroyCard(Battle battle, PlayableCard card) => BeforeDestroyCard.Invoke(battle, card);
    public void TriggerOnDestroyCard(Battle battle, PlayableCard card) => OnDestroyCard.Invoke(battle, card);

    public void TriggerAfterAddTag(Battle battle, PlayableCard card, Tag tag) => AfterAddTag.Invoke(battle, card, tag);
    public void TriggerAfterRemoveTag(Battle battle, PlayableCard card, Tag tag) => AfterRemoveTag.Invoke(battle, card, tag);
    public void TriggerControllerChange(Battle battle, PlayableCard card, Player oldController, Player newController) => ControllerChange.Invoke(battle, card, oldController, newController);


    public void TriggerBeforeBaseScores(Battle battle, BaseSlot slot) => BeforeBaseScores.Invoke(battle, slot);
    public void TriggerAfterBaseScores(Battle battle, BaseSlot slot) => AfterBaseScores.Invoke(battle, slot);

}
