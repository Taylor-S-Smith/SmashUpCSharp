using Microsoft.VisualBasic;

namespace SmashUp.Backend.Models;

internal class BaseCard(Faction faction, string name, string[] graphic, int breakpoint, (int, int, int) pointSpread) : Card(faction, name, graphic)
{
    public int PrintedBreakpoint { get; set; } = breakpoint;
    public int CurrentBreakpoint { get; set; } = breakpoint;

    public (int, int, int) PointSpread { get; } = pointSpread;

    public int CurrentPower { get; set; } = 0;

    public override BaseCard Clone()
    {
        return new BaseCard(Faction, Name, Graphic, PrintedBreakpoint, PointSpread);
    }
}