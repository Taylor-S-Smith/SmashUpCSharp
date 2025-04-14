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
    public event Action<BaseSlot> OnPlay = delegate { };
    public void TriggerOnPlay(BaseSlot baseSlot) => OnPlay(baseSlot);
    public event Action<BaseCard> OnAddToBase = delegate { };
    public void TriggerOnAddToBase(BaseCard baseCard) => OnAddToBase(baseCard);
    public event Action<BaseCard> OnRemoveFromBase = delegate { };
    public void TriggerOnRemoveFromBase(BaseCard baseCard) => OnRemoveFromBase(baseCard);
    public event Action OnRemoveFromBattlefield;

    public void ChangeCurrentPower(int amountToChange)
    {
        if(CurrentPower != null)
        {
            amountToChange = Math.Max(amountToChange, 0 - (int)CurrentPower);

            OnPowerChange.Invoke(amountToChange);

            CurrentPower += amountToChange;
        }
    }

    public PlayableCard(Faction faction, PlayableCardType cardType, string name, string[] graphic, int? power) : base(faction, name, graphic)
    {
        CardType = cardType;
        PrintedPower = power;
        CurrentPower = power;

        OnRemoveFromBattlefield = () =>
        {
            CurrentPower = PrintedPower;
        };
    }
}
