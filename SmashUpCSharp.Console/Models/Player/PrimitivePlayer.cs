using Models.Cards;

namespace Models.Player
{
    internal class PrimitivePlayer
    {
        public PlayerDeck DrawDeck { get; set; } = new();
        public PlayerDeck DiscardDeck { get; set; } = new();
        public List<PlayableCard> Hand { get; set; } = [];
    }
}
