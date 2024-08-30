using Models.Cards;

namespace Models.Player
{
    public class PrimitivePlayer : IIdentifiable
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public List<Faction> Factions = [];
        public PlayerDeck DrawDeck { get; set; } = new();
        public PlayerDeck DiscardDeck { get; set; } = new();
        public List<PlayableCard> Hand { get; set; } = [];
    }
}
