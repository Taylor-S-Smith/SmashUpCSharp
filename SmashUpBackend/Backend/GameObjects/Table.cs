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
    private readonly List<Player> _players = players;
    private readonly ActivePlayer _activePlayer = activePlayer;
    private readonly Board _board = board;

    public Guid GetCurrentPlayerId()
    {
        return _activePlayer.Player.Id;
    }
    public List<PlayableCard> GetPlayerHand(Guid playerId)
    {
        return GetPlayer(playerId).Hand.ToList();
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
    public List<(Guid, int)> GetPlayerVP()
    {
        return _players.Select(x => (x.Id, x.VictoryPoints)).ToList();
    }

    public int Draw2Cards()
    {
        Player activePlayer = _activePlayer.Player;
        activePlayer.Draw(2);
        return activePlayer.Hand.Count;
    }
    public void DiscardCard(Guid playerId, Guid cardId)
    {
        Player player = GetPlayer(playerId);
        player.Discard(cardId);
    }

    private Player GetPlayer(Guid playerId)
    {
        return _players.Where(x => x.Id == playerId).FirstOrDefault() ?? throw new Exception($"No player exists with ID: {playerId}");
    }

    internal List<BaseSlot> GetBaseSlots()
    {
        return _board.ActiveBases;
    }
    internal List<BaseCard> GetActiveBases()
    {
        return _board.ActiveBases.Select(x => x.BaseCard).ToList();
    }

    internal ActivePlayer GetActivePlayer()
    {
        return _activePlayer;
    }

    internal void PlayCard(Player player, PlayableCard cardToPlay, BaseCard targetedBaseCard)
    {
        // Validate Play
        bool isValidPlay = ValidatePlay(player, cardToPlay);

        if(isValidPlay)
        {
            //Remove resource from player
            RemoveResource(player, cardToPlay);

            // Remove Card from Previous Location
            player.Play(cardToPlay);

            // Add Card to territory
            BaseSlot slot = _board.ActiveBases.Where(x => x.BaseCard == targetedBaseCard).Single();
            PlayerTerritory territory = slot.Territories.Where(x => x.player == cardToPlay.Owner).Single();
            territory.Cards.Add(cardToPlay);
        }
    }

    private static bool ValidatePlay(Player player, PlayableCard cardToPlay)
    {
        if (cardToPlay.CardType == PlayableCardType.minion)
        {
            return player.MinionPlays > 0;
        }
        else if (cardToPlay.CardType == PlayableCardType.action)
        {
            return player.ActionPlays > 0;
        }
        return false;
    }

    private static void RemoveResource(Player player, PlayableCard cardToPlay)
    {
        if (cardToPlay.CardType == PlayableCardType.minion)
        {
            player.MinionPlays--;
        }
        else if (cardToPlay.CardType == PlayableCardType.action)
        {
            player.ActionPlays--;
        }
    }
}
