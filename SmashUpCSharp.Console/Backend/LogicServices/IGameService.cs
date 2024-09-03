using Models.Cards;
using Models.Player;
using SmashUp.Models.Games;

namespace SmashUp.Backend.LogicServices
{
    internal interface IGameService
    {
        Game GetCurrentGame();
        void StartNewGame(List<PrimitivePlayer> players, List<BaseCard> baseCards);
        void StartTestGame();
    }
}