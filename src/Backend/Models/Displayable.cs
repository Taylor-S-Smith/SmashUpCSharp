namespace SmashUp.Backend.Models;

internal abstract class Displayable(string[] graphic) : Identifiable
{
    public string[] Graphic { get; } = graphic;
}