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
        //Faction faction, string name, string[] graphic, int breakpoint, (int, int, int) pointSpread, int currentPower
        return new BaseCard(
            Faction.aliens, 
            "Base Alpha", 
            [


                    "      4      2      1       ",
                    "                            ",
                    "After each time a minion is ",
                    " played here, it's owner may",
                    "  play an extra minion of   ",
                    "      power 3 or less.      ",


            ], 
            15, 
            (3, 2, 1)
        );
    }
}
