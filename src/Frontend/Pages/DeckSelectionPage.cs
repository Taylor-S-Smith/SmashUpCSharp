using SmashUp.Frontend.Utilities;
using SmashUp.Backend.API;
using System.Text;

namespace SmashUp.Frontend.Pages
{
    internal class DeckSelectionPage(List<string> playerNames, List<FactionModel> factionOptions) : ReferencePage<List<(string, List<FactionModel>)>>
    {
        //Static Variables
        static readonly int NUM_FACTIONS = 2;

        //State Variables
        private readonly List<(string, List<FactionModel>)> _allSelections = [];
        private readonly List<string> _playerNames = playerNames;
        private readonly List<FactionModel> _selectedFactions = [];
        private int _playerIndex = 0;
        private int _factionIndex = 1;

        private int _selectedOption = 0;
        private readonly List<FactionModel> _factionOptions = factionOptions;

        //Display Variables
        private string[] _header = [];

        protected override StringBuilder GenerateRender(int consoleWidth, int consoleHeight)
        {
            _header = AsciiUtil.ToAscii($"{_playerNames[_playerIndex]}, choose faction #{_factionIndex}:");
            return RenderUtil.GenerateTextSelect(consoleWidth, consoleHeight, _header, _factionOptions.Select(x => x.Name).ToArray(), _selectedOption, HEADER_PADDING, OPTION_PADDING);
        }

        public override List<(string, List<FactionModel>)>? HandleKeyPress(UserKeyPress keyPress)
        {
            switch (keyPress)
            {
                case UserKeyPress.Up:
                    _selectedOption = Math.Max(0, _selectedOption - 1);
                    _needToRender = true;
                    break;
                case UserKeyPress.Down:
                    _selectedOption = Math.Min(_factionOptions.Count - 1, _selectedOption + 1);
                    _needToRender = true;
                    break;
                case UserKeyPress.Confirm:
                    _selectedFactions.Add(_factionOptions[_selectedOption]);
                    if (_factionIndex == NUM_FACTIONS)
                    {
                        _allSelections.Add((_playerNames[_playerIndex], _selectedFactions));
                        _playerIndex++;
                        if (_playerIndex == _playerNames.Count)
                        {
                            return _allSelections;
                        }
                        _factionIndex = 0;
                        _selectedFactions.Clear();
                    }
                    _factionIndex++;
                    _needToRender = true;
                    break;
                case UserKeyPress.Escape:
                    Console.WriteLine("Escaping game...");
                    break;
            }

            return null;
        }
    }
}
