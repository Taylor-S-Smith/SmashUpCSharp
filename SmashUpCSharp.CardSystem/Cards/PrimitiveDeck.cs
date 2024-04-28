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
        public void DrawCard()
        {

        }

    }

}
