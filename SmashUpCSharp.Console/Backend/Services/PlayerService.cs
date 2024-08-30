using Models.Player;
using SmashUp.Backend.Repositories;

namespace SmashUp.Backend.Services
{
    internal class PlayerService(IPlayerRepository personRepo) : IPlayerService
    {
        readonly IPlayerRepository _personRepo = personRepo;

        public string GetName(int Id)
        {
            return _personRepo.Get(Id)?.Name ?? throw new Exception($"No player exists with ID {Id}");
        }

        public List<PrimitivePlayer> GetAll()
        {
            return _personRepo.GetAll();
        }

        public void Create(PrimitivePlayer player)
        {
            _personRepo.Create(player);
        }
    }
}
