using System.Collections.Generic;
using System.Numerics;
using System;
using Backend.Models;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Backend.GameObjects;


internal class CurrentPlayer(Guid id)
{
    public Guid PlayerId { get; set; } = id;
    public int MinionPlays { get; set; } = 1;
    public int ActionPlays { get; set; } = 1;
}

/// <summary>
/// Holds game state data and handles changes in state. The highest level API for raw game commands
/// If you were to recreate the old shell version of this code, this is what the user input would directly call
/// If we implemented a developer console, it would directly interact with this.
/// </summary>
internal class Table(List<Player> players, CurrentPlayer currentPlayer, Board board)
{
    private readonly List<Player> _players = players;
    private readonly CurrentPlayer _currentPlayer = currentPlayer;
    private readonly Board _board = board;

    public List<string[]> GetPlayerHandGraphics(Guid playerId)
    {
        return GetPlayer(playerId).Hand.Select(x => x.Graphic).ToList();
    }

    public List<string> GetPlayerDiscard(Guid playerId)
    {
        return GetPlayer(playerId).GetDiscard();
    }

    public List<string> GetPlayerDeck(Guid playerId)
    {
        return GetPlayer(playerId).GetDeck();

    }

    public List<string> GetPlayerDeck(Guid playerId, int numToGet)
    {
        var deck = GetPlayerDeck(playerId);
        numToGet = Math.Min(numToGet, deck.Count);
        return deck.Take(numToGet).ToList();
    }

    public Player GetPlayer(Guid playerId)
    {
        return _players.Where(x => x.Id == playerId).FirstOrDefault() ?? throw new Exception($"No player exists with ID: {playerId}");
    }

}
