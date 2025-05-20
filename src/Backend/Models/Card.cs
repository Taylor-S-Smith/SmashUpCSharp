namespace SmashUp.Backend.Models;

internal enum CardType
{
    Minion,
    Action,
    Base
}

internal abstract class Card(Faction faction, CardType cardType, string name, string[] graphic) : Displayable(graphic)
{
    public Faction Faction { get; set; } = faction;
    public string Name { get; set; } = name;
    public CardType CardType { get; } = cardType;
    public List<PlayableCard> Attachments { get; private set; } = [];
}