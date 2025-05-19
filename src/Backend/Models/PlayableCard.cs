using LinqKit;
using SmashUp.Backend.GameObjects;

namespace SmashUp.Backend.Models;

internal enum PlayableCardType
{
    Minion,
    Action
}
internal enum PlayLocation
{
    Base,
    Discard,
    Minion
}

/// <summary>
/// Any card that can by played by a Player
/// </summary>
internal class PlayableCard : Card
{
    public Player Owner { get; private set; } = null!;
    public Player Controller { get; private set; } = null!;
    public PlayableCardType CardType { get; }
    public int? PrintedPower { get; set; }
    public int? CurrentPower { get; private set; }
    public PlayLocation PlayLocation { get; private set; }


    public enum Tag 
    {
        SpecialBeforeScores,
        Microbot,
        TempMicrobot
    }
    public static List<Tag> TempTags = [Tag.TempMicrobot];
    private List<Tag> _tags = new();
    public IReadOnlyList<Tag> Tags => _tags;

    /// <summary>
    /// Anything that would be considered "Affect"ing a card
    /// </summary>
    public enum EffectType
    {
        Destroy,
        Move,
        Return,
        Attach,
        ApplyPower
    }
    /// <summary>
    /// An object that represents a card's immunity against certain abilities
    /// </summary>
    /// <param name="from">The action the protection is for</param>
    /// <param name="cardType">The card type the protection is against. If null, it will protect against all card types</param>
    /// <param name="grantedBy">The card that grants this protection, important for triggering OnProtect effects</param>
    public record Protection(EffectType from, PlayableCard grantedBy, List<Player>? fromPlayers=null, PlayableCardType? cardType=null)
    {
        public EffectType From { get; init; } = from;
        public PlayableCard GrantedBy { get; init; } = grantedBy;
        public List<Player>? FromPlayers { get; set; } = fromPlayers;
        public PlayableCardType? CardType { get; init; } = cardType;
    }
    public List<Protection> Protections = [];

    /// <summary>
    /// This event is ONLY subscribed to by bases to internally manage breakpoints
    /// </summary>
    public event Action<int> OnPowerChange = delegate { };



    /// <summary>
    /// Called immmediatly after card is in play, before OnPlay
    /// </summary>
    public Action<Battle> EnterBattlefield = delegate { };
    /// <summary>
    /// Gets called immediatly after the card is added to the territory list
    /// </summary>
    public Action<Battle, BaseSlot> EnterBase = delegate { };
    /// <summary>
    /// Is called immediatly after the card is played. If minion or modifier, it will be called after AfterEnterBattleField. If standard action it will be immediatly before it is discarded
    /// </summary>
    public Action<Battle, BaseSlot?> OnPlay = delegate { };
    /// <summary>
    /// Gets called immediatly after the card is removed to the territory list
    /// </summary>
    public Action<Battle, BaseSlot> ExitBase = delegate { };
    /// <summary>
    /// After card is removed from battlefield, but before it exists any other location (e.g. discard pile, hand, etc.)
    /// By Default, this includes setting power to printed power, and resetting the controller (See Constructor)
    /// </summary>
    public Action<Battle> ExitBattlefield = delegate { };
    /// <summary>
    /// After card is placed in discard pile from a DESTROY effect
    /// </summary>
    public Action<Battle, BaseSlot> OnDestroyed = delegate { };

    /// <summary>
    /// After this card successfully prevents an effect from occuring to a card
    /// </summary>
    public Action<Battle> OnProtect = delegate { };
    /// <summary>
    /// After the card is attached to a minion
    /// </summary>
    public Action<Battle, PlayableCard> OnAttach = delegate { };
    /// <summary>
    /// After the card is detached from a minion
    /// </summary>
    public Action<Battle, PlayableCard> OnDetach = delegate { };
    /// <summary>
    /// After a change in controller takes place. First Player is the current Controller, the second is the one we are changing to
    /// </summary>
    public Action<Battle, Player, Player> OnChangeController = delegate { };


    public PlayableCard(Faction faction, PlayableCardType cardType, string name, string[] graphic, PlayLocation playLocation, int? power = null, List<Tag>? tags=null) : base(faction, name, graphic)
    {
        CardType = cardType;
        PrintedPower = power;
        CurrentPower = power;
        PlayLocation = playLocation;
        _tags = tags ?? [];

        ExitBattlefield = (battle) =>
        {
            foreach (var tag in _tags)
            {
                if(TempTags.Contains(tag))
                {
                    _tags.Remove(tag);
                }
            }

            CurrentPower = PrintedPower;

            ResetController();
        };
    }


    /// <summary>
    /// Use this whenever a card directly changes the power of another card. 
    /// NOT when power expires.
    /// This counts as "affecting" the card and can be countered
    /// </summary>
    public bool ApplyPowerChange(Battle battle, PlayableCard affectingCard, int amountToChange)
    {
        if(battle.AttemptToAffect(this, EffectType.ApplyPower, affectingCard.CardType, affectingCard.Controller))
        {
            ChangeCurrentPower(amountToChange);
            return true;
        }
        return false;
    }
    /// <summary>
    /// Use when expiring an existing power change.
    /// NOT when power is directly applied
    /// This does not count as "affecting" the card and cannot be countered
    /// </summary>
    public void ExpirePowerChange(int amountToChange)
    {
        ChangeCurrentPower(amountToChange);
    }
    private void ChangeCurrentPower(int amountToChange)
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
    public void AddTag(Battle battle, Tag tag)
    {
        _tags.Add(tag);
        battle.EventManager.TriggerAfterAddTag(battle, this, tag);
    }
    public void RemoveTag(Battle battle, Tag tag)
    {
        bool removed = _tags.Remove(tag);
        if(removed) battle.EventManager.TriggerAfterRemoveTag(battle, this, tag);
    }

    public void SetOwner(Player owner)
    {
        Owner = owner;
        Controller = owner;
    }
    public void ChangeController(Battle battle, Player newController)
    {
        var oldController = Controller;
        Controller = newController;
        OnChangeController.Invoke(battle, oldController, newController);
    }

    /// <summary>
    /// Sets the controller to be the same as the owner
    /// Does NOT count as affecting it, similar to an expired effect
    /// </summary>
    internal void ResetController()
    {
        Controller = Owner;
    }
}
