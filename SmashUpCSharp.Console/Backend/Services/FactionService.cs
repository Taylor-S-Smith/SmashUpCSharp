using Models.Cards;
using SmashUp.Backend.Repositories;

namespace SmashUp.Backend.Services;

internal class FactionService(IFactionRepository factionRepo) : IFactionService
{
    readonly IFactionRepository _factionRepo = factionRepo;

    public Faction Get(int id)
    {
        return _factionRepo.Get(id);
    }

    public List<Faction> GetAll()
    {
        return _factionRepo.GetAll();
    }
}
