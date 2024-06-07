using Models.Cards;
using static System.Net.Mime.MediaTypeNames;

namespace Repositories
{
    internal class FactionRepository : PrimitiveRepository<Faction>
    {
        protected override IList<Faction> _items { get; } = [];

        private int _idCount = 0;

        Faction Aliens = new Faction("Aliens");
        Faction Dinosaurs = new Faction("Dinosaurs");
        Faction Ninjas = new Faction("Ninjas");
        Faction Pirates = new Faction("Pirates");
        Faction Robots = new Faction("Robots");
        Faction Tricksters = new Faction("Tricksters");
        Faction Wizards = new Faction("Wizards");
        Faction Zombies = new Faction("Zombies");

        public FactionRepository()
        {
            Create(Aliens);
            Create(Dinosaurs);
            Create(Ninjas);
            Create(Pirates);
            Create(Robots);
            Create(Tricksters);
            Create(Wizards);
            Create(Zombies);
        }
    }
}
