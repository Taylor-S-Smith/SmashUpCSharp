using System.Collections.Generic;

namespace SmashUp.Backend.Models;

internal class Deck<T> where T : Card
{
    private List<T> _cards;

    public Deck(List<T> cards)
    {
        _cards = cards;
        Shuffle();
    }
    public Deck()
    {
        _cards = [];
    }


    public List<string> GetCards()
    {
        return _cards.Select(c => c.Name).ToList();
    }
    public void AddToBottom(List<T> cards)
    {
        _cards.AddRange(cards);
    }
    public void AddToBottom(T card)
    {
        _cards.Add(card);
    }
    public void Shuffle()
    {
        Random rng = new();
        int n = _cards.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            (_cards[n], _cards[k]) = (_cards[k], _cards[n]);
        }
    }
    public void Shuffle(List<T> cards)
    {
        _cards.AddRange(cards);
        Shuffle();
    }
    public List<T> Draw(int numCards)
    {
        int drawCount = Math.Min(numCards, _cards.Count);
        if (drawCount == 0 || _cards.Count == 0) return [];

        List<T> drawnCards = _cards.GetRange(0, drawCount);
        _cards.RemoveRange(0, drawCount);

        return drawnCards;
    }
    public T? Draw()
    {
        if (_cards.Count == 0) return null;

        T drawnCard = _cards[0];
        _cards.Remove(drawnCard);

        return drawnCard;
    }

}
