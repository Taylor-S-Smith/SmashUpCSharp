using SmashUp.Backend.API;
using SmashUp.Backend.Models;

namespace SmashUp.Backend.GameObjects;


internal class PlayerTerritory(Player player)
{
    public Player player = player;
    public List<PlayableCard> Cards { get; set; } = [];
}

internal class BaseSlot
{
    public BaseCard BaseCard { get; }
    public List<PlayerTerritory> Territories { get; } = [];

    public BaseSlot(BaseCard baseCard, List<Player> players)
    {
        BaseCard = baseCard;

        foreach(var player in players)
        {
            Territories.Add(new(player));
        }
    }
}

/// <summary>
/// Handles the interactions between all non-player game objects. Recieves input from players
/// </summary>
internal class Board
{
    public enum GamePhase
    {

    }
    public Deck<BaseCard> BaseDeck { get; set; }
    public Deck<BaseCard> BaseDiscard { get; set; }
    public List<BaseSlot> ActiveBases { get; set; }

    public Board(Deck<BaseCard> baseDeck, List<BaseCard> startingBaseCards, List<Player> players)
    {
        BaseDeck = baseDeck;
        BaseDiscard = new([]);
        ActiveBases = [];

        foreach (BaseCard baseCard in startingBaseCards)
        {
            ActiveBases.Add(new(baseCard, players));
        }
    }
}
