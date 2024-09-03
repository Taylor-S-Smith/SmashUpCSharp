using Models.Cards;

namespace SmashUp.Backend.Services
{
    internal interface IBaseService
    {
        List<BaseCard> Get(List<Faction> factions);
    }
}