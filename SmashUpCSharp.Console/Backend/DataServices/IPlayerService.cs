using Models.Player;

namespace SmashUp.Backend.Services
{
    internal interface IPlayerService
    {
        void Create(PrimitivePlayer player);
        List<PrimitivePlayer> GetAll();
        string GetName(int Id);
    }
}