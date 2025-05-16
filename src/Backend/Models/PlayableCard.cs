using System;
using SmashUp.Backend.GameObjects;
using SmashUp.Backend.Services;

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
    public List<Tag> Tags { get; set; } = [];

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

    public event Action<int> OnPowerChange = delegate { };



    /// <summary>
    /// Called immmediatly after card is in play, before OnPlay
    /// </summary>
    public void TriggerAfterEnterBattleField(Battle battle) => AfterEnterBattleField(battle);
    public event Action<Battle> AfterEnterBattleField = delegate { };

    /// <summary>
    /// Is called immediatly after the card is played. If minion or modifier, it will be called after AfterEnterBattleField. If standard action it will be immediatly before it is discarded
    /// </summary>
    public void TriggerOnPlay(Battle battle, BaseSlot? baseSlot=null) => OnPlay(battle, baseSlot);
    public event Action<Battle, BaseSlot?> OnPlay = delegate { };

    /// <summary>
    /// Gets called immediatly after the card is added to the territory list
    /// </summary>
    public void TriggerAfterAddToBase(Battle battle, BaseSlot baseSlot) => OnAddToBase(battle, baseSlot);
    public event Action<Battle, BaseSlot> OnAddToBase = delegate { };
    /// <summary>
    /// Gets called immediatly after the card is removed to the territory list
    /// </summary>
    public void TriggerOnRemoveFromBase(Battle battle, BaseSlot baseSlot) => OnRemoveFromBase(battle, baseSlot);
    public event Action<Battle, BaseSlot> OnRemoveFromBase = delegate { };
    /// <summary>
    /// After card is removed from battlefield, but before it exists in the discard pile
    /// </summary>
    public void TriggerOnDiscard(Battle battle) => OnDiscard(battle);
    public event Action<Battle> OnDiscard = delegate { };
    public void TriggerAfterDestroyed(Battle battle, BaseSlot baseSlot) => AfterDestroyed(battle, baseSlot);
    public event Action<Battle, BaseSlot> AfterDestroyed = delegate { };
    public void TriggerOnProtect(Battle battle) => OnProtect(battle);
    public event Action<Battle> OnProtect = delegate { };
    public void TriggerOnAttach(Battle battle, PlayableCard cardAttachedTo) => OnAttach(battle, cardAttachedTo);
    public event Action<Battle, PlayableCard> OnAttach = delegate { };
    public void TriggerOnDetach(Battle battle, PlayableCard cardDetatchedFrom) => OnDetach(battle, cardDetatchedFrom);
    public event Action<Battle, PlayableCard> OnDetach = delegate { };

    // First Player is the current Controller, the second is the one we are changing to
    public event Action<Battle, Player, Player> OnChangeController = delegate { };

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

    public void SetOwner(Player owner)
    {
        Owner = owner;
        Controller = owner;
    }
    public void ChangeController(Battle battle, Player newController)
    {
        OnChangeController.Invoke(battle, Controller, newController);
        Controller = newController;
    }

    /// <summary>
    /// Reset's the controller to be the same as the owner
    /// Does NOT count as affecting it, similar to an expired effect
    /// </summary>
    internal void ResetController()
    {
        Controller = Owner;
    }

    public PlayableCard(Faction faction, PlayableCardType cardType, string name, string[] graphic, PlayLocation playLocation, int? power=null) : base(faction, name, graphic)
    {
        CardType = cardType;
        PrintedPower = power;
        CurrentPower = power;
        PlayLocation = playLocation;

        OnDiscard = (eventManager) =>
        {
            CurrentPower = PrintedPower;
            ResetController();
        };
    }
}
