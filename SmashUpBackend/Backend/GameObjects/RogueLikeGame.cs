using SmashUp.Backend.API;
using SmashUp.Backend.Services;

namespace SmashUp.Backend.GameObjects;

/// <summary>
/// Handles managing battles and holding data that persists between battle sessions
/// </summary>
internal class RogueLikeGame(IFrontendBattleAPI userInputHandler, EventManager eventManager, Random random, BaseCardService baseCardService, PlayableCardService playableCardService)
{
    private readonly IFrontendBattleAPI _userInputHandler = userInputHandler;
    private readonly EventManager _eventManager = eventManager;
    private readonly Random _random = random;

    private readonly BaseCardService _baseCardService = baseCardService;
    private readonly PlayableCardService _playableCardService = playableCardService;

    private Battle? _currentBattle;

    public void BeginBattle()
    {
        _currentBattle = new(_userInputHandler, _eventManager, _random, _baseCardService, _playableCardService);
        _currentBattle.StartBattle();
    }
}