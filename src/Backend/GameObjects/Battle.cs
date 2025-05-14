using static SmashUp.Backend.Models.PlayableCard;
using SmashUp.Backend.Repositories;
using SmashUp.Backend.Services;
using SmashUp.Backend.Models;
using SmashUp.Backend.API;
using FluentResults;
using LinqKit;
using System.Linq;

namespace SmashUp.Backend.GameObjects;

/// <summary>
/// Handles turn structure (phases) and control flow between front and backend
/// </summary>
internal class Battle
{
    public enum Phase
    {
        PlayCards,
        Special
    }
    public Phase CardPhase { get; set; }

    private readonly IFrontendBattleAPI _userInput;
    private readonly Random _random;
    public readonly GlobalEventManager EventManager;

    private readonly Table _table;
    private bool _battleEnd;

    private const int WINNING_VP = 15;

    private List<Faction> factionsRepresented = [];

    public Battle(IFrontendBattleAPI userInputHandler, GlobalEventManager eventManager, Random random)
    {
        _userInput = userInputHandler;

        EventManager = eventManager;
        _random = random;

        _table = SetUp();

        _userInput.InitializeData(_table);
    }

    // TURN STRUCTURE & CONTROL FLOW 

    /// <summary>
    /// Handles logic relating to the "Set Up" phase of the Smash Up Rule Book
    /// </summary>
    private Table SetUp()
    {
        //Invite Players
        List<string> playerNames = _userInput.ChoosePlayerNames();
        List<Player> players = [];

        //Choose 2 Factions
        List<(string PlayerName, List<Faction> Factions)> factionChoices = _userInput.ChooseFactions(playerNames, Repository.GetFactions());

        factionsRepresented = factionChoices.SelectMany(choice => choice.Factions).ToList();

        //Build Player Decks
        foreach (var choice in factionChoices)
        {
            string playerName = choice.PlayerName;
            List<Faction> factions = choice.Factions;
            List<PlayableCard> cards = Repository.GetPlayableCards(factions);

            var player = new Player(playerName, cards);
            players.Add(player);
        }

        //Build the Base Deck
        List<Faction> allFactions = factionChoices.SelectMany(x => x.Factions).ToList();
        Deck<BaseCard> baseDeck = new(Repository.GetBaseCards(allFactions));

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
        player.Hand = player.Draw(5);

        //Mulligan
        if (!player.Hand.Any(x => x.CardType == PlayableCardType.Minion))
        {
            Option yes = new("YES");
            Option no = new("NO");
            Guid optionId = _userInput.SelectOption([yes, no], player.Hand.ToList(), $"{player.Name}'s opening hand contains no minions, would you like to mulligan?");

            if (optionId == yes.Id)
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
        _table.ActivePlayer.Player.MinionPlays = 10;
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
            Guid? chosenCardId = _userInput.SelectCardOrInvokable(_table.ActivePlayer.Player.Hand.ToList(), _table.GetFieldCards(), displayMessage);
            displayMessage = "";
            if (chosenCardId == null) break;

            PlayableCard? cardToPlay = GetHandCardById((Guid)chosenCardId);

            if (cardToPlay != null)
            {
                Result result = ValidatePlay(_table.ActivePlayer.Player, cardToPlay);
                if (result.IsSuccess)
                {
                    _table.ActivePlayer.Player.RemoveFromHand(cardToPlay);
                    PlayCard(cardToPlay);
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
        while (true)
        {
            // Step 1: Check which bases are ready to score
            List<BaseSlot> scoringBases = [];
            foreach (BaseSlot slot in _table.GetBaseSlots())
            {
                if (slot.BaseCard.CurrentPower >= slot.BaseCard.CurrentBreakpoint)
                {
                    scoringBases.Add(slot);
                }
            }

            if (scoringBases.Count == 0) break;

            // Step 2: Choose one base that is ready to score
            BaseSlot baseToScore;
            if (scoringBases.Count == 1) baseToScore = scoringBases[0];
            else
            {
                Guid chosenBaseId = _userInput.SelectBaseCard(scoringBases.Select(slot => slot.BaseCard.Id).ToList(), null, "Multiple bases are ready to score. Choose a base to score first:");
                baseToScore = GetBaseSlotById(chosenBaseId);
            }

            // Step 3: Play/invoke "before scoring" abilities

            // 3a. Invoke mandatory abilities
            baseToScore.BaseCard.TriggerBeforeBaseScores(this, baseToScore);

            // 3b: Invoke optional abilities, give each player an opportunity to play "Before Scores" specials
            //     starting with the current player and going clockwise (in this case in order players were added)
            SpecialPlayCards(Tag.SpecialBeforeScores);


            // Step 4: Award VPs and play/invoke "when scoring" abilities

            // Order players by position
            Dictionary<int, List<Player>> scoreDict = [];
            foreach (Player player in _table.Players)
            {
                // Must have at least one minion on the base, or at least 1 total power on the base to receive VP
                var playersCards = baseToScore.Cards.Where(card => card.Controller == player);
                int totalPower = playersCards.Sum(x => x.CurrentPower) ?? 0;
                if (playersCards.Any(card => card.CardType == PlayableCardType.Minion) || totalPower > 0)
                {
                    scoreDict.TryGetValue(totalPower, out List<Player>? players);

                    if (players != null) players.Add(player);
                    else scoreDict.Add(totalPower, [player]);
                }
            }

            // Award points
            int baseAwardIndex = 0;
            int playerPostitionIndex = 0;
            List<int> powerVals = scoreDict.Keys.OrderDescending().ToList();

            while (baseAwardIndex < baseToScore.BaseCard.PointSpread.Length && baseAwardIndex < powerVals.Count)
            {
                List<Player> players = scoreDict[powerVals[playerPostitionIndex]];
                playerPostitionIndex++;
                foreach (Player player in players)
                {
                    player.GainVP(baseToScore.BaseCard.PointSpread[baseAwardIndex]);
                }

                baseAwardIndex += players.Count;
            }

            // Step 5: Award treasures
            // Step 6: Play/invoke "after scoring" abilities
            BaseCard? newBase = baseToScore.BaseCard.TriggerAfterBaseScores(this, baseToScore, scoreDict[powerVals[0]]);

            // Step 7: Discard all the cards on the base
            baseToScore.Cards.ForEach(x => RemoveCardFromBattleField(x));
            baseToScore.Cards.ForEach(Discard);

            // Step 8: Discard the base
            _table.AddBaseToDiscard(baseToScore.BaseCard);

            // Step 9: Replace the base
            if (newBase != null) baseToScore.BaseCard = newBase;
            else baseToScore.BaseCard = _table.DrawBaseCard();
        }
    }

    private void SpecialPlayCards(Tag specialPhase)
    {
        // We will continue until all players have passed
        Dictionary<Player, bool> passTracker = [];

        foreach (Player player in _table.Players)
        {
            passTracker.Add(player, false);
        }

        int playerIndex = _table.Players.FindIndex((player) => player == _table.ActivePlayer.Player);
        bool hasPassed = true;
        while (!passTracker.Values.All(hasPassed => hasPassed))
        {
            var activePlayer = _table.Players[playerIndex];

            List<PlayableCard> specialCards = activePlayer.Hand.Where(x => x.Tags.Contains(specialPhase)).ToList();
            Guid? specialId = _userInput.SelectCardOrInvokable(specialCards, [], $"{activePlayer.Name}, play special cards, or pass");

            if (specialId != null)
            {
                var specialToPlay = specialCards.Where(x => x.Id == specialId).Single();
                _table.ActivePlayer.Player.RemoveFromHand(specialToPlay);
                PlayCard(specialToPlay);
                hasPassed = false;
            }
            else
            {
                passTracker[activePlayer] = hasPassed;
                playerIndex++;
                if (playerIndex == _table.Players.Count) playerIndex = 0;
                hasPassed = true;
            }
        }
    }

    /// <summary>
    /// Handles logic relating to the "Draw 2 Cards" phase of the Smash Up Rule Book
    /// </summary>
    private void Draw2Cards()
    {
        var cardsDrawn = _table.ActivePlayer.Player.Draw(2);
        _table.ActivePlayer.Player.Hand.AddRange(cardsDrawn);
        _userInput.ViewCards(cardsDrawn.ConvertAll(x => (Card)x), "You drew the following:", "CONTINUE");

        int numCards = _table.ActivePlayer.Player.Hand.Count;
        if (numCards > 10)
        {
            int numToDiscard = numCards - 10;
            var handCards = _table.ActivePlayer.Player.Hand.ToList().ConvertAll(x => (Card)x);
            var cardIdsToDiscard = _userInput.Select(handCards.AsDisplayable(), handCards.AsDisplayable(), $"Choose {numToDiscard} card{(numToDiscard > 1 ? 's' : null)} to discard", numToDiscard);
            foreach (var cardId in cardIdsToDiscard)
            {
                var card = GetHandCardById(cardId);
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
        List<(Player Player, int VP)> playerVpTotals = _table.GetPlayerVP();

        List<int> vpTotals = playerVpTotals.Select(x => x.VP).ToList();
        int vpMax = vpTotals.Max();

        //One clear winner, no ties in VP
        if (vpMax >= WINNING_VP && vpTotals.Where(x => x == vpMax).ToList().Count == 1)
        {
            _battleEnd = true;
            var winningPlayer = playerVpTotals.Single(x => x.VP == vpMax).Player;
            _userInput.EndBattle(winningPlayer);
        }
    }
    private void SwitchActivePlayer()
    {
        int newActivePlayerIndex = _table.Players.IndexOf(_table.ActivePlayer.Player) + 1;
        if (newActivePlayerIndex == _table.Players.Count) newActivePlayerIndex = 0;
        _table.ActivePlayer.Player = _table.Players[newActivePlayerIndex];
    }


    // PLAY CARDS
    public void PlayCard(PlayableCard cardToPlay)
    {
        RemovePlayResource(cardToPlay.Controller, cardToPlay);
        ResolveCardPlay(cardToPlay);
    }
    public void PlayExtraCard(PlayableCard cardToPlay)
    {
        ResolveCardPlay(cardToPlay);
    }
    private void ResolveCardPlay(PlayableCard cardToPlay)
    {
        if (cardToPlay.PlayLocation == PlayLocation.Base)
        {
            PlayCardToBase(cardToPlay);
        }
        else if (cardToPlay.PlayLocation == PlayLocation.Minion)
        {
            PlayCardToMinion(cardToPlay);
        }
        else if (cardToPlay.PlayLocation == PlayLocation.Discard)
        {
            PlayCardToDiscard(cardToPlay);
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
        Guid chosenBaseId = _userInput.SelectBaseCard(validBaseIds, cardToPlay, $"Choose a base to play {cardToPlay.Name} on");
        BaseCard chosenBase = GetBaseCardById(chosenBaseId);

        // Add Card to territory
        // NOTE: It is a rules req that the card exists in the base BEFORE on play is triggered
        // Which happens before add to base effects, which means that it can't be triggered in a
        // more primitive funciton in the table
        BaseSlot slot = _table.GetBaseSlots().Where(x => x.BaseCard == chosenBase).Single();
        AddCardToBase(cardToPlay, slot);

        // Activate Card Ability
        cardToPlay.TriggerOnPlay(this, slot);
    }
    private void AddCardToBase(PlayableCard cardToPlay, BaseSlot slot)
    {
        PlayerTerritory territory = slot.Territories.Where(x => x.player == cardToPlay.Controller).Single();
        territory.Cards.Add(cardToPlay);
        cardToPlay.TriggerOnAddToBase(this, slot);
        slot.BaseCard.TriggerOnAddCard(this, cardToPlay);
    }
    private void PlayCardToDiscard(PlayableCard cardToPlay)
    {
        cardToPlay.TriggerOnPlay(this);
        Discard(cardToPlay);
    }
    private void PlayCardToMinion(PlayableCard cardToPlay)
    {
        SelectFieldCardQuery query = new()
        {
            CardType = PlayableCardType.Minion,
        };
        PlayableCard? cardToAttachTo = SelectFieldCard(cardToPlay, $"Choose a minion to attach {cardToPlay.Name} to", query)?.SelectedCard;

        if (cardToAttachTo != null)
        {
            if (AttemptToAffect(cardToAttachTo, EffectType.Attach, PlayableCardType.Action, cardToPlay.Controller))
            {
                cardToAttachTo.Attach(cardToPlay);
                cardToPlay.TriggerOnAttach(this, cardToAttachTo);
            }
            else
            {
                cardToPlay.Owner.DiscardPile.Add(cardToPlay);
            }
        }
    }


    // CARD EFFECTS
    public void Destroy(PlayableCard cardToDestroy, PlayableCard affector)
    {
        if (AttemptToAffect(cardToDestroy, EffectType.Destroy, affector.CardType, affector.Controller))
        {
            var baseCard = RemoveCardFromBattleField(cardToDestroy);
            Discard(cardToDestroy);
            baseCard.TriggerAfterDestroyCard(cardToDestroy);
        }
    }
    /// <summary>
    /// Discarding removes the card from play, but does not count as affecting the card
    /// </summary>
    public void Discard(PlayableCard cardToPlay)
    {
        cardToPlay.TriggerOnDiscard(EventManager);
        cardToPlay.Owner.DiscardPile.Add(cardToPlay);
    }
    public void Move(PlayableCard cardToMove, BaseCard newBase, PlayableCard affector)
    {
        if (AttemptToAffect(cardToMove, EffectType.Move, affector.CardType, affector.Controller))
        {
            // We don't replace the two methods with a call RemoveCardFromBattleField() since we don't want to tigger any leave battlefield effects
            BaseSlot baseSlot = GetBaseSlot(cardToMove);
            RemoveCardFromBase(cardToMove, baseSlot);
            var newBaseSlot = GetBaseSlotById(newBase.Id);
            AddCardToBase(cardToMove, newBaseSlot);
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
                protector = Select(protections.Select(x => x.GrantedBy).ToList(), $"{cardToAffect.Controller.Name}, {cardToAffect.Name} is being protected by multiple cards, choose which one to recieve protection from:");
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
    private BaseCard RemoveCardFromBattleField(PlayableCard cardToRemove)
    {
        foreach (BaseSlot slot in _table.GetBaseSlots())
        {
            if (RemoveCardFromBase(cardToRemove, slot)) return slot.BaseCard;
        }

        throw new Exception($"{cardToRemove.Name} with ID {cardToRemove.Id} is not on the battlefield, so can't be removed");
    }
    private bool RemoveCardFromBase(PlayableCard cardToRemove, BaseSlot slot)
    {
        foreach (PlayerTerritory territory in slot.Territories)
        {
            // Check all played cards on base
            if (territory.Cards.Remove(cardToRemove))
            {
                // Trigger leave of base
                cardToRemove.TriggerOnRemoveFromBase(this, slot);
                slot.BaseCard.TriggerOnRemoveCard(this, cardToRemove);
                return true;
            }
            else
            {
                // Check cards attached to played cards
                foreach (var card in territory.Cards)
                {
                    if (card.Detach(cardToRemove))
                    {
                        //Trigger on detach
                        cardToRemove.TriggerOnDetach(this, card);
                        slot.BaseCard.TriggerOnRemoveCard(this, cardToRemove);
                        return true;
                    }
                }
            }
        }

        return false;
    }


    // INTERACT WITH TABLE
    public List<BaseSlot> GetBaseSlots()
    {
        return _table.GetBaseSlots();
    }
    public BaseSlot GetBaseSlot(BaseCard baseCard)
    {
        return _table.GetBaseSlots().Where(x => x.BaseCard == baseCard).SingleOrDefault() ?? throw new Exception($"No slot exists for {baseCard.Name} with ID: {baseCard.Id}");
    }
    private BaseSlot GetBaseSlot(PlayableCard cardToFind)
    {
        foreach (BaseSlot slot in _table.GetBaseSlots())
        {
            foreach (PlayerTerritory territory in slot.Territories)
            {
                // Check all played cards on base
                if (territory.Cards.Contains(cardToFind))
                {
                    return slot;
                }
                else
                {
                    // Check cards attached to played cards
                    foreach (var card in territory.Cards)
                    {
                        if (card.Attachments.Contains(cardToFind))
                        {
                            return slot;
                        }
                    }
                }
            }
        }
        throw new Exception($"No base contains {cardToFind.Name} with ID {cardToFind.Id}");
    }
    public List<BaseCard> GetBases()
    {
        return _table.GetBaseSlots().Select(slot => slot.BaseCard).ToList();
    }
    public BaseCard GetBase(PlayableCard cardToFind)
    {
        return GetBaseSlot(cardToFind).BaseCard;
    }
    public List<Player> GetPlayers()
    {
        return _table.Players;
    }
    public List<Player> GetOtherPlayers(Player player)
    {
        return _table.Players.Where(x => x != player).ToList();
    }
    public Player GetActivePlayer()
    {
        return _table.ActivePlayer.Player;
    }
    public List<BaseCard> DrawBases(int numToDraw)
    {
        return _table.Board.BaseDeck.Draw(numToDraw);
    }
    internal void PutBasesToTop(List<BaseCard> chosenOrder)
    {
        foreach (var baseCard in chosenOrder)
        {
            _table.Board.BaseDeck.AddToTop(baseCard);
        }
    }


    // INTERACTING WITH UI
    public class SelectFieldCardQuery
    {
        public PlayableCardType? CardType { get; set; }
        public Faction? Faction { get; set; }
        public BaseCard? BaseCard { get; set; }
        public int? MaxPower { get; set; }
        public List<Player> Controllers { get; set; } = [];
        public List<PlayableCard> CardsToExclude { get; set; } = [];
    }
    public record SelectFieldCardResult(PlayableCard? SelectedCard, BaseCard? SelectedCardBase, ResultType Type);
    public SelectFieldCardResult SelectFieldCard(PlayableCard cardToDisplay, string displaytext, SelectFieldCardQuery query, bool cancellable = false)
    {
        Func<PlayableCard, bool> cardPred = GetPred(query);

        List<List<Guid>> validFieldCardIds = GetFieldCardIds(cardPred, query.BaseCard);
        if (validFieldCardIds.SelectMany(ids => ids).ToList().Count == 0) return new(null, null, ResultType.NoValidTargets);

        SelectResult result = _userInput.SelectFieldCard(validFieldCardIds, cardToDisplay, displaytext, cancellable);

        return new(
            result.SelectedId != null ? GetFieldCardById((Guid)result.SelectedId) : null,
            result.SelectedId != null ? GetBaseCardByFieldCardId((Guid)result.SelectedId) : null,
            result.Type
        );
    }

    private static Func<PlayableCard, bool> GetPred(SelectFieldCardQuery query)
    {
        var cardPred = PredicateBuilder.New((PlayableCard card) => !query.CardsToExclude.Contains(card));
        if (query.CardType != null) cardPred.And((PlayableCard card) => card.CardType == query.CardType);
        if (query.Faction != null) cardPred.And((PlayableCard card) => card.Faction == query.Faction);
        if (query.MaxPower != null) cardPred.And((PlayableCard card) => card.CurrentPower <= query.MaxPower);
        if (query.Controllers.Count > 0) cardPred.And((PlayableCard card) => query.Controllers.Contains(card.Controller));
        return cardPred;
    }

    public class SelectFieldCardsQuery : SelectFieldCardQuery
    {
        public int Num { get; set; }
    }
    public record SelectFieldCardsResult(List<(PlayableCard Card, BaseCard Base)> SelectedCards, ResultType Type);
    public SelectFieldCardsResult SelectFieldCards(PlayableCard cardToDisplay, string displaytext, SelectFieldCardsQuery query, bool cancellable = false)
    {
        List<(PlayableCard Card, BaseCard Base)> chosenCards = [];
        for (int i = 0; i < query.Num; i++)
        {
            var result = SelectFieldCard(cardToDisplay, displaytext, query, cancellable);
            if (result.Type != ResultType.Success) break;
            chosenCards.Add((result.SelectedCard!, result.SelectedCardBase!));
            query.CardsToExclude.Add(result.SelectedCard!);
        }
        return new(chosenCards, ResultType.Success);
    }
    public BaseCard SelectBaseCard(List<BaseCard> validBases, PlayableCard cardToDisplay, string displayText)
    {
        Guid chosenId = _userInput.SelectBaseCard(validBases.Select(x => x.Id).ToList(), cardToDisplay, displayText);
        return validBases.Where(x => x.Id == chosenId).SingleOrDefault() ?? throw new Exception($"No option with ID: {chosenId}");
    }
    public T Select<T>(List<T> options, string displayText, Func<T, bool>? isValidChoice = null)
        where T : Displayable
    {
        isValidChoice ??= _ => true;
        Guid chosenId = _userInput.Select(options.AsDisplayable(), options.Where(isValidChoice).AsDisplayable(), displayText, 1).Single();
        
        return options.Where(x => x.Id == chosenId).SingleOrDefault() ?? throw new Exception($"No option with ID: {chosenId}");
    }
    /// <summary>
    /// Accesses UI to allow player to select several cards at once from options. 
    /// The order of the returned list will be reverse the order they were selected.
    /// That is, the last card of the list was chosen first by the player.
    /// </summary>
    /// <param name="numToSelect">The num to return, or null if any number works</param>
    /// <param name="isValidChoice">Determins which options are selectable</param>
    public List<T> SelectMultiple<T>(List<T> options, string displayText, int? numToSelect = null, Func<T, bool>? isValidChoice = null)
        where T : Displayable
    {
        isValidChoice ??= _ => true;
        List<Guid> chosenIds = _userInput.Select(options.AsDisplayable(), options.Where(isValidChoice).AsDisplayable(), displayText, numToSelect).ToList();

        // We get the result based on a select on chosenIds because order matters
        return chosenIds.Select(id => options.Where(option => option.Id == id).SingleOrDefault() ?? throw new Exception($"No option with ID: {id}")).ToList();
    }
    public Guid SelectOption(List<Option> options, List<PlayableCard> cardsToDisplay, string displayText)
    {
        return _userInput.SelectOption(options, cardsToDisplay, displayText);
    }
    public bool SelectBool(List<PlayableCard> cardsToDisplay, string displayText)
    {
        Option yes = new("YES");
        Option no = new("NO");
        return yes.Id == SelectOption([yes, no], cardsToDisplay, displayText);
    }

    //GET
    public List<PlayableCard> GetFieldCards(SelectFieldCardQuery query)
    {
        var pred = GetPred(query);
        return GetFieldCards(pred, query.BaseCard);
    }
    private List<PlayableCard> GetFieldCards(Func<PlayableCard, bool> pred, BaseCard? baseCard = null)
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
    private List<List<Guid>> GetFieldCardIds(Func<PlayableCard, bool> pred, BaseCard? baseCard = null)
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
    private BaseSlot GetBaseSlotById(Guid chosenBaseId)
    {
        return _table.GetBaseSlots().Where(x => x.BaseCard.Id == chosenBaseId).FirstOrDefault() ?? throw new Exception($"No active base exists with ID {chosenBaseId}");
    }

    /// <summary>
    /// Gets all the factions, ordered by the ones actually used in the battle
    /// </summary>
    public List<Faction> GetFactions()
    {
        return Repository.GetFactions().OrderBy(x => !factionsRepresented.Contains(x)).ToList();
    }
}