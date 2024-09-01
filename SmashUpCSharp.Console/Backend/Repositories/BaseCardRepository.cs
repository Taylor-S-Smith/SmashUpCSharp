using Models.Cards;

namespace SmashUp.Backend.Repositories
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

        public List<BaseCard> GetByFaction(int factionId)
        {
            return Items.Where(x => x.FactionId == factionId).ToList();
        }

        public List<BaseCard> GetAll()
        {
            return new(Items);
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
