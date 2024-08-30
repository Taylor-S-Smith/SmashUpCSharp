using Models.Cards;

namespace SmashUp.Backend.Services
{
    internal interface IBaseService
    {
        List<BaseCard> GetBaseCards(List<Faction> factions);
    }
}