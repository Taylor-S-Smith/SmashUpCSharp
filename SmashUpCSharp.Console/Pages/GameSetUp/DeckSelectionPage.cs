using SmashUp.Utilities;
using SmashUp.Rendering;
using Models.Player;
using Models.Cards;
using System.Text;
using Services;

namespace SmashUp.Pages.GameSetUp
{
    internal class DeckSelectionPage : PrimitivePage
    {
        //Static Variables
        static readonly int MAX_FACTIONS = 2;

        //State Variables
        private readonly List<PrimitivePlayer> Players;
        private int PlayerNum = 0;
        private int FactionNum = 0;

        private int SelectedOption = 0;
        private readonly List<Faction> FactionOptions;

        //Display Variables
        private string[] Header = [];

        //SERVICES
        readonly IFactionService _factionService;
        readonly IPlayerService _playerService;

        //Navigation
        readonly BattlePage _battlePage;

        public DeckSelectionPage(BattlePage battlePage, IFactionService factionService, IPlayerService playerService)
        {
            _battlePage = battlePage;

            _factionService = factionService;
            _playerService = playerService;

            Players = _playerService.GetAll();
            FactionOptions = _factionService.GetAll();

        }

        public override void Render(int consoleWidth, int consoleHeight)
        {
            Header = AsciiUtil.ToAscii($"{Players[PlayerNum].Name}, choose faction #{FactionNum + 1}:");
            StringBuilder? buffer = RenderUtil.generateTextSelect(consoleWidth, consoleHeight, Header, FactionOptions.Select(x => x.Name).ToArray(), SelectedOption);

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
                    Players[PlayerNum].Factions.Add(FactionOptions[SelectedOption]);

                    FactionNum++;
                    if (FactionNum == MAX_FACTIONS)
                    {
                        FactionNum = 0;
                        PlayerNum++;
                        if (PlayerNum == Players.Count)
                        {
                            //TODO: Instead of using a GameSetUp object, just save to memory.
                            foreach (PrimitivePlayer player in Players)
                            {
                                _playerService.Create(player);
                            }
                            return _battlePage;
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
