using SmashUp.Frontend.Utilities;
using System.Text;

namespace SmashUp.Frontend.Pages;
internal class PlayerNumPage : ValuePage<int>
{
    //State Variables
    private int SelectedOption = 0;
    private readonly string[] Options =
    [
        "2 Players",
        "3 Players",
        "4 Players"
    ];

    //Display Variables
    private readonly string[] Header = AsciiUtil.ToAscii("How many players?");

    protected override StringBuilder GenerateRender(int consoleWidth, int consoleHeight)
    {
        return RenderUtil.GenerateTextSelect(consoleWidth, consoleHeight, Header, Options, SelectedOption, HEADER_PADDING, OPTION_PADDING);
    }

    public override int? HandleKeyPress(UserKeyPress keyPress)
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
                return SelectedOption + 2;
        }

        return null;
    }
}