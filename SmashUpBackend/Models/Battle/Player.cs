using SmashUpBackend.Models.Cards;

namespace SmashUpBackend.Models.Battle;

internal class Player
{

    int Id { get; }
    public string Name { get; }
    public int VictoryPoints { get; }

    public List<PlayableCard> Hand {  get; }
    public Deck<PlayableCard> Deck { get; }
    public Deck<PlayableCard> DiscardPile { get; }

    public Player(int id, string name)
    {
        Id = id;
        Name = name;
        VictoryPoints = 0;

        Deck = new([]);
        Hand = Deck.Draw(5);
        DiscardPile = new([]);
    }
}
