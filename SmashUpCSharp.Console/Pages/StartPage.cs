using SmashUp.Pages.GameSetUp;
using SmashUp.Utilities;
using System.Text;

namespace SmashUp.Rendering
{
    internal class StartPage(PlayerNumPage playerNumPage) : PrimitivePage
    {
        private string[] Header  =
        [
            "███████╗███╗   ███╗ █████╗ ███████╗██╗  ██╗    ██╗   ██╗██████╗ ██╗",
            "██╔════╝████╗ ████║██╔══██╗██╔════╝██║  ██║    ██║   ██║██╔══██╗██║",
            "███████╗██╔████╔██║███████║███████╗███████║    ██║   ██║██████╔╝██║",
            "╚════██║██║╚██╔╝██║██╔══██║╚════██║██╔══██║    ██║   ██║██╔═══╝ ╚═╝",
            "███████║██║ ╚═╝ ██║██║  ██║███████║██║  ██║    ╚██████╔╝██║     ██╗",
            "╚══════╝╚═╝     ╚═╝╚═╝  ╚═╝╚══════╝╚═╝  ╚═╝     ╚═════╝ ╚═╝     ╚═╝"
        ];

        //State Variables
        private int SelectedOption = 0;
        private string[] Options =
        [
            "Start",
            "Show Collection",
            "Options",
            "Exit"
        ];

        //Navigation
        private PlayerNumPage _playerNumPage = playerNumPage;

        public override void Render(int consoleWidth, int consoleHeight)
        {
            StringBuilder? buffer = RenderUtil.generateTextSelect(consoleWidth, consoleHeight, Header, Options, SelectedOption);

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
                    switch (SelectedOption)
                    {
                        case 0:
                            return _playerNumPage;
                        case 1:
                            Console.WriteLine("Showing collection...");
                            break;
                        case 2:
                            Console.WriteLine("Showing options...");
                            break;
                        case 3:
                            return new Quit();
                        default:
                            throw new NotImplementedException();
                    }
					break;
                case UserKeyPress.Escape:
                    Console.WriteLine("Escaping game...");
					break;
            }

			return null;
        }
    }
}
