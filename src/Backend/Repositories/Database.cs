using SmashUp.Backend.Models;
using SmashUp.Backend.GameObjects;
using static SmashUp.Backend.GameObjects.Battle;
using static SmashUp.Backend.Models.PlayableCard;
using LinqKit;
using SmashUp.Backend.API;

namespace SmashUp.Backend.Repositories;

internal static class Database
{
    // DINOSAURS
    public static PlayableCard WarRaptor()
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

        void ChangePowerForEachWarRaptorOnBase(Battle battle, BaseSlot baseSlot, int amount = 1)
        {
            int currRaptorCount = baseSlot.Territories.SelectMany(x => x.Cards).Where(x => x.Name == WAR_RAPTOR_NAME).ToList().Count;
            warRaptor.ApplyPowerChange(battle, warRaptor, currRaptorCount);
        }

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

        warRaptor.OnAddToBase += (battle, baseSlot) =>
        {
            ChangePowerForEachWarRaptorOnBase(battle, baseSlot);
            // Set Up Listeners for future War Raptor Changes
            baseSlot.BaseCard.OnAddCard += addCardHandler;
            baseSlot.BaseCard.OnRemoveCard += removeCardHandler;
        };

        warRaptor.OnRemoveFromBase += (battle, baseSlot) =>
        {
            ChangePowerForEachWarRaptorOnBase(battle, baseSlot, -1);
            // Remove Listeners
            baseSlot.BaseCard.OnAddCard -= addCardHandler;
            baseSlot.BaseCard.OnRemoveCard -= removeCardHandler;
        };

        return warRaptor;
    }
    public static PlayableCard ArmoredStego()
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
    }
    public static PlayableCard Laseratops()
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
            PlayableCard? cardToDestroy = battle.SelectFieldCard(laseratops, "Select a card for Lasertops to destroy", query)?.SelectedCard;
            if (cardToDestroy != null) battle.Destroy(cardToDestroy, laseratops);
        };

        return laseratops;
    }
    public static PlayableCard KingRex()
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
    }
    public static PlayableCard Augmentation()
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
    }
    public static PlayableCard Howl()
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
    }
    public static PlayableCard NaturalSelection()
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
    }
    public static PlayableCard Rampage()
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
    }
    public static PlayableCard SurvivalOfTheFittest()
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
    }
    public static PlayableCard ToothClawAndGuns()
    {
        PlayableCard toothAndClawAndGuns = new
        (
            Faction.Dinosuars,
            PlayableCardType.Action,
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
    }
    public static PlayableCard WildlifePreserve()
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

        wildlifePreserve.OnAddToBase += (battle,  baseSlot) =>
        {
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
                                                      .Where(x => x.CardType == PlayableCardType.Minion)
                                                      .ToList();

            minionsOnBase.ForEach(x => AddProtectionsToCard(battle, x));


            baseSlot.BaseCard.OnAddCard += AddProtectionsToCard;
            baseSlot.BaseCard.OnRemoveCard += RemoveProtectionsFromCard;

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

        wildlifePreserve.OnRemoveFromBase += (battle, baseSlot) =>
        {
            List<PlayableCard> protectedCards = battle.GetBaseSlots()
                                                      .Single(slot => slot == baseSlot)
                                                      .Cards
                                                      .ToList();

            protectedCards.ForEach(x => RemoveProtectionsFromCard(battle, x));

            baseSlot.BaseCard.OnAddCard -= AddProtectionsToCard;
            baseSlot.BaseCard.OnRemoveCard -= RemoveProtectionsFromCard;
        };      

        return wildlifePreserve;
    }

    public static BaseCard JungleOasis()
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


    }
    public static BaseCard TarPits()
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
    }

    // PIRATES
    public static PlayableCard FirstMate()
    {
        PlayableCard firstMate = new
        (
            Faction.Pirates,
            PlayableCardType.Minion,
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

        BaseCard? MoveToAnotherBase(Battle battle, BaseSlot slot, List<Player> winners)
        {
            var currentBase = slot.BaseCard;
            if (battle.SelectBool([firstMate], $"{firstMate.Controller.Name}, would you like to move First Mate to another base?"))
            {
                var validBases = battle.GetBaseSlots().Select(x => x.BaseCard).Where(x => x != currentBase).ToList();
                BaseCard chosenBase = battle.SelectCard(validBases, $"{firstMate.Controller}, select a base to move First Mate to:");
                battle.Move(firstMate, chosenBase, firstMate);
            }

            return null;
        }

        firstMate.OnAddToBase += (battle, baseSlot) => 
        {
            baseSlot.BaseCard.AfterBaseScores += MoveToAnotherBase;
        };

        firstMate.OnRemoveFromBase += (battle, baseSlot) =>
        {
            baseSlot.BaseCard.AfterBaseScores -= MoveToAnotherBase;
        };

        return firstMate;
    }
    public static PlayableCard SaucyWench()
    {
        PlayableCard saucyWench = new
        (
            Faction.Pirates,
            PlayableCardType.Minion,
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
            bool validTargetExist = battle.GetValidFieldCards((card) => card.CardType == PlayableCardType.Minion && card.CurrentPower <= 2, baseSlot.BaseCard).Count > 0;

            if (validTargetExist && battle.SelectBool([saucyWench], $"Would you like to destroy a minion of power or two or less on {baseSlot.BaseCard.Name}?"))
            {
                SelectFieldCardQuery query = new()
                {
                    CardType = PlayableCardType.Minion,
                    MaxPower = 2,
                    BaseCard = baseSlot.BaseCard
                };
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
            Faction.Pirates,
            PlayableCardType.Minion,
            "Buccaneer",
            [
                @"          /v\            ",
                @"          ("") |          ",
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
            BaseCard chosenBase = battle.SelectCard(validBases, $"{buccaneer.Controller.Name}, select a base to move {buccaneer.Name} to:");
            battle.Move(buccaneer, chosenBase, buccaneer);
        };

        return buccaneer;
    }
    public static PlayableCard PirateKing()
    {
        PlayableCard pirateKing = new
        (
            Faction.Pirates,
            PlayableCardType.Minion,
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

        void OnBeforeScoresHandler(Battle battle, BaseSlot slot)
        {
            var baseAboutToScore = slot.BaseCard;
            if (battle.SelectBool([pirateKing], $"{pirateKing.Controller.Name}, would you like to move {pirateKing.Name} to {slot.BaseCard.Name} before it scores?"))
            {
                battle.Move(pirateKing, baseAboutToScore, pirateKing);
            }
        }

        pirateKing.OnAddToBase += (battle, baseSlot) =>
        {
            var otherBases = battle.GetBaseSlots().Select(x => x.BaseCard).Where(x => x != baseSlot.BaseCard);
            otherBases.ForEach(x => x.BeforeBaseScores += OnBeforeScoresHandler);
        };

        pirateKing.OnRemoveFromBase += (battle, baseSlot) =>
        {
            var otherBases = battle.GetBaseSlots().Select(x => x.BaseCard).Where(x => x != baseSlot.BaseCard);
            otherBases.ForEach(x => x.BeforeBaseScores -= OnBeforeScoresHandler);
        };

        return pirateKing;
    }
    public static PlayableCard Broadside()
    {
        PlayableCard broadside = new
        (
            Faction.Pirates,
            PlayableCardType.Action,
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
                if (card.CardType == PlayableCardType.Minion && card.CurrentPower <= 2) battle.Destroy(card, broadside);
            }
        };

        return broadside;
    }
    public static PlayableCard Cannon()
    {
        PlayableCard cannon = new
        (
            Faction.Pirates,
            PlayableCardType.Action,
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
                CardType = PlayableCardType.Minion,
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
            Faction.Pirates,
            PlayableCardType.Action,
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
                CardType = PlayableCardType.Minion,
                Controller = dinghy.Controller,
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


    // WIZARDS  
    public static PlayableCard Neophyte()
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
                // When a card that others can see goes to the hand, deck or discard pile,
                // it goes to the one belonging to the card’s owner.
                cardToReveal.ResetController();
                cardToReveal.Owner.Hand.Add(cardToReveal);
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
    }
    public static PlayableCard Chronomage()
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
    }
    public static PlayableCard Archmage()
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
            Faction.Wizards,
            PlayableCardType.Action,
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
                    revealedCards.Add(card);
                    cardNames.Add($"{player.Name}'s {card.Name}");
                }
            }
            string displayText = $"You revealed {string.Join(", ", cardNames)}";

            // Play one revealed action as an extra action
            PlayableCard? chosenCard = null;
            if (revealedCards.Any(PlayableCardPredicates.IsAction))
            {
                chosenCard = battle.SelectCard(revealedCards, displayText, PlayableCardPredicates.IsAction);
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
            Faction.Wizards,
            PlayableCardType.Action,
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
            var drawnCards = mysticStudies.Controller.Draw(2);
            mysticStudies.Controller.Hand.AddRange(drawnCards);
        };

        return mysticStudies;
    }
    public static PlayableCard Portal()
    {
        PlayableCard portal = new
        (
            Faction.Wizards,
            PlayableCardType.Action,
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
            if (revealedCards.Any(PlayableCardPredicates.IsMinion))
            {
                List<PlayableCard> chosenCards = battle.SelectCards(revealedCards, displayText, null, PlayableCardPredicates.IsMinion);

                foreach (var card in chosenCards)
                {
                    // When a card that others can see goes to the hand, deck or discard pile,
                    // it goes to the one belonging to the card’s owner.
                    card.ResetController();
                    card.Owner.Hand.Add(card);
                    revealedCards.Remove(card);
                }

                revealedCards = battle.SelectCards(revealedCards, "Choose the order these cards they should put back on your deck (e.i. the last one chosen will be the next one drawn).", revealedCards.Count);
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
            Faction.Wizards,
            PlayableCardType.Action,
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
            SelectFieldCardQuery query = new()
            {
                CardType = PlayableCardType.Minion,
                Controller = sacrifice.Controller
            };
            PlayableCard? cardToDestroy = battle.SelectFieldCard(sacrifice, "Select a card to sacrifice", query)?.SelectedCard;
            if (cardToDestroy != null)
            {
                List<PlayableCard> drawnCards = sacrifice.Controller.Draw(cardToDestroy.CurrentPower ?? 0);
                sacrifice.Controller.Hand.AddRange(drawnCards);
                battle.Destroy(cardToDestroy, sacrifice);
            }
        };


        return sacrifice;
    }
    public static PlayableCard Scry()
    {
        PlayableCard scry = new
        (
            Faction.Wizards,
            PlayableCardType.Action,
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
            if (deckCards.Any(PlayableCardPredicates.IsAction))
            {
                PlayableCard selectedAction = battle.SelectCard(deckCards, displayText, PlayableCardPredicates.IsAction);
                if (!scry.Controller.Deck.Draw(selectedAction)) throw new Exception($"{selectedAction.Name} with ID {selectedAction.Id} doesn't exist in {scry.Controller.Name}'s deck");
                scry.Controller.Hand.Add(selectedAction);
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
            Faction.Wizards,
            PlayableCardType.Action,
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
            summon.Controller.MinionPlays += 1;
        };

        return summon;
    }
    public static PlayableCard TimeLoop()
    {
        PlayableCard timeLoop = new
        (
            Faction.Wizards,
            PlayableCardType.Action,
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
            Faction.Wizards,
            PlayableCardType.Action,
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
            windsOfChange.Controller.Deck.Shuffle(windsOfChange.Controller.Hand);
            var drawnCards = windsOfChange.Controller.Draw(5);
            windsOfChange.Controller.Hand = drawnCards;

            windsOfChange.Controller.ActionPlays += 1;
        };

        return windsOfChange;
    }

    public static BaseCard SchoolOfWizardry()
    {
        BaseCard schoolOfWizardry = new
        (
            Faction.Wizards,
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

        schoolOfWizardry.AfterBaseScores += (battle, slot, winners) =>
        {
            List<BaseCard> bases = battle.DrawBases(3);
            BaseCard chosenBase = battle.SelectCard(bases, $"{string.Join(" and ", winners.Select(x => x.Name))}, choose a base to replace School of Wizardry");
            bases.Remove(chosenBase);
            if (bases.Count > 1)
            {
                // This wil reorder them according to selection
                bases = battle.SelectCards(bases, $"{string.Join(" and ", winners.Select(x => x.Name))}, choose bases in the order they should put back on the deck (e.i. the last one chosen will be the next one drawn).", bases.Count);
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
            Faction.Wizards,
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
            var playersWithMinionHere = battle.GetPlayers().Where(player => slot.Cards.Any(card => card.Controller == player && card.CardType == PlayableCardType.Minion));
            foreach (var player in playersWithMinionHere)
            {
                bool drawCard = battle.SelectBool([], $"{player.Name}, would you like to draw a card?");
                if (drawCard)
                {
                    player.Hand.Add(player.Draw());
                }
            }

            return null;
        };

        return theGreatLibrary;
    }


    // GENERAL
    internal static class PlayableCardPredicates
    {
        public static bool IsAction(PlayableCard card) => card.CardType == PlayableCardType.Action;
        public static bool IsMinion(PlayableCard card) => card.CardType == PlayableCardType.Minion;
    }

    public static PlayableCard Minion()
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
    }


    public static readonly Dictionary<Faction, List<Func<PlayableCard>>> PlayableCardsByFactionDict = new()
    {
        { Faction.Dinosuars, [WarRaptor, WarRaptor, WarRaptor, WarRaptor, ArmoredStego, ArmoredStego, ArmoredStego, Laseratops, Laseratops, KingRex, Augmentation, Augmentation, Howl, Howl, NaturalSelection, Rampage, SurvivalOfTheFittest, ToothClawAndGuns, Upgrade, WildlifePreserve] },
        //{ Faction.Wizards, [Neophyte, Neophyte, Neophyte, Neophyte, Enchantress, Enchantress, Enchantress, Chronomage, Chronomage, Archmage, MassEnchantment, MysticStudies, MysticStudies, Portal, Sacrifice, Scry, Summon, Summon, TimeLoop, WindsOfChange] },
        //{ Faction.Pirates, [FirstMate, FirstMate, FirstMate, FirstMate, SaucyWench, SaucyWench, SaucyWench, Buccaneer, Buccaneer, PirateKing, Broadside, Broadside, Cannon, Dinghy, Dinghy] }
        { Faction.Wizards, [FirstMate, FirstMate, FirstMate, FirstMate, FirstMate, FirstMate, PirateKing, PirateKing, PirateKing, Dinghy, Dinghy, Dinghy, Dinghy, Cannon, Cannon, Cannon] },

    };

    public static readonly Dictionary<Faction, List<Func<BaseCard>>> BaseCardsByFactionDict = new()
    {
        { Faction.Dinosuars, [JungleOasis, TarPits] },
        { Faction.Wizards, [SchoolOfWizardry, TheGreatLibrary] }
    };
}
