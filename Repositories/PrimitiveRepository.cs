using Models;
using Models.Cards;

namespace Repositories
{
    public abstract class PrimitiveRepository<T> : IRepository<T> where T : class, IIdentifiable
    {
        protected abstract IList<T> _items { get; }

        private int _idCount = 1;

        public int Create(T item)
        {
            item.Id = _idCount;
            _items.Add(item);
            _idCount++;

            return item.Id;
        }

        public T? Get(int id)
        {
            return _items.SingleOrDefault(x => x.Id == id);
        }

        public IList<T> GetAll()
        {
            return _items;
        }

        public int Save(T item)
        {
            T? databaseVersion = Get(item.Id);
            if (databaseVersion == null)
            {
                Create(item);
            }
            else
            {
                _items[_items.IndexOf(databaseVersion)] = item;
            }

            return item.Id;
        }
    }
}
