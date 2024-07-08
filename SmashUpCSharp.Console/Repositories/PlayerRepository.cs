using Models.Player;

namespace Repositories
{
    internal class PlayerRepository : IPlayerRepository
    {
        private int _idCount = 0;
        private List<PrimitivePlayer> Items { get; } = [];

        //Protects against instantiation outised of dependency injection
        public PlayerRepository() { }

        public int Create(PrimitivePlayer item)
        {
            item.Id = _idCount++;
            Items.Add(item);
            return item.Id;
        }

        public PrimitivePlayer Get(int id)
        {
            return Items.SingleOrDefault(x => x.Id == id) ?? throw new Exception($"Can't get Player with Id {id}"); ;
        }

        public List<PrimitivePlayer> GetAll()
        {
            return Items;
        }

        public int Save(PrimitivePlayer item)
        {
            PrimitivePlayer? databaseVersion = Get(item.Id);
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
