using Models.Player;
using Repositories;
using Services;

namespace Models.Cards
{
    public class BaseCard(int factionId, string title, IList<string> graphic, int printedBreakpoint, int[] pointArray) : PrimitiveCard(title, graphic)
    {
        public int FactionId = factionId;
        public int PrintedBreakpoint { get; set; } = printedBreakpoint;
        public int CurrentBreakpoint { get; set; } = printedBreakpoint;
        public int[] PointArray = pointArray;
        private List<PlayableCard> attachedCards = [];
             

        public void AttachCard(PlayableCard Card)
        {
            attachedCards.Add(Card);
            UpdateTotalPower();
        }

        public int TotalPower { get; set; } = 0;

        //HELPER
        public void UpdateTotalPower()
        {
            TotalPower = GetAttachedCards().Sum(x => x.CurrentPower);
        }

        //GET
        private List<PlayableCard> GetAttachedCards()
        {
            return attachedCards;
        }

        public List<String> GetDisplayList()
        {
            List<String> DisplayList = [];
            attachedCards = attachedCards.OrderBy(x => x.Owner).ToList();
            int? currPlayer = null;

            int cardIndex = 1;
            foreach (PlayableCard card in attachedCards)
            {
                if (currPlayer != card.Owner)
                {
                    DisplayList.Add($"Player {card.Owner}'s cards:");
                    currPlayer = card.Owner;
                }

                if(card.CurrentPower > 0)
                {
                    DisplayList.Add($"{cardIndex}. {card.Title} ({card.CurrentPower})");
                } else
                {
                    DisplayList.Add($"{cardIndex}. {card.Title}");
                }
            }

            return DisplayList;
        }

        internal string GetCardsByIndex(int num)
        {
            throw new NotImplementedException();
        }

        /*
        public abstract void AfterScores();
        public abstract void StartOfTurn();
        public abstract void CardPlayedHere();
        public abstract void CardDestroyedHere();
        */

    }
}
