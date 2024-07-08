using Models.Player;

namespace Services
{
    internal interface IPlayerService
    {
        void Create(PrimitivePlayer player);
        List<PrimitivePlayer> GetAll();
        string GetName(int Id);
    }
}