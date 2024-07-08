using Repositories;
using Models.Cards;

namespace Services
{
    internal class BaseService : IBaseService
    {
        readonly IBaseCardRepository _baseCardRepo;

        public BaseService(IBaseCardRepository baseCardRepo)
        {
            _baseCardRepo = baseCardRepo;
        }

        public List<BaseCard> GetBaseCards(List<Faction> factions)
        {
            List<BaseCard> baseCards = [];

            foreach (Faction faction in factions)
            {
                baseCards.AddRange(_baseCardRepo.Get(faction));
            }

            return baseCards;
        }
    }
}
