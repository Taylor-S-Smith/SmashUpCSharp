using Models.Cards;

namespace Models.Player
{
    public class PrimitivePlayer
    {
        public string Name { get; set; } = "";
        public List<Faction> Factions = new();
        public PlayerDeck DrawDeck { get; set; } = new();
        public PlayerDeck DiscardDeck { get; set; } = new();
        public List<PlayableCard> Hand { get; set; } = [];
    }
}
