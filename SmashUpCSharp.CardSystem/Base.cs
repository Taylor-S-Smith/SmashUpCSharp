using SmashUpCSharp.Models;

namespace Models
{
    public abstract class Base : Card
    {
        public int PrintedBreakpoint { get; set; }

        public int CurrentBreakpoint { get; set; }
        
        private int playerOnePower;
        public int PlayerOnePower
        {
            get { return playerOnePower; }
            set
            {
                playerOnePower = value;
                UpdateTotalPower();
            }
        }

        private int playerTwoPower;
        public int PlayerTwoPower
        {
            get { return playerTwoPower; }
            set
            {
                playerTwoPower = value;
                UpdateTotalPower();
            }
        }

        private int totalPower;
        public int TotalPower { get { return totalPower; } }

        private void UpdateTotalPower()
        {
            totalPower = PlayerOnePower + PlayerTwoPower;
        }

        public abstract void AfterScores();
        public abstract void StartOfTurn();
        public abstract void CardPlayedHere();
        public abstract void CardDestroyedHere();

    }
}
