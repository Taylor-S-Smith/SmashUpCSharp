using SmashUp.Backend.API;
using SmashUp.Backend.Services;

namespace SmashUp.Backend.GameObjects;

/// <summary>
/// Handles managing battles and holding data that persists between battle sessions
/// </summary>
internal class RogueLikeGame(IFrontendBattleAPI userInputHandler, GlobalEventManager eventManager, Random random)
{
    private readonly IFrontendBattleAPI _userInputHandler = userInputHandler;
    private readonly GlobalEventManager _eventManager = eventManager;
    private readonly Random _random = random;

    private Battle? _currentBattle;

    public void BeginBattle()
    {
        _currentBattle = new(_userInputHandler, _eventManager, _random);
        _currentBattle.StartBattle();
    }
}