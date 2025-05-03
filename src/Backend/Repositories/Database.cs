using SmashUp.Backend.Models;
using SmashUp.Backend.GameObjects;
using static SmashUp.Backend.GameObjects.Battle;
using static SmashUp.Backend.Models.PlayableCard;
using System.Diagnostics;
using System.Numerics;
using SmashUp.Frontend.Utilities;

namespace SmashUp.Backend.Repositories;

internal static class Database
{
    // DINOSAURS

    public static Func<PlayableCard> WarRaptor = () =>
    {
        string WAR_RAPTOR_NAME = "War Raptor";

        PlayableCard warRaptor = new
        (
            Faction.Dinosuars,
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
            Faction.Dinosuars,
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

        armoredStego.OnDiscard += (eventManager) =>
        {
            eventManager.StartOfTurn -= turnStartHandler;
            eventManager.EndOfTurn -= endTurnHandler;
        };

        armoredStego.OnChangeController += (battle, oldController, newController) =>
        {
            var activePlayer = battle.GetActivePlayer();
            if (activePlayer == oldController && activePlayer != newController)
            {
                armoredStego.ExpirePowerChange(-2);
            }
            else if (activePlayer != oldController && activePlayer == newController)
            {
                armoredStego.ApplyPowerChange(battle, armoredStego, 2);
            }
        };


        return armoredStego;
    };
    public static Func<PlayableCard> Laseratops = () =>
    {
        PlayableCard laseratops = new
        (
            Faction.Dinosuars,
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
            Faction.Dinosuars,
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
            Faction.Dinosuars,
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
                        if(activePlayer.Player == augmentation.Controller)
                        {
                            cardToGainPower.ExpirePowerChange(-4);
                            battle.EventManager.EndOfTurn -= endTurnHandler;
                        }
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
            Faction.Dinosuars,
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
                        if (activePlayer.Player == howl.Controller)
                        {
                            card.ExpirePowerChange(-1);
                            battle.EventManager.EndOfTurn -= endTurnHandler;
                        }
                            
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
            Faction.Dinosuars,
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
            Faction.Dinosuars,
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
            Faction.Dinosuars,
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
            Faction.Dinosuars,
            PlayableCardType.Action,
            "Tooth And Claw...And Guns",
            [
                @"A  Tooth Claw and Guns  A",
                @"    ----------------     ",
                @"    Play on a minion     ",
                @"   Ongoing: If another   ",
                @" player's ability would  ",
                @"   affect this minion,   ",
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
            protectionsGranted.ForEach(protection => protection.FromPlayers = battle.GetPlayers().Where(player => player != toothAndClawAndGuns.Controller).ToList());
            minionAttachedTo.Protections.AddRange(protectionsGranted);
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
            Faction.Dinosuars,
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
            Faction.Dinosuars,
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
        wildlifePreserve.OnChangeController += (battle, oldController, newController) =>
        {
            List<Player> otherPlayers = battle.GetPlayers().Where(x => x != newController).ToList();
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
            List<PlayableCard> minionsOnBase = battle.GetBaseSlots()
                                                      .Single(slot => slot.BaseCard == baseAttachedTo)
                                                      .Cards
                                                      .Where(x => x.CardType == PlayableCardType.Minion)
                                                      .ToList();

            minionsOnBase.ForEach(x => AddProtectionsToCard(battle, x));


            baseAttachedTo.OnAddCard += AddProtectionsToCard;
            baseAttachedTo.OnRemoveCard += RemoveProtectionsFromCard;

            // Remove any actions from other players on your minions
            foreach (var minion in minionsOnBase)
            {
                if(minion.Controller == wildlifePreserve.Controller)
                {
                    foreach(var action in minion.Attachments)
                    {
                        if (action.Controller != wildlifePreserve.Controller)
                        {
                            battle.Discard(action);
                        }
                    }
                }
            }            
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

    public static Func<BaseCard> JungleOasis = () =>
    {
        return new
        (
            Faction.Dinosuars,
            "Jungle Oasis",
            [
                "      2      0      0       ",
                "                            ",
                "                            ",
                "                            ",
                "                            ",
                "                            ",
            ],
            12,
            [2, 0, 0]
        );


    };
    public static Func<BaseCard> TarPits = () =>
    {
        BaseCard tarPits = new
        (
            Faction.Dinosuars,
            "Tar Pits",
            [
                "      4      3      1       ",
                "                            ",
                "After each time a minion is ",
                "destroyed here, place it at ",
                "     the bottom of its      ",
                "        owners deck.        ",


            ],
            16,
            [4, 3, 1]
        );


        tarPits.AfterMinionDestroyed += (minion) =>
        {
            minion.Owner.DiscardPile.Remove(minion);
            minion.Owner.Deck.AddToBottom(minion);
        };

        return tarPits;
    };


    // WIZARDS

    public static Func<PlayableCard> Neophyte = () =>
    {
        PlayableCard neophyte = new
        (
            Faction.Wizards,
            PlayableCardType.Minion,
            "Neophyte",
            [
                @"          o              ",
                @"          |- [_]         ",
                @"         / \ | |         ",
                @" Reveal the top card of  ",
                @"your deck. If action, you",
                @" may put it in your hand ",
                @" or play it as an action.",
                @"  Otherwise, return it.  ",
            ],
            PlayLocation.Base,
            2
        );

        neophyte.OnPlay += (battle, baseSlot) =>
        {
            var cardToReveal = neophyte.Controller.Draw();

            Option playIt = new("Play It as Extra Action");
            Option drawIt = new("Draw It");
            Option returnIt = new("Return It");

            List<Option> options = [];
            if (cardToReveal.CardType == PlayableCardType.Action)
            {
                options.Add(playIt);
                options.Add(drawIt);
            }
            else
            {
                options.Add(returnIt);
            }
            Guid chosenOption = battle.SelectOption(options, [cardToReveal], $"Neophyte revealed {cardToReveal.Name}");

            if (playIt.Id == chosenOption)
            {
                battle.PlayExtraCard(cardToReveal);
            }
            else if (drawIt.Id == chosenOption)
            {
                neophyte.Controller.Hand.Add(cardToReveal);
            }
            else if (returnIt.Id == chosenOption)
            {
                neophyte.Controller.Deck.AddToTop(cardToReveal);
            }
        };

        return neophyte;
    };
    public static Func<PlayableCard> Enchantress = () =>
    {
        PlayableCard enchantress = new
        (
            Faction.Wizards,
            PlayableCardType.Minion,
            "Enchantress",
            [
                @"         _*_*_*_         ",
                @"       */* * * *\*       ",
                @"      *|*   O | *|*      ",
                @"      *|*  -|-| *|*      ",
                @"      *|*  / \  *|*      ",
                @"                         ",
                @"       Draw a card.      ",
                @"                         ",
            ],
            PlayLocation.Base,
            2
        );

        enchantress.OnPlay += (battle, baseslot) =>
        {
            var drawnCard = enchantress.Controller.Draw();
            enchantress.Controller.Hand.Add(drawnCard);
        };

        return enchantress;
    };
    public static Func<PlayableCard> Chronomage = () =>
    {
        PlayableCard chronomage = new
        (
            Faction.Wizards,
            PlayableCardType.Minion,
            "Chronomage",
            [
                @"              _______    ",
                @"             |*******|   ",
                @"              \_***_/    ",
                @"    O__ __     _|*|_     ",
                @"   /|         /  *  \    ",
                @"   / \       |_______|   ",
                @"  You may play an extra  ",
                @"    action this turn.    ",
            ],
            PlayLocation.Base,
            3
        );

        chronomage.OnPlay += (battle, baseslot) =>
        {
            var activePlayer = battle.GetActivePlayer();

            if (chronomage.Controller == activePlayer) activePlayer.ActionPlays += 1;
        };

        return chronomage;
    };
    public static Func<PlayableCard> Archmage = () =>
    {
        PlayableCard archmage = new
        (
            Faction.Dinosuars,
            PlayableCardType.Minion,
            "Archmage",
            [
                @"         /*_*_* \        ",
                @"        ||/ O |*|        ",
                @"         |__|_/*/        ",
                @"         |  |            ",
                @"         | / \           ",
                @"Ongoing: You may play an ",
                @" extra action on each of ",
                @"       your turns.       ",
            ],
            PlayLocation.Base,
            4
        );

        void turnStartHandler(Battle battle, ActivePlayer activePlayer)
        {
            if (activePlayer.Player == archmage.Controller)
            {
                activePlayer.Player.ActionPlays += 1;
            }
        }

        archmage.OnPlay += (battle, baseSlot) =>
        {
            var activePlayer = battle.GetActivePlayer();
            if(activePlayer == archmage.Controller) activePlayer.ActionPlays += 1;
            battle.EventManager.StartOfTurn += turnStartHandler;
        };

        archmage.OnDiscard += (eventManager) =>
        {
            eventManager.StartOfTurn -= turnStartHandler;
        };

        archmage.OnChangeController += (battle, oldController, newController) =>
        {
            var activePlayer = battle.GetActivePlayer();
            if (activePlayer == oldController && activePlayer != newController)
            {
                if(activePlayer.ActionPlays > 0)
                {
                    activePlayer.ActionPlays -= 1;
                }
            }
            else if (activePlayer != oldController && activePlayer == newController)
            {
                activePlayer.ActionPlays += 1;
            }
        };


        return archmage;
    };


    // GENERAL
    public static Func<PlayableCard> Minion = () =>
    {

        PlayableCard minion = new
        (
            Faction.Dinosuars,
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


    public static readonly Dictionary<Faction, List<Func<PlayableCard>>> PlayableCardsByFactionDict = new()
    {
        { Faction.Dinosuars, [WarRaptor, WarRaptor, WarRaptor, WarRaptor, ArmoredStego, ArmoredStego, ArmoredStego, Laseratops, Laseratops, KingRex, Augmentation, Augmentation, Howl, Howl, NaturalSelection, Rampage, SurvivalOfTheFittest, ToothAndClawAndGuns, Upgrade, WildlifePreserve] },
        { Faction.Wizards, [Neophyte, Neophyte, Neophyte, Neophyte, Enchantress, Enchantress, Chronomage, Chronomage, Archmage] }
        //{ Faction.Wizards, [Archmage, Augmentation, Augmentation, Augmentation, Augmentation] }
    };

    public static readonly Dictionary<Faction, List<Func<BaseCard>>> BaseCardsByFactionDict = new()
    {
        { Faction.Dinosuars, [JungleOasis, TarPits] },
        { Faction.Wizards, [JungleOasis, TarPits] }
    };
}
