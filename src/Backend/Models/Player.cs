using System.Collections.Generic;
using SmashUp.Backend.GameObjects;

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
    public int MinionPlays { get; set; }
    public int ActionPlays { get; set; }
    public List<PlayableCard> Hand { get; set; } = [];
    public Deck<PlayableCard> Deck { get; }
    public List<PlayableCard> DiscardPile { get; private set; } = [];

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
        if (Hand.Remove(cardToDiscard) != true) throw new Exception($"{Name}'s hand doesn't contain {cardToDiscard.ToString()}");
        DiscardPile.Add(cardToDiscard);
    }

    public List<PlayableCard> Draw(int numToDraw)
    {
        List<PlayableCard> cardsToDraw = [];
        for(int i = 0; i < numToDraw; i++)
        {
            cardsToDraw.Add(Draw());
        }

        return cardsToDraw;
    }

    public PlayableCard Draw()
    {
        var cardToDraw = Deck.Draw();
        if(cardToDraw == null)
        {
            Deck.Shuffle(DiscardPile);
            DiscardPile = [];
            cardToDraw = Deck.Draw();
            if (cardToDraw == null) throw new Exception($"Failed to draw card, there are no cards in {Name}'s deck or dicard pile");
        }
        return cardToDraw;
    }

    /// <summary>
    /// Shuffles cards from hand to deck.
    /// </summary>
    public void Recard()
    {
        Deck.AddToBottom(Hand);
        Deck.Shuffle();
        Hand.Clear();
    }

    public void RemoveFromHand(PlayableCard cardToPlay)
    {
        Hand.Remove(cardToPlay);
    }

    /// <summary>
    /// Draws a new set of cards equal to the current hand size, then recards the old Hand
    /// </summary>
    public void ReplaceHand()
    {
        var cardsToDraw = Draw(Hand.Count);
        Recard();
        Hand.AddRange(cardsToDraw);
    }
}
