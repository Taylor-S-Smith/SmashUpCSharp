using SmashUpBackend.Models;

namespace Backend.Models;

/// <summary>
/// Draw: Deck -> Hand
/// Discard: Hand -> Discard
/// Mill: Deck -> Discard
/// Recover: Discard -> Hand
/// Tuck: Hand -> Deck
/// Recure: Discard -> Deck
/// Generate: Outside of Game -> Hand
/// </summary>
internal class Player : Identifiable
{
    public string Name { get; }
    public int VictoryPoints { get; private set; }

    private readonly List<PlayableCard> _hand = [];
    public IReadOnlyList<PlayableCard> Hand => _hand.AsReadOnly();
    private Deck<PlayableCard> Deck { get; }
    private Deck<PlayableCard> DiscardPile { get; } = new();

    /// <param name="name"></param>
    public Player(string name, List<PlayableCard> cards)
    {
        cards.ForEach(card => card.SetOwner(Id));

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

    public void Draw(int numToDraw)
    {
        _hand.AddRange(Deck.Draw(numToDraw));
    }


    /// <summary>
    /// Shuffles cards from hand to deck.
    /// </summary>
    public void Recard()
    {
        Deck.AddCards(_hand);
        _hand.Clear();
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
