namespace Models.Cards
{
    public class BaseDeck : PrimitiveDeck<BaseCard>
    {
        public override IList<BaseCard> Cards { get; set; } = [];

    }

}
