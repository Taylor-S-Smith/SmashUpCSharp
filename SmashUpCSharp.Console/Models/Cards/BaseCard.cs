using Models.Player;

namespace Models.Cards
{
    public class BaseCard(int factionId, string title, IList<string> graphic, int printedBreakpoint, int[] pointArray) : PrimitiveCard(title, graphic)
    {
        public int FactionId = factionId;
        public int PrintedBreakpoint { get; set; } = printedBreakpoint;
        public int CurrentBreakpoint { get; set; } = printedBreakpoint;
        public int[] PointArray = pointArray;

        private Dictionary<PrimitivePlayer, List<PlayableCard>> PlayerCards = new();

        public int TotalPower { get; set; } = 0;

        //HELPER
        public void UpdateTotalPower()
        {
            TotalPower = 0;
            foreach (KeyValuePair<PrimitivePlayer, List<PlayableCard>> cards in PlayerCards)
            {
                TotalPower += cards.Value.Sum(x => x.CurrentPower);
            }
        }

        //GET
        public List<PlayableCard> GetCardsByPlayer(PrimitivePlayer player)
        {
            return PlayerCards[player];
        }

        /*
        public abstract void AfterScores();
        public abstract void StartOfTurn();
        public abstract void CardPlayedHere();
        public abstract void CardDestroyedHere();
        */

    }
}
