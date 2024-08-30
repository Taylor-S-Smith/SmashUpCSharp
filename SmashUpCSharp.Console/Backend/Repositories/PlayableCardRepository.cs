using Models.Cards;

namespace SmashUp.Backend.Repositories
{
    internal class PlayableCardRepository : IPlayableCardRepository
    {
        private int _idCount = 0;
        private List<PlayableCard> Items { get; } = [];

        //Protects against instantiation outside of dependency injection
        public PlayableCardRepository() { }

        public int Create(PlayableCard item)
        {
            item.Id = _idCount++;
            Items.Add(item);
            return item.Id;
        }

        public PlayableCard Get(int id)
        {
            return Items.SingleOrDefault(x => x.Id == id) ?? throw new Exception($"Can't get PlayableCard with Id {id}"); ;
        }

        public List<PlayableCard> GetAll()
        {
            return new(Items);
        }

        public int Save(PlayableCard item)
        {
            PlayableCard? databaseVersion = Get(item.Id);
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
