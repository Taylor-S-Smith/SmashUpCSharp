using Backend.Models;

namespace Backend.GameObjects;


internal class PlayerTerritory
{
    public List<PlayableCard> Cards { get; set; } = [];
}

internal class BaseSlot
{
    public BaseCard BaseCard { get; }
    public List<PlayerTerritory> Territories { get; } = [];

    public BaseSlot(BaseCard baseCard, int numTerritories)
    {
        BaseCard = baseCard;

        for (int i = 0; i < numTerritories; i++)
        {
            Territories.Add(new());
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

    public Board(Deck<BaseCard> baseDeck, List<BaseCard> startingBaseCards, int territoryNum)
    {
        BaseDeck = baseDeck;
        BaseDiscard = new([]);
        ActiveBases = [];

        foreach (BaseCard baseCard in startingBaseCards)
        {
            ActiveBases.Add(new(baseCard, territoryNum));
        }
    }
}
