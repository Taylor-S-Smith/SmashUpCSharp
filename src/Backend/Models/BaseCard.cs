using SmashUp.Backend.GameObjects;

namespace SmashUp.Backend.Models;

internal class BaseCard : Card
{
    public int PrintedBreakpoint { get; set; }
    public int CurrentBreakpoint { get; set; }

    public int[] PointSpread { get; }

    public int CurrentPower { get; set; } = 0;

    public void TriggerOnAddCard(Battle battle, PlayableCard card) => OnAddCard.Invoke(battle, card);
    public event Action<Battle, PlayableCard> OnAddCard;
    public void TriggerOnRemoveCard(Battle battle, PlayableCard card) => OnRemoveCard.Invoke(battle, card);
    public event Action<Battle, PlayableCard> OnRemoveCard;
    public void TriggerAfterDestroyCard(PlayableCard card) => AfterMinionDestroyed.Invoke(card);
    public event Action<PlayableCard> AfterMinionDestroyed = delegate { };
    public BaseCard? TriggerAfterAfterBaseScores(Battle battle, BaseSlot slot, List<Player> winners) => AfterBaseScores.Invoke(battle, slot, winners);
    /// <summary>
    /// Return the base that will replace that one that scored, or NULL to draw a new one normally
    /// </summary>
    public event Func<Battle, BaseSlot, List<Player>, BaseCard?> AfterBaseScores = delegate { return null; };

    public BaseCard(Faction faction, string name, string[] graphic, int breakpoint, int[] pointSpread) : base(faction, name, graphic)
    {
        PrintedBreakpoint = breakpoint;
        CurrentBreakpoint = breakpoint;
        PointSpread = pointSpread;


        void powerChangeHandler(int amountChanged)
        {
            CurrentPower += amountChanged;
        }

        OnAddCard = (battle, card) =>
        {
            if (card.CurrentPower != null)
            {
                CurrentPower += (int)card.CurrentPower;
                card.OnPowerChange += powerChangeHandler;
            }
        };

        OnRemoveCard = (battle, card) =>
        {
            if (card.CurrentPower != null)
            {
                CurrentPower -= (int)card.CurrentPower;
                card.OnPowerChange -= powerChangeHandler;
            }
        };
    }
}