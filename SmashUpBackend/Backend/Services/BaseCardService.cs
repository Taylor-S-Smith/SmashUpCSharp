using SmashUp.Backend.Models;
using SmashUp.Backend.Repositories;

namespace SmashUp.Backend.Services;

internal class BaseCardService(BaseCardRepository baseCardRepo)
{
    private readonly BaseCardRepository _baseCardRepo = baseCardRepo;

    /// <summary>
    /// Gets a list of base cards by a list of factions. Every playable card whose base appears in the list will be returned.
    /// </summary>
    public List<BaseCard> GetCards(List<Faction> factions)
    {
        return _baseCardRepo.GetCards(factions);
    }
}