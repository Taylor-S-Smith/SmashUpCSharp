using Models;

namespace Repositories
{
    public interface IRepository<T> where T : class, IIdentifiable
    {
        T? Get(int id);

        /// <summary>
        /// Gets all items of type T
        /// </summary>
        /// <returns>A IList of all objects of type T</returns>
        IList<T> GetAll();

        /// <summary>
        /// Create an item
        /// </summary>
        /// <param name="item">The item to create</param>
        /// <returns>The ID of the item created</returns>
        int Create(T item);

        /// <summary>
        /// Updates an existing item, or creates a new one if it doesn't exist
        /// </summary>
        /// <param name="item"></param>
        /// <returns>The ID of the item updated or created</returns>
        int Save(T item);
    }
}
