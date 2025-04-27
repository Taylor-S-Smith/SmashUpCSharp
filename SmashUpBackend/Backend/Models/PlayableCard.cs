using SmashUp.Backend.GameObjects;
using SmashUp.Backend.Services;

namespace SmashUp.Backend.Models;

internal enum PlayableCardType
{
    minion,
    action
}

/// <summary>
/// Any card that can by played by a Player
/// </summary>
internal class PlayableCard : Card
{
    public Player Owner { get; set; } = null!;
    public PlayableCardType CardType { get; }
    public int? PrintedPower { get; set; }
    public int? CurrentPower { get; private set; }

    public event Action<int> OnPowerChange = delegate { };
    public void TriggerOnPlay(Battle battle, BaseSlot? baseSlot=null) => OnPlay(battle, baseSlot);
    public event Action<Battle, BaseSlot?> OnPlay = delegate { };
    public void TriggerOnAddToBase(BaseCard baseCard) => OnAddToBase(baseCard);
    public event Action<BaseCard> OnAddToBase = delegate { };
    public void TriggerOnRemoveFromBase(BaseCard baseCard) => OnRemoveFromBase(baseCard);
    public event Action<BaseCard> OnRemoveFromBase = delegate { };
    public void TriggerOnRemoveFromBattleField(GlobalEventManager eventManager) => OnRemoveFromBattlefield(eventManager);
    public event Action<GlobalEventManager> OnRemoveFromBattlefield;

    public void ChangeCurrentPower(int amountToChange)
    {
        if(CurrentPower != null)
        {
            amountToChange = Math.Max(amountToChange, 0 - (int)CurrentPower);

            OnPowerChange.Invoke(amountToChange);

            CurrentPower += amountToChange;
        }
    }

    public PlayableCard(Faction faction, PlayableCardType cardType, string name, string[] graphic, int? power=null) : base(faction, name, graphic)
    {
        CardType = cardType;
        PrintedPower = power;
        CurrentPower = power;

        OnRemoveFromBattlefield = (eventManager) =>
        {
            CurrentPower = PrintedPower;
        };
    }
}
