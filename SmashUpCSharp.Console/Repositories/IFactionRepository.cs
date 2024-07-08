using Models.Cards;

namespace Repositories
{
    internal interface IFactionRepository
    {
        /// <summary>
        /// Gets the Faction with the specified id
        /// </summary>
        /// <returns>An object of type T</returns>
        Faction Get(int id);

        /// <summary>
        /// Gets all Factions
        /// </summary>
        /// <returns>A List of all objects of type T</returns>
        List<Faction> GetAll();

        /// <summary>
        /// Create a Faction
        /// </summary>
        /// <param name="faction">The Faction to create</param>
        /// <returns>The ID of the Faction created</returns>
        int Create(Faction faction);

        /// <summary>
        /// Updates an existing Faction, or creates a new one if it doesn't exist
        /// </summary>
        /// <param name="faction"></param>
        /// <returns>The ID of the Faction updated or created</returns>
        int Save(Faction faction);
    }
}
