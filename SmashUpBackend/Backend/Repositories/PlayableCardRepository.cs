using SmashUp.Backend.Models;

namespace SmashUp.Backend.Repositories;


internal class PlayableCardRepository
{
    /// <summary>
    /// Gets a list of playable cards by a list of factions. Every playable card whose faction appears in the list will be returned.
    /// </summary>
    public static List<PlayableCard> GetCards(List<Faction> factions)
    {
        List<PlayableCard> cardInstances = [];
        foreach (var faction in factions)
        {
            cardInstances.AddRange(Database.CardsByFaction(faction).ToList());
        }
        return cardInstances;
    }
}
