using Models.Cards;
using Models.Player;

namespace Repositories
{
    internal interface IPlayerRepository
    {
        /// <summary>
        /// Gets the PrimitivePlayer with the specified id
        /// </summary>
        /// <returns>An object of type T</returns>
        PrimitivePlayer Get(int id);

        /// <summary>
        /// Gets all PrimitivePlayers
        /// </summary>
        /// <returns>A List of all objects of type T</returns>
        List<PrimitivePlayer> GetAll();

        /// <summary>
        /// Create a PrimitivePlayer
        /// </summary>
        /// <param name="primitivePlayer">The PrimitivePlayer to create</param>
        /// <returns>The ID of the PrimitivePlayer created</returns>
        int Create(PrimitivePlayer primitivePlayer);

        /// <summary>
        /// Updates an existing PrimitivePlayer, or creates a new one if it doesn't exist
        /// </summary>
        /// <param name="primitivePlayer"></param>
        /// <returns>The ID of the PrimitivePlayer updated or created</returns>
        int Save(PrimitivePlayer primitivePlayer);
    }
}
