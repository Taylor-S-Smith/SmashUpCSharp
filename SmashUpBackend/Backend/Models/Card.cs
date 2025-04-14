using SmashUp.Backend.Services;

namespace SmashUp.Backend.Models;

internal abstract class Card(Faction faction, string name, string[] graphic) : Identifiable
{
    public Faction Faction { get; set; } = faction;
    public string Name { get; set; } = name;
    public string[] Graphic { get; set; } = graphic;
}
