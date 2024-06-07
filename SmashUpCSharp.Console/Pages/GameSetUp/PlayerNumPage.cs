using Models.Player;
using SmashUp.Models.Games;
using SmashUp.Rendering;
using SmashUp.Utilities;
using System.Text;

namespace SmashUp.Pages.GameSetUp
{
    public class PlayerNumPage(GameSetUpModel gameSetUp) : PrimitivePage
    {
        //Initial Variables
        private string[] Header = AsciiUtil.ToAscii("How many players?");

        //State Variables
        private int SelectedOption = 0;
        private string[] Options =
        {
            "2 Players",
            "3 Players",
            "4 Players"
        };

        public override void Render(int consoleWidth, int consoleHeight)
        {
            StringBuilder? buffer = PageUtil.generateTextSelect(consoleWidth, consoleHeight, Header, Options, SelectedOption);

            Console.SetCursorPosition(0, 0);
            Console.Write(buffer);
        }

        public override PrimitivePage? ChangeState(UserKeyPress keyPress, ref bool stateChanged)
        {
            switch (keyPress)
            {
                case UserKeyPress.Up:
                    SelectedOption = Math.Max(0, SelectedOption - 1);
                    stateChanged = true;
                    break;
                case UserKeyPress.Down:
                    SelectedOption = Math.Min(Options.Length - 1, SelectedOption + 1);
                    stateChanged = true;
                    break;
                case UserKeyPress.Confirm:
                    gameSetUp.NumPlayers = SelectedOption + 2;
                    for(int i=0; i < gameSetUp.NumPlayers; i++)
                    {
                        gameSetUp.Players.Add(new HumanPlayer());
                    }
                    return new PlayerNamePage(gameSetUp);
                case UserKeyPress.Escape:
                    Console.WriteLine("Escaping game...");
                    break;
            }

            return null;
        }
    }
}
