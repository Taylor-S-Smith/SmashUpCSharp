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
        "Classic Mode",
        "Rogue",
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
                return SelectedOption switch
                {
                    0 => (StartPageResult?)StartPageResult.StartGame,
                    1 => (StartPageResult?)StartPageResult.RogueLike,
                    2 => (StartPageResult?)StartPageResult.ShowCollection,
                    3 => (StartPageResult?)StartPageResult.Options,
                    4 => (StartPageResult?)StartPageResult.Exit,
                    _ => throw new NotImplementedException(),
                };
            case UserKeyPress.Escape:
                return StartPageResult.Exit;
        }

        return null;
    }
}

internal enum StartPageResult
{
    StartGame,
    RogueLike,
    ShowCollection,
    Options,
    Exit
}