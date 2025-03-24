using SmashUp.Backend.GameObjects;
using SmashUp.Backend.Services;

namespace SmashUp;

internal partial class Application()
{
    internal void Run()
    {
        IUserInputHandler userInputHandler = new DummyUserInputHandler();
        Random random = new();
        BaseCardService baseCardService = new(new());
        PlayableCardService playableCardService = new(new());

        Game game = new(userInputHandler, random, baseCardService, playableCardService);
        game.BeginBattle();
    }
}
