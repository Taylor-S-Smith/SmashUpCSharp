using Models.Cards;
using SmashUp.Backend.Repositories;

namespace SmashUp.Backend.Services
{
    internal class BaseService(IBaseCardRepository baseCardRepo) : IBaseService
    {
        readonly IBaseCardRepository _baseCardRepo = baseCardRepo;

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
