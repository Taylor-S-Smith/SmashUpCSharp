using static SmashUp.Backend.Models.PlayableCard;
using SmashUp.Backend.Services;
using SmashUp.Backend.Models;
using SmashUp.Backend.API;
using FluentResults;
using LinqKit;
using SmashUp.Backend.Repositories;

namespace SmashUp.Backend.GameObjects;

/// <summary>
/// Handles turn structure (phases) and control flow between front and backend
/// </summary>
internal class Battle
{
    private readonly IFrontendBattleAPI _userInputHandler;
    private readonly Random _random;
    public readonly GlobalEventManager EventManager;

    private readonly Table _table;
    private bool _battleEnd;

    private const int WINNING_VP = 15;

    public Battle(IFrontendBattleAPI userInputHandler, GlobalEventManager eventManager, Random random)
    {
        _userInputHandler = userInputHandler;

        EventManager = eventManager;
        _random = random;

        _table = SetUp();

        _userInputHandler.InitializeData(_table);
    }

    // TURN STRUCTURE & CONTROL FLOW 

    /// <summary>
    /// Handles logic relating to the "Set Up" phase of the Smash Up Rule Book
    /// </summary>
    private Table SetUp()
    {
        //Invite Players
        List<string> playerNames = _userInputHandler.ChoosePlayerNames();
        List<Player> players = [];

        //Choose 2 Factions
        List<(string, List<FactionModel>)> factionChoices = _userInputHandler.ChooseFactions(playerNames, FactionService.GetFactionModels());

        //Build Player Decks
        foreach (var choice in factionChoices)
        {
            string playerName = choice.Item1;
            List<Faction> factions = choice.Item2.Select(x => x.Faction).ToList();
            List<PlayableCard> cards = CardRepository.GetPlayableCards(factions);

            var player = new Player(playerName, cards);
            players.Add(player);
        }

        //Build the Base Deck
        List<Faction> allFactions = factionChoices.SelectMany(x => x.Item2.Select(y => y.Faction)).ToList();
        Deck<BaseCard> baseDeck = new(CardRepository.GetBaseCards(allFactions));

        //Draw And Play Bases
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
        if (!player.Hand.Any(x => x.CardType == PlayableCardType.Minion))
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
        EventManager.TriggerStartOfTurn(this, _table.ActivePlayer);
    }
    /// <summary>
    /// Handles logic relating to the "Play Cards" phase of the Smash Up Rule Book
    /// </summary>
    private void PlayCards()
    {
        string displayMessage = "";
        while (true)
        {
            //Activate Talents (and "On Your Turn") effects
            //Use plays

            // Select Card From Hand
            Guid? chosenCardId = _userInputHandler.SelectHandCard(_table.ActivePlayer.Player.Hand.ToList(), _table.GetFieldCards(), displayMessage);
            displayMessage = "";
            if (chosenCardId == null) break;

            PlayableCard? cardToPlay = GetHandCardById((Guid)chosenCardId);

            if (cardToPlay != null)
            {
                Result result = ValidatePlay(_table.ActivePlayer.Player, cardToPlay);
                if (result.IsSuccess)
                {
                    PlayCard(_table.ActivePlayer.Player, cardToPlay);
                }
                else
                {
                    displayMessage = string.Join(" ", result.Errors.Select(x => x.Message));
                }
            }
        }
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
        EventManager.TriggerEndOfTurn(_table.ActivePlayer);
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


    // PLAY CARDS
    private void PlayCard(Player player, PlayableCard cardToPlay)
    {
        RemovePlayResource(player, cardToPlay);

        player.RemoveFromHand(cardToPlay);

        if (cardToPlay.PlayLocation == PlayLocation.Base)
        {
            PlayCardToBase(cardToPlay);
        }
        else if (cardToPlay.PlayLocation == PlayLocation.Minion)
        {
            PlayCardToMinion(player, cardToPlay);
        }
        else if(cardToPlay.PlayLocation == PlayLocation.Discard)
        {
            PlayCardToDiscard(player, cardToPlay);
        }
    }
    private static Result ValidatePlay(Player player, PlayableCard cardToPlay)
    {
        if (cardToPlay.CardType == PlayableCardType.Minion)
        {
            if (player.MinionPlays == 0) return Result.Fail("You don't have any more minion plays");
        }
        else if (cardToPlay.CardType == PlayableCardType.Action)
        {
            if (player.ActionPlays == 0) return Result.Fail("You don't have any more action plays");
        }
        return Result.Ok();
    }
    private static void RemovePlayResource(Player player, PlayableCard cardToPlay)
    {
        if (cardToPlay.CardType == PlayableCardType.Minion)
        {
            player.MinionPlays--;
        }
        else if (cardToPlay.CardType == PlayableCardType.Action)
        {
            player.ActionPlays--;
        }
    }
    private void PlayCardToBase(PlayableCard cardToPlay)
    {
        List<Guid> validBaseIds = _table.GetActiveBases().Select(x => x.Id).ToList();
        Guid chosenBaseId = _userInputHandler.SelectBaseCard(validBaseIds, cardToPlay, $"Choose a base to play {cardToPlay.Name} on");
        BaseCard chosenBase = GetBaseCardById(chosenBaseId);

        // Add Card to territory
        // NOTE: It is a rules req that the card exists in the base BEFORE on play is triggered
        // Which happens before add to base effects, which means that it can't be triggered in a
        // more primitive funciton in the table
        BaseSlot slot = _table.GetBaseSlots().Where(x => x.BaseCard == chosenBase).Single();
        PlayerTerritory territory = slot.Territories.Where(x => x.player == cardToPlay.Controller).Single();
        territory.Cards.Add(cardToPlay);

        // Activate Card Ability
        cardToPlay.TriggerOnAddToBase(this, chosenBase);
        cardToPlay.TriggerOnPlay(this, slot);

        // Trigger Base Effects (This includes updating the base's total power)
        chosenBase.TriggerOnAddCard(this, cardToPlay);
    }
    private void PlayCardToDiscard(Player player, PlayableCard cardToPlay)
    {
        // Activate Card Ability
        cardToPlay.TriggerOnPlay(this);

        // Put card in discard
        player.DiscardPile.Add(cardToPlay);
    }
    private void PlayCardToMinion(Player player, PlayableCard cardToPlay)
    {
        SelectFieldCardQuery query = new()
        {
            CardType = PlayableCardType.Minion,
        };
        PlayableCard? cardToAttachTo = SelectFieldCard(cardToPlay, $"Choose a minion to attach {cardToPlay.Name} to", query)?.SelectedCard;

        if (cardToAttachTo != null)
        {
            if (AttemptToAffect(cardToAttachTo, EffectType.Attach, PlayableCardType.Action, player))
            {
                cardToAttachTo.Attach(cardToPlay);
                cardToPlay.TriggerOnAttach(this, cardToAttachTo);
            }
            else
            {
                player.DiscardPile.Add(cardToPlay);
            }
        }
    }


    // CARD EFFECTS
    public void Destroy(PlayableCard cardToDestroy, PlayableCard affector)
    {
        if (AttemptToAffect(cardToDestroy, EffectType.Destroy, affector.CardType, affector.Controller))
        {
            //Reset the card's controller
            cardToDestroy.ChangeController(this, cardToDestroy.Owner);
            var baseCard = RemoveCardFromBattleField(cardToDestroy, cardToDestroy.Owner.DiscardPile.Add);
            baseCard.TriggerAfterDestroyCard(cardToDestroy);
        }
    }
    /// <summary>
    /// Checks if a card can be affected by a specific effect. 
    /// If it is protected, this will also resolve the protection triggers
    /// </summary>
    /// <returns>True if card can be affected, otherwise false</returns>
    public bool AttemptToAffect(PlayableCard cardToAffect, EffectType effect, PlayableCardType affectorCardType, Player affectorPlayer)
    {
        List<Protection> protections = cardToAffect.Protections.Where(x => x.From == effect && (x.CardType == null || x.CardType == affectorCardType) && (x.FromPlayers == null || x.FromPlayers.Contains(affectorPlayer))).ToList();
        
        if (protections.Count == 0) return true;
        else
        {
            PlayableCard protector;
            if (protections.Count == 1)
            {
                protector = protections.Single().GrantedBy;
            }
            else
            {
                protector = SelectCard(protections.Select(x => x.GrantedBy).ToList(), $"{cardToAffect.Controller.Name}, {cardToAffect.Name} is being protected by multiple cards, choose which one to recieve protection from:");
            }

            protector.TriggerOnProtect(this);
            return false;
        }
    }
    /// <summary>
    /// Removes card from base, calls appropriate triggers
    /// </summary>
    /// <param name="AddFunction">Function that is called after removal, usually determines where the card ends up</param>
    /// <returns>Base the card was removed from</returns>
    private BaseCard RemoveCardFromBattleField(PlayableCard cardToRemove, Action<PlayableCard> AddFunction)
    {
        foreach (BaseSlot slot in _table.GetBaseSlots())
        {
            foreach (PlayerTerritory territory in slot.Territories)
            {
                // Check all played cards on base
                if (territory.Cards.Remove(cardToRemove))
                {
                    // Add card to other location
                    AddFunction(cardToRemove);

                    // Trigger leave of base
                    cardToRemove.TriggerOnRemoveFromBase(this, slot.BaseCard);
                    slot.BaseCard.TriggerOnRemoveCard(this, cardToRemove);

                    // Trigger leave of battlefield
                    cardToRemove.TriggerOnRemoveFromBattleField(EventManager);
                    return slot.BaseCard;
                }
                else
                {
                    // Check cards attached to played cards
                    foreach(var card in territory.Cards)
                    {
                        if (card.Detach(cardToRemove))
                        {
                            // Add card to other location
                            AddFunction(cardToRemove);

                            //Trigger on detach
                            cardToRemove.TriggerOnDetach(this, card);

                            // Trigger leave of battlefield
                            cardToRemove.TriggerOnRemoveFromBattleField(EventManager);
                            return slot.BaseCard;
                        }
                    }
                }
            }
        }

        throw new Exception($"{cardToRemove.Name} with ID {cardToRemove.Id} is not on the battlefield, so can't be removed");
    }


    // TABLE FUNCTIONS
    public List<BaseSlot> GetBaseSlots()
    {
        return _table.GetBaseSlots();
    }
    public List<Player> GetPlayers()
    {
        return _table.Players;
    }
    public Player GetActivePlayer()
    {
        return _table.ActivePlayer.Player;
    }


    // INTERACTING WITH UI
    public class SelectFieldCardQuery
    {
        public PlayableCardType? CardType { get; set; } 
        public BaseCard? BaseCard { get; set; }
        public int? MaxPower { get; set; }
        public Player? Controller { get; set; }
    }
    public record SelectFieldCardResult(PlayableCard? SelectedCard, BaseCard? SelectedCardBase, bool ActionCanceled);
    /// <returns>Selected Field Card Result, or null if there are no available targets</returns>
    public SelectFieldCardResult? SelectFieldCard(PlayableCard cardToDisplay, string displaytext, SelectFieldCardQuery query)
    {
        var cardPred = PredicateBuilder.New<PlayableCard>();
        if(query.CardType != null) cardPred.And((PlayableCard card) => card.CardType == query.CardType);
        if (query.MaxPower != null) cardPred.And((PlayableCard card) => card.CurrentPower <= query.MaxPower);
        if (query.Controller != null) cardPred.And((PlayableCard card) => card.Controller == query.Controller);

        List<List<Guid>> validFieldCardIds = GetValidFieldCardIds(cardPred, query.BaseCard);
        if (validFieldCardIds.Count == 0) return null;

        var result = _userInputHandler.SelectFieldCard(validFieldCardIds, cardToDisplay, displaytext);
        return new(
            result.SelectedCardId != null ? GetFieldCardById((Guid)result.SelectedCardId) : null,
            result.SelectedCardId != null ? GetBaseCardByFieldCardId((Guid)result.SelectedCardId) : null, 
            result.ActionCanceled);
    }

    public record SelectBaseCardResult(BaseCard? SelectedBase, bool ActionCanceled);
    /// <returns>Selected Base Card Result, or null if there are no available targets</returns>
    public SelectBaseCardResult? SelectBaseCard()
    {
        List<Guid> validBaseIds = _table.GetActiveBases().Select(x => x.Id).ToList();
        Guid chosenBaseId = _userInputHandler.SelectBaseCard(validBaseIds);
        return new(GetBaseCardById(chosenBaseId), false);
    }

    public PlayableCard SelectCard(List<PlayableCard> options, string displayText)
    {
        Guid chosenId = _userInputHandler.SelectPlayableCard(options, displayText);
        return options.Where(x => x.Id == chosenId).SingleOrDefault() ?? throw new Exception($"No option with ID: {chosenId}");
    }

    public List<PlayableCard> GetValidFieldCards(Func<PlayableCard, bool> pred, BaseCard? baseCard = null)
    {
        IEnumerable<BaseSlot> validBaseSlots = _table.GetBaseSlots();

        if (baseCard != null)
        {
            validBaseSlots = [validBaseSlots.Where(x => x.BaseCard == baseCard).Single()];
        }
        return validBaseSlots
            .SelectMany(x => x.Cards.Where(pred))
            .ToList();
    }
    private List<List<Guid>> GetValidFieldCardIds(Func<PlayableCard, bool> pred, BaseCard? baseCard = null)
    {
        IEnumerable<BaseSlot> validBaseSlots = _table.GetBaseSlots();

        if (baseCard != null)
        {
            validBaseSlots = [validBaseSlots.Where(x => x.BaseCard == baseCard).Single()];
        }
        return validBaseSlots
            .Select(x => x.Cards.Where(pred).Select(x => x.Id).ToList())
            .Where(x => x.Count > 0)
            .ToList();
    }

    private PlayableCard GetFieldCardById(Guid Id)
    {
        return _table.GetBaseSlots().SelectMany(x => x.Cards).Where(x => x.Id == Id).SingleOrDefault() ?? throw new Exception($"No field card exists with ID {Id}");
    }
    private BaseCard GetBaseCardByFieldCardId(Guid selectedCardId)
    {
        return _table.GetBaseSlots().SingleOrDefault(slot => slot.Territories.SelectMany(t => t.Cards).SingleOrDefault(card => card.Id == selectedCardId) != null)?.BaseCard ?? throw new Exception($"No base exists that contains a card with ID {selectedCardId}");
    }
    private PlayableCard GetHandCardById(Guid Id)
    {
        return _table.ActivePlayer.Player.Hand.Where(x => x.Id == Id).SingleOrDefault() ?? throw new Exception($"No hand card exists with ID {Id}");
    }
    private BaseCard GetBaseCardById(Guid chosenBaseId)
    {
        return _table.GetActiveBases().Where(x => x.Id == chosenBaseId).FirstOrDefault() ?? throw new Exception($"No active base exists with ID {chosenBaseId}");
    }

}