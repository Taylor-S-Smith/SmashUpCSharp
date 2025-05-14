namespace SmashUp.Backend.Models;

internal class Faction(string name, string[] graphic) : Displayable(graphic)
{
    public string Name { get; } = name;
}
