using Models.Cards;

namespace Repositories
{
    internal class BaseCardRepository : IBaseCardRepository
    {
        private int _idCount = 0;
        private List<BaseCard> Items { get; } = [];

        //Protects against instantiation outised of dependency injection
        public BaseCardRepository() { }

        public int Create(BaseCard item)
        {
            item.Id = _idCount++;
            Items.Add(item);
            return item.Id;
        }

        public BaseCard Get(int id)
        {
            return Items.SingleOrDefault(x => x.Id == id) ?? throw new Exception($"Can't get BaseCard with Id {id}");
        }

        public List<BaseCard> Get(Faction faction)
        {
            return Items.Where(x => x.FactionId == faction.Id).ToList();
        }

        public List<BaseCard> GetAll()
        {
            return Items;
        }

        public int Save(BaseCard item)
        {
            BaseCard? databaseVersion = Get(item.Id);
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
