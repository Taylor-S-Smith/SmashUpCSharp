namespace SmashUp.Backend.Models;

public abstract class Identifiable
{
    public Guid Id { get; } = new Guid();
}
