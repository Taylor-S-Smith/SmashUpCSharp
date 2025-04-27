using SmashUp.Backend.Models;
using SmashUp.Backend.GameObjects;

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

        warRaptor.OnPlay += (eventManager, baseSlot) =>
        {
            // Gain Power for current War Raptors on base
            int currRaptorCount = baseSlot.Territories.SelectMany(x => x.Cards).Where(x => x.Name == WAR_RAPTOR_NAME).ToList().Count;
            warRaptor.ChangeCurrentPower(currRaptorCount);
        };

        void addCardHandler(PlayableCard card)
        {
            // It already gains power for itself OnPlay, we don't want to double count it
            if (card.Name == WAR_RAPTOR_NAME && card.Id != warRaptor.Id)
                warRaptor.ChangeCurrentPower(1);
        }

        void removeCardHandler(PlayableCard card)
        {
            if (card.Name == WAR_RAPTOR_NAME)
                warRaptor.ChangeCurrentPower(-1);
        }

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
    public static Func<PlayableCard> ArmorStego = () =>
    {
        PlayableCard armoredStego = new
        (
            Faction.dinosuars,
            PlayableCardType.minion,
            "Armored Stego",
            [
                @"                  __     ",
                @"        _/\/\/\/\/ _)    ",
                @"      _|          /      ",
                @"    _|  (  | (   /       ",
                @"   /__.-'|_|--|_|        ",
                @"  Ongoing: Has +2 power  ",
                @"      during other       ",
                @"     players' turns.     ",
            ],
            3

        );

        void turnStartHandler(ActivePlayer activePlayer)
        {
            if (activePlayer.Player != armoredStego.Owner)
            {
                armoredStego.ChangeCurrentPower(2);
            }
        }

        void endTurnHandler(ActivePlayer activePlayer)
        {
            if (activePlayer.Player != armoredStego.Owner)
            {
                armoredStego.ChangeCurrentPower(-2);
            }
        }

        armoredStego.OnPlay += (battle, baseSlot) =>
        {
            battle.EventManager.StartOfTurn += turnStartHandler;
            battle.EventManager.EndOfTurn += endTurnHandler;
        };

        armoredStego.OnRemoveFromBattlefield += (eventManager) =>
        {
            eventManager.StartOfTurn -= turnStartHandler;
            eventManager.EndOfTurn -= endTurnHandler;
        };


        return armoredStego;
    };
    public static Func<PlayableCard> Laseratops = () =>
    {
        PlayableCard laseratops = new
        (
            Faction.dinosuars,
            PlayableCardType.minion,
            "Laseratops",
            [
                @"     ====<[]             ",
                @"     /| __||___          ",
                @"  \\| |/       \         ",
                @"  (___   ) |  )  \_      ",
                @"      |_|--|_|'-.__\     ",
                @" ----------------------  ",
                @"Destroy a minion of power",
                @" 2 or less on this base. ",
            ],
            4
        );

        laseratops.OnPlay += (battle, baseSlot) =>
        {
            PlayableCard? cardToDestroy = battle.SelectFieldCard(PlayableCardType.minion, 2, laseratops, "Select a card for lasertops to destroy", baseSlot.BaseCard);
            if (cardToDestroy != null) battle.Destroy(cardToDestroy);
        };

        return laseratops;
    };
    public static Func<PlayableCard> KingRex = () =>
    {
        return new
        (
            Faction.dinosuars,
            PlayableCardType.minion,
            "King Rex",
            [
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

    public static Func<PlayableCard> Minion = () =>
    {

        PlayableCard minion = new
        (
            Faction.dinosuars,
            PlayableCardType.minion,
            "minion",
            [
                @"     _oVo--.__           ",
                @"    '^^`)._  `\'_'_'     ",
                @"     """"' //(( ,_.-'      ",
                @"           / /           ",
                @"         `~`~            ",
                @"                         ",
                @"                         ",
                @"                         ",
            ],
            2
        );


        return minion;
    };

    public static List<PlayableCard> GetCardsByFaction(Faction faction)
    {
        return CardsByFactionDict[faction].Select(x => x()).ToList();
    } 

    private static readonly Dictionary<Faction, List<Func<PlayableCard>>> CardsByFactionDict = new()
    {
        //{ Faction.dinosuars, [WarRaptor, WarRaptor, WarRaptor, WarRaptor, ArmorStego, ArmorStego, ArmorStego, Laseratops, Laseratops, KingRex] }
        { Faction.dinosuars, [Minion, Minion, Laseratops, Laseratops] }
    };
}
