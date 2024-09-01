using Models.Player;
using Models.Cards;

namespace SmashUp.Models.Games
{
    public class Battle
    {
        // Fields
        private readonly Random randomGenerator = new();

        // Properties
        public List<PrimitivePlayer> Players { get; set; }
        public BaseDeck BaseDeck { get; set; }
        public List<BaseCard> ActiveBases { get; set; }

        public Turn CurrentTurn { get; set; }

        public Battle(List<PrimitivePlayer> players, List<BaseCard> baseCards)
        {
            Players = players;
            BaseDeck = new(baseCards);
            ActiveBases = BaseDeck.Draw(Players.Count + 1);

            CurrentTurn = new(Players[randomGenerator.Next(Players.Count)]);
        }


        public class Turn(PrimitivePlayer currentPlayer)
        {
            public PrimitivePlayer ActivePlayer { get; set; } = currentPlayer;

            public int MinionPlays { get; set; } = 0;

            public int ActionPlays { get; set; } = 0;

            public void StartTurn()
            {
                MinionPlays = 1;
                ActionPlays = 1;
            }
            public void ScoreBases() 
            { 
                throw new NotImplementedException(); 
            }
            
            public void EndTurn()
            {
                ActivePlayer.Draw(2);
            }
        }
    }
}
