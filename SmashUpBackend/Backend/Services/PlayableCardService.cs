using SmashUp.Backend.Models;
using SmashUp.Backend.Repositories;

namespace SmashUp.Backend.Services;

internal class PlayableCardService(PlayableCardRepository playableCardRepo)
{
    private readonly PlayableCardRepository _playableCardRepo = playableCardRepo;

    /// <summary>
    /// Gets a list of playable cards by a list of factions. Every playable card whose faction appears in the list will be returned.
    /// </summary>
    public List<PlayableCard> GetCards(List<Faction> factions)
    {
        return PlayableCardRepository.GetCards(factions);
    }
}
