using SmashUp.Backend.Services;
using SmashUp.Backend.Models;
using SmashUp.Backend.API;

namespace SmashUp.Backend.GameObjects;

/// <summary>
/// Handles turn structure (phases) and control flow between front and backend
/// </summary>
internal class Battle
{
    private readonly IFrontendBattleAPI _userInputHandler;
    private readonly Random _random;
    private readonly EventManager _eventManager = new();

    private readonly BaseCardService _baseCardService;
    private readonly PlayableCardService _playableCardService;

    private readonly Table _table;
    private bool _battleEnd;

    private const int WINNING_VP = 15;

    public Battle(IFrontendBattleAPI userInputHandler, Random random, BaseCardService baseCardService, PlayableCardService playableCardService)
    {
        _userInputHandler = userInputHandler;
        _random = random;

        _baseCardService = baseCardService;
        _playableCardService = playableCardService;

        _table = SetUp();
    }

    // TURN STRUCTURE & CONTROL FLOW 


    /// <summary>
    /// Handles logic relating to the "Set Up" phase of the Smash Up Rule Book
    /// </summary>
    private Table SetUp()
    {
        //Invite Players
        List<string> playerNames = _userInputHandler.GetPlayerNames();
        List<Player> players = [];

        //Choose 2 Factions
        List<(string, List<FactionModel>)> factionChoices = _userInputHandler.ChooseFactions(playerNames, FactionService.GetFactionModels());

        //Build Player Decks
        foreach (var choice in factionChoices)
        {
            string playerName = choice.Item1;
            List<Faction> factions = choice.Item2.Select(x => x.Faction).ToList();
            List<PlayableCard> cards = _playableCardService.GetCards(factions);

            var player = new Player(playerName, cards);
            players.Add(player);
        }

        //Build the Base Deck
        List<Faction> allFactions = factionChoices.SelectMany(x => x.Item2.Select(y => y.Faction)).ToList();
        Deck<BaseCard> baseDeck = new(_baseCardService.GetCards(allFactions));

        //Draw Bases
        List<BaseCard> startingBases = baseDeck.Draw(players.Count + 1);

        //Draw Hands
        players.ForEach(DrawInitialHand);

        //Determine the First Player
        Player currentPlayer = players[_random.Next(0, players.Count)];

        Board board = new(baseDeck, startingBases, players);

        return new(players, new(currentPlayer), board);
    }
    private void DrawInitialHand(Player player)
    {
        player.Draw(5);

        //Mulligan
        if (!player.Hand.Any(x => x.CardType == PlayableCardType.minion))
        {
            bool wantsToMulligan = _userInputHandler.AskMulligan();

            if (wantsToMulligan)
            {
                player.ReplaceHand();
            }
        }
    }

    public void StartBattle()
    {
        _userInputHandler.StartBattle(_table);
        while (!_battleEnd)
        {
            TurnLoop();
        }
    }

    private void TurnLoop()
    {
        StartTurn();
        PlayCards();
        ScoreBases();
        Draw2Cards();
        EndTurn();
        SwitchActivePlayer();
    }

    /// <summary>
    /// Handles logic relating to the "Start Turn" phase of the Smash Up Rule Book
    /// </summary>
    private void StartTurn()
    {
        _table.GetActivePlayer().Player.MinionPlays = 1;
        _table.GetActivePlayer().Player.ActionPlays = 1;
        _eventManager.TriggerStartOfTurn();
    }
    /// <summary>
    /// Handles logic relating to the "Play Cards" phase of the Smash Up Rule Book
    /// </summary>
    private void PlayCards()
    {
        //Use plays
        //Activate Talents (and "On Your Turn")
        _userInputHandler.PlayCards();
    }
    /// <summary>
    /// Handles logic relating to the "Score Bases" phase of the Smash Up Rule Book
    /// </summary>
    private void ScoreBases()
    {
        //Not Implemented Yet :(
    }
    /// <summary>
    /// Handles logic relating to the "Draw 2 Cards" phase of the Smash Up Rule Book
    /// </summary>
    private void Draw2Cards()
    {
        int totalCards = _table.Draw2Cards();
        if (totalCards > 10)
        {
            Guid currentPlayerId = _table.GetCurrentPlayerId();
            var cardIdsToDiscard = _userInputHandler.DiscardTo10(currentPlayerId);
            foreach (var cardId in cardIdsToDiscard)
            {
                _table.DiscardCard(_table.GetCurrentPlayerId(), cardId);
            }
        }
    }
    /// <summary>
    /// Handles logic relating to the "End Turn" phase of the Smash Up Rule Book
    /// </summary>
    private void EndTurn()
    {
        _eventManager.TriggerEndOfTurn();
        CheckEndOfGame();
    }

    private void CheckEndOfGame()
    {
        List<(Guid, int)> playerVpTotals = _table.GetPlayerVP();

        List<int> vpTotals = playerVpTotals.Select(x => x.Item2).ToList();
        int vpMax = vpTotals.Max();

        //One clear winner, no ties in VP
        if (vpMax >= WINNING_VP && vpTotals.Where(x => x == vpMax).ToList().Count == 1)
        {
            _battleEnd = true;
            Guid winningPlayerId = playerVpTotals.Single(x => x.Item2 == vpMax).Item1;
            _userInputHandler.EndBattle(winningPlayerId);
        }
    }
    private void SwitchActivePlayer()
    {
        throw new NotImplementedException();
    }
}
