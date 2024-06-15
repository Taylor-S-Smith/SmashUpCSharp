using Models.Cards;
using Models.Player;

namespace SmashUp.Models.Games
{
    public class Battle
    {
        public BaseDeck BaseDeck { get; set; } = new();
        public List<BaseCard> ActiveBases { get; set; } = [];
        public List<PrimitivePlayer> Players { get; set; } = new();


    }
}
