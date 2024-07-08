using Repositories;
using Models.Cards;

namespace Services
{
    internal class FactionService : IFactionService
    {
        readonly IFactionRepository _factionRepo;

        public FactionService(IFactionRepository factionRepo)
        {
            _factionRepo = factionRepo;
        }

        public List<Faction> GetAll()
        {
            return _factionRepo.GetAll();
        }
    }
}
