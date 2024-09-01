using Models.Cards;
using SmashUp.Backend.Repositories;
using SmashUp.Backend.Services;
using System;

namespace Models.Player;

public class PrimitivePlayer : IIdentifiable
{
    public int Id { get; set; }
    public string Name { get; set; }
    public List<Faction> Factions;
    public PlayerDeck DrawDeck { get; set; }
    public PlayerDeck DiscardDeck { get; set; }
    public List<PlayableCard> Hand { get; set; }
    public int VictoryPoints { get; set; }

    public PrimitivePlayer(string name, List<Faction> factions)
    {
        Name = name;
        Factions = factions;

        var deckCards = new PlayableCardService(new PlayableCardRepository()).Get(factions);

        DrawDeck = new(deckCards);
        DiscardDeck = new([]);

        Hand = DrawDeck.Draw(5);
    }


    public void Draw(int numCards = 1)
    {
        Hand.AddRange(DrawDeck.Draw(numCards));
    }

}
