using SmashUp.Backend.Models;
using SmashUp.Backend.GameObjects;
using static SmashUp.Backend.GameObjects.Battle;
using System.Reflection;
using System.Data.Entity.Core.Mapping;

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
            if (baseSlot == null) throw new Exception("No base passed in for War Raptor");
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
    public static Func<PlayableCard> ArmoredStego = () =>
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
            if (baseSlot == null) throw new Exception("No base passed in for Laseratops");

            SelectFieldCardQuery query = new()
            {
                CardType = PlayableCardType.minion,
                MaxPower = 2,
                BaseCard = baseSlot.BaseCard
            };
            PlayableCard? cardToDestroy = battle.SelectFieldCard(laseratops, "Select a card for lasertops to destroy", query)?.SelectedCard;
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
    public static Func<PlayableCard> Augmentation = () =>
    {
        PlayableCard augmentation = new
        (
            Faction.dinosuars,
            PlayableCardType.action,
            "Augmentation",
            [
                @"         ________/\      ",
                @"      _ / |_O_|   0|     ",
                @"     /_|       ____|     ",
                @"     /_|      _____|     ",
                @"     /_|     |           ",
                @"   One minion gains +4   ",
                @" power until the end of  ",
                @"       your turn.        ",
            ]
        );

        augmentation.OnPlay += (battle, baseSlot) =>
        {
            SelectFieldCardQuery query = new()
            {
                CardType = PlayableCardType.minion
            };
            PlayableCard? cardToGainPower = battle.SelectFieldCard(augmentation, "Select a card to gain +4 power until the end of the turn", query)?.SelectedCard;
            if (cardToGainPower != null)
            {
                cardToGainPower.ChangeCurrentPower(4);

                void endTurnHandler(ActivePlayer activePlayer)
                {
                    cardToGainPower.ChangeCurrentPower(-4);
                    battle.EventManager.EndOfTurn -= endTurnHandler;
                }

                battle.EventManager.EndOfTurn += endTurnHandler;
            }
        };

        return augmentation;
    };
    public static Func<PlayableCard> Howl = () =>
    {
        PlayableCard howl = new
        (
            Faction.dinosuars,
            PlayableCardType.action,
            "Howl",
            [
                @"      ____        \      ",
                @"     /    \     \  \     ",
                @"    |   ===O  )  |  |    ",
                @"     \____/     /  /     ",
                @"       ||         /      ",
                @"  Each of your minions   ",
                @"gains +1 power until the ",
                @"    end of your turn     ",
            ]
        );

        howl.OnPlay += (battle, baseSlot) =>
        {
            List<PlayableCard> cardsToChange = battle.GetValidFieldCards((card) => card.Owner == howl.Owner);

            if (cardsToChange.Count > 0)
            {
                foreach (var card in cardsToChange)
                {
                    card.ChangeCurrentPower(1);
                }

                void endTurnHandler(ActivePlayer activePlayer)
                {
                    foreach (var card in cardsToChange)
                    {
                        card.ChangeCurrentPower(-1);
                    }

                    battle.EventManager.EndOfTurn -= endTurnHandler;
                }

                battle.EventManager.EndOfTurn += endTurnHandler;
            }
        };

        return howl;
    };
    public static Func<PlayableCard> NaturalSelection = () =>
    {
        PlayableCard naturalSelection = new
        (
            Faction.dinosuars,
            PlayableCardType.action,
            "Natural Selection",
            [
                @"        O                ",
                @"      --|--    o         ",
                @"        |     /|\        ",
                @"       / \    / \        ",
                @"   Choose one of your    ",
                @"   minions on a base.    ",
                @" Destroy a minion there  ",
                @"with less power than it. ",
            ],
            4
        );

        naturalSelection.OnPlay += (battle, baseSlot) =>
        {
            SelectFieldCardQuery query1 = new()
            {
                CardType = PlayableCardType.minion,
                Owner = naturalSelection.Owner
            };
            var result = battle.SelectFieldCard(naturalSelection, "Choose one of your minions on a base", query1);
            PlayableCard? ownMinion = result?.SelectedCard;
            BaseCard? baseCard = result?.SelectedCardBase;

            if (ownMinion != null)
            {
                SelectFieldCardQuery query2 = new()
                {
                    CardType = PlayableCardType.minion,
                    MaxPower = ownMinion.CurrentPower - 1,
                    BaseCard = baseCard
                };
                PlayableCard? minionToDestroy = battle.SelectFieldCard(naturalSelection, $"Choose a minion with power less than {ownMinion?.CurrentPower} to destroy", query2)?.SelectedCard;
                if (minionToDestroy != null) battle.Destroy(minionToDestroy);
            }
            
        };

        return naturalSelection;
    };
    public static Func<PlayableCard> Rampage = () =>
    {
        PlayableCard rampage = new
        (
            Faction.dinosuars,
            PlayableCardType.action,
            "Rampage",
            [
                @"    __      O    |  |    ",
                @"   | {__   /|\    } |    ",
                @"   |    |  / \   |  |    ",
                @"Reduce the breakpoint of ",
                @" a base by the power of  ",
                @" one of your minions on  ",
                @" that base until the end ",
                @"      of the turn.       ",
            ]
        );

        rampage.OnPlay += (battle, baseSlot) =>
        {
            SelectFieldCardQuery query1 = new()
            {
                CardType = PlayableCardType.minion,
                Owner = rampage.Owner
            };
            var result = battle.SelectFieldCard(rampage, "Choose one of your minions on a base", query1);
            PlayableCard? ownMinion = result?.SelectedCard;
            BaseCard? baseCard = result?.SelectedCardBase;

            if (ownMinion != null && baseCard != null)
            {
                int powerToReduce = ownMinion.CurrentPower ?? 0;
                baseCard.CurrentBreakpoint -= powerToReduce;

                void endTurnHandler(ActivePlayer activePlayer)
                {
                    baseCard.CurrentBreakpoint += powerToReduce;
                    battle.EventManager.EndOfTurn -= endTurnHandler;
                }

                battle.EventManager.EndOfTurn += endTurnHandler;
            }
        };

        return rampage;
    };
    public static Func<PlayableCard> SurvivalOfTheFittest = () =>
    {
        PlayableCard survivalOfTheFittest = new
        (
            Faction.dinosuars,
            PlayableCardType.action,
            "Survival Of The Fittest",
            [
                @"A    Survival Of The    A",
                @"         Fittest         ",
                @"         |_____|         ",
                @"        |/     \|        ",
                @"         \_____/         ",
                @"           >->o          ",
                @"Destroy the lowest-power ",
                @"minion on each base with ",
                @" a higher-power minion.  ",
            ]
        );

        survivalOfTheFittest.OnPlay += (battle, baseSlot) =>
        {
            List<BaseSlot> baseSlots = battle.GetBaseSlots();

            foreach(var slot in baseSlots)
            {
                var allCards = slot.Territories.SelectMany(x => x.Cards);
                int? lowestPower = allCards.Min(card => card.CurrentPower);
                var lowestPowerCards = allCards.Where(card => card.CurrentPower == lowestPower).ToList();
                if(lowestPowerCards.Count > 0 && allCards.Any(card => card.CurrentPower > lowestPower))
                {
                    if (lowestPowerCards.Count == 1) battle.Destroy(lowestPowerCards.Single());
                    else if (lowestPowerCards.Count > 1)
                    {
                        PlayableCard cardToDestroy = battle.SelectCard(lowestPowerCards, "These minions are tied. Select one to destroy:");
                        battle.Destroy(cardToDestroy);
                    }
                }
            }

            
        };

        return survivalOfTheFittest;
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
            new Random().Next(0, 10)
        );


        return minion;
    };

    public static List<PlayableCard> GetCardsByFaction(Faction faction)
    {
        return CardsByFactionDict[faction].Select(x => x()).ToList();
    } 

    private static readonly Dictionary<Faction, List<Func<PlayableCard>>> CardsByFactionDict = new()
    {
        //{ Faction.dinosuars, [WarRaptor, WarRaptor, WarRaptor, WarRaptor, ArmoredStego, ArmoredStego, ArmoredStego, Laseratops, Laseratops, KingRex, Augmentation, Augmentation, Howl, Howl] }
        { Faction.dinosuars, [Minion, Minion, Minion, Minion, Minion, SurvivalOfTheFittest, SurvivalOfTheFittest, SurvivalOfTheFittest, SurvivalOfTheFittest, SurvivalOfTheFittest, SurvivalOfTheFittest] }
    };
}
