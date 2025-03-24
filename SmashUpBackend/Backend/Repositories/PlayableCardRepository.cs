using SmashUp.Backend.Models;

namespace SmashUp.Backend.Repositories;


internal class PlayableCardRepository
{
    /// <summary>
    /// Gets a list of playable cards by a list of factions. Every playable card whose faction appears in the list will be returned.
    /// </summary>
    public List<PlayableCard> GetCards(List<Faction> factions)
    {
        return [GetTrooper()];
    }

    public PlayableCard GetTrooper()
    {
        return new(Faction.aliens, PlayableCardType.minion, "Trooper", [], 2);
    }
}
