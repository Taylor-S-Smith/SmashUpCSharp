using Repositories;
using Models.Cards;

namespace Services
{
    internal class PlayableCardService : IPlayableCardService
    {
        readonly IPlayableCardRepository _playableCardRepo;

        public PlayableCardService(IPlayableCardRepository playableCardRepo)
        {
            _playableCardRepo = playableCardRepo;
        }

        public PlayableCard Get(int id)
        {
            return _playableCardRepo.Get(id);
        }
    }
}
