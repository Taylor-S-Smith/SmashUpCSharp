namespace Models.Cards
{
    public class PlayerDeck(IList<PlayableCard> cards) : PrimitiveDeck<PlayableCard>(cards)
    {
        public override IList<PlayableCard> Cards { get; set; } = cards;

    }

}
