namespace SmashUpBackend.Models;

public abstract class Identifiable
{
    public Guid Id { get; } = new Guid();
}
