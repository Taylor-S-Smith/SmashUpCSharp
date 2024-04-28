using Models.Player;
using static System.Net.Mime.MediaTypeNames;

namespace Models.Cards
{
    public abstract class BaseCard(string title, int printedBreakpoint, string text, IList<string> graphic) : PrimitiveCard(title, text, graphic)
    {
        public int PrintedBreakpoint { get; set; } = printedBreakpoint;

        public int CurrentBreakpoint { get; set; } = printedBreakpoint;

        private Dictionary<PrimitivePlayer, List<PlayableCard>> PlayerCards = new();

        public int TotalPower { get; set; } = 0;

        public void UpdateTotalPower()
        {
            TotalPower = 0;
            foreach (KeyValuePair<PrimitivePlayer, List<PlayableCard>> cards in PlayerCards)
            {
                TotalPower += cards.Value.Sum(x => x.CurrentPower);
            }

        }

        public abstract void AfterScores();
        public abstract void StartOfTurn();
        public abstract void CardPlayedHere();
        public abstract void CardDestroyedHere();

    }
}
