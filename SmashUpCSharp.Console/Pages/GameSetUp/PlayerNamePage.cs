using Models.Player;
using SmashUp.Models.Games;
using SmashUp.Rendering;
using SmashUp.Utilities;
using System.Reflection.PortableExecutable;
using System.Text;

namespace SmashUp.Pages.GameSetUp
{
    public class PlayerNamePage(GameSetUpModel gameSetUp) : PrimitivePage
    {
        //Initial Variables
        public override Dictionary<ConsoleKey, UserKeyPress> KeyMappings { get; set; } = AlphaKeyMappings;

        //Display Variables
        private string[] Header = AsciiUtil.ToAscii($"Player 1 Name:");
        private string[] Body = new string[5];
        private string PlayerName = "";

        //State Variables
        private int ActivePlayerIndex = 0;

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

            for(int j = 0; j < asciiName[0].Length; j++)
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

            buffer = ScreenUtil.Center(render, (consoleHeight - 1, consoleWidth - 1));
            Console.SetCursorPosition(0, 0);
            Console.Write(buffer);
        }

        public override PrimitivePage? ChangeState(UserKeyPress keyPress, ref bool stateChanged)
        {
            switch (keyPress)
            {
                case UserKeyPress.Backspace:
                    if(PlayerName.Length > 0) PlayerName = PlayerName.Remove(PlayerName.Length - 1);
                    stateChanged = true;
                    break;
                case UserKeyPress.Confirm:
                    //Set current player name, reset name buffer, progress to next player or deck selection
                    gameSetUp.Players[ActivePlayerIndex].Name = PlayerName;
                    if (++ActivePlayerIndex == gameSetUp.Players.Count) {
                        return new DeckSelectionPage(gameSetUp);
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
}
