namespace SmashUp.Backend.Models;

internal class BaseCard(Faction faction, string name, string[] graphic, int breakpoint, (int, int, int) pointSpread) : Card(faction, name, graphic)
{
    public int Breakpoint { get; set; } = breakpoint;

    public (int, int, int) PointSpread { get; } = pointSpread;
}