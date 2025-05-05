using System.Numerics;
using SmashUp.Backend.Models;

namespace SmashUp.Backend.GameObjects;


internal class ActivePlayer(Player player)
{
    public Player Player { get; set; } = player;
}

/// <summary>
/// Holds game state data and handles changes in state. The highest level API for raw game commands
/// If you were to recreate the old shell version of this code, this is what the user input would directly call
/// If we implemented a developer console, it would directly interact with this.
/// </summary>
internal class Table(List<Player> players, ActivePlayer activePlayer, Board board)
{
    public readonly List<Player> Players = players;
    public readonly ActivePlayer ActivePlayer = activePlayer;
    public readonly Board Board = board;

    public List<(Player, int)> GetPlayerVP()
    {
        return Players.Select(x => (x, x.VictoryPoints)).ToList();
    }

    public List<BaseSlot> GetBaseSlots()
    {
        return Board.ActiveBases;
    }
    public List<BaseCard> GetActiveBases()
    {
        return Board.ActiveBases.Select(x => x.BaseCard).ToList();
    }

    public List<List<PlayableCard>> GetFieldCards()
    {
        return Board.ActiveBases.Select(x => x.Territories.SelectMany(x => x.Cards).ToList()).Where(x => x.Count > 0).ToList();
    }

    public void AddBaseToDiscard(BaseCard baseCard)
    {
        Board.BaseDiscard.Add(baseCard);
    }

    public BaseCard DrawBaseCard()
    {
        var drawnBase = Board.BaseDeck.Draw();
        if (drawnBase == null)
        {
            Board.BaseDeck.Shuffle(Board.BaseDiscard);
            Board.BaseDiscard = [];
            drawnBase = Board.BaseDeck.Draw();
            if (drawnBase == null) throw new Exception("Failed to draw base card, there are no cards in the deck or discard");
        }
        return drawnBase;
    }
}
