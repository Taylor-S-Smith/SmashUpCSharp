using Models.Player;
using System.Text;
using SmashUp.Backend.Services;
using SmashUp.Frontend.Utilities;

namespace SmashUp.Frontend.Pages.GameSetUp
{
    internal class PlayerNumPage : PrimitivePage
    {
        //State Variables
        private int numPlayers;
        private int SelectedOption = 0;
        private readonly string[] Options =
        [
            "2 Players",
            "3 Players",
            "4 Players"
        ];

        //Display Variables
        private readonly string[] Header = AsciiUtil.ToAscii("How many players?");

        public override void Render(int consoleWidth, int consoleHeight)
        {
            StringBuilder? buffer = RenderUtil.GenerateTextSelect(consoleWidth, consoleHeight, Header, Options, SelectedOption);

            Console.SetCursorPosition(0, 0);
            Console.Write(buffer);
        }

        public override string? ChangeState(UserKeyPress keyPress, ref bool stateChanged)
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
                    numPlayers = SelectedOption + 2;
                    return "PlayerNamePage";
                case UserKeyPress.Escape:
                    Console.WriteLine("Escaping game...");
                    break;
            }

            return null;
        }
    }
}
