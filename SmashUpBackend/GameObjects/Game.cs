using System;
using Backend.Models;
using Backend.Services;

namespace Backend.GameObjects;

/// <summary>
/// Handles managing battles and holding data that persists between battle sessions
/// </summary>
internal class Game(IUserInputHandler userInputHandler, Random random, BaseCardService baseCardService, PlayableCardService playableCardService)
{
    private readonly IUserInputHandler _userInputHandler = userInputHandler;
    private readonly Random _random = random;

    private readonly BaseCardService _baseCardService = baseCardService;
    private readonly PlayableCardService _playableCardService = playableCardService;

    private Battle? _currentBattle;

    public void BeginBattle() {
        _currentBattle = new(_userInputHandler, _random, _baseCardService, _playableCardService);
        _currentBattle.Start();
    }
}