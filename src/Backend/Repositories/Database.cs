using SmashUp.Backend.Models;
using SmashUp.Backend.GameObjects;
using static SmashUp.Backend.GameObjects.Battle;
using static SmashUp.Backend.Models.PlayableCard;
using LinqKit;
using SmashUp.Backend.API;
using System.Reflection.Metadata.Ecma335;
using System.Drawing.Text;
using SmashUp.Backend.Services;
using System.Drawing.Printing;
using System.Numerics;
using static SmashUp.Backend.Models.ScoreResult;

namespace SmashUp.Backend.Repositories;

internal static class Database
{
    // FACTIONS
    public static Faction Aliens = new
    (
        "Aliens",
        [
            @"    /                \   ",
            @"   ( A  L  I  E  N  S )  ",
            @"    \__            __/   ",
            @"       \__________/      ",
            @"         |     |         ",
            @"         |     |         ",
            @"         | \ / |         ",
            @"         |  |  |         ",
            @"         | /o\ |         ",
        ]
    );
    public static Faction Dinosaurs = new
    (
        "Dinosuars",
        [
            @"  /\/\/\/\/\/\/\/\/\/\   ",
            @" |  D I N O S A U R S |  ",
            @"  \/\/\/\/\/\/\/\/\/\/   ",
            @"                         ",
            @"       ====<[]           ",
            @"       /| __||___        ",
            @"    \\| |/       \       ",
            @"    (___   ) |  )  \_    ",
            @"        |_|--|_|'-.__\   ",
        ]
    );
    public static Faction Pirates = new
    (
        "Pirates",
        [
            @"     ____/_\________     ",
            @"    | P I R A T E S |    ",
            @"     \_______\ ___ /     ",
            @"             \_\         ",
            @"                         ",
            @"          /v\            ",
            @"          ("")           ",
            @"          -|-|===>       ",
            @"          / \            ",
        ]
    );
    public static Faction Ninjas = new
    (
        "Ninjas",
        [
            @"      +-+-+-+-+-+-+      ",
            @"      |N|I|N|J|A|S|      ",
            @"      +-+-+-+-+-+-+      ",
            @"                         ",
            @"        \   \            ",
            @"           _/\_          ",
            @"        \ \    /         ",
            @"          /_  _\         ",
            @"            \/           ",
        ]
    );
    public static Faction Robots = new
    ( 
        "Robots",
        [
            @"|\ /|\ /|\ /|\ /|\ /|\ /|",
            @"| R | O | B | O | T | S |",
            @"|/_\|/_\|/_\|/_\|/_\|/_\|",
            @"           ___           ",
            @"        <=|_=_|=>        ",
            @"            |            ",
            @"        /--[_]--\        ",
            @"       /    |    \       ",
            @"      /     |     \      ",
        ]
    );
    public static Faction Tricksters = new
    (
        "Tricksters",
        [
            @"  /\  /\  /\ /\  /\  /\  ",
            @" / T R I C K S T E R S \ ",
            @"  ||  ||  ||  ||  || ||  ",
            @"                         ",
            @"           / \           ",
            @"          /___\          ",
            @"        \ (o o)-U        ",
            @"         \/|_|\          ",
            @"           | |           ",
        ]
    );
    public static Faction Wizards = new
    ( 
        "Wizards",
        [
            @"     *-*-*-*-*-*-*-*     ",
            @"     |W|I|Z|A|R|D|S|     ",
            @"     *-*-*-*-*-*-*-*     ",
            @"                         ",
            @"         _*_*_*_         ",
            @"       */* * * *\*       ",
            @"      *|*   O | *|*      ",
            @"      *|*  -|-| *|*      ",
            @"      *|*  / \  *|*      ",
        ]
    );
    public static Faction Zombies = new
    (
        "Zombies",
        [
            @"    ___    ___    ___    ",
            @"   |ZOM|  |BIE|  | S |   ",
            @"___|___|__|___|__|___|___",
            @"                         ",
            @"            _            ",
            @"          _/_\_ |        ",
            @"           |_| /|        ",
            @"         [_]| / |        ",
            @"           / \           ",
        ]
    );

    public static List<Faction> Factions = [Aliens, Dinosaurs, Pirates, Ninjas, Robots, Tricksters, Wizards, Zombies];


    // DINOSAURS
    public static PlayableCard WarRaptor()
    {
        string WAR_RAPTOR_NAME = "War Raptor";

        PlayableCard warRaptor = new
        (
            Dinosaurs,
            CardType.Minion,
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

        BaseSlot? currentBaseSlot = null;

        void ChangePowerForEachWarRaptorOnBase(Battle battle, BaseSlot baseSlot, int amount)
        {
            int currRaptorCount = baseSlot.Territories.SelectMany(x => x.Cards).Where(x => x.Name == WAR_RAPTOR_NAME).ToList().Count;
            warRaptor.ApplyPowerChange(battle, warRaptor, currRaptorCount*amount);
        }

        void addCardHandler(Battle battle, PlayableCard card, BaseSlot baseSlot)
        {
            // It already gains power for itself when it enters the base,
            // since global events trigger after card events not checking
            // for itself would double count it
            if (baseSlot == currentBaseSlot && card.Name == WAR_RAPTOR_NAME && card.Id != warRaptor.Id)
                warRaptor.ApplyPowerChange(battle, warRaptor, 1);
        }

        void removeCardHandler(Battle battle, PlayableCard card, BaseSlot baseSlot)
        {
            if (baseSlot == currentBaseSlot && card.Name == WAR_RAPTOR_NAME)
                warRaptor.ApplyPowerChange(battle, warRaptor, -1);
        }

        warRaptor.EnterBattlefield += (battle) =>
        {
            // Listen for any changes in war raptor counts
            battle.EventManager.CardEnteredBase += addCardHandler;
            battle.EventManager.CardExitedBase += removeCardHandler;
        };
        warRaptor.ExitBattlefield += (battle) =>
        {
            // Remove listeners
            battle.EventManager.CardEnteredBase -= addCardHandler;
            battle.EventManager.CardExitedBase -= removeCardHandler;
        };

        warRaptor.EnterBase += (battle, baseSlot) =>
        {
            currentBaseSlot = baseSlot;
            ChangePowerForEachWarRaptorOnBase(battle, baseSlot, 1);
        };

        warRaptor.ExitBase += (battle, baseSlot) =>
        {
            //This line is not technically needed, but I will include it to ensure
            //that it is null when not on the field, which is more intuitive
            currentBaseSlot = null;
            ChangePowerForEachWarRaptorOnBase(battle, baseSlot, -1);
            // Remove one more to account for itself
            warRaptor.ApplyPowerChange(battle, warRaptor, -1);
        };

        return warRaptor;
    }
    public static PlayableCard ArmoredStego()
    {
        PlayableCard armoredStego = new
        (
            Dinosaurs,
            CardType.Minion,
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

        void endTurnHandler(Battle battle, ActivePlayer activePlayer)
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

        armoredStego.ExitBattlefield += (battle) =>
        {
            battle.EventManager.StartOfTurn -= turnStartHandler;
            battle.EventManager.EndOfTurn -= endTurnHandler;
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
    }
    public static PlayableCard Laseratops()
    {
        PlayableCard laseratops = new
        (
            Dinosaurs,
            CardType.Minion,
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

            SelectCardQuery query = new()
            {
                CardType = CardType.Minion,
                MaxPower = 2,
                BaseCard = baseSlot.BaseCard
            };
            PlayableCard? cardToDestroy = battle.SelectFieldCard(laseratops, "Select a card for Lasertops to destroy", query)?.SelectedCard;
            if (cardToDestroy != null) battle.Destroy(cardToDestroy, laseratops);
        };

        return laseratops;
    }
    public static PlayableCard KingRex()
    {
        return new
        (
            Dinosaurs,
            CardType.Minion,
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
    }
    public static PlayableCard Augmentation()
    {
        PlayableCard augmentation = new
        (
            Dinosaurs,
            CardType.Action,
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
            SelectCardQuery query = new()
            {
                CardType = CardType.Minion
            };
            PlayableCard? cardToGainPower = battle.SelectFieldCard(augmentation, "Select a card to gain +4 power until the end of the turn", query)?.SelectedCard;
            if (cardToGainPower != null)
            {
                bool addedPower = cardToGainPower.ApplyPowerChange(battle, augmentation, 4);

                if(addedPower)
                {
                    void endTurnHandler(Battle battle, ActivePlayer activePlayer)
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
    }
    public static PlayableCard Howl()
    {
        PlayableCard howl = new
        (
            Dinosaurs,
            CardType.Action,
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

        howl.OnPlay += (battle, baseSlot) => GiveYourMinionsPowerUntilEndOfTurn(battle, howl);

        return howl;
    }
    public static PlayableCard NaturalSelection()
    {
        PlayableCard naturalSelection = new
        (
            Dinosaurs,
            CardType.Action,
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
            SelectCardQuery query1 = new()
            {
                CardType = CardType.Minion,
                Controllers = [naturalSelection.Controller]
            };
            var result = battle.SelectFieldCard(naturalSelection, "Choose one of your minions on a base", query1);
            PlayableCard? ownMinion = result?.SelectedCard;
            BaseCard? baseCard = result?.SelectedCardBase;

            if (ownMinion != null)
            {
                SelectCardQuery query2 = new()
                {
                    CardType = CardType.Minion,
                    MaxPower = ownMinion.CurrentPower - 1,
                    BaseCard = baseCard
                };
                PlayableCard? minionToDestroy = battle.SelectFieldCard(naturalSelection, $"Choose a minion with power less than {ownMinion?.CurrentPower} to destroy", query2)?.SelectedCard;
                if (minionToDestroy != null) battle.Destroy(minionToDestroy, naturalSelection);
            }
            
        };

        return naturalSelection;
    }
    public static PlayableCard Rampage()
    {
        PlayableCard rampage = new
        (
            Dinosaurs,
            CardType.Action,
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
            SelectCardQuery query1 = new()
            {
                CardType = CardType.Minion,
                Controllers = [rampage.Controller]
            };
            var result = battle.SelectFieldCard(rampage, "Choose one of your minions on a base", query1);
            PlayableCard? ownMinion = result?.SelectedCard;
            BaseCard? baseCard = result?.SelectedCardBase;

            if (ownMinion != null && baseCard != null)
            {
                int powerToReduce = ownMinion.CurrentPower ?? 0;
                baseCard.CurrentBreakpoint -= powerToReduce;

                void endTurnHandler(Battle battle, ActivePlayer activePlayer)
                {
                    baseCard.CurrentBreakpoint += powerToReduce;
                    battle.EventManager.EndOfTurn -= endTurnHandler;
                }

                battle.EventManager.EndOfTurn += endTurnHandler;
            }
        };

        return rampage;
    }
    public static PlayableCard SurvivalOfTheFittest()
    {
        PlayableCard survivalOfTheFittest = new
        (
            Dinosaurs,
            CardType.Action,
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
                        PlayableCard cardToDestroy = battle.Select(lowestPowerCards, "These minions are tied. Select one to destroy:");
                        battle.Destroy(cardToDestroy, survivalOfTheFittest);
                    }
                }
            }

            
        };

        return survivalOfTheFittest;
    }
    public static PlayableCard ToothClawAndGuns()
    {
        PlayableCard toothAndClawAndGuns = new
        (
            Dinosaurs,
            CardType.Action,
            "Tooth Claw And Guns",
            [
                @"A  Tooth Claw And Guns  A",
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
    }
    public static PlayableCard Upgrade()
    {
        PlayableCard upgrade = new
        (
            Dinosaurs,
            CardType.Action,
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
    }
    public static PlayableCard WildlifePreserve()
    {
        PlayableCard wildlifePreserve = new
        (
            Dinosaurs,
            CardType.Action,
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

        BaseSlot? currentBaseSlot = null;

        List<Protection> protectionsGranted = [];
        foreach (var protectionType in Enum.GetValues(typeof(EffectType)).Cast<EffectType>())
        {
            Protection protection = new(protectionType, wildlifePreserve, null, CardType.Action);
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

        void AddProtectionsToCard(Battle battle, PlayableCard card, BaseSlot slot)
        {
            if(card.Controller == wildlifePreserve.Controller && currentBaseSlot == slot) card.Protections.AddRange(protectionsGranted);
        }

        void RemoveProtectionsFromCard(Battle battle, PlayableCard card, BaseSlot slot)
        {
            if (card.Controller == wildlifePreserve.Controller && currentBaseSlot == slot) protectionsGranted.ForEach(x => card.Protections.Remove(x));
        }

        wildlifePreserve.EnterBase += (battle,  baseSlot) =>
        {
            currentBaseSlot = baseSlot;

            // Update protections to protect against active opponents
            List<Player> otherPlayers = battle.GetPlayers().Where(x => x != wildlifePreserve.Controller).ToList();
            foreach (var protection in protectionsGranted)
            {
                protection.FromPlayers = otherPlayers;
            }

            /// Only cards from the base this is on are protected
            List<PlayableCard> minionsOnBase = battle.GetBaseSlots()
                                                      .Single(slot => slot == baseSlot)
                                                      .Cards
                                                      .Where(x => x.CardType == CardType.Minion)
                                                      .ToList();

            minionsOnBase.ForEach(x => AddProtectionsToCard(battle, x, baseSlot));


            battle.EventManager.CardEnteredBase += AddProtectionsToCard;
            battle.EventManager.CardExitedBase += RemoveProtectionsFromCard;

            // Remove any actions from other players on your minions
            foreach (var minion in minionsOnBase)
            {
                if(minion.Controller == wildlifePreserve.Controller)
                {
                    List<PlayableCard> cardsToDiscard = [];
                    foreach(var action in minion.Attachments)
                    {
                        if (action.Controller != wildlifePreserve.Controller) cardsToDiscard.Add(action);
                    }

                    cardsToDiscard.ForEach(x => battle.DiscardFromField(x));
                }
            }            
        };

        wildlifePreserve.ExitBase += (battle, baseSlot) =>
        {
            List<PlayableCard> protectedCards = battle.GetBaseSlots()
                                                      .Single(slot => slot == baseSlot)
                                                      .Cards
                                                      .ToList();

            protectedCards.ForEach(x => RemoveProtectionsFromCard(battle, x, baseSlot));

            battle.EventManager.CardEnteredBase -= AddProtectionsToCard;
            battle.EventManager.CardExitedBase -= RemoveProtectionsFromCard;

            currentBaseSlot = null;
        };      

        return wildlifePreserve;
    }

    public static BaseCard JungleOasis()
    {
        return new
        (
            Dinosaurs,
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


    }
    public static BaseCard TarPits()
    {
        BaseCard tarPits = new
        (
            Dinosaurs,
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
    }

    // PIRATES
    public static PlayableCard FirstMate()
    {
        PlayableCard firstMate = new
        (
            Pirates,
            CardType.Minion,
            "First Mate",
            [
                @"           n_            ",
                @"          ("")            ",
                @"          -|-            ",
                @"          / \            ",
                @"Special: After this base ",
                @" is scored, you may move ",
                @" this minion to another  ",
                @"base instead of discard. ",
            ],
            PlayLocation.Base,
            2
        );

        BaseSlot? currentBaseSlot = null;

        void HandleBaseScore(Battle battle, BaseSlot slotScoring)
        {
            if (currentBaseSlot == slotScoring && battle.SelectBool([firstMate], $"{firstMate.Controller.Name}, would you like to move First Mate to another base?"))
            {
                var validBases = battle.GetBaseSlots().Select(x => x.BaseCard).Where(x => x != currentBaseSlot.BaseCard).ToList();
                BaseCard chosenBase = battle.Select(validBases, $"{firstMate.Controller}, select a base to move First Mate to:");
                battle.Move(firstMate, chosenBase, firstMate);
            }
        }

        firstMate.EnterBattlefield += (battle) =>
        {
            battle.EventManager.AfterBaseScores += HandleBaseScore;
        };
        firstMate.ExitBattlefield += (battle) =>
        {
            battle.EventManager.AfterBaseScores -= HandleBaseScore;
        };

        firstMate.EnterBase += (battle, baseSlot) => 
        {
            currentBaseSlot = baseSlot;
        };
        firstMate.ExitBase += (battle, baseSlot) =>
        {
            currentBaseSlot = null;
        };

        return firstMate;
    }
    public static PlayableCard SaucyWench()
    {
        PlayableCard saucyWench = new
        (
            Pirates,
            CardType.Minion,
            "Saucy Wench",
            [
                @"            _            ",
                @"          /("")\          ",
                @"           <|-r          ",
                @"           / \           ",
                @"                         ",
                @"You may destroy a minion ",
                @"   of power 2 or less    ",
                @"      on this base.      ",
            ],
            PlayLocation.Base,
            3
        );

        saucyWench.OnPlay += (battle, baseSlot) =>
        {
            if (baseSlot == null) throw new Exception("No base passed in for Saucy Wench");
            SelectCardQuery query = new()
            {
                CardType = CardType.Minion,
                MaxPower = 2,
                BaseCard = baseSlot.BaseCard
            };

            bool validTargetExist = battle.GetFieldCards(query).Count > 0;

            if (validTargetExist && battle.SelectBool([saucyWench], $"Would you like to destroy a minion of power or two or less on {baseSlot.BaseCard.Name}?"))
            {
                PlayableCard? cardToDestroy = battle.SelectFieldCard(saucyWench, "Select a card for Saucy Wench to destroy", query)?.SelectedCard;

                if (cardToDestroy != null) battle.Destroy(cardToDestroy, saucyWench);
            }
        };

        return saucyWench;
    }
    public static PlayableCard Buccaneer()
    {
        PlayableCard buccaneer = new
        (
            Pirates,
            CardType.Minion,
            "Buccaneer",
            [
                @"          /v\            ",
                @"          ("")           ",
                @"          -|-|===>       ",
                @"          / \            ",
                @"                         ",
                @" Special: If this minion ",
                @"would be destroyed, move ",
                @"   it to another base.   ",
            ],
            PlayLocation.Base,
            4
        );

        buccaneer.Protections.Add(new(EffectType.Destroy, buccaneer));

        buccaneer.OnProtect += (battle) =>
        {
            BaseCard currentBase = battle.GetBase(buccaneer);
            var validBases = battle.GetBaseSlots().Select(x => x.BaseCard).Where(x => x != currentBase).ToList();
            BaseCard chosenBase = battle.Select(validBases, $"{buccaneer.Controller.Name}, select a base to move {buccaneer.Name} to:");
            battle.Move(buccaneer, chosenBase, buccaneer);
        };

        return buccaneer;
    }
    public static PlayableCard PirateKing()
    {
        PlayableCard pirateKing = new
        (
            Pirates,
            CardType.Minion,
            "Pirate King",
            [
                @"          /_V_\          ",
                @"          (' ') ?        ",
                @"    <===|--WWW--┘        ",
                @"            |            ",
                @"           / \           ",
                @"          |   |          ",
                @" Before a base scores you",
                @"  may move this there.   ",
            ],
            PlayLocation.Base,
            5
        );

        IEnumerable<BaseCard> otherBases = [];

        void OnBeforeScoresHandler(Battle battle, BaseSlot slot)
        {
            var baseAboutToScore = slot.BaseCard;
            if (otherBases.Contains(baseAboutToScore) && battle.SelectBool([pirateKing], $"{pirateKing.Controller.Name}, would you like to move {pirateKing.Name} to {slot.BaseCard.Name} before it scores?"))
            {
                battle.Move(pirateKing, baseAboutToScore, pirateKing);
            }
        }

        pirateKing.EnterBattlefield += (battle) =>
        {
            battle.EventManager.BeforeBaseScores += OnBeforeScoresHandler;
        };
        pirateKing.ExitBattlefield += (battle) =>
        {
            battle.EventManager.BeforeBaseScores -= OnBeforeScoresHandler;
        };
        pirateKing.EnterBase += (battle, baseSlot) =>
        {
            otherBases = battle.GetBaseSlots().Select(x => x.BaseCard).Where(x => x != baseSlot.BaseCard);
        };
        pirateKing.ExitBase += (battle, baseSlot) =>
        {
            otherBases = [];
        };

        return pirateKing;
    }
    public static PlayableCard Broadside()
    {
        PlayableCard broadside = new
        (
            Pirates,
            CardType.Action,
            "Broadside",
            [
                @"   ________| |________   ",
                @"  |  _   _    _   _   |  ",
                @"  | (O) (O)  (O) (O)  |  ",
                @"   \_________________/   ",
                @"   Destroy all of one    ",
                @"player's minions of power",
                @"2 or less at a base where",
                @"   you have a minion.    ",
            ],
            PlayLocation.Discard
        );

        broadside.OnPlay += (battle, baseSlot) =>
        {
            List<BaseCard> validBases = battle.GetBaseSlots().Where(slot => slot.Territories.SelectMany(terr => terr.Cards).Any(card => card.Controller == broadside.Controller)).Select(slot => slot.BaseCard).ToList();
            if (validBases.Count == 0) return;

            BaseCard baseCard = battle.SelectBaseCard(validBases, broadside, "Choose a base with one of your minions.");
            BaseSlot slot = battle.GetBaseSlot(baseCard);

            foreach (PlayableCard card in slot.Cards)
            {
                if (card.CardType == CardType.Minion && card.CurrentPower <= 2) battle.Destroy(card, broadside);
            }
        };

        return broadside;
    }
    public static PlayableCard Cannon()
    {
        PlayableCard cannon = new
        (
            Pirates,
            CardType.Action,
            "Cannon",
            [
                @"   ___________/ |     __ ",
                @"  /           | |   __  /",
                @"O|____        | |   __ | ",
                @" /    \_______| |     __\",
                @"|      |      \_|        ",
                @" \____/                  ",
                @"Destroy up to two minions",
                @"   of power 2 or less.   ",
            ],
            PlayLocation.Discard
        );

        cannon.OnPlay += (battle, baseSlot) =>
        {
            // Choose up to two cards
            SelectFieldCardsQuery query = new()
            {
                CardType = CardType.Minion,
                MaxPower = 2,
                Num = 2
            };
            var result = battle.SelectFieldCards(cannon, $"Select a card for Cannon to destroy", query, true);

            // Destroy Each
            result.SelectedCards.ForEach(selection => battle.Destroy(selection.Card, cannon));
        };

        return cannon;
    }
    public static PlayableCard Dinghy()
    {
        PlayableCard dinghy = new
        (
            Pirates,
            CardType.Action,
            "Dinghy",
            [
                @"                         ",
                @"      ____/_\______      ",
                @"     |       \     |     ",
                @"~~~~~~\_______\ __/~~~~~~",
                @"              \_\        ",
                @"                         ",
                @" Move up to two of your  ",
                @" minions to other bases. ",
            ],
            PlayLocation.Discard
        );

        dinghy.OnPlay += (battle, baseSlot) =>
        {
            // Choose up to two cards
            SelectFieldCardsQuery query = new()
            {
                CardType = CardType.Minion,
                Controllers = [dinghy.Controller],
                Num = 2
            };
            var result = battle.SelectFieldCards(dinghy, $"Select a card for Dinghy to move", query, true);

            // Move Each
            foreach (var selection in result.SelectedCards)
            {
                var chosenBase = battle.SelectBaseCard(battle.GetBases().Where(baseCard => baseCard != selection.Base).ToList(), dinghy, $"Select a base to move {selection.Card.Name} to.");
                battle.Move(selection.Card, chosenBase, dinghy);
            }
        };

        return dinghy;
    }
    public static PlayableCard FullSail()
    {
        PlayableCard fullSail = new
        (
            Pirates,
            CardType.Action,
            "Full Sail",
            [
                @"A       Full Sail       A",
                @"      |\        |\       ",
                @"      | \       | \      ",
                @"    __|--`_   __|--`_    ",
                @"    \_____/   \_____/    ",
                @" Move any number of your ",
                @" minions to other bases. ",
                @" Special: Before a base  ",
                @"scores, you may play this",
            ],
            PlayLocation.Discard,
            null,
            [Tag.SpecialBeforeScores]
        );

        fullSail.OnPlay += (battle, baseSlot) =>
        {
            // Choose up to two cards
            SelectFieldCardsQuery query = new()
            {
                CardType = CardType.Minion,
                Controllers = [fullSail.Controller],
                Num = int.MaxValue
            };
            var result = battle.SelectFieldCards(fullSail, $"Select any number of minions for {fullSail.Name} to move", query, true);

            // Move Each
            foreach (var selection in result.SelectedCards)
            {
                var chosenBase = battle.SelectBaseCard(battle.GetBases().Where(baseCard => baseCard != selection.Base).ToList(), selection.Card, $"Select a base to move {selection.Card.Name} to.");
                battle.Move(selection.Card, chosenBase, fullSail);
            }
        };

        return fullSail;
    }
    public static PlayableCard Powderkeg()
    {
        PlayableCard powderkeg = new
        (
            Pirates,
            CardType.Action,
            "Powderkeg",
            [
                @"A       Powderkeg       A",
                @"          ______         ",
                @"         |      |        ",
                @"         |Powder|        ",
                @"         |______|        ",
                @"   Destroy one of your   ",
                @" minions and all minions ",
                @"with equal or less power ",
                @"    on the same base.    ",
            ],
            PlayLocation.Discard
        );

        powderkeg.OnPlay += (battle, baseSlot) =>
        {
            SelectCardQuery ownMinionQuery = new()
            {
                CardType = CardType.Minion,
                Controllers = [powderkeg.Controller]
            };
            var result = battle.SelectFieldCard(powderkeg, "Choose one of your minions on a base to destroy", ownMinionQuery);
            PlayableCard? ownMinion = result?.SelectedCard;
            BaseCard? baseCard = result?.SelectedCardBase;

            if (ownMinion != null)
            {
                //Since they are all destroyed at the same time, we need to query for the
                //other minions before resolving the destruction of the chosen minion
                SelectCardQuery minionsToDestroyQuery = new()
                {
                    CardType = CardType.Minion,
                    MaxPower = ownMinion.CurrentPower,
                    BaseCard = baseCard
                };
                var cardsToDestroy = battle.GetFieldCards(minionsToDestroyQuery).Where(card => card != ownMinion);
                
                battle.Destroy(ownMinion, powderkeg);
                cardsToDestroy.ForEach(card => battle.Destroy(card, powderkeg));
            }
        };

        return powderkeg;
    }
    public static PlayableCard SeaDogs()
    {
        PlayableCard seaDogs = new
        (
            Pirates,
            CardType.Action,
            "Sea Dogs",
            [
                @"              /\_        ",
                @"       |\____/ o_)       ",
                @"       |  __   /         ",
                @"       |_|  |_|          ",
                @"Name a faction. Move all ",
                @"other players' minions of",
                @"  that faction from one  ",
                @"     base to another.    ",
            ],
            PlayLocation.Discard
        );

        seaDogs.OnPlay += (battle, baseSlot) =>
        {
            // Name a faction
            List<Faction> factions = battle.GetFactions();
            Faction chosenFaction = battle.Select(factions, "Choose a faction to move");
            
            // Choose a base containing that faction
            var basesWithFaction = battle.GetBaseSlots().Where(slot => slot.Cards.Any(card => card.Controller != seaDogs.Controller && card.Faction == chosenFaction)).Bases();

            BaseCard base1;
            if(basesWithFaction.Count > 1)
            {
                base1 = battle.SelectBaseCard(basesWithFaction, seaDogs, $"Choose a base to move all cards from the {chosenFaction.Name} faction from:");
            } 
            else if (basesWithFaction.Count == 1)
            {
                base1 = basesWithFaction.Single();
            }
            else return;

            // Choose another base
            var otherBases = battle.GetBases().Where(baseCard => baseCard != base1).ToList();
            BaseCard base2 = battle.SelectBaseCard(otherBases, seaDogs, $"Choose a base to move all cards from the {chosenFaction.Name} faction to:");


            // Move all minions of chosen faction from base 1 to base 2
            SelectCardQuery query = new()
            {
                BaseCard = base1,
                Controllers = battle.GetOtherPlayers(seaDogs.Controller),
                Faction = chosenFaction
            };
            var cards = battle.GetFieldCards(query);
            foreach (var card in cards)
            {
                battle.Move(card, base2, seaDogs);
            }
        };

        return seaDogs;
    }
    public static PlayableCard Shanghai()
    {
        PlayableCard shanghai = new
        (
            Pirates,
            CardType.Action,
            "Shanghai",
            [
                @"A        Shanghai       A",
                @"        _                ",
                @"      /("")\     \O/      ",
                @"       <|-r      |       ",
                @"       / \      / \      ",
                @"    -----------------    ",
                @"                         ",
                @"  Move another player's  ",
                @" minion to another base. ",
            ],
            PlayLocation.Discard
        );

        shanghai.OnPlay += (battle, baseSlot) =>
        {
            SelectFieldCardsQuery query = new()
            {
                CardType = CardType.Minion,
                Controllers = battle.GetOtherPlayers(shanghai.Controller)
            };
            var result = battle.SelectFieldCard(shanghai, $"Select a card to Shanghai!", query, true);

            if(result.SelectedCard != null)
            {
                var chosenBase = battle.SelectBaseCard(battle.GetBases().Where(baseCard => baseCard != result.SelectedCardBase).ToList(), shanghai, $"Select a base to move {result.SelectedCard.Name} to.");
                battle.Move(result.SelectedCard, chosenBase, shanghai);
            }
        };

        return shanghai;
    }
    public static PlayableCard Swashbuckling()
    {
        PlayableCard swashbuckling = new
        (
            Dinosaurs,
            CardType.Action,
            "Swashbuckling",
            [
                @"A     Swashbuckling     A",
                @"                         ",
                @"       /v\  \/           ",
                @"       ("")  /\  o        ",
                @"        |--/  \-|        ",
                @"       / \     < \       ",
                @"  Each of your minions   ",
                @"gains +1 power until the ",
                @"    end of the turn.     ",
            ],
            PlayLocation.Discard
        );

        swashbuckling.OnPlay += (battle, baseSlot) => GiveYourMinionsPowerUntilEndOfTurn(battle, swashbuckling);

        return swashbuckling;
    }

    public static BaseCard TheGreyOpal()
    {
        BaseCard theGreyOpal = new
        (
            Pirates,
            "The Grey Opal",
            [


                "      3      1      1       ",
                "After this base scores, all ",
                "   players other than the   ",
                "  winner may move a minion  ",
                "  from here to another base ",
                "instead of the discard pile.",

            ],
            17,
            [3, 1, 1]
        );

        theGreyOpal.AfterBaseScores += (battle, slot, scoreResult) =>
        {
            var winnerScores = scoreResult.First;
            var playersWithMinionHereNotWinner = battle.GetPlayers()
                                                       .Where(player => slot.Cards.Any(card => card.Controller == player &&
                                                                                       card.CardType == CardType.Minion &&
                                                                                       !winnerScores.Select(score => score.Player).Contains(card.Controller)));
            foreach (var player in playersWithMinionHereNotWinner)
            {
                bool moveMinion = battle.SelectBool([], $"{player.Name}, would you like to move one of your minions from {theGreyOpal.Name}?");
                if (moveMinion)
                {
                    SelectCardQuery query = new()
                    {
                        Controllers = [player],
                        BaseCard = theGreyOpal
                    };

                    var result = battle.SelectFieldCard(theGreyOpal, $"{player.Name}, choose a minion to move from {theGreyOpal.Name}", query);
                    if (result.SelectedCard == null) continue;
                    PlayableCard chosenMinion = result.SelectedCard;
                    BaseCard chosenBase = battle.SelectBaseCard(battle.GetBases().Where(x => x != theGreyOpal).ToList(), chosenMinion, $"{player.Name}, choose a base to move {chosenMinion.Name} to:");
                    battle.Move(chosenMinion, chosenBase, theGreyOpal, player);
                }
            }

            return null;
        };

        return theGreyOpal;
    }
    public static BaseCard Tortuga()
    {
        BaseCard tortuga = new
        (
            Pirates,
            "Tortuga",
            [


                "      4      3      2       ",
                "                            ",
                "                            ",
                " The runner up may move one ",
                "of his or her minions to the",
                "base that replaces this base",

            ],
            21,
            [4, 3, 2]
        );

        tortuga.OnReplaced += (battle, slot, scoreResults) =>
        {
            var runnersUp = scoreResults.Second;

            foreach (var playerScore in runnersUp)
            {
                Player player = playerScore.Player;
                bool moveMinion = battle.SelectBool([], $"{player.Name}, would you like to move one of your minions to {slot.BaseCard.Name}?");
                if (moveMinion)
                {
                    SelectCardQuery query = new()
                    {
                        Controllers = [player]
                    };

                    var result = battle.SelectFieldCard(slot.BaseCard, $"{player.Name}, choose a minion to move to {slot.BaseCard.Name}", query);
                    if (result.SelectedCard == null) continue;

                    PlayableCard chosenMinion = result.SelectedCard;
                    battle.Move(chosenMinion, slot.BaseCard, tortuga, player);
                }
            }
        };

        return tortuga;
    }

    // ROBOTS
    public static PlayableCard Zapbot()
    {
        PlayableCard zapbot = new
        (
            Robots,
            CardType.Minion,
            "Zapbot",
            [
                @"           ___           ",
                @"     |__| [__()          ",
                @"       \ __|__           ",
                @"        |     |--[}==    ",
                @"        |_____|          ",
                @"        O     O          ",
                @"  You may play an extra  ",
                @"minion of power 2 or less",
            ],
            PlayLocation.Base,
            2
        );

        zapbot.OnPlay += (battle, baseSlot) =>
        {
            zapbot.Controller.AddMinionPlay(2);
        };

        return zapbot;
    }
    public static PlayableCard Hoverbot()
    {
        PlayableCard hoverbot = new
        (
            Robots,
            CardType.Minion,
            "Hoverbot",
            [
                @"        __|--|__         ",
                @"       /__    __\        ",
                @"        / \  / \         ",
                @"         / \/ \          ",
                @" Reveal the top card of  ",
                @"your deck. If minion, you",
                @" may play it as an extra ",
                @"  minion, or return it.  ",
            ],
            PlayLocation.Base,
            3
        );

        hoverbot.OnPlay += (battle, baseSlot) =>
        {
            var cardToReveal = hoverbot.Controller.Draw();

            if (cardToReveal == null) return;

            Option playIt = new("Play It as Extra Minion");
            Option returnIt = new("Return It");

            List<Option> options = [];
            if (cardToReveal.CardType == CardType.Minion)
            {
                options.Add(playIt);
            }

            options.Add(returnIt);

            Guid chosenOption = battle.SelectOption(options, [cardToReveal], $"{hoverbot.Name} revealed {cardToReveal.Name}");

            if (playIt.Id == chosenOption)
            {
                battle.PlayExtraCard(cardToReveal);
            }
            else if (returnIt.Id == chosenOption)
            {
                hoverbot.Controller.Deck.AddToTop(cardToReveal);
            }
        };

        return hoverbot;
    }
    public static PlayableCard Warbot()
    {
        PlayableCard warbot = new
        (
            Robots,
            CardType.Minion,
            "Warbot",
            [
                @"4        Warbot         4",
                @"       __________        ",
                @"      | ________ |       ",
                @"    /-||________||-\     ",
                @"  _/_ |          | _\_   ",
                @" |   ||__________||   |  ",
                @"        |_|  |_|         ",
                @"  Ongoing: This minion   ",
                @"  cannot be destroyed.   ",
            ],
            PlayLocation.Base,
            4
        );

        warbot.Protections.Add(new(EffectType.Destroy, warbot));

        return warbot;
    }
    public static PlayableCard Nukebot()
    {
        PlayableCard nukebot = new
        (
            Robots,
            CardType.Minion,
            "Nukebot",
            [
                @"           ___           ",
                @"        <=|_=_|=>        ",
                @"            |            ",
                @"        /--[_]--\        ",
                @"       /    |    \       ",
                @" After this is destroyed,",
                @"   destroy each other    ",
                @"  player's minions here. ",
            ],
            PlayLocation.Base,
            5
        );

        nukebot.OnDestroyed += (battle, baseSlot) =>
        {
            //Since they are all destroyed at the same time, we need to query for the
            //other minions before resolving the destruction of the chosen minion
            SelectCardQuery minionsToDestroyQuery = new()
            {
                CardType = CardType.Minion,
                BaseCard = baseSlot.BaseCard,
                Controllers = battle.GetOtherPlayers(nukebot.Controller)
            };
            var cardsToDestroy = battle.GetFieldCards(minionsToDestroyQuery);
            cardsToDestroy.ForEach(card => battle.Destroy(card, nukebot));
        };

        return nukebot;
    }
    public static PlayableCard MicrobotAlpha()
    {
        PlayableCard microbotAlpha = new
        (
            Robots,
            CardType.Minion,
            "Microbot Alpha",
            [
                @"          _[o]_          ",
                @"       ]-|_ | _|-[       ",
                @"          _|_|_          ",
                @"         /_____\         ",
                @" Ongoing: Gains +1 power ",
                @" for each of your other  ",
                @"   Microbots. All your   ",
                @"  minions are Microbots. ",
            ],
            PlayLocation.Base,
            1,
            [Tag.Microbot]
        );

        void CardEnteredBattlefieldHandler(Battle battle, PlayableCard card)
        {
            if(card.Controller == microbotAlpha.Controller)
            {
                // Add a temp microbot tag if not already microbot
                if (!card.Tags.Contains(Tag.Microbot)) card.AddTag(battle, Tag.TempMicrobot);

                // Increase own power
                microbotAlpha.ApplyPowerChange(battle, microbotAlpha, 1);
            }
        }
        void CardExitedBattlefieldHandler(Battle battle, PlayableCard card)
        {
            if (card.Controller == microbotAlpha.Controller)
            {
                // Decrease own power
                microbotAlpha.ApplyPowerChange(battle, microbotAlpha, -1);
            }
        }

        microbotAlpha.EnterBattlefield += (battle) =>
        {
            // Get all controller's minions
            SelectCardQuery query = new()
            {
                Controllers = [microbotAlpha.Controller],
                CardsToExclude = [microbotAlpha]
            };
            var cards = battle.GetFieldCards(query);

            // Add temp microbot tag to all who are not already microbots
            foreach (var card in cards)
            {
                if (!card.Tags.Contains(Tag.Microbot)) card.AddTag(battle, Tag.TempMicrobot);
            }

            // Increase power for each, except self since it is accouted for in the addCardHandler() which is called right after
            microbotAlpha.ApplyPowerChange(battle, microbotAlpha, cards.Count - 1);

            // Add Listeners
            battle.EventManager.CardEnteredBattlefield += CardEnteredBattlefieldHandler;
            battle.EventManager.CardExitedBattlefield += CardExitedBattlefieldHandler;
        };
        microbotAlpha.ExitBattlefield += (battle) =>
        {
            // Get all controller's minions
            SelectCardQuery query = new()
            {
                Controllers = [microbotAlpha.Controller]
            };
            var cards = battle.GetFieldCards(query).Where(x => x != microbotAlpha);

            // Remove one instance of Tag.TempMicrobot
            cards.ForEach(x => x.RemoveTag(battle, Tag.TempMicrobot));

            // Remove Listeners
            battle.EventManager.CardEnteredBattlefield -= CardEnteredBattlefieldHandler;
            battle.EventManager.CardExitedBattlefield -= CardExitedBattlefieldHandler;
        };

        return microbotAlpha;
    }
    public static PlayableCard MicrobotArchive()
    {
        PlayableCard microbotArchive = new
        (
            Robots,
            CardType.Minion,
            "Microbot Archive",
            [
                @"            [=}          ",
                @"         ___|_           ",
                @"        /   _ \--\o      ",
                @"        |  |O||          ",
                @"        |_____|          ",
                @"Ongoing: After a Microbot",
                @"you control is destroyed,",
                @"including this, draw card",
            ],
            PlayLocation.Base,
            1,
            [Tag.Microbot]
        );

        int numCardsToDraw = 0;

        void BeforeDestroyCardHandler(Battle battle, PlayableCard card)
        {
            if (card.Controller == microbotArchive.Controller && (card.Tags.Contains(Tag.Microbot) || card.Tags.Contains(Tag.TempMicrobot)))
            {
                numCardsToDraw++;
            }
        }

        void OnDestroyCardHandler(Battle battle, PlayableCard card)
        {
            microbotArchive.Controller.DrawToHand(numCardsToDraw);
            numCardsToDraw = 0;
        }

        microbotArchive.OnDestroyed += (battle, slot) => OnDestroyCardHandler(battle, microbotArchive);

        microbotArchive.EnterBattlefield += (battle) =>
        {
            battle.EventManager.BeforeDestroyCard += BeforeDestroyCardHandler;
            battle.EventManager.OnDestroyCard += OnDestroyCardHandler;
        };

        microbotArchive.ExitBattlefield += (battle) =>
        {
            battle.EventManager.BeforeDestroyCard -= BeforeDestroyCardHandler;
            battle.EventManager.OnDestroyCard -= OnDestroyCardHandler;
        };

        return microbotArchive;
    }
    public static PlayableCard MicrobotFixer()
    {
        PlayableCard microbotFixer = new
        (
            Robots,
            CardType.Minion,
            "Microbot Fixer",
            [
                @"     o-\  __|_           ",
                @"        \| o  |-[        ",
                @"      }--|____|--X       ",
                @"  If this is the first   ",
                @"minion you play this turn",
                @"  you may play an extra  ",
                @"  minion. Ongoing: Your  ",
                @" Microbots have +1 power ",
            ],
            PlayLocation.Base,
            1,
            [Tag.Microbot]
        );

        void addCardHandler(Battle battle, PlayableCard card)
        {
            if (card.Controller == microbotFixer.Controller && card.Tags.Contains(Tag.Microbot))
            {
                card.ApplyPowerChange(battle, microbotFixer, 1);
            }
        }
        void tagAddHandler(Battle battle, PlayableCard card, Tag tag)
        {
            if (card.Controller == microbotFixer.Controller && tag == Tag.TempMicrobot)
            {
                // Only apply if it wasn't already a microbot
                if(!card.Tags.Contains(Tag.Microbot) && !(card.Tags.Where(x => x == Tag.TempMicrobot).Count() > 1)) card.ApplyPowerChange(battle, microbotFixer, 1);
            }
        }
        void tagRemoveHandler(Battle battle, PlayableCard card, Tag tag)
        {
            if (card.Controller == microbotFixer.Controller && tag == Tag.TempMicrobot)
            {
                //Make sure it isn't still a microbot
                if (!card.Tags.Contains(Tag.Microbot) && !card.Tags.Contains(Tag.TempMicrobot)) card.ExpirePowerChange(-1);
            }
        }

        microbotFixer.EnterBattlefield += (battle) =>
        {
            // Get all controller's microbots
            SelectCardQuery query = new()
            {
                Controllers = [microbotFixer.Controller],
                Tags = [Tag.Microbot, Tag.TempMicrobot]
            };
            var cards = battle.GetFieldCards(query);

            // Add +1 power
            foreach (var card in cards)
            {
                card.ApplyPowerChange(battle, microbotFixer, 1);
            }

            // Add Listeners
            battle.EventManager.CardEnteredBattlefield += addCardHandler;
            battle.EventManager.AfterAddTag += tagAddHandler;
            battle.EventManager.AfterRemoveTag += tagRemoveHandler;
        };
        microbotFixer.ExitBattlefield += (battle) =>
        {
            // Get all controller's microbots
            SelectCardQuery query = new()
            {
                Controllers = [microbotFixer.Controller],
                Tags = [Tag.Microbot, Tag.TempMicrobot]
            };
            var cards = battle.GetFieldCards(query);

            // Remove power
            foreach (var card in cards)
            {
                card.ExpirePowerChange(-1);
            }

            // Remove Listeners
            battle.EventManager.CardEnteredBattlefield -= addCardHandler;
            battle.EventManager.AfterAddTag -= tagAddHandler;
            battle.EventManager.AfterRemoveTag -= tagRemoveHandler;
        };

        microbotFixer.OnPlay += (battle, baseSlot) =>
        {
            if (battle.GetTurnPlays(microbotFixer.Controller) == 1)
            {
                microbotFixer.Controller.AddMinionPlay();
            }
        };

        return microbotFixer;
    }
    public static PlayableCard MicrobotGuard()
    {
        PlayableCard microbotGuard = new
        (
            Robots,
            CardType.Minion,
            "Microbot Guard",
            [
                @"           ___           ",
                @"         /\ _ /\         ",
                @"        |--|O|--|        ",
                @"         \ | | /         ",
                @"Destroy a minion on this ",
                @"base with power less than",
                @"  the number of minions  ",
                @"      you have here.     ",
            ],
            PlayLocation.Base,
            1,
            [Tag.Microbot]
        );

        microbotGuard.OnPlay += (battle, baseSlot) =>
        {
            if (baseSlot == null) throw new Exception($"No base passed in for {microbotGuard.Name}");
            SelectCardQuery query1 = new()
            {
                CardType = CardType.Minion,
                BaseCard = baseSlot.BaseCard,
                Controllers = [microbotGuard.Controller]
            };
            int minionCount = battle.GetFieldCards(query1).Count;


            SelectCardQuery query2 = new()
            {
                CardType = CardType.Minion,
                MaxPower = minionCount - 1,
                BaseCard = baseSlot.BaseCard
            };
            bool validTargetExist = battle.GetFieldCards(query2).Count > 0;

            if (validTargetExist)
            {
                PlayableCard? cardToDestroy = battle.SelectFieldCard(microbotGuard, $"Select a card for {microbotGuard.Name} to destroy", query2)?.SelectedCard;

                if (cardToDestroy != null) battle.Destroy(cardToDestroy, cardToDestroy);
            }
        };

        return microbotGuard;
    }
    public static PlayableCard MicrobotReclaimer()
    {
        PlayableCard microbotReclaimer = new
        (
            Robots,
            CardType.Minion,
            "Microbot Reclaimer",
            [
                @"       |---[o]--\        ",
                @"      [ ] __|__  \       ",
                @"         /_____\  D      ",
                @"  If this is the first   ",
                @"minion you play this turn",
                @"  you may play an extra  ",
                @" minion. Shuffle in deck ",
                @"any # discarded Microbots"
            ],
            PlayLocation.Base,
            1,
            [Tag.Microbot]
        );

        microbotReclaimer.OnPlay += (battle, baseSlot) =>
        {
            if (battle.GetTurnPlays(microbotReclaimer.Controller) == 1)
            {
                microbotReclaimer.Controller.AddMinionPlay();
            }

            List<PlayableCard> microbotCards = microbotReclaimer.Controller.DiscardPile.Where(card => card.Tags.Contains(Tag.Microbot)).ToList();
            if (microbotCards.Count > 0)
            {
                List<PlayableCard> chosenCards = battle.SelectMultiple(microbotCards, "Choose any number of Microbots to shuffle into your deck:");

                chosenCards.ForEach(card => microbotReclaimer.Controller.DiscardPile.Remove(card));
                microbotReclaimer.Controller.Deck.Shuffle(chosenCards);
            }
            else
            {
                battle.SelectOption([new("CONTINUE")], microbotCards, "You have no microbots in your discard pile");
            }
        };

        return microbotReclaimer;
    }
    public static PlayableCard TechCenter()
    {
        PlayableCard techCenter = new
        (
            Robots,
            CardType.Action,
            "Tech Center",
            [
                @"A      Tech Center      A",
                @"          __|__          ",
                @"        _|     |_        ",
                @"    ]--/_       _\--[    ",
                @"       _|__ _ __|_       ",
                @"      / |__| |__| \      ",
                @"     Choose a base.      ",
                @"  Draw a card for each   ",
                @" of your minions there.  ",
            ],
            PlayLocation.Discard
        );

        techCenter.OnPlay += (battle, baseslot) =>
        {
            List<BaseCard> validBases = battle.GetBaseSlots()
                                              .Where(x => x.Cards.Any(card => card.Controller == techCenter.Controller))
                                              .Select(slot => slot.BaseCard).ToList();

            if (validBases.Count < 1) return;
                
            BaseCard chosenBase = battle.SelectBaseCard(validBases, techCenter, "Choose a base. You will draw cards for each of your minions there.");

            SelectFieldCardsQuery query = new()
            {
                BaseCard = chosenBase,
                CardType = CardType.Minion,
                Controllers = [techCenter.Controller]
            };
            int numMinions = battle.GetFieldCards(query).Count;
            techCenter.Controller.DrawToHand(numMinions);
        };

        return techCenter;
    }

    public static BaseCard Factory436_1337()
    {
        BaseCard factory436_1337 = new
        (
            Pirates,
            "Factory 436-1337",
            [


                "       2      2      1      ",
                "                            ",
                "                            ",
                " When this base scores, the ",
                "winner gains 1 VP for every ",
                "5 power that player has here",

            ],
            22,
            [2, 2, 1]
        );

        factory436_1337.WhenBaseScores += (battle, scoreResults) =>
        {
            List<PlayerScore> winnerScores = scoreResults.First;

            foreach (PlayerScore score in winnerScores)
            {
                score.Player.GainVP(score.TotalPower / 5);
            }

        };

        return factory436_1337;
    }
    public static BaseCard TheCentralBrain()
    {
        BaseCard theCentralBrain = new
        (
            Robots,
            "The Central Brain",
            [


                "      4      2      1       ",
                "                            ",
                "                            ",
                "                            ",
                "    Each minion here has    ",
                "         +1 power.          ",

            ],
            19,
            [4, 2, 1]
        );

        theCentralBrain.OnAddCard += (battle, card) =>
        {
            card.ApplyPowerChange(battle, theCentralBrain, 1);
        };

        return theCentralBrain;
    }


    // WIZARDS
    public static PlayableCard Neophyte()
    {
        PlayableCard neophyte = new
        (
            Wizards,
            CardType.Minion,
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
            if (cardToReveal == null) return;

            Option playIt = new("Play It as Extra Action");
            Option drawIt = new("Draw It");
            Option returnIt = new("Return It");

            List<Option> options = [];
            if (cardToReveal.CardType == CardType.Action)
            {
                options.Add(playIt);
                options.Add(drawIt);
            }
            else
            {
                options.Add(returnIt);
            }
            Guid chosenOption = battle.SelectOption(options, [cardToReveal], $"{neophyte.Name} revealed {cardToReveal.Name}");

            if (playIt.Id == chosenOption)
            {
                battle.PlayExtraCard(cardToReveal);
            }
            else if (drawIt.Id == chosenOption)
            {
                // When a card that others can see goes to the hand, deck or discard pile,
                // it goes to the one belonging to the card’s owner.
                cardToReveal.ResetController();
                cardToReveal.Owner.AddToHand(cardToReveal);
            }
            else if (returnIt.Id == chosenOption)
            {
                neophyte.Controller.Deck.AddToTop(cardToReveal);
            }
        };

        return neophyte;
    }
    public static PlayableCard Enchantress()
    {
        PlayableCard enchantress = new
        (
            Wizards,
            CardType.Minion,
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

        enchantress.OnPlay += (battle, baseslot) => enchantress.Controller.DrawToHand();

        return enchantress;
    }
    public static PlayableCard Chronomage()
    {
        PlayableCard chronomage = new
        (
            Wizards,
            CardType.Minion,
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
    }
    public static PlayableCard Archmage()
    {
        PlayableCard archmage = new
        (
            Dinosaurs,
            CardType.Minion,
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

        archmage.ExitBattlefield += (battle) =>
        {
            battle.EventManager.StartOfTurn -= turnStartHandler;
        };

        archmage.OnChangeController += (battle, oldController, newController) =>
        {
            var activePlayer = battle.GetActivePlayer();
            if (activePlayer != oldController && activePlayer == newController)
            {
                activePlayer.ActionPlays += 1;
            }
        };


        return archmage;
    }
    public static PlayableCard MassEnchantment()
    {
        PlayableCard massEnchantment = new
        (
            Wizards,
            CardType.Action,
            "Mass Enchantment",
            [
                @"A    Mass Enchantment   A",
                @"              |          ",
                @"      O  *****| __o      ",
                @"     -|-/*****|    \     ",
                @"     / \      |    /\    ",
                @"  Reveal the top card of ",
                @"each other player's deck.",
                @"Play one revealed action ",
                @"   as an extra action.   ",
            ],
            PlayLocation.Discard
        );

        massEnchantment.OnPlay += (battle, baseSlot) =>
        {
            // Reveal the top card of each other player’s deck
            List<PlayableCard> revealedCards = [];
            List<string> cardNames = [];
            
            foreach (Player player in battle.GetPlayers())
            {
                if(player != massEnchantment.Controller)
                {
                    var card = player.Draw();
                    if (card != null)
                    {
                        revealedCards.Add(card);
                        cardNames.Add($"{player.Name}'s {card.Name}");
                    }
                }
            }
            string displayText = $"You revealed {string.Join(", ", cardNames)}";

            // Play one revealed action as an extra action
            PlayableCard? chosenCard = null;
            if (revealedCards.Any(card => card.CardType == CardType.Action))
            {
                chosenCard = battle.Select(revealedCards, displayText, card => card.CardType == CardType.Action);
                chosenCard.ChangeController(battle, massEnchantment.Controller);
                battle.PlayExtraCard(chosenCard);
            }
            else
            {
                battle.SelectOption([new("CONTINUE")], revealedCards, displayText);
            }


            // Return unused cards to the top of their decks
            foreach (var card in revealedCards)
            {
                if(card != chosenCard) card.Controller.Deck.AddToTop(card);
            }
        };

        return massEnchantment;
    }
    public static PlayableCard MysticStudies()
    {
        PlayableCard mysticStudies = new
        (
            Wizards,
            CardType.Action,
            "Mystic Studies",
            [
                @"A    Mystic Studies     A",
                @"                         ",
                @"    _,-----. .-----,_    ",
                @"   //~~~~~~ | ~~~~~~\\   ",
                @"  //~~~~~~  |  ~~~~~~\\  ",
                @" //________ | ________\\ ",
                @" '--------.___.--------' ",
                @"                         ",
                @"     Draw two cards.     ",
            ],
            PlayLocation.Discard
        );

        mysticStudies.OnPlay += (battle, baseslot) =>
        {
            mysticStudies.Controller.DrawToHand(2);
        };

        return mysticStudies;
    }
    public static PlayableCard Portal()
    {
        PlayableCard portal = new
        (
            Wizards,
            CardType.Action,
            "Portal",
            [
                @"A        Portal         A",
                @"         /****\          ",
                @"        |******|         ",
                @"         \****/          ",
                @"Reveal the top 5 cards of",
                @"your deck. Place any # of",
                @"  minions revealed into  ",
                @" your hand. Return the   ",
                @"other cards to your deck.",
            ],
            PlayLocation.Discard
        );

        portal.OnPlay += (battle, baseSlot) =>
        {
            string displayText = "Choose any number of minions to put in your hand:";
            var revealedCards = portal.Controller.Draw(5);
            if (revealedCards.Any(card => card.CardType == CardType.Minion))
            {
                List<PlayableCard> chosenCards = battle.SelectMultiple(revealedCards, displayText, null, card => card.CardType == CardType.Minion);

                foreach (var card in chosenCards)
                {
                    // When a card that others can see goes to the hand, deck or discard pile,
                    // it goes to the one belonging to the card’s owner.
                    card.ResetController();
                    card.Owner.AddToHand(card);
                    revealedCards.Remove(card);
                }

                revealedCards = battle.SelectMultiple(revealedCards, "Choose the order these cards they should put back on your deck (e.i. the last one chosen will be the next one drawn).", revealedCards.Count);
                foreach (var card in revealedCards)
                {
                    // When a card that others can see goes to the hand, deck or discard pile,
                    // it goes to the one belonging to the card’s owner.
                    card.ResetController();
                    card.Owner.Deck.AddToTop(card);
                }
            }
            else
            {
                battle.SelectOption([new("CONTINUE")], revealedCards, displayText);
            }          
            
        };

        return portal;
    }
    public static PlayableCard Sacrifice()
    {
        PlayableCard sacrifice = new
        (
            Wizards,
            CardType.Action,
            "Sacrifice",
            [
                @"A       Sacrifice       A",
                @"                         ",
                @"                         ",
                @"        | |  | |         ",
                @"       (__|  |__)        ",
                @"   Choose one of your    ",
                @"minions. Draw cards equal",
                @"  to its power. Destroy  ",
                @"      that minion.       ",
            ],
            PlayLocation.Discard

        );

        sacrifice.OnPlay += (battle, baseSlot) =>
        {
            SelectCardQuery query = new()
            {
                CardType = CardType.Minion,
                Controllers = [sacrifice.Controller]
            };
            PlayableCard? cardToDestroy = battle.SelectFieldCard(sacrifice, "Select a card to sacrifice", query)?.SelectedCard;
            if (cardToDestroy != null)
            {
                sacrifice.Controller.DrawToHand(cardToDestroy.CurrentPower ?? 0);
                battle.Destroy(cardToDestroy, sacrifice);
            }
        };


        return sacrifice;
    }
    public static PlayableCard Scry()
    {
        PlayableCard scry = new
        (
            Wizards,
            CardType.Action,
            "Scry",
            [
                @"A         Scry          A",
                @"         ______          ",
                @"        /      \         ",
                @"       |        |        ",
                @"        \______/         ",
                @"       /________\        ",
                @" Search your deck for an ",
                @"  action and reveal it.  ",
                @"  Put it into your hand. ",
            ],
            PlayLocation.Discard
        );

        scry.OnPlay += (battle, baseSlot) =>
        {
            string displayText = "Choose an action from your deck to put in your hand:";
            var deckCards = scry.Controller.Deck.Cards;
            if (deckCards.Any(card => card.CardType == CardType.Action))
            {
                PlayableCard selectedAction = battle.Select(deckCards, displayText, card => card.CardType == CardType.Action);
                if (!scry.Controller.Deck.Draw(selectedAction)) throw new Exception($"{selectedAction.Name} with ID {selectedAction.Id} doesn't exist in {scry.Controller.Name}'s deck");
                scry.Controller.AddToHand(selectedAction);
                scry.Controller.Deck.Shuffle();
            }
            else
            {
                battle.SelectOption([new("CONTINUE")], deckCards, displayText);
            }
        };

        return scry;
    }
    public static PlayableCard Summon()
    {
        PlayableCard summon = new
        (
            Wizards,
            CardType.Action,
            "Summon",
            [
                @"A   ___  Summon         A",
                @".-~     '.               ",
                @" / /  ~@\   )            ",
                @" |  |< ~\(..)            ",
                @" \  \<   .,,    _____    ",
                @"/~\ \< /         \O/     ",
                @"/-~\ \_|          |      ",
                @"                 / \     ",
                @"  Play an extra minion.  ",
            ],
            PlayLocation.Discard
        );

        summon.OnPlay += (battle, baseslot) =>
        {
            summon.Controller.AddMinionPlay();
        };

        return summon;
    }
    public static PlayableCard TimeLoop()
    {
        PlayableCard timeLoop = new
        (
            Wizards,
            CardType.Action,
            "Time Loop",
            [
                @"A       Time Loop       A",
                @"       _________         ",
                @"      /   1|2   \        ",
                @"     |     |     |       ",
                @"     |9    O    3|       ",
                @"     |      \    |       ",
                @"      \____6_\__/        ",
                @"                         ",
                @" Play two extra actions. ",
            ],
            PlayLocation.Discard
        );

        timeLoop.OnPlay += (battle, baseslot) =>
        {
            timeLoop.Controller.ActionPlays += 2;
        };

        return timeLoop;
    }
    public static PlayableCard WindsOfChange()
    {
        PlayableCard windsOfChange = new
        (
            Wizards,
            CardType.Action,
            "Winds of Change",
            [
                @"A    Winds of Change    A",
                @"~  ~  ~  ~  ~  ~  ~  ~  ~",
                @"  ~  ~  ~  ~  ~  ~  ~  ~ ",
                @"~  ~  ~  ~  ~  ~  ~  ~  ~",
                @"  ~  ~  ~  ~  ~  ~  ~  ~ ",
                @" Shuffle your hand into  ",
                @" your deck and draw five ",
                @" cards. You may play an  ",
                @"      extra action.      ",
            ],
            PlayLocation.Discard
        );

        windsOfChange.OnPlay += (battle, baseslot) =>
        {
            windsOfChange.Controller.Recard();
            windsOfChange.Controller.DrawToHand(5);

            windsOfChange.Controller.ActionPlays += 1;
        };

        return windsOfChange;
    }

    public static BaseCard SchoolOfWizardry()
    {
        BaseCard schoolOfWizardry = new
        (
            Wizards,
            "School of Wizardry",
            [
                "      3      2      1       ",
                "After this base scores, the ",
                "  winner looks at the top   ",
                "  cards of the base deck,   ",
                "chooses one to replace this ",
                "base, and returns the others",
            ],
            20,
            [3, 2, 1]
        );

        schoolOfWizardry.AfterBaseScores += (battle, slot, scoreResult) =>
        {
            var winners = scoreResult.First;
            List<BaseCard> bases = battle.DrawBases(3);
            BaseCard chosenBase = battle.Select(bases, $"{string.Join(" and ", winners.Select(x => x.Player.Name))}, choose a base to replace School of Wizardry");
            bases.Remove(chosenBase);
            if (bases.Count > 1)
            {
                // This wil reorder them according to selection
                bases = battle.SelectMultiple(bases, $"{string.Join(" and ", winners.Select(x => x.Player.Name))}, choose bases in the order they should put back on the deck (e.i. the last one chosen will be the next one drawn).", bases.Count);
            }
            battle.PutBasesToTop(bases);

            return chosenBase;
        };

        return schoolOfWizardry;
    }
    public static BaseCard TheGreatLibrary()
    {
        BaseCard theGreatLibrary = new
        (
            Wizards,
            "The Great Library",
            [


                "      4      2      1       ",
                "                            ",
                "                            ",
                "After this base scores, all ",
                " players with minions here  ",
                "     may draw one card.     ",

            ],
            22,
            [4, 2, 1]
        );

        theGreatLibrary.AfterBaseScores += (battle, slot, winners) =>
        {
            var playersWithMinionHere = battle.GetPlayers().Where(player => slot.Cards.Any(card => card.Controller == player && card.CardType == CardType.Minion));
            foreach (var player in playersWithMinionHere)
            {
                bool drawCard = battle.SelectBool([], $"{player.Name}, would you like to draw a card?");
                if (drawCard)
                {
                    player.DrawToHand();
                }
            }

            return null;
        };

        return theGreatLibrary;
    }


    // GENERAL
    public static PlayableCard Minion()
    {

        PlayableCard minion = new
        (
            Dinosaurs,
            CardType.Minion,
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
    }

    // Card Effects
    private static void GiveYourMinionsPowerUntilEndOfTurn(Battle battle, PlayableCard affector)
    {
        SelectCardQuery query = new()
        {
            Controllers = [affector.Controller]
        };

        List<PlayableCard> cardsToChange = battle.GetFieldCards(query);

        if (cardsToChange.Count > 0)
        {
            foreach (var card in cardsToChange)
            {
                bool appliedPower = card.ApplyPowerChange(battle, affector, 1);

                void endTurnHandler(Battle battle, ActivePlayer activePlayer)
                {
                    if (activePlayer.Player == affector.Controller)
                    {
                        if(battle.IsInField(card))
                        {
                            card.ExpirePowerChange(-1);
                        }
                        battle.EventManager.EndOfTurn -= endTurnHandler;
                    }
                }

                battle.EventManager.EndOfTurn += endTurnHandler;
            }
        }
    }

    //TEST
    public static readonly Dictionary<Faction, List<Func<PlayableCard>>> PlayableCardsByFactionDict = new()
    {
        //{ Dinosaurs, [MicrobotAlpha, MicrobotArchive, MicrobotFixer, SaucyWench, MicrobotAlpha, MicrobotArchive, MicrobotFixer, SaucyWench, MicrobotAlpha, MicrobotArchive, MicrobotFixer, SaucyWench] },
        { Robots, [Hoverbot, MicrobotFixer, MicrobotFixer, MicrobotFixer, MicrobotFixer, MicrobotFixer, MicrobotFixer] },
    };

    //REAL
    public static readonly Dictionary<Faction, List<Func<PlayableCard>>> _PlayableCardsByFactionDict = new()
    {
        { Dinosaurs, [WarRaptor, WarRaptor, WarRaptor, WarRaptor, ArmoredStego, ArmoredStego, ArmoredStego, Laseratops, Laseratops, KingRex, Augmentation, Augmentation, Howl, Howl, NaturalSelection, Rampage, SurvivalOfTheFittest, ToothClawAndGuns, Upgrade, WildlifePreserve] },
        { Pirates, [FirstMate, FirstMate, FirstMate, FirstMate, SaucyWench, SaucyWench, SaucyWench, Buccaneer, Buccaneer, PirateKing, Broadside, Broadside, Cannon, Dinghy, Dinghy, FullSail, Powderkeg, SeaDogs, Shanghai, Swashbuckling] },
        { Robots, [Zapbot, Zapbot, Zapbot, Zapbot, Hoverbot, Hoverbot, Hoverbot, Warbot, Warbot, Nukebot, MicrobotAlpha, MicrobotArchive, MicrobotFixer, MicrobotFixer, MicrobotGuard, MicrobotGuard, MicrobotReclaimer, MicrobotReclaimer, TechCenter, TechCenter] },
        { Wizards, [Neophyte, Neophyte, Neophyte, Neophyte, Enchantress, Enchantress, Enchantress, Chronomage, Chronomage, Archmage, MassEnchantment, MysticStudies, MysticStudies, Portal, Sacrifice, Scry, Summon, Summon, TimeLoop, WindsOfChange] },
    };

    public static readonly Dictionary<Faction, List<Func<BaseCard>>> BaseCardsByFactionDict = new()
    {
        { Dinosaurs, [JungleOasis, TarPits] },
        { Pirates, [TheGreyOpal, Tortuga] },
        { Robots, [Factory436_1337, TheCentralBrain] },
        { Wizards, [SchoolOfWizardry, TheGreatLibrary] }
    };
}
