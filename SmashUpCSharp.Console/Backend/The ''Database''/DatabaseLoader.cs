using Models.Cards;
using SmashUp.Backend.Repositories;

namespace SmashUp.The___Database__
{
    internal class DatabaseLoader(IBaseCardRepository baseCardRepo, IFactionRepository factionRepo, IPlayableCardRepository playableCardRepo) : IDatabaseLoader
    {
        readonly IBaseCardRepository _baseCardRepo = baseCardRepo;
        readonly IFactionRepository _factionRepo = factionRepo;
        readonly IPlayableCardRepository _playableCardRepo = playableCardRepo;

        public void Load()
        {
            _factionRepo.Create(Aliens);
            _factionRepo.Create(Dinosaurs);
            _factionRepo.Create(Ninjas);
            _factionRepo.Create(Pirates);
            _factionRepo.Create(Robots);
            _factionRepo.Create(Tricksters);
            _factionRepo.Create(Wizards);
            _factionRepo.Create(Zombies);

            _baseCardRepo.Create(TheHomeworld);
            _baseCardRepo.Create(TheMotherShip);
            _baseCardRepo.Create(JungleOasis);
            _baseCardRepo.Create(TarPits);
            _baseCardRepo.Create(NinjaDojo);
            _baseCardRepo.Create(TempleOfGoju);
            _baseCardRepo.Create(TheGreyOpal);
            _baseCardRepo.Create(Tortuga);
            _baseCardRepo.Create(Factory4361337);
            _baseCardRepo.Create(TheCentralBrain);
            _baseCardRepo.Create(CaveOfShinies);
            _baseCardRepo.Create(MushroomKingdom);
            _baseCardRepo.Create(SchoolOfWizardry);
            _baseCardRepo.Create(GreatLibrary);
            _baseCardRepo.Create(EvansCityCemetary);
            _baseCardRepo.Create(RhodesPlazaMall);

            _playableCardRepo.Create(Collector);
            _playableCardRepo.Create(Scout);
            _playableCardRepo.Create(Invader);
        }

        //FACTIONS

        readonly Faction Aliens = new("Aliens");
        readonly Faction Dinosaurs = new("Dinosaurs");
        readonly Faction Ninjas = new("Ninjas");
        readonly Faction Pirates = new("Pirates");
        readonly Faction Robots = new("Robots");
        readonly Faction Tricksters = new("Tricksters");
        readonly Faction Wizards = new("Wizards");
        readonly Faction Zombies = new("Zombies");

        //BASE CARDS

        private readonly BaseCard TheHomeworld = new(
            0,
            "The Homeworld",
                [
                    " ____________________________ ",
                    "|       The Mothership       |",
                    "|      4      2      1       |",
                    "|                            |",
                    "|After this base scores, the |",
                    "|winner may return one of his|",
                    "|or her minions of power 3 or|",
                    "|less from here to their hand|",
                    "|____________________________|",
                ],
                23,
                [4, 2, 1]
        );

        private readonly BaseCard TheMotherShip = new(
            0,
            "The Mother Ship",
            [
                "╔════════════════════════════╗",
                "║      The Mothership        ║",
                "║      4      3      1       ║",
                "║                            ║",
                "║After this base scores, the ║",
                "║winner may return one of his║",
                "║or her minions of power 3 or║",
                "║less from here to their hand║",
                "╚════════════════════════════╝",
            ],
            20,
            [4, 3, 1]
        );
        private readonly BaseCard JungleOasis = new(
            1,
            "Jungle Oasis",
            [
                "┌────────────────────────────┐",
                "│      The Mothership        │",
                "│      2      2      1       │",
                "│                            │",
                "│After this base scores, the │",
                "│winner may return one of his│",
                "│or her minions of power 3 or│",
                "│less from here to their hand│",
                "└────────────────────────────┘",
                "                              ",
            ],
            12,
            [2, 0, 0]
        );
        private readonly BaseCard TarPits = new(
            1,
            "Tar Pits",
            [
                " ____________________________ ",
                "",
                "|      4      3      1       |",
                "|                            |",
                "|After each time a minion is |",
                "|destroyed here, place it at |",
                "|     the bottom of its      |",
                "|        owners deck.        |",
                "|____________________________|",
                "                              ",
            ],
            16,
            [4, 3, 1]
        );
        private readonly BaseCard NinjaDojo = new(
            2,
            "Ninja Dojo",
            [
                " ____________________________ ",
                "",
                "|      2      3      2       |",
                "|                            |",
                "|                            |",
                "|After this base scores, the |",
                "|   winner may destroy any   |",
                "|        one minion.         |",
                "|____________________________|",
            ],
            18,
            [2, 3, 2]
        );
        private readonly BaseCard TempleOfGoju = new(
            2,
            "Temple of Goju",
            [
                " ____________________________ ",
                "",
                "|       4      2      1      |",
                "|                            |",
                "|  After this base scores,   |",
                "|place each player's highest |",
                "|  power minion here on the  |",
                "|bottom of its owner's deck. |",
                "|____________________________|",
            ],
            18,
            [4, 2, 1]
        );
        private readonly BaseCard TheGreyOpal = new(
            3,
            "The Grey Opal",
            [
                " ____________________________ ",
                "",
                "|      3      1      1       |",
                "|After this base scores, all |",
                "|   players other than the   |",
                "|  winner may move a minion  |",
                "|  from here to another base |",
                "|instead of the discard pile.|",
                "|____________________________|",
            ],
            17,
            [3, 1, 1]
        );
        private readonly BaseCard Tortuga = new(
            3,
            "Tortuga",
            [
                " ____________________________ ",
                "",
                "|      4      3      2       |",
                "|                            |",
                "|                            |",
                "| The runner up may move one |",
                "|of his or her minions to the|",
                "|base that replaces this base|",
                "|____________________________|",
            ],
            21,
            [4, 3, 2]
        );
        private readonly BaseCard Factory4361337 = new(
            4,
            "Factory 436-1337",
            [
                " ____________________________ ",
                "",
                "|       2      2      1      |",
                "|                            |",
                "|                            |",
                "| When this base scores, the |",
                "|winner gains 1 VP for every |",
                "|5 power that player has here|",
                "|____________________________|",
            ],
            22,
            [2, 2, 1]
        );
        private readonly BaseCard TheCentralBrain = new(
            4,
            "The Central Brain",
            [
                " ____________________________ ",
                "",
                "|      4      2      1       |",
                "|                            |",
                "|                            |",
                "|                            |",
                "|    Each minion here has    |",
                "|         +1 power.          |",
                "|____________________________|",
            ],
            19,
            [4, 2, 1]
        );
        private readonly BaseCard CaveOfShinies = new(
            5,
            "Cave of Shinies",
            [
                " ____________________________ ",
                "",
                "|      4      2      1       |",
                "|                            |",
                "|                            |",
                "|After each time a minion is |",
                "| destroyed here, its owner  |",
                "|        gains 1 VP.         |",
                "|____________________________|",
            ],
            23,
            [4, 2, 1]
        );
        private readonly BaseCard MushroomKingdom = new(
            5,
            "Mushroom Kingdom",
            [
                " ____________________________ ",
                "",
                "|       4      2      1      |",
                "|                            |",
                "|    At the start of each    |",
                "| player's turn, that player |",
                "|may move one other player's |",
                "|minion from any base to here|",
                "|____________________________|",
            ],
            20,
            [4, 2, 1]
        );
        private readonly BaseCard SchoolOfWizardry = new(
            6,
            "School of Wizardry",
            [
                " ____________________________ ",
                "",
                "|      4      2      1       |",
                "|After this base scores, the |",
                "|  winner looks at the top   |",
                "|  cards of the base deck,   |",
                "|chooses one to replace this |",
                "|base, and returns the others|",
                "|____________________________|",
            ],
            20,
            [4, 2, 1]
        );
        private readonly BaseCard GreatLibrary = new(
            6,
            "The Great Library",
            [
                " ____________________________ ",
                "",
                "|      4      2      1       |",
                "|                            |",
                "|                            |",
                "|After this base scores, all |",
                "| players with minions here  |",
                "|     may draw one card.     |",
                "|____________________________|",
            ],
            22,
            [4, 2, 1]
        );
        private readonly BaseCard EvansCityCemetary = new(
            7,
            "Evans City Cemetery",
            [
                " ____________________________ ",
                "",
                "|      5      3      1       |",
                "|                            |",
                "|                            |",
                "|After this base scores, the |",
                "| winner discards his or her |",
                "| hand and draws five cards. |",
                "|____________________________|",
            ],
            20,
            [5, 3, 1]
        );
        private readonly BaseCard RhodesPlazaMall = new(
            7,
            "Rhodes Plaza Mall",
            [
                " ____________________________ ",
                "",
                "|                            |",
                "|      0      0      0       |",
                "|                            |",
                "|When this base scores, each |",
                "| player gains 1 VP for each |",
                "|minion that player has here.|",
                "|____________________________|",
            ],
            24,
            [0, 0, 0]
         );

        //PLAYABLE CARDS

        private readonly PlayableCard Collector = new
        (
            "Collector",
            2,
            [@"(o)", 
             @"       -|-r======", 
             @"/ \", 
             @"-------"],
            PlayLocation.Base,
            () => Console.WriteLine("Returning a minion of power 3 or less on this base to its owner's hand")
        );
        private readonly PlayableCard Scout = new
        (
            "Scout",
            3,
            ["(o)",
             " 0|-r ",
             "/ >"],
            PlayLocation.Base,
            () => { }
        );
        private readonly PlayableCard Invader = new
        (
            "Invader",
            3,
            [@"    ___",
             @"(o) [___]",
             @"   -|--|    ",
             @"  / \ |"],
            PlayLocation.Base,
            () => Console.WriteLine("You gained 1 VP")
        );


    }
}
