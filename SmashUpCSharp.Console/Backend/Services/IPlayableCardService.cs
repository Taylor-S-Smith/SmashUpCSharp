using Models.Cards;

namespace SmashUp.Backend.Services
{
    internal interface IPlayableCardService
    {
        PlayableCard Get(int id);
    }
}