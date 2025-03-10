namespace SmashUpBackend.Models.Cards;

internal class BaseCard(int id, Faction faction, string name, string[] graphic, int breakpoint, List<CardTrigger> cardTriggers)
{
    int Id { get; } = id;
    public Faction Faction { get; set; } = faction;
    public string Name { get; set; } = name;
    public string[] Graphic { get; set; } = graphic;
    public int Breakpoint { get; set; } = breakpoint;
    public List<CardTrigger> CardTriggers { get; set; } = cardTriggers;
}