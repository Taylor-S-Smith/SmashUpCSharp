namespace SmashUp.Backend.Models;

internal abstract class Card(Faction faction, string name, string[] graphic) : Displayable(graphic)
{
    public Faction Faction { get; set; } = faction;
    public string Name { get; set; } = name;
    public List<PlayableCard> Attachments { get; private set; } = [];
}