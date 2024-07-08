using Models.Cards;

namespace Services
{
    internal interface IPlayableCardService
    {
        PlayableCard Get(int id);
    }
}