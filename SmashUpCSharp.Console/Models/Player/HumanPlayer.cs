using Models.Cards;

namespace Models.Player;

public class HumanPlayer(string name, List<Faction> factions) : PrimitivePlayer(name, factions)
{
}
