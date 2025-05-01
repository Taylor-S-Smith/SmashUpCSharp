using System.Text;
using SmashUp.Frontend.Utilities;

namespace SmashUp.Frontend.Pages;

internal class StartPage : ValuePage<StartPageResult>
{
    private readonly string[] Header =
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
    private readonly string[] Options =
    [
        "Start",
        "Show Collection",
        "Options",
        "Exit"
    ];

    protected override StringBuilder GenerateRender(int consoleWidth, int consoleHeight)
    {
        return RenderUtil.GenerateTextSelect(consoleWidth, consoleHeight, Header, Options, SelectedOption, HEADER_PADDING, OPTION_PADDING);
    }

    public override StartPageResult? HandleKeyPress(UserKeyPress keyPress)
    {
        switch (keyPress)
        {
            case UserKeyPress.Up:
                SelectedOption = Math.Max(0, SelectedOption - 1);
                _needToRender = true;
                break;
            case UserKeyPress.Down:
                SelectedOption = Math.Min(Options.Length - 1, SelectedOption + 1);
                _needToRender = true;
                break;
            case UserKeyPress.Confirm:
                switch (SelectedOption)
                {
                    case 0:
                        return StartPageResult.StartGame;
                    case 1:
                        return StartPageResult.ShowCollection;
                    case 2:
                        return StartPageResult.Options;
                    case 3:
                        return StartPageResult.Exit;
                    default:
                        throw new NotImplementedException();
                }
            case UserKeyPress.Escape:
                return StartPageResult.Exit;
        }

        return null;
    }
}

internal enum StartPageResult
{
    StartGame,
    ShowCollection,
    Options,
    Exit
}