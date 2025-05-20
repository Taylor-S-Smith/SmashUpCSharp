using SmashUp.Backend.GameObjects;

namespace SmashUp.Backend.Models;

internal class ScoreResult
{
    public record PlayerScore(Player Player, int TotalPower);

    public List<PlayerScore> First;
    public List<PlayerScore> Second;
    public List<PlayerScore> Third;
    public List<PlayerScore> Others;

    public ScoreResult(Dictionary<int, List<Player>> playersByScore)
    {
        var orderedScores = playersByScore.OrderByDescending(x => x.Key).ToList();

        First = [];
        Second = [];
        Third = [];
        Others = [];

        for (int i = 0; i < orderedScores.Count; i++)
        {
            var (score, players) = (orderedScores[i].Key, orderedScores[i].Value);
            var converted = players.Select(p => new PlayerScore(p, score));

            if (i == 0)
                First = converted.ToList();
            else if (i == 1)
                Second = converted.ToList();
            else if (i == 2)
                Third = converted.ToList();
            else
                Others.AddRange(converted);
        }
    }
}


internal class BaseCard : Card
{
    public int PrintedBreakpoint { get; set; }
    public int CurrentBreakpoint { get; set; }

    public int[] PointSpread { get; }

    public int CurrentPower { get; set; } = 0;


    /// <summary>
    /// After a card is played here. By default CurrentPower is updated and 
    /// we subscribe to the card's OnPowerChange event (See Constructor)
    /// </summary>
    public Action<PlayableCard> OnAddCard;
    /// <summary>
    /// After a card is removed here. By default CurrentPower is updated and 
    /// we unsubscribe to the card's OnPowerChange event (See Constructor)
    /// </summary>
    public Action<PlayableCard> RemoveCard;
    public Action<PlayableCard> AfterMinionDestroyed = delegate { };
    public Action<Battle, BaseSlot> BeforeBaseScores = delegate { };
    public Action<Battle, ScoreResult> WhenBaseScores = delegate { };

    /// <summary>
    /// Return the base that will replace that one that scored, or NULL to draw a new one normally
    /// </summary>
    public Func<Battle, BaseSlot, ScoreResult, BaseCard?> AfterBaseScores = delegate { return null; };

    public Action<Battle, BaseSlot, ScoreResult> OnReplaced = delegate { };

    public BaseCard(Faction faction, string name, string[] graphic, int breakpoint, int[] pointSpread) : base(faction, name, graphic)
    {
        PrintedBreakpoint = breakpoint;
        CurrentBreakpoint = breakpoint;
        PointSpread = pointSpread;

        void PowerChangeHandler(int amountChanged)
        {
            CurrentPower += amountChanged;
        }

        OnAddCard = (PlayableCard card) =>
        {
            if (card.CurrentPower != null)
            {
                CurrentPower += (int)card.CurrentPower;
                card.OnPowerChange += PowerChangeHandler;
            }
        };

        RemoveCard = (PlayableCard card) =>
        {
            if (card.CurrentPower != null)
            {
                CurrentPower -= (int)card.CurrentPower;
                card.OnPowerChange -= PowerChangeHandler;
            }
        };
    }
}