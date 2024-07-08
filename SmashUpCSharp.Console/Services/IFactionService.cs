using Models.Cards;

namespace Services
{
    internal interface IFactionService
    {
        List<Faction> GetAll();
    }
}