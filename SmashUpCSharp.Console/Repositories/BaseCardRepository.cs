using Models.Cards;
using static System.Net.Mime.MediaTypeNames;

namespace Repositories
{
    internal class BaseCardRepository : PrimitiveRepository<BaseCard>
    {
        protected override IList<BaseCard> _items { get; } = [];

        public BaseCardRepository()
        {
            Create(TheHomeworld);
            Create(TheMotherShip);
            Create(JungleOasis);
            Create(TarPits);
            Create(NinjaDojo);
            Create(TempleOfGoju);
            Create(TheGreyOpal);
            Create(Tortuga);
            Create(Factory4361337);
            Create(TheCentralBrain);
            Create(CaveOfShinies);
            Create(MushroomKingdom);
            Create(SchoolOfWizardry);
            Create(GreatLibrary);
            Create(EvansCityCemetary);
            Create(RhodesPlazaMall);
        }

        public List<BaseCard> Get(Faction faction)
        {
            return _items.Where(x => x.FactionId == faction.Id).ToList();
        }

        private BaseCard TheHomeworld = new(
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

        private BaseCard TheMotherShip = new(
            0,
            "The Mother Ship",
            [
                @"╔════════════════════════════╗",
                @"║      The Mothership        ║",
                @"║      4      2      1       ║",
                @"║                            ║",
                @"║After this base scores, the ║",
                @"║winner may return one of his║",
                @"║or her minions of power 3 or║",
                @"║less from here to their hand║",
                @"╚════════════════════════════╝",
            ],
            20,
            [4, 2, 1]
        );
        private BaseCard JungleOasis = new(
            1,
            "Jungle Oasis",
            [
                "┌────────────────────────────┐",
                "│      The Mothership        │",
                "│      4      2      1       │",
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
        private BaseCard TarPits = new(
            1,
            "Tar Pits",
            [
                " ____________________________ ",
                "        Tar Pits        ",
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
        private BaseCard NinjaDojo = new(
            2,
            "Ninja Dojo",
            [
                " ____________________________ ",
                "       Ninja Dojo       ",
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
        private BaseCard TempleOfGoju = new(
            2,
            "Temple of Goju",
            [
                " ____________________________ ",
                "     Temple of Goju     ",
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
        private BaseCard TheGreyOpal = new(
            3,
            "The Grey Opal",
            [
                " ____________________________ ",
                "     The Grey Opal      ",
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
        private BaseCard Tortuga = new(
            3,
            "Tortuga",
            [
                " ____________________________ ",
                "        Tortuga         ",
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
        private BaseCard Factory4361337 = new(
            4,
            "Factory 436-1337",
            [
                " ____________________________ ",
                "    Factory 436-1337    ",
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
        private BaseCard TheCentralBrain = new(
            4,
            "The Central Brain",
            [
                " ____________________________ ",
                "   The Central Brain    ",
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
        private BaseCard CaveOfShinies = new(
            5,
            "Cave of Shinies",
            [
                " ____________________________ ",
                "    Cave of Shinies     ",
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
        private BaseCard MushroomKingdom = new(
            5,
            "Mushroom Kingdom",
            [
                " ____________________________ ",
                "    Mushroom Kingdom    ",
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
        private BaseCard SchoolOfWizardry = new(
            6,
            "School of Wizardry",
            [
                " ____________________________ ",
                "   School of Wizardry   ",
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
        private BaseCard GreatLibrary = new(
            6,
            "The Great Library",
            [
                " ____________________________ ",
                "   The Great Library    ",
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
        private BaseCard EvansCityCemetary = new(
            7,
            "Evans City Cemetery",
            [
				" ____________________________ ",
				"  Evans City Cemetery   ",
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
        private BaseCard RhodesPlazaMall = new(
            7,
            "Rhodes Plaza Mall",
                [
                    " ____________________________ ",
                    "   Rhodes Plaza Mall    ",
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
    }
}
