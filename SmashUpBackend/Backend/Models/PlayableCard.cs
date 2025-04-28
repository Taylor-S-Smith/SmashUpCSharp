using SmashUp.Backend.GameObjects;
using SmashUp.Backend.Services;

namespace SmashUp.Backend.Models;

internal enum PlayableCardType
{
    Minion,
    Action
}
internal enum Tag
{
    MinionAttachment
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
    public List<PlayableCard> Attachments { get; private set; } = [];
    public List<Tag> Tags { get; set; } = [];

    public enum ProtectionType
    {
        Destroy,
        Return,
        Attach
    }
    /// <summary>
    /// An object that represents a card's immunity against certain abilities
    /// </summary>
    /// <param name="From">The action the protection is for</param>
    /// <param name="CardType">The card type the protection is against. If null, it will protect against all card types</param>
    /// <param name="GrantedBy">The card that grants this protection, important for triggering OnProtect effects</param>
    public record Protection(ProtectionType From, PlayableCardType? CardType, PlayableCard GrantedBy);
    public List<Protection> Protections = [];

    public event Action<int> OnPowerChange = delegate { };
    public void TriggerOnPlay(Battle battle, BaseSlot? baseSlot=null) => OnPlay(battle, baseSlot);
    public event Action<Battle, BaseSlot?> OnPlay = delegate { };
    public void TriggerOnAddToBase(BaseCard baseCard) => OnAddToBase(baseCard);
    public event Action<BaseCard> OnAddToBase = delegate { };
    public void TriggerOnRemoveFromBase(BaseCard baseCard) => OnRemoveFromBase(baseCard);
    public event Action<BaseCard> OnRemoveFromBase = delegate { };
    public void TriggerOnRemoveFromBattleField(GlobalEventManager eventManager) => OnRemoveFromBattlefield(eventManager);
    public event Action<GlobalEventManager> OnRemoveFromBattlefield;
    public void TriggerOnProtect(Battle battle) => OnProtect(battle);
    public event Action<Battle> OnProtect;
    public void TriggerOnAttach(PlayableCard cardAttachedTo) => OnAttach(cardAttachedTo);
    public event Action<PlayableCard> OnAttach;
    public void TriggerOnDetach(PlayableCard cardDetatchedFrom) => OnDetach(cardDetatchedFrom);
    public event Action<PlayableCard> OnDetach;

    public void ChangeCurrentPower(int amountToChange)
    {
        if(CurrentPower != null)
        {
            amountToChange = Math.Max(amountToChange, 0 - (int)CurrentPower);

            OnPowerChange.Invoke(amountToChange);

            CurrentPower += amountToChange;
        }
    }
    public void Attach(PlayableCard cardToAttach)
    {
        Attachments.Add(cardToAttach);
    }
    public bool Detach(PlayableCard cardToDetach)
    {
        return Attachments.Remove(cardToDetach);
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
