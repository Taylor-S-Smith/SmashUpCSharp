using Models.Cards;
using static System.Net.Mime.MediaTypeNames;

namespace SmashUp.Backend.Repositories
{
    internal class FactionRepository : IFactionRepository
    {
        private int _idCount = 0;
        private List<Faction> Items { get; } = [];

        //Protects against instantiation outised of dependency injection
        public FactionRepository() { }

        public int Create(Faction item)
        {
            item.Id = _idCount++;
            Items.Add(item);
            return item.Id;
        }

        public Faction Get(int id)
        {
            return Items.SingleOrDefault(x => x.Id == id) ?? throw new Exception($"Can't get Faction with Id {id}"); ;
        }

        public List<Faction> GetAll()
        {
            return new(Items);
        }

        public int Save(Faction item)
        {
            Faction? databaseVersion = Get(item.Id);
            if (databaseVersion == null)
            {
                Create(item);
            }
            else
            {
                Items[Items.IndexOf(databaseVersion)] = item;
            }

            return item.Id;
        }
    }
}
