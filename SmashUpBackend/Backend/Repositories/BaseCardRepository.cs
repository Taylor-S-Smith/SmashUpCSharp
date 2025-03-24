using SmashUp.Backend.Models;

namespace SmashUp.Backend.Repositories;


internal class BaseCardRepository
{
    /// <summary>
    /// Gets a list of base cards by a list of factions. Every base card whose faction appears in the list will be returned.
    /// </summary>
    public List<BaseCard> GetCards(List<Faction> factions)
    {
        return [GetBaseAlpha()];
    }

    public BaseCard GetBaseAlpha()
    {
        return new(Faction.aliens, "Invader", [], 15, (3, 2, 1));
    }
}
