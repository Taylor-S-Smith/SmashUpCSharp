using static SmashUp.Backend.Services.GlobalEventManager;
using SmashUp.Backend.Models;

namespace SmashUp.Backend.Repositories;

internal static class Database
{
    public static Func<PlayableCard> WarRaptor = () =>
    {
        string WAR_RAPTOR_NAME = "War Raptor";

        PlayableCard warRaptor = new
        (
            Faction.dinosuars,
            PlayableCardType.minion,
            WAR_RAPTOR_NAME,
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

        Action<PlayableCard> addCardHandler = (card) =>
        {
            // It already gains power for itself OnPlay, we don't want to double count it
            if (card.Name == WAR_RAPTOR_NAME && card.Id != warRaptor.Id)
                warRaptor.ChangeCurrentPower(1);
        };

        Action<PlayableCard> removeCardHandler = (card) =>
        {
            if (card.Name == WAR_RAPTOR_NAME)
                warRaptor.ChangeCurrentPower(-1);
        };

        warRaptor.OnPlay += (baseSlot) =>
        {
            // Gain Power for current War Raptors
            int currRaptorCount = baseSlot.Territories.SelectMany(x => x.Cards).Where(x => x.Name == WAR_RAPTOR_NAME).ToList().Count;
            warRaptor.ChangeCurrentPower(currRaptorCount);
        };

        warRaptor.OnAddToBase += (baseCard) =>
        {
            // Set Up Listeners for future War Raptor Changes
            baseCard.OnAddCard += addCardHandler;
            baseCard.OnRemoveCard += removeCardHandler;
        };

        warRaptor.OnRemoveFromBase += (baseCard) =>
        {
            // Remove Listeners
            baseCard.OnAddCard -= addCardHandler;
            baseCard.OnRemoveCard -= removeCardHandler;
        };

        return warRaptor;
    };
    public static Func<PlayableCard> KingRex = () =>
    {
        return new
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
    };

    public static List<PlayableCard> CardsByFaction(Faction faction)
    {
        return CardsByFactionDict[faction].Select(x => x()).ToList();
    } 

    private static readonly Dictionary<Faction, List<Func<PlayableCard>>> CardsByFactionDict = new()
    {
        { Faction.dinosuars, [WarRaptor, WarRaptor, WarRaptor, WarRaptor, KingRex] }
    };
}
