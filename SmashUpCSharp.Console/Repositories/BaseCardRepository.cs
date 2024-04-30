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

        private BaseCard TheHomeworld = new(
            "The Homeworld",
                [
                    " ____________________________ ",
                    "     The Homeworld      ",
                    "|      4      2      1       |",
                    "|                            |",
                    "|After each time a minion is |",
                    "| played here, it's owner may|",
                    "|  play an extra minion of   |",
                    "|      power 3 or less.      |",
                    "|____________________________|",
                    "                              ",
                ],
                23,
                [4, 2, 1]
        );

        private BaseCard TheMotherShip = new(
            "The Mother Ship",
            [
                @" ____________________________ ",
                @"     The Mothership     ",
                @"|      4      2      1       |",
                @"|                            |",
                @"|After this base scores, the |",
                @"|winner may return one of his|",
                @"|or her minions of power 3 or|",
                @"|less from here to their hand|",
                @"|____________________________|",
            ],
            20,
            [4, 2, 1]
        );
        private BaseCard JungleOasis = new(
            "Jungle Oasis",
            [
                " ____________________________ ",
                "      Jungle Oasis      ",
                "|      2      0      0       |",
                "|                            |",
                "|                            |",
                "|                            |",
                "|                            |",
                "|                            |",
                "|____________________________|",
                "                              ",
            ],
            12,
            [2, 0, 0]
        );
        private BaseCard TarPits = new(
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
                "                              ",
            ],
            18,
            [2, 3, 2]
        );
        private BaseCard TempleOfGoju = new(
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
                "                              ",
            ],
            18,
            [4, 2, 1]
        );
        private BaseCard TheGreyOpal = new(
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
                "                              ",
            ],
            17,
            [3, 1, 1]
        );
        private BaseCard Tortuga = new(
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
                "                              ",
            ],
            21,
            [4, 3, 2]
        );
        private BaseCard Factory4361337 = new(
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
                "                              ",
            ],
            22,
            [2, 2, 1]
        );
        private BaseCard TheCentralBrain = new(
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
                "                              ",
            ],
            19,
            [4, 2, 1]
        );
        private BaseCard CaveOfShinies = new(
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
                "                              ",
            ],
            23,
            [4, 2, 1]
        );
        private BaseCard MushroomKingdom = new(
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
                "                              ",
            ],
            20,
            [4, 2, 1]
        );
        private BaseCard SchoolOfWizardry = new(
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
                "                              ",
            ],
            20,
            [4, 2, 1]
        );
        private BaseCard GreatLibrary = new(
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
                "                              ",
            ],
            22,
            [4, 2, 1]
        );
        private BaseCard EvansCityCemetary = new(
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
				"                              ",
			],
			20,
			[5, 3, 1]
        );
        private BaseCard RhodesPlazaMall = new(
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
                    "                              ",
                ],
                24,
                [0, 0, 0]
        );
    }
}
