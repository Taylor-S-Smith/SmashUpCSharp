using System.Collections.Generic;

namespace Models.Cards
{
    public abstract class PrimitiveDeck<T>(IList<T> cards) : IIdentifiable where T : PrimitiveCard
    {

        public virtual IList<T> Cards { get; set; } = cards;

        public int Id { get; set;  }

        public void Shuffle()
        {
            Random rng = new();
            int n = Cards.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                (Cards[n], Cards[k]) = (Cards[k], Cards[n]);
            }
        }
        public void GetCard()
        {

        }
        public List<T> Draw(int numCards = 1)
        {
            List<T> drawnCards = [];
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
                    break;
                }
            }

            return drawnCards;
        }

    }

}
