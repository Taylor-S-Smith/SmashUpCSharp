using Models.Cards;

namespace SmashUp.Backend.Repositories
{
    internal interface IBaseCardRepository
    {
        /// <summary>
        /// Gets the BaseCard with the specified id
        /// </summary>
        /// <returns>An object of type T</returns>
        BaseCard Get(int id);

        /// <summary>
        /// Gets all BaseCards
        /// </summary>
        /// <returns>A List of all objects of type T</returns>
        List<BaseCard> GetAll();

        /// <summary>
        /// Create a BaseCard
        /// </summary>
        /// <param name="baseCard">The BaseCard to create</param>
        /// <returns>The ID of the BaseCard created</returns>
        int Create(BaseCard baseCard);

        /// <summary>
        /// Updates an existing BaseCard, or creates a new one if it doesn't exist
        /// </summary>
        /// <param name="baseCard"></param>
        /// <returns>The ID of the baseCard updated or created</returns>
        int Save(BaseCard baseCard);

        /// <summary>
        /// Gets all BaseCards with the specified faction
        /// </summary>
        /// <returns>A List of type BaseCard</returns>
        List<BaseCard> Get(Faction faction);
    }
}
