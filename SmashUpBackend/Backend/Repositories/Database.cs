using SmashUp.Backend.Models;
using SmashUp.Backend.GameObjects;
using static SmashUp.Backend.GameObjects.Battle;
using System.Reflection;
using System.Data.Entity.Core.Mapping;
using static SmashUp.Backend.Models.PlayableCard;

namespace SmashUp.Backend.Repositories;

internal static class Database
{
    public static Func<PlayableCard> WarRaptor = () =>
    {
        string WAR_RAPTOR_NAME = "War Raptor";

        PlayableCard warRaptor = new
        (
            Faction.dinosuars,
            PlayableCardType.Minion,
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
            PlayLocation.Base,
            2
        );

        warRaptor.OnPlay += (battle, baseSlot) =>
        {
            if (baseSlot == null) throw new Exception("No base passed in for War Raptor");
            // Gain Power for current War Raptors on base
            int currRaptorCount = baseSlot.Territories.SelectMany(x => x.Cards).Where(x => x.Name == WAR_RAPTOR_NAME).ToList().Count;
            warRaptor.ApplyPowerChange(battle, warRaptor, currRaptorCount);
        };

        void addCardHandler(Battle battle, PlayableCard card)
        {
            // It already gains power for itself OnPlay, we don't want to double count it
            if (card.Name == WAR_RAPTOR_NAME && card.Id != warRaptor.Id)
                warRaptor.ApplyPowerChange(battle, warRaptor, 1);
        }

        void removeCardHandler(Battle battle, PlayableCard card)
        {
            if (card.Name == WAR_RAPTOR_NAME)
                warRaptor.ApplyPowerChange(battle, warRaptor, -1);
        }

        warRaptor.OnAddToBase += (battle, baseCard) =>
        {
            // Set Up Listeners for future War Raptor Changes
            baseCard.OnAddCard += addCardHandler;
            baseCard.OnRemoveCard += removeCardHandler;
        };

        warRaptor.OnRemoveFromBase += (battle, baseCard) =>
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
            PlayableCardType.Minion,
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
            PlayLocation.Base,
            3

        );

        void turnStartHandler(Battle battle, ActivePlayer activePlayer)
        {
            if (activePlayer.Player != armoredStego.Controller)
            {
                armoredStego.ApplyPowerChange(battle, armoredStego, 2);
            }
        }

        void endTurnHandler(ActivePlayer activePlayer)
        {
            if (activePlayer.Player != armoredStego.Controller)
            {
                armoredStego.ExpirePowerChange(-2);
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
            PlayableCardType.Minion,
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
            PlayLocation.Base,
            4
        );

        laseratops.OnPlay += (battle, baseSlot) =>
        {
            if (baseSlot == null) throw new Exception("No base passed in for Laseratops");

            SelectFieldCardQuery query = new()
            {
                CardType = PlayableCardType.Minion,
                MaxPower = 2,
                BaseCard = baseSlot.BaseCard
            };
            PlayableCard? cardToDestroy = battle.SelectFieldCard(laseratops, "Select a card for lasertops to destroy", query)?.SelectedCard;
            if (cardToDestroy != null) battle.Destroy(cardToDestroy, laseratops);
        };

        return laseratops;
    };
    public static Func<PlayableCard> KingRex = () =>
    {
        return new
        (
            Faction.dinosuars,
            PlayableCardType.Minion,
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
            PlayLocation.Base, 
            7
        );
    };
    public static Func<PlayableCard> Augmentation = () =>
    {
        PlayableCard augmentation = new
        (
            Faction.dinosuars,
            PlayableCardType.Action,
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
            ],
            PlayLocation.Discard
        );

        augmentation.OnPlay += (battle, baseSlot) =>
        {
            SelectFieldCardQuery query = new()
            {
                CardType = PlayableCardType.Minion
            };
            PlayableCard? cardToGainPower = battle.SelectFieldCard(augmentation, "Select a card to gain +4 power until the end of the turn", query)?.SelectedCard;
            if (cardToGainPower != null)
            {
                bool addedPower = cardToGainPower.ApplyPowerChange(battle, augmentation, 4);

                if(addedPower)
                {
                    void endTurnHandler(ActivePlayer activePlayer)
                    {
                        cardToGainPower.ExpirePowerChange(-4);
                        battle.EventManager.EndOfTurn -= endTurnHandler;
                    }

                    battle.EventManager.EndOfTurn += endTurnHandler;
                }                
            }
        };

        return augmentation;
    };
    public static Func<PlayableCard> Howl = () =>
    {
        PlayableCard howl = new
        (
            Faction.dinosuars,
            PlayableCardType.Action,
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
            ],
            PlayLocation.Discard
        );

        howl.OnPlay += (battle, baseSlot) =>
        {
            List<PlayableCard> cardsToChange = battle.GetValidFieldCards((card) => card.Controller == howl.Controller);

            if (cardsToChange.Count > 0)
            {
                foreach (var card in cardsToChange)
                {
                    bool appliedPower = card.ApplyPowerChange(battle, howl, 1);

                    void endTurnHandler(ActivePlayer activePlayer)
                    {
                        card.ExpirePowerChange(-1);
                        battle.EventManager.EndOfTurn -= endTurnHandler;
                    }

                    battle.EventManager.EndOfTurn += endTurnHandler;
                }
            }
        };

        return howl;
    };
    public static Func<PlayableCard> NaturalSelection = () =>
    {
        PlayableCard naturalSelection = new
        (
            Faction.dinosuars,
            PlayableCardType.Action,
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
            PlayLocation.Discard
        );

        naturalSelection.OnPlay += (battle, baseSlot) =>
        {
            SelectFieldCardQuery query1 = new()
            {
                CardType = PlayableCardType.Minion,
                Controller = naturalSelection.Controller
            };
            var result = battle.SelectFieldCard(naturalSelection, "Choose one of your minions on a base", query1);
            PlayableCard? ownMinion = result?.SelectedCard;
            BaseCard? baseCard = result?.SelectedCardBase;

            if (ownMinion != null)
            {
                SelectFieldCardQuery query2 = new()
                {
                    CardType = PlayableCardType.Minion,
                    MaxPower = ownMinion.CurrentPower - 1,
                    BaseCard = baseCard
                };
                PlayableCard? minionToDestroy = battle.SelectFieldCard(naturalSelection, $"Choose a minion with power less than {ownMinion?.CurrentPower} to destroy", query2)?.SelectedCard;
                if (minionToDestroy != null) battle.Destroy(minionToDestroy, naturalSelection);
            }
            
        };

        return naturalSelection;
    };
    public static Func<PlayableCard> Rampage = () =>
    {
        PlayableCard rampage = new
        (
            Faction.dinosuars,
            PlayableCardType.Action,
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
            ],
            PlayLocation.Discard
        );

        rampage.OnPlay += (battle, baseSlot) =>
        {
            SelectFieldCardQuery query1 = new()
            {
                CardType = PlayableCardType.Minion,
                Controller = rampage.Controller
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
            PlayableCardType.Action,
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
            ],
            PlayLocation.Discard
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
                    if (lowestPowerCards.Count == 1) battle.Destroy(lowestPowerCards.Single(), survivalOfTheFittest);
                    else if (lowestPowerCards.Count > 1)
                    {
                        PlayableCard cardToDestroy = battle.SelectCard(lowestPowerCards, "These minions are tied. Select one to destroy:");
                        battle.Destroy(cardToDestroy, survivalOfTheFittest);
                    }
                }
            }

            
        };

        return survivalOfTheFittest;
    };
    public static Func<PlayableCard> ToothAndClawAndGuns = () =>
    {
        PlayableCard toothAndClawAndGuns = new
        (
            Faction.dinosuars,
            PlayableCardType.Action,
            "Tooth And Claw...And Guns",
            [
                @"A   Tooth And Claw...   A",
                @"        And Guns         ",
                @"    ----------------     ",
                @"    Play on a minion     ",
                @" Ongoing: if an ability  ",
                @"would affect this minion,",
                @"destroy this card and the",
                @" ability does not affect ",
                @"       this minion       ",
            ],
            PlayLocation.Minion
        );

        List<Protection> protectionsGranted = [];
        foreach (var protectionType in Enum.GetValues(typeof(EffectType)).Cast<EffectType>())
        {
            Protection protection = new(protectionType, toothAndClawAndGuns);
            protectionsGranted.Add(protection);
        }

        toothAndClawAndGuns.OnProtect += (battle) =>
        {
            battle.Destroy(toothAndClawAndGuns, toothAndClawAndGuns);
        };

        toothAndClawAndGuns.OnAttach += (battle, minionAttachedTo) =>
        {
            ((PlayableCard)minionAttachedTo).Protections.AddRange(protectionsGranted);
        };

        toothAndClawAndGuns.OnDetach += (battle, cardDetachedFrom) =>
        {
            protectionsGranted.ForEach(protection => ((PlayableCard)cardDetachedFrom).Protections.Remove(protection));
        };

        return toothAndClawAndGuns;
    };
    public static Func<PlayableCard> Upgrade = () =>
    {
        PlayableCard upgrade = new
        (
            Faction.dinosuars,
            PlayableCardType.Action,
            "Upgrade",
            [
                @"A        Upgrade   __   A",
                @"                  / _)   ",
                @"        _________/ /_    ",
                @"      _(________()/_()   ",
                @"    _/  (  | (   /       ",
                @"   /__.-'|_|--|_|        ",
                @"    Play on a minion     ",
                @"Ongoing: This minion has ",
                @"        +2 power         ",
            ],
            PlayLocation.Minion
        );

        bool appliedPower = false;
        upgrade.OnAttach += (battle, cardAttachedTo) =>
        {
            appliedPower = cardAttachedTo.ApplyPowerChange(battle, upgrade, 2);
        };

        upgrade.OnDetach += (battle, cardDetachedFrom) =>
        {
            if(appliedPower) cardDetachedFrom.ExpirePowerChange(-2);
        };

        return upgrade;
    };
    public static Func<PlayableCard> WildlifePreserve = () =>
    {
        PlayableCard wildlifePreserve = new
        (
            Faction.dinosuars,
            PlayableCardType.Action,
            "Wildlife Preserve",
            [
                @"A   Wildlife Preserve   A",
                @"             /\/\/\      ",
                @"          O /\/\/\/\     ",
                @"      ___/     ||        ",
                @"     '/\/\     ||        ",
                @"     Play on a base      ",
                @"  Ongoing: Your minions  ",
                @"here are not affected by ",
                @" other players' actions  ",
            ],
            PlayLocation.Base
        );

        List<Protection> protectionsGranted = [];
        foreach (var protectionType in Enum.GetValues(typeof(EffectType)).Cast<EffectType>())
        {
            Protection protection = new(protectionType, wildlifePreserve, null, PlayableCardType.Action);
            protectionsGranted.Add(protection);
        }

        // If the controller changes, the protections we give need to update
        wildlifePreserve.OnChangeController += (battle, controller) =>
        {
            List<Player> otherPlayers = battle.GetPlayers().Where(x => x != controller).ToList();
            foreach (var protection in protectionsGranted)
            {
                protection.FromPlayers = otherPlayers;
            }
        };

        void AddProtectionsToCard(Battle battle, PlayableCard card)
        {
            if(card.Controller == wildlifePreserve.Controller) card.Protections.AddRange(protectionsGranted);
        }

        void RemoveProtectionsFromCard(Battle battle, PlayableCard card)
        {
            if (card.Controller == wildlifePreserve.Controller) protectionsGranted.ForEach(x => card.Protections.Remove(x));
        }

        wildlifePreserve.OnAddToBase += (battle,  baseAttachedTo) =>
        {
            // Update protections to protect against active opponents
            List<Player> otherPlayers = battle.GetPlayers().Where(x => x != wildlifePreserve.Controller).ToList();
            foreach (var protection in protectionsGranted)
            {
                protection.FromPlayers = otherPlayers;
            }

            /// Only cards from the base this is on are protected
            List<PlayableCard> protectedCards = battle.GetBaseSlots()
                                                      .Single(slot => slot.BaseCard == baseAttachedTo)
                                                      .Cards
                                                      .ToList();

            protectedCards.ForEach(x => AddProtectionsToCard(battle, x));


            baseAttachedTo.OnAddCard += AddProtectionsToCard;
            baseAttachedTo.OnRemoveCard += RemoveProtectionsFromCard;
        };

        wildlifePreserve.OnRemoveFromBase += (battle, cardDetachedFrom) =>
        {
            List<PlayableCard> protectedCards = battle.GetBaseSlots()
                                                      .Single(slot => slot.BaseCard == cardDetachedFrom)
                                                      .Cards
                                                      .ToList();

            protectedCards.ForEach(x => RemoveProtectionsFromCard(battle, x));

            cardDetachedFrom.OnAddCard -= AddProtectionsToCard;
            cardDetachedFrom.OnRemoveCard -= RemoveProtectionsFromCard;
        };      

        return wildlifePreserve;
    };


    public static Func<PlayableCard> Minion = () =>
    {

        PlayableCard minion = new
        (
            Faction.dinosuars,
            PlayableCardType.Minion,
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
            PlayLocation.Base,
            new Random().Next(0, 2)
        );


        return minion;
    };

    public static List<PlayableCard> GetCardsByFaction(Faction faction)
    {
        return CardsByFactionDict[faction].Select(x => x()).ToList();
    } 

    private static readonly Dictionary<Faction, List<Func<PlayableCard>>> CardsByFactionDict = new()
    {
        //{ Faction.dinosuars, [WarRaptor, WarRaptor, WarRaptor, WarRaptor, ArmoredStego, ArmoredStego, ArmoredStego, Laseratops, Laseratops, KingRex, Augmentation, Augmentation, Howl, Howl, NaturalSelection, Rampage, SurvivalOfTheFittest, ToothAndClawAndGuns, Upgrade, WildlifePreserve] }
        { Faction.dinosuars, [Minion, Minion, Laseratops, Laseratops, SurvivalOfTheFittest, SurvivalOfTheFittest, WildlifePreserve, WildlifePreserve, Upgrade, Upgrade] }
    };
}
