using Models.Cards;

namespace SmashUp.Models.Games
{
    public class Battle
    {
        public BaseDeck BaseDeck { get; set; } = new();
        public List<BaseCard> ActiveBases { get; set; } = [];

        
    }
}
