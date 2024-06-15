namespace Models.Cards
{
    public abstract class PrimitiveDeck<T> : IIdentifiable where T : PrimitiveCard
    {

        public virtual IList<T> Cards { get; set; } = [];

        public int Id { get; set;  }

        public void Shuffle()
        {

        }
        public void GetCard()
        {

        }
        public List<T> DrawCards(int numCards = 1)
        {
            List<T> drawnCards = new();
            for (int i = 0; i < numCards; i++)
            {
                if (Cards.Count > 0)
                {
                    T card = Cards[Cards.Count - 1];
                    Cards.RemoveAt(Cards.Count - 1);
                    drawnCards.Add(card);
                }
                else
                {
                    // Handle case where there are not enough cards to draw
                    break;
                }
            }

            return drawnCards;
        }

    }

}
