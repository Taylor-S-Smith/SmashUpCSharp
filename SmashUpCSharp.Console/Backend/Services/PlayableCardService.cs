using Models.Cards;
using SmashUp.Backend.Repositories;

namespace SmashUp.Backend.Services
{
    internal class PlayableCardService(IPlayableCardRepository playableCardRepo) : IPlayableCardService
    {
        readonly IPlayableCardRepository _playableCardRepo = playableCardRepo;

        public PlayableCard Get(int id)
        {
            return _playableCardRepo.Get(id);
        }
    }
}
