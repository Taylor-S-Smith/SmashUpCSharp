﻿using System.Collections.Generic;
using FluentResults;

namespace SmashUp.Backend.Models;

/// <summary>
/// Draw: Deck -> Hand
/// Discard: Hand -> Discard
/// Mill: Deck -> Discard
/// Recover: Discard -> Hand
/// Tuck: Hand -> Deck
/// Recure: Discard -> Deck
/// </summary>
internal class Player : Identifiable
{
    public string Name { get; }
    public int VictoryPoints { get; private set; }

    private readonly List<PlayableCard> _hand = [];
    public IReadOnlyList<PlayableCard> Hand => _hand;

    public Deck<PlayableCard> Deck { get; }
    public List<PlayableCard> DiscardPile { get; private set; } = [];


    // Elements are placed in the queue with priority equal to power, or 999 if there is no power constraint
    private const int DEFAULT_MINION_PLAY_PRIORITY = 999;
    public record MinionPlay(int? MaxPower);
    public PriorityQueue<MinionPlay, int> MinionPlayQueue { get; set; } = new();
    public int MinionPlays { get => MinionPlayQueue.Count; }
    public int ActionPlays { get; set; }


    /// <param name="name"></param>
    public Player(string name, List<PlayableCard> cards)
    {
        cards.ForEach(card => card.SetOwner(this));

        Name = name;
        Deck = new(cards);
    }

    public void GainVP(int numVPGained)
    {
        VictoryPoints += numVPGained;
    }

    public void Discard(PlayableCard cardToDiscard)
    {
        if (_hand.Remove(cardToDiscard) != true) throw new Exception($"{Name}'s hand doesn't contain {cardToDiscard.ToString()}");
        DiscardPile.Add(cardToDiscard);
    }

    public PlayableCard? Draw()
    {
        var cardToDraw = Deck.Draw();
        if (cardToDraw == null)
        {
            Deck.Shuffle(DiscardPile);
            DiscardPile = [];
            cardToDraw = Deck.Draw();
        }
        return cardToDraw;
    }
    public List<PlayableCard> Draw(int numToDraw)
    {
        List<PlayableCard> cardsToDraw = [];
        for(int i = 0; i < numToDraw; i++)
        {
            var card = Draw();
            if(card != null) cardsToDraw.Add(card);
        }

        return cardsToDraw;
    }
    public List<PlayableCard> DrawToHand(int numToDraw = 1)
    {
        var cardsToDraw = Draw(numToDraw);
        _hand.AddRange(cardsToDraw);
        return cardsToDraw;
    }
    public void AddToHand(PlayableCard card)
    {
        _hand.Add(card);
    }


    /// <summary>
    /// Shuffles cards from hand to deck.
    /// </summary>
    public void Recard()
    {
        Deck.AddToBottom(_hand);
        Deck.Shuffle();
        _hand.Clear();
    }

    public void RemoveFromHand(PlayableCard cardToPlay)
    {
        _hand.Remove(cardToPlay);
    }

    /// <summary>
    /// Draws a new set of cards equal to the current hand size, then recards the old Hand
    /// </summary>
    public void ReplaceHand()
    {
        var cardsToDraw = Draw(Hand.Count);
        Recard();
        _hand.AddRange(cardsToDraw);
    }

    public void AddMinionPlay(int? maxPower = null)
    {
        MinionPlayQueue.Enqueue(new(maxPower), maxPower ?? DEFAULT_MINION_PLAY_PRIORITY);
    }
    public void SetMinionPlays(int numMinionPlays=1)
    {
        MinionPlayQueue.Clear();
        for (int i = 0; i < numMinionPlays; i++)
        {
            MinionPlayQueue.Enqueue(new(null), DEFAULT_MINION_PLAY_PRIORITY);
        }
    }

    /// <summary>
    /// Removes the most retrictive banked play that applies.
    /// </summary>
    /// <param name="minionPower">The power of the minion to play</param>
    /// <returns>True if a valid play was removed, false if no valid play exists.</returns>
    public bool UseMinionPlay(int minionPower)
    {
        return GetValidMinionPlay(minionPower).IsSuccess;
    }
    public Result HasMinionPlay(int minionPower)
    {
        var result = GetValidMinionPlay(minionPower);

        if(result.IsSuccess)
        {
            MinionPlayQueue.Enqueue(result.Value, result.Value.MaxPower ?? DEFAULT_MINION_PLAY_PRIORITY);
        }
        
        return result.ToResult();
    }
    private Result<MinionPlay> GetValidMinionPlay(int minionPower)
    {
        MinionPlay? validMinionPlay = null;
        List <MinionPlay> minionPlays = [];
        while (MinionPlayQueue.Count > 0)
        {
            MinionPlay minionPlay = MinionPlayQueue.Dequeue();
            if (minionPlay.MaxPower == null || minionPlay.MaxPower >= minionPower)
            {
                validMinionPlay = minionPlay;
                break;
            }

            minionPlays.Add(minionPlay);
        }


        Result<MinionPlay> result;
        if(validMinionPlay == null)
        {
            while (MinionPlayQueue.Count > 0)  minionPlays.Add(MinionPlayQueue.Dequeue());
            var leastRestrictivePlay = minionPlays.LastOrDefault();

            string errorMsg;
            if (leastRestrictivePlay == null) errorMsg = "You don't have any more minion plays";
            else errorMsg = $"You only have plays available for minions of power {leastRestrictivePlay.MaxPower} or less";
            result = Result.Fail(errorMsg);
        } 
        else
        {
            result = Result.Ok(validMinionPlay);
        }

        MinionPlayQueue.EnqueueRange(minionPlays.Select(x => (x, x.MaxPower ?? DEFAULT_MINION_PLAY_PRIORITY)));
        return result;


    }
}
