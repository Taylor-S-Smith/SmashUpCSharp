namespace SmashUpBackend.Models.Cards;

internal enum PlayableCardType
{
    minion,
    action
}


internal class PlayableCard(int id, Faction faction, PlayableCardType cardType, string name, string[] graphic, int power, List<CardTrigger> cardTriggers)
{
    int Id { get; } = id;
    public Faction Faction { get; } = faction;
    public PlayableCardType CardType { get; } = cardType;
    public string Name { get; } = name;
    public string[] Graphic { get; } = graphic;
    public int? Power { get; set; } = power;
    public List<CardTrigger> CardTriggers { get; } = cardTriggers;
}
