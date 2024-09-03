using Models.Cards;

namespace SmashUp.Backend.Services
{
    internal interface IPlayableCardService
    {
        PlayableCard Get(int id);
        List<PlayableCard> Get(List<Faction> factions);
    }
}