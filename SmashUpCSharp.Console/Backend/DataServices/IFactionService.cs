using Models.Cards;

namespace SmashUp.Backend.Services
{
    internal interface IFactionService
    {
        Faction Get(int id);
        List<Faction> GetAll();
    }
}