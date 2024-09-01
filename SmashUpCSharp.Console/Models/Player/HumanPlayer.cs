using Models.Cards;

namespace Models.Player;

public class HumanPlayer(string name, List<Faction> factions, List<PlayableCard> deckCards) : PrimitivePlayer(name, factions, deckCards)
{
}
