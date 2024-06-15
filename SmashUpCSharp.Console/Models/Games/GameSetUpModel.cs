using Models.Cards;
using Models.Player;

namespace SmashUp.Models.Games
{
    public class GameSetUpModel
    {
        public List<PrimitivePlayer> Players { get; set; } = new();
    }
}
