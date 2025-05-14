using SmashUp.Backend.Models;

namespace SmashUp.Backend.Repositories;


internal class Repository
{
    /// <summary>
    /// Gets a list of playable cards by a list of factions. Every playable card whose faction appears in the list will be returned.
    /// </summary>
    public static List<PlayableCard> GetPlayableCards(List<Faction> factions)
    {
        List<PlayableCard> cards = [];
        foreach (var faction in factions)
        {
            cards.AddRange(Database.PlayableCardsByFactionDict[faction].Select(x => x()).ToList());
        }
        return cards;
    }

    /// <summary>
    /// Gets a list of base cards by a list of factions. Every base card whose faction appears in the list will be returned.
    /// </summary>
    public static List<BaseCard> GetBaseCards(List<Faction> factions)
    {
        List<BaseCard> cards = [];
        foreach (var faction in factions)
        {
            cards.AddRange(Database.BaseCardsByFactionDict[faction].Select(x => x()).ToList());
        }
        return cards;
    }

    public static List<Faction> GetFactions()
    {
        return Database.Factions;
    }
}
