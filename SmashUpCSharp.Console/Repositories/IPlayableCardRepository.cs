using Models.Cards;

namespace Repositories
{
    internal interface IPlayableCardRepository
    {
        /// <summary>
        /// Gets the PlayableCard with the specified id
        /// </summary>
        /// <returns>An object of type T</returns>
        PlayableCard Get(int id);

        /// <summary>
        /// Gets all PlayableCards
        /// </summary>
        /// <returns>A List of all objects of type T</returns>
        List<PlayableCard> GetAll();

        /// <summary>
        /// Create a PlayableCard
        /// </summary>
        /// <param name="playableCard">The PlayableCard to create</param>
        /// <returns>The ID of the PlayableCard created</returns>
        int Create(PlayableCard playableCard);

        /// <summary>
        /// Updates an existing PlayableCard, or creates a new one if it doesn't exist
        /// </summary>
        /// <param name="playableCard"></param>
        /// <returns>The ID of the PlayableCard updated or created</returns>
        int Save(PlayableCard playableCard);
    }
}
