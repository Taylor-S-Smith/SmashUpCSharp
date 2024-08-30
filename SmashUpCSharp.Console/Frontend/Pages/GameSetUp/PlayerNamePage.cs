using Models.Player;
using SmashUp.Backend.Services;
using SmashUp.Frontend.Pages;
using SmashUp.Frontend.Utilities;
using System.Text;

namespace SmashUp.Frontend.Pages.GameSetUp;

internal class PlayerNamePage : PrimitivePage
{

    //State Variables
    private readonly List<PrimitivePlayer> Players;
    private int ActivePlayerIndex = 0;

    //Display Variables
    private string[] Header = AsciiUtil.ToAscii($"Player 1 Name:");
    private readonly string[] Body = new string[5];
    private string PlayerName = "";

    //Other Variables
    public override Dictionary<ConsoleKey, UserKeyPress> KeyMappings { get; set; } = AlphaKeyMappings;

    //SERVICES
    readonly IPlayerService _playerService;

    public PlayerNamePage(IPlayerService playerService)
    {
        _playerService = playerService;
        Players = _playerService.GetAll();
    }

    public override void Render(int consoleWidth, int consoleHeight)
    {
        StringBuilder? buffer;

        //Generate Body
        Body[0] = "╔═";
        Body[1] = "║ ";
        Body[2] = "║ ";
        Body[3] = "║ ";
        Body[4] = "╚═";


        string[] asciiName = AsciiUtil.ToAscii(PlayerName);

        for (int j = 0; j < asciiName[0].Length; j++)
        {
            Body[0] += "═";
            Body[4] += "═";
        }
        for (int j = 0; j < 3; j++)
        {
            Body[j + 1] += asciiName[j];
        }

        Body[0] += "═╗";
        Body[1] += " ║";
        Body[2] += " ║";
        Body[3] += " ║";
        Body[4] += "═╝";

        int renderHeight = Header.Length + HEADER_PADDING + Body.Length;

        string[] render = new string[renderHeight];
        int i = 0;
        foreach (string line in Header)
        {
            render[i++] = line;
        }
        i += HEADER_PADDING;
        foreach (string line in Body)
        {
            render[i++] = line;
        }

        buffer = RenderUtil.Center(render, (consoleHeight - 1, consoleWidth - 1));
        Console.SetCursorPosition(0, 0);
        Console.Write(buffer);
    }

    public override string? ChangeState(UserKeyPress keyPress, ref bool stateChanged)
    {
        switch (keyPress)
        {
            case UserKeyPress.Backspace:
                if (PlayerName.Length > 0) PlayerName = PlayerName.Remove(PlayerName.Length - 1);
                stateChanged = true;
                break;
            case UserKeyPress.Confirm:
                //Set current player name, reset name buffer, progress to next player or deck selection
                Players[ActivePlayerIndex].Name = PlayerName;
                if (++ActivePlayerIndex == Players.Count)
                {
                    return "DeckSelectionPage";
                }
                PlayerName = "";
                Header = AsciiUtil.ToAscii($"Player {ActivePlayerIndex + 1} Name:");

                stateChanged = true;
                break;
            default:
                if (PlayerName.Length < 13) PlayerName += Enum.GetName(keyPress);
                stateChanged = true;
                break;
        }

        return null;
    }
}
