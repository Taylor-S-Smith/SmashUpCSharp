using SmashUp.Frontend.Utilities;
using System.Text;

namespace SmashUp.Frontend.Pages;

internal class PlayerNamePage(int playerCount) : ReferencePage<List<string>>
{
    //State Variables
    private int _activePlayerIndex = 0;
    private readonly int _playerCount = playerCount
;
    private readonly List<string> _playerNames = [];

    //Display Variables
    private string[] _header = AsciiUtil.ToAscii($"Player 1 Name:");
    private readonly string[] _body = new string[5];
    private string _inputBuffer = "";

    //Other Variables
    public override Dictionary<ConsoleKey, UserKeyPress> KeyMappings { get; set; } = FrontendGlobals.AlphaKeyMappings;

    protected override StringBuilder GenerateRender(int consoleWidth, int consoleHeight)
    {
        //Generate Body
        _body[0] = "╔═";
        _body[1] = "║ ";
        _body[2] = "║ ";
        _body[3] = "║ ";
        _body[4] = "╚═";


        string[] asciiName = AsciiUtil.ToAscii(_inputBuffer);

        for (int j = 0; j < asciiName[0].Length; j++)
        {
            _body[0] += "═";
            _body[4] += "═";
        }
        for (int j = 0; j < 3; j++)
        {
            _body[j + 1] += asciiName[j];
        }

        _body[0] += "═╗";
        _body[1] += " ║";
        _body[2] += " ║";
        _body[3] += " ║";
        _body[4] += "═╝";

        int renderHeight = _header.Length + HEADER_PADDING + _body.Length;

        string[] render = new string[renderHeight];
        int i = 0;
        foreach (string line in _header)
        {
            render[i++] = line;
        }
        i += HEADER_PADDING;
        foreach (string line in _body)
        {
            render[i++] = line;
        }

        return RenderUtil.Center(render, (consoleHeight - 1, consoleWidth - 1));
    }

    public override List<string>? HandleKeyPress(UserKeyPress keyPress)
    {
        switch (keyPress)
        {
            case UserKeyPress.Backspace:
                if (_inputBuffer.Length > 0) _inputBuffer = _inputBuffer.Remove(_inputBuffer.Length - 1);
                _needToRender = true;
                break;
            case UserKeyPress.Confirm:
                //Set current player name, reset name buffer, progress to next player or deck selection
                _playerNames.Add(_inputBuffer);
                if (_activePlayerIndex + 1 >= _playerCount)
                {
                    return _playerNames;
                }
                _activePlayerIndex++;
                _inputBuffer = "";
                _header = AsciiUtil.ToAscii($"Player {_activePlayerIndex + 1} Name:");

                _needToRender = true;
                break;
            default:
                if (_inputBuffer.Length < 13)
                {
                    if(keyPress == UserKeyPress.Space)
                    {
                        _inputBuffer += " ";
                    } else
                    {
                        _inputBuffer += Enum.GetName(keyPress);
                    }
                }
                _needToRender = true;
                break;
        }

        return null;
    }
}