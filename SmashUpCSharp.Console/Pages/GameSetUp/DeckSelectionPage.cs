using SmashUp.Models.Games;
using SmashUp.Utilities;
using SmashUp.Rendering;
using Models.Cards;
using Repositories;
using System.Text;

namespace SmashUp.Pages.GameSetUp
{
    public class DeckSelectionPage(GameSetUpModel gameSetUp) : PrimitivePage
    {
        int MAX_FACTIONS = 2;

        //Initial Variables
        private string[] Header = [];

        //State Variables
        private int PlayerNum = 0;
        private int FactionNum = 0;

        private int SelectedOption = 0;
        private List<Faction> FactionOptions = new FactionRepository().GetAll().ToList();

        public override void Render(int consoleWidth, int consoleHeight)
        {
            Header = AsciiUtil.ToAscii($"{gameSetUp.Players[PlayerNum].Name}, choose faction #{FactionNum + 1}:");
            StringBuilder? buffer = PageUtil.generateTextSelect(consoleWidth, consoleHeight, Header, FactionOptions.Select(x => x.Name).ToArray(), SelectedOption);

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
                    SelectedOption = Math.Min(FactionOptions.Count - 1, SelectedOption + 1);
                    stateChanged = true;
                    break;
                case UserKeyPress.Confirm:
                    gameSetUp.Players[PlayerNum].Factions.Add(FactionOptions[SelectedOption]);

                    FactionNum++;
                    if (FactionNum == MAX_FACTIONS)
                    {
                        FactionNum = 0;
                        PlayerNum++;
                        if (PlayerNum == gameSetUp.Players.Count)
                        {
                            return new Quit();
                        }
                    }
                    stateChanged = true;
                    break;
                case UserKeyPress.Escape:
                    Console.WriteLine("Escaping game...");
                    break;
            }

            return null;
        }
    }
}
