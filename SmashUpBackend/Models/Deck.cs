using System.Collections.Generic;

namespace Backend.Models;

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
    public void Add(List<T> cards)
    {
        _cards.AddRange(cards);
    }
    public void Add(T card)
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
    public List<T> Draw(int numCards = 1)
    {
        int drawCount = Math.Min(numCards, _cards.Count);
        if (drawCount == 0) return [];

        List<T> drawnCards = _cards.GetRange(_cards.Count - drawCount, drawCount);
        _cards.RemoveRange(_cards.Count - drawCount, drawCount);

        return drawnCards;
    }

}
