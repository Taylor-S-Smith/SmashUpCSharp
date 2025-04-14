using SmashUp.Backend.Models;

namespace SmashUp.Backend.Repositories;

internal static class Database
{

    public static readonly PlayableCard WarRaptor = new
        (
            Faction.dinosuars,
            PlayableCardType.minion,
            "War Raptor",
            [
                @"2      War Raptor       2",
                @"     _oVo--.__           ",
                @"    '^^`)._  `\'_'_'     ",
                @"     """"' //(( ,_.-'      ",
                @"           / /           ",
                @"         `~`~            ",
                @" Ongoing: Gains +1 power ",
                @" for each War Raptor on  ",
                @"this base, including this",
            ],
            2
        );

    private static readonly PlayableCard KingRex = new
        (
            Faction.dinosuars,
            PlayableCardType.minion,
            "King Rex",
            [
                @"7       King Rex        7",
                @"          ____           ",
                @"       .-~    '.         ",
                @"      / /  ~@\   )       ",
                @"     | /  \~\.  `\       ",
                @"    /  |  |< ~\(..)      ",
                @"       \  \<   .,,       ",
                @"       /~\ \< /          ",
                @"       /-~\ \_|          ",
            ],
            7
        );

    public static readonly Dictionary<Faction, List<PlayableCard>> CardsByFaction = new()
    {
        { Faction.dinosuars, [WarRaptor, WarRaptor, WarRaptor, WarRaptor, KingRex] }
    };
}
