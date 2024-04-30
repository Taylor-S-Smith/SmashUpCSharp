namespace Models.Cards
{
    public class PlayerDeck : PrimitiveDeck<PlayableCard>
    {
        public override IList<PlayableCard> Cards { get; set; } = [];

    }

}
