using SmashUp.Backend.Services;
using SmashUp.Backend.Models;
using SmashUp.Backend.API;

namespace SmashUp.Backend.GameObjects;

/// <summary>
/// Handles turn structure (phases) and control flow between front and backend
/// </summary>
internal class Battle : IBackendBattleAPI
{
    private readonly IFrontendBattleAPI _userInputHandler;
    private readonly Random _random;
    private readonly EventManager _eventManager;

    private readonly BaseCardService _baseCardService;
    private readonly PlayableCardService _playableCardService;

    private readonly Table _table;
    private bool _battleEnd;

    private const int WINNING_VP = 15;

    public Battle(IFrontendBattleAPI userInputHandler, EventManager eventManager, Random random, BaseCardService baseCardService, PlayableCardService playableCardService)
    {
        _userInputHandler = userInputHandler;
        _eventManager = eventManager;
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
        _table.ActivePlayer.Player.MinionPlays = 1;
        _table.ActivePlayer.Player.ActionPlays = 1;
        _eventManager.TriggerStartOfTurn();
    }
    /// <summary>
    /// Handles logic relating to the "Play Cards" phase of the Smash Up Rule Book
    /// </summary>
    private void PlayCards()
    {
        //Use plays
        //Activate Talents (and "On Your Turn") effects
        _userInputHandler.PlayCards(_table, this);
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
        _table.ActivePlayer.Player.Draw(2);
        if (_table.ActivePlayer.Player.Hand.Count > 10)
        {
            var cardsToDiscard = _userInputHandler.DiscardTo10(_table.ActivePlayer.Player);
            foreach (var card in cardsToDiscard)
            {
                _table.ActivePlayer.Player.Discard(card);
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
        List<(Player, int)> playerVpTotals = _table.GetPlayerVP();

        List<int> vpTotals = playerVpTotals.Select(x => x.Item2).ToList();
        int vpMax = vpTotals.Max();

        //One clear winner, no ties in VP
        if (vpMax >= WINNING_VP && vpTotals.Where(x => x == vpMax).ToList().Count == 1)
        {
            _battleEnd = true;
            var winningPlayer = playerVpTotals.Single(x => x.Item2 == vpMax).Item1;
            _userInputHandler.EndBattle(winningPlayer);
        }
    }
    private void SwitchActivePlayer()
    {
        int newActivePlayerIndex = _table.Players.IndexOf(_table.ActivePlayer.Player) + 1;
        if (newActivePlayerIndex == _table.Players.Count) newActivePlayerIndex = 0;
        _table.ActivePlayer.Player = _table.Players[newActivePlayerIndex];
    }

    public void PlayCard(Player player, PlayableCard cardToPlay, BaseCard targetedBaseCard)
    {
        // Validate Play
        bool isValidPlay = ValidatePlay(player, cardToPlay);

        if (isValidPlay)
        {
            //Remove resource from player
            RemoveResource(player, cardToPlay);

            // Remove Card from Previous Location
            player.Play(cardToPlay);

            // Add Card to territory
            BaseSlot slot = _table.GetBaseSlots().Where(x => x.BaseCard == targetedBaseCard).Single();
            PlayerTerritory territory = slot.Territories.Where(x => x.player == cardToPlay.Owner).Single();
            territory.Cards.Add(cardToPlay);

            // Update Base Total
            if(cardToPlay.CurrentPower != null)
            {
                slot.BaseCard.CurrentPower += (int)cardToPlay.CurrentPower;
            }
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

internal interface IBackendBattleAPI
{
    void PlayCard(Player player, PlayableCard cardToPlay, BaseCard targetedBaseCard);
}