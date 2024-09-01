namespace Models.Cards
{
    public class BaseDeck(IList<BaseCard> cards) : PrimitiveDeck<BaseCard>(cards)
    {
        public override IList<BaseCard> Cards { get; set; } = cards;

    }

}
