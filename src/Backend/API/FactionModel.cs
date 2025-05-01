using SmashUp.Backend.Models;

namespace SmashUp.Backend.API;

internal class FactionModel(int factionId, string name)
{
    public int FactionId { get; set; } = factionId;
    public string Name { get; set; } = name;

    internal Faction Faction => (Faction)FactionId;
}