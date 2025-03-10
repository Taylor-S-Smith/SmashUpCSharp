using SmashUpBackend.Models.Cards;

namespace SmashUpBackend.Models.Battle;

internal class Battle
{
    private readonly Board _board;
    public List<Player> _players;

    public Battle(List<Player> players)
    {
        List<BaseCard> baseCards = [];
        Deck<BaseCard> baseDeck = new(baseCards);
        _board = new(baseDeck, baseDeck.Draw(players.Count + 1), players.Count);
        _players = players;
    }

    public List<Player> GetPlayers()
    {
        return _players;
    }
}
