using SmashUp.Backend.Services;

namespace SmashUp.Backend.Models;

internal class BaseCard : Card
{
    public int PrintedBreakpoint { get; set; }
    public int CurrentBreakpoint { get; set; }

    public (int, int, int) PointSpread { get; }

    public int CurrentPower { get; set; } = 0;

    public event Action<PlayableCard> OnAddCard;
    public void TriggerOnAddCard(PlayableCard card) => OnAddCard.Invoke(card);
    public event Action<PlayableCard> OnRemoveCard;
    public void TriggerOnRemoveCard(PlayableCard card) => OnRemoveCard.Invoke(card);


    public BaseCard(Faction faction, string name, string[] graphic, int breakpoint, (int, int, int) pointSpread) : base(faction, name, graphic)
    {
        PrintedBreakpoint = breakpoint;
        CurrentBreakpoint = breakpoint;
        PointSpread = pointSpread;


        void powerChangeHandler(int amountChanged)
        {
            CurrentPower += amountChanged;
        }

        OnAddCard = (card) =>
        {
            if (card.CurrentPower != null)
            {
                CurrentPower += (int)card.CurrentPower;
                card.OnPowerChange += powerChangeHandler;
            }
        };

        OnRemoveCard = (card) =>
        {
            if (card.CurrentPower != null)
            {
                CurrentPower -= (int)card.CurrentPower;
                card.OnPowerChange -= powerChangeHandler;
            }
        };
    }
}