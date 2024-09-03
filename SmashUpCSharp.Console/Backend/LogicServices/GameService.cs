using SmashUp.Models.Games;
using Models.Player;
using Models.Cards;
using SmashUp.Backend.Services;

namespace SmashUp.Backend.LogicServices;

/// <summary>
/// Handles manipulating the game. We use this rather than using the object directly 
/// to enable us to be more modular in handling the start/end of games and to store 
/// game information easier.
/// </summary>
internal class GameService(IBaseService baseService, IFactionService factionService, IPlayableCardService playableCardService, IPlayerService playerService) : IGameService
{
    private static Game? _game;

    // SERVICES
    readonly IBaseService _baseService = baseService;
    readonly IFactionService _factionService = factionService;
    readonly IPlayableCardService _playableCardService = playableCardService;
    readonly IPlayerService _playerService = playerService;

    public Game GetCurrentGame()
    {
        return _game ?? throw new Exception("You must start a game before getting it");
    }
    public void StartNewGame(List<PrimitivePlayer> players, List<BaseCard> baseCards)
    {
        throw new NotImplementedException();
    }
    public void StartTestGame()
    {
        Random testRandom = new();

        List<PlayableCard> testCards =
        [
            _playableCardService.Get(testRandom.Next(93)),
                _playableCardService.Get(testRandom.Next(93)),
                _playableCardService.Get(testRandom.Next(93))
        ];

        List<Faction> Factions1 = [_factionService.Get(testRandom.Next(8)), _factionService.Get(testRandom.Next(8))];
        List<Faction> Factions2 = [_factionService.Get(testRandom.Next(8)), _factionService.Get(testRandom.Next(8))];

        var deckCards = _playableCardService.Get(Factions1);
        _playerService.Create(new HumanPlayer("Taylor", Factions1, deckCards));
        deckCards = _playableCardService.Get(Factions2);
        _playerService.Create(new HumanPlayer("Andrew", Factions2, deckCards));

        var players = _playerService.GetAll();
        var bases = _baseService.Get(players.SelectMany(x => x.Factions).ToList());

        _game = new(players, bases);

        _game.ActiveBases[0].AttachCard(testCards[0]);
        _game.ActiveBases[1].AttachCard(testCards[1]);
        _game.ActiveBases[2].AttachCard(testCards[2]);
    }
}
