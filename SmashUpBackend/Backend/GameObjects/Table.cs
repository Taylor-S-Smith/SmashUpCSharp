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
    private readonly Board _board = board;

    public List<(Player, int)> GetPlayerVP()
    {
        return Players.Select(x => (x, x.VictoryPoints)).ToList();
    }
    public void DiscardCard(Player player, PlayableCard card)
    {
        player.Discard(card);
    }

    internal List<BaseSlot> GetBaseSlots()
    {
        return _board.ActiveBases;
    }
    internal List<BaseCard> GetActiveBases()
    {
        return _board.ActiveBases.Select(x => x.BaseCard).ToList();
    }
}
