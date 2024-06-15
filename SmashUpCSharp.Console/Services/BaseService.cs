using Models.Cards;
using static System.Net.Mime.MediaTypeNames;

namespace Repositories
{
    internal class BaseService
    {
        BaseCardRepository _baseRepo = new();

        public BaseService() { }

        public List<BaseCard> GetBaseCards(List<Faction> factions)
        {
            List<BaseCard> baseCards = new();

            foreach (Faction faction in factions)
            {
                baseCards.AddRange(_baseRepo.Get(faction));
            }

            return baseCards;
        }
    }
}
