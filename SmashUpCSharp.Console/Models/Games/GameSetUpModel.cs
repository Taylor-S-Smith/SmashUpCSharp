using Models.Cards;
using Models.Player;

namespace SmashUp.Models.Games
{
    public class GameSetUpModel
    {
        public int NumPlayers = 0;
        public List<PrimitivePlayer> Players { get; set; } = new();
    }
}
