using Models.Cards;

namespace Services
{
    internal interface IBaseService
    {
        List<BaseCard> GetBaseCards(List<Faction> factions);
    }
}