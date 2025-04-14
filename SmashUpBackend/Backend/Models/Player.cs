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

    private readonly List<PlayableCard> _hand = [];
    public IReadOnlyList<PlayableCard> Hand => _hand.AsReadOnly();
    private Deck<PlayableCard> Deck { get; }
    private Deck<PlayableCard> DiscardPile { get; } = new();

    /// <param name="name"></param>
    public Player(string name, List<PlayableCard> cards)
    {
        cards.ForEach(card => card.Owner = this);

        Name = name;
        Deck = new(cards);
    }

    public List<string> GetDeck()
    {
        return Deck.GetCards();
    }
    public List<string> GetDiscard()
    {
        return DiscardPile.GetCards();
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

    public void Draw(int numToDraw)
    {
        _hand.AddRange(Deck.Draw(numToDraw));
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
        Deck.Add(_hand);
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
        var cardsToDraw = Deck.Draw(Hand.Count);
        Recard();
        _hand.AddRange(cardsToDraw);
    }
}
