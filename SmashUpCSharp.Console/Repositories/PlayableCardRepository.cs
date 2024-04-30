using Models.Cards;
using static System.Net.Mime.MediaTypeNames;

namespace Repositories
{
    internal class PlayableCardRepository : PrimitiveRepository<PlayableCard>
    {
        protected override IList<PlayableCard> _items { get; } = [];

        private int _idCount = 1;

        private PlayableCard Collector = new
        (
            "Collector",
            2,
            ["(o)", "       -|-r======", "/ \\", "-------"],
            PlayLocation.Base,
            () => Console.WriteLine("Returning a minion of power 3 or less on this base to its owner's hand")
        );
        private PlayableCard Scout = new
        (
            "Scout",
            3,
            ["(o)", " 0|-r ", "/ >"],
            PlayLocation.Base,
            () => { }
        );
        private PlayableCard Invader = new
        (
            "Invader",
            3,
            ["    ___", "(o) [___]", "   -|--|    ", "  / \\ |"],
            PlayLocation.Base,
            () => Console.WriteLine("You gained 1 VP")
        );


        public PlayableCardRepository()
        {
            Create(Collector);
            Create(Scout);
        }
    }
}
