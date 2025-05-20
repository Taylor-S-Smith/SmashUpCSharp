using static SmashUp.Backend.Models.PlayableCard;
using SmashUp.Backend.Repositories;
using SmashUp.Backend.Services;
using SmashUp.Backend.Models;
using SmashUp.Backend.API;
using FluentResults;
using LinqKit;
using System.Linq;
using System.Numerics;
using static SmashUp.Backend.Models.ScoreResult;

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

    class PlayTracker()
    {
        private Dictionary<Player, int> playDict = [];

        public void AddPlayers(List<Player> players)
        {
            players.ForEach(player => playDict.Add(player, 0));
        }

        internal void AddPlay(Player player)
        {
            playDict[player]++;
        }
        public int GetPlayerPlays(Player player)
        {
            return playDict[player];
        }
        public void Clear()
        {
            playDict.ForEach(x => playDict[x.Key] = 0);
        }
    }

    private PlayTracker plays = new();


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

        //Build Players and Decks
        foreach (var choice in factionChoices)
        {
            string playerName = choice.PlayerName;
            List<Faction> factions = choice.Factions;
            List<PlayableCard> cards = Repository.GetPlayableCards(factions);

            var player = new Player(playerName, cards);
            players.Add(player);
        }
        plays.AddPlayers(players);


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
        player.DrawToHand(5);

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
        _table.ActivePlayer.Player.SetMinionPlays(1);

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
                Result result = ValidatePlay(cardToPlay);
                if (result.IsSuccess)
                {
                    plays.AddPlay(cardToPlay.Controller);
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
            BaseSlot slotToScore;
            if (scoringBases.Count == 1) slotToScore = scoringBases[0];
            else
            {
                Guid chosenBaseId = _userInput.SelectBaseCard(scoringBases.Select(slot => slot.BaseCard.Id).ToList(), null, "Multiple bases are ready to score. Choose a base to score first:");
                slotToScore = GetBaseSlotById(chosenBaseId);
            }

            // Step 3: Play/invoke "before scoring" abilities

            // 3a. Invoke mandatory abilities
            slotToScore.BaseCard.BeforeBaseScores(this, slotToScore);
            EventManager.TriggerBeforeBaseScores(this, slotToScore);

            // 3b: Invoke optional abilities, give each player an opportunity to play "Before Scores" specials
            //     starting with the current player and going clockwise (in this case in order players were added)
            SpecialPlayCards(Tag.SpecialBeforeScores);


            // Step 4: Award VPs and play/invoke "when scoring" abilities

            // Order players by position
            Dictionary<int, List<Player>> playersByScore = [];
            foreach (Player player in _table.Players)
            {
                // Must have at least one minion on the base, or at least 1 total power on the base to receive VP
                var playersCards = slotToScore.Cards.Where(card => card.Controller == player);
                int totalPower = playersCards.Sum(x => x.CurrentPower) ?? 0;
                if (playersCards.Any(card => card.CardType == PlayableCardType.Minion) || totalPower > 0)
                {
                    if (playersByScore.TryGetValue(totalPower, out List<Player>? players)) players.Add(player);
                    else playersByScore.Add(totalPower, [player]);
                }
            }

            // Award points
            int baseAwardIndex = 0;
            int playerPostitionIndex = 0;
            List<int> powerVals = playersByScore.Keys.OrderDescending().ToList();

            while (baseAwardIndex < slotToScore.BaseCard.PointSpread.Length && baseAwardIndex < powerVals.Count)
            {
                List<Player> players = playersByScore[powerVals[playerPostitionIndex]];
                playerPostitionIndex++;
                foreach (Player player in players)
                {
                    player.GainVP(slotToScore.BaseCard.PointSpread[baseAwardIndex]);
                }

                baseAwardIndex += players.Count;
            }

            ScoreResult scoreResult = new(playersByScore);

            // When Base scores
            slotToScore.BaseCard.WhenBaseScores(this, scoreResult);


            // Step 5: Award treasures
            // Step 6: Play/invoke "after scoring" abilities
            BaseCard? newBase = slotToScore.BaseCard.AfterBaseScores(this, slotToScore, scoreResult);
            EventManager.TriggerAfterBaseScores(this, slotToScore);

            // Step 7: Discard all the cards on the base
            List<PlayableCard> cardsToDiscard = slotToScore.Cards;
            cardsToDiscard.ForEach(x => RemoveCardFromBattleField(x));
            cardsToDiscard.ForEach(Discard);

            // Step 8: Discard the base
            _table.AddBaseToDiscard(slotToScore.BaseCard);

            // Step 9: Replace the base
            BaseCard oldBase = slotToScore.BaseCard;
            if (newBase != null) slotToScore.BaseCard = newBase;
            else slotToScore.BaseCard = _table.DrawBaseCard();

            // Trigger After Replaced Abilities
            oldBase.OnReplaced(this, slotToScore, scoreResult);
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
        var cardsDrawn = _table.ActivePlayer.Player.DrawToHand(2);
        if (cardsDrawn.Count > 0) _userInput.ViewCards(cardsDrawn.ConvertAll(x => (Card)x), "You drew the following:", "CONTINUE");
        else _userInput.ViewCards(cardsDrawn.ConvertAll(x => (Card)x), "You have no cards in your deck or discard pile to draw", "CONTINUE");

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
        EventManager.TriggerEndOfTurn(this, _table.ActivePlayer);
        CheckEndOfGame();
        plays.Clear();
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
    private static Result ValidatePlay(PlayableCard cardToPlay)
    {
        if (cardToPlay.CardType == PlayableCardType.Minion)
        {
            return cardToPlay.Controller.HasMinionPlay(cardToPlay.CurrentPower ?? 0);
        }
        else if (cardToPlay.CardType == PlayableCardType.Action)
        {
            if (cardToPlay.Controller.ActionPlays == 0) return Result.Fail("You don't have any more action plays");
        }
        return Result.Ok();
    }
    private static void RemovePlayResource(Player player, PlayableCard cardToPlay)
    {
        if (cardToPlay.CardType == PlayableCardType.Minion)
        {
            var result = player.UseMinionPlay(cardToPlay.CurrentPower ?? 0);
            if (!result) throw new Exception($"Attempted to remove minion play for power {cardToPlay.CurrentPower}, but no valid minion play exists");
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

        // Enter Battlefield effects - This triggers maintenance effects,
        // such as subscribing to global events. It is NOT 'On Play' or
        // other card timings directly referenced by card text
        cardToPlay.EnterBattlefield(this);
        EventManager.TriggerCardEnteredBattlefield(this, cardToPlay);

        // Add Card to territory
        // NOTE: It is a rules req that the card exists in the base BEFORE on play is triggered
        // Which happens before add to base effects, which means that it can't be triggered in a
        // more primitive function in the table
        BaseSlot slot = _table.GetBaseSlots().Where(x => x.BaseCard == chosenBase).Single();
        AddCardToBase(cardToPlay, slot);

        // Activate Card Ability
        cardToPlay.OnPlay(this, slot);
    }
    private void AddCardToBase(PlayableCard cardToPlay, BaseSlot slot)
    {
        PlayerTerritory territory = slot.Territories.Where(x => x.player == cardToPlay.Controller).Single();
        territory.Cards.Add(cardToPlay);

        // Enter Base effects
        cardToPlay.EnterBase(this, slot);
        slot.BaseCard.OnAddCard(cardToPlay);
        EventManager.TriggerCardEnteredBase(this, cardToPlay, slot);
    }
    private void PlayCardToDiscard(PlayableCard cardToPlay)
    {
        cardToPlay.OnPlay(this, null);
        Discard(cardToPlay);
    }
    private void PlayCardToMinion(PlayableCard cardToPlay)
    {
        SelectCardQuery query = new()
        {
            CardType = PlayableCardType.Minion,
        };
        PlayableCard? cardToAttachTo = SelectFieldCard(cardToPlay, $"Choose a minion to attach {cardToPlay.Name} to", query)?.SelectedCard;

        if (cardToAttachTo != null)
        {
            if (AttemptToAffect(cardToAttachTo, EffectType.Attach, PlayableCardType.Action, cardToPlay.Controller))
            {
                cardToAttachTo.Attach(cardToPlay);
                cardToPlay.OnAttach(this, cardToAttachTo);
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
            EventManager.TriggerBeforeDestroyCard(this, cardToDestroy);
            var baseSlot = DiscardFromField(cardToDestroy);
            cardToDestroy.OnDestroyed(this, baseSlot);
            if(cardToDestroy.CardType == PlayableCardType.Minion) baseSlot.BaseCard.AfterMinionDestroyed(cardToDestroy);
            EventManager.TriggerOnDestroyCard(this, cardToDestroy);
        }
    }

    public BaseSlot DiscardFromField(PlayableCard cardToDiscard)
    {
        var baseSlot = RemoveCardFromBattleField(cardToDiscard);
        Discard(cardToDiscard);
        return baseSlot;
    }

    /// <summary>
    /// Discarding puts the card in it's owner's discard pile, it does not count as affecting the card
    /// This does NOT remove the card from it's old location
    /// </summary>
    private void Discard(PlayableCard cardToPlay)
    {
        cardToPlay.Owner.DiscardPile.Add(cardToPlay);
    }
    public void Move(PlayableCard cardToMove, BaseCard newBase, PlayableCard affector)
    {
        if (AttemptToAffect(cardToMove, EffectType.Move, affector.CardType, affector.Controller))
        {
            PerformMove(cardToMove, newBase);
        }
    }
    public void Move(PlayableCard cardToMove, BaseCard newBase, Player affector)
    {
        if (AttemptToAffect(cardToMove, EffectType.Move, null, affector))
        {
            PerformMove(cardToMove, newBase);
        }
    }
    private void PerformMove(PlayableCard cardToMove, BaseCard newBase)
    {
        BaseSlot oldBaseSlot = GetBaseSlot(cardToMove);
        RemoveCardFromBase(cardToMove, oldBaseSlot);
        var newBaseSlot = GetBaseSlotById(newBase.Id);
        AddCardToBase(cardToMove, newBaseSlot);
    }

    /// <summary>
    /// Checks if a card can be affected by a specific effect. 
    /// If it is protected, this will also resolve the protection triggers
    /// </summary>
    /// <param name="affectorCardType">Action or Minion. Leave null if a playable card is not
    ///                                causing the action to occur, e.g. Base or passive effect</param>
    /// <returns>True if card can be affected, otherwise false</returns>
    public bool AttemptToAffect(PlayableCard cardToAffect, EffectType effect, PlayableCardType? affectorCardType, Player affectorPlayer)
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

            protector.OnProtect(this);
            return false;
        }
    }
    /// <summary>
    /// Removes card from base, calls appropriate triggers
    /// </summary>
    /// <param name="AddFunction">Function that is called after removal, usually determines where the card ends up</param>
    /// <returns>Base the card was removed from</returns>
    private BaseSlot RemoveCardFromBattleField(PlayableCard cardToRemove)
    {
        foreach (BaseSlot slot in _table.GetBaseSlots())
        {
            if (RemoveCardFromBase(cardToRemove, slot))
            {
                EventManager.TriggerCardExitedBattlefield(this, cardToRemove);
                cardToRemove.ExitBattlefield(this);
                return slot;
            }
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
                cardToRemove.ExitBase(this, slot);
                slot.BaseCard.RemoveCard(cardToRemove);
                EventManager.TriggerCardExitedBase(this, cardToRemove, slot);
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
                        cardToRemove.OnDetach(this, card);
                        slot.BaseCard.RemoveCard(cardToRemove);
                        EventManager.TriggerCardExitedBase(this, cardToRemove, slot);
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
    public class SelectCardQuery
    {
        public PlayableCardType? CardType { get; set; }
        public Faction? Faction { get; set; }
        public BaseCard? BaseCard { get; set; }
        public int? MaxPower { get; set; }
        public List<Player> Controllers { get; set; } = [];
        public List<Tag> Tags { get; set; } = []; 
        public List<PlayableCard> CardsToExclude { get; set; } = [];
    }
    public record SelectFieldCardResult(PlayableCard? SelectedCard, BaseCard? SelectedCardBase, ResultType Type);
    public SelectFieldCardResult SelectFieldCard(Displayable displayable, string displaytext, SelectCardQuery query, bool cancellable = false)
    {
        Func<PlayableCard, bool> cardPred = GetPred(query);

        List<List<Guid>> validFieldCardIds = GetFieldCardIds(cardPred, query.BaseCard);
        List<Guid> validIds = validFieldCardIds.SelectMany(ids => ids).ToList();
        if (validIds.Count == 0) return new(null, null, ResultType.NoValidTargets);
        if (validIds.Count == 1 && !cancellable) return new(GetFieldCardById(validIds[0]), GetBaseCardByFieldCardId(validIds[0]), ResultType.Success);

        SelectResult result = _userInput.SelectFieldCard(validFieldCardIds, displayable, displaytext, cancellable);

        return new(
            result.SelectedId != null ? GetFieldCardById((Guid)result.SelectedId) : null,
            result.SelectedId != null ? GetBaseCardByFieldCardId((Guid)result.SelectedId) : null,
            result.Type
        );
    }

    private static Func<PlayableCard, bool> GetPred(SelectCardQuery query)
    {
        var cardPred = PredicateBuilder.New((PlayableCard card) => !query.CardsToExclude.Contains(card));
        if (query.CardType != null) cardPred.And((PlayableCard card) => card.CardType == query.CardType);
        if (query.Faction != null) cardPred.And((PlayableCard card) => card.Faction == query.Faction);
        if (query.MaxPower != null) cardPred.And((PlayableCard card) => card.CurrentPower <= query.MaxPower);
        if (query.Tags.Count > 0) cardPred.And((PlayableCard card) => query.Tags.Any(tag => card.Tags.Contains(tag)));
        if (query.Controllers.Count > 0) cardPred.And((PlayableCard card) => query.Controllers.Contains(card.Controller));
        return cardPred;
    }

    public class SelectFieldCardsQuery : SelectCardQuery
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
    public BaseCard SelectBaseCard(List<BaseCard> validBases, Displayable displayable, string displayText)
    {
        Guid chosenId = _userInput.SelectBaseCard(validBases.Select(x => x.Id).ToList(), displayable, displayText);
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
    public List<PlayableCard> GetFieldCards(SelectCardQuery query)
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

    private PlayableCard? GetFieldCardById(Guid Id)
    {
        return _table.GetBaseSlots().SelectMany(x => x.Cards).Where(x => x.Id == Id).SingleOrDefault();
    }
    public bool IsInField(PlayableCard card)
    {
        return GetFieldCardById(card.Id) != null;
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
    public List<PlayableCard> GetHandCards(Player player, SelectCardQuery query)
    {
        var pred = GetPred(query);
        return player.Hand.Where(pred).ToList();
    }

    /// <summary>
    /// Gets all the factions, ordered by the ones actually used in the battle
    /// </summary>
    public List<Faction> GetFactions()
    {
        return Repository.GetFactions().OrderBy(x => !factionsRepresented.Contains(x)).ToList();
    }

    public int GetTurnPlays(Player player)
    {
        return plays.GetPlayerPlays(player);
    }
}