using System.Dynamic;
using Backend.Models;
using Backend.Services;

namespace Backend.GameObjects;

internal interface IUserInputHandler
{
    public List<string> GetPlayers();
    public List<(string, List<Faction>)> ChooseFactions();
    public bool AskMulligan();

    public void PlayCards();
}

/// <summary>
/// Handles turn structure (phases) and control flow between front and backend
/// </summary>
internal class Battle
{
    private readonly IUserInputHandler _userInputHandler;
    private readonly Random _random;
    private readonly EventManager _eventManager = new();

    private readonly BaseCardService _baseCardService;
    private readonly PlayableCardService _playableCardService;

    private readonly Table _table = null!;
    private readonly bool _battleEnd;

    public Battle(IUserInputHandler userInputHandler, Random random, BaseCardService baseCardService, PlayableCardService playableCardService)
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
        List<string> playerNames = _userInputHandler.GetPlayers();
        List<Player> players = [];

        //Choose 2 Factions
        List<(string, List<Faction>)> factionChoices = _userInputHandler.ChooseFactions();

        //Build Player Decks
        foreach (var choice in factionChoices)
        {
            string playerName = choice.Item1;
            List<Faction> factions = choice.Item2;
            List<PlayableCard> cards = _playableCardService.GetCards(factions);

            Player player = new(playerName, cards);
            players.Add(player);
        }

        //Build the Base Deck
        List<Faction> allFactions = factionChoices.SelectMany(x => x.Item2).ToList();
        Deck<BaseCard> baseDeck = new(_baseCardService.GetCards(allFactions));

        //Draw Bases
        List<BaseCard> startingBases = baseDeck.Draw(players.Count + 1);

        //Draw Hands
        players.ForEach(DrawInitialHand);

        //Determine the First Player
        Player currentPlayer = players[_random.Next(0, players.Count)];

        Board board = new(baseDeck, startingBases, players.Count);

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

    /// <summary>
    /// This is the main turn loop
    /// </summary>
    public void Start()
    {
        while(!_battleEnd)
        {
            StartTurn();
            PlayCards();
            ScoreBases();
            Draw2Cards();
            EndTurn();
        }
    }

    /// <summary>
    /// Handles logic relating to the "Start Turn" phase of the Smash Up Rule Book
    /// </summary>
    private void StartTurn()
    {
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

    }

    /// <summary>
    /// Handles logic relating to the "Draw 2 Cards" phase of the Smash Up Rule Book
    /// </summary>
    private void Draw2Cards()
    {

    }

    /// <summary>
    /// Handles logic relating to the "End Turn" phase of the Smash Up Rule Book
    /// </summary>
    private void EndTurn()
    {

    }
}
