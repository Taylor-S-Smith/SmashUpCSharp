using SmashUp.Models.Games;
using Models.Cards;
using System.Text;
using SmashUp.Backend.Services;
using SmashUp.Frontend.Utilities;
using Models.Player;

namespace SmashUp.Frontend.Pages
{
    internal class BattlePage : PrimitivePage
    {
        //STATIC VARIABLES
        readonly int MIN_CARD_FIELD_SIZE = 15;

        //SERVICES
        readonly IBaseService _baseService;
        readonly IFactionService _factionService;
        readonly IPlayableCardService _playableCardService;
        readonly IPlayerService _playerService;

        //RENDERING VARS
        //Bases in play
        private string[] BaseField = [""];
        private readonly int BaseFieldPadding = 0;

        //Minions and actions in play
        private string[] CardField = [""];
        private readonly int CardFieldPadding = 0;

        //Game statistics (Active Player, Minion/Action count , VP total, etc.)
        private string[] StatField = [""];
        private readonly int StatFieldPadding = 0;

        //The player's input area. Usually will be their hand, but is also how they view their decks
        private string[] ConsoleField = [""];

        //GAME OBJECTS
        private Battle Game { get; set; } = new();

        public BattlePage(IBaseService baseService, IFactionService factionService, IPlayableCardService playableCardService, IPlayerService playerService)
        {
            _baseService = baseService;
            _factionService = factionService;
            _playableCardService = playableCardService;
            _playerService = playerService;

            Faction faction0 = _factionService.Get(0);
            Faction faction1 = _factionService.Get(1);
            Faction faction2 = _factionService.Get(2);
            Faction faction3 = _factionService.Get(3);

            _playerService.Create(new HumanPlayer()
            {
                Name = "Taylor",
                Factions = [faction0, faction1]
            });
            _playerService.Create(new HumanPlayer()
            {
                Name = "Andrew",
                Factions = [faction2, faction3]
            });

            Game.Players = _playerService.GetAll();
            Game.BaseDeck.Cards = _baseService.GetBaseCards(Game.Players.SelectMany(x => x.Factions).ToList());
            Game.ActiveBases = Game.BaseDeck.DrawCards(Game.Players.Count + 1);

            //JUST TO TEST
            Game.ActiveBases[0].AttachCard(_playableCardService.Get(0));
            Game.ActiveBases[1].AttachCard(_playableCardService.Get(1));
            Game.ActiveBases[2].AttachCard(_playableCardService.Get(2));
        }

        public override void Render(int consoleWidth, int consoleHeight)
        {
            //Initalize vars and generate fields
            StringBuilder? buffer = null;
            int baseSectionWidth = GenerateBaseField(consoleWidth);
            GenerateCardField(baseSectionWidth);

            //Ensure the current console size will fit the header
            int renderWidth = new[] {
                BaseField.Max(line => line.Length),
                CardField.Max(line => line.Length),
                StatField.Max(line => line.Length),
                ConsoleField.Max(line => line.Length)
            }.Max();

            int renderHeight = BaseField.Length +
                               BaseFieldPadding +
                               CardField.Length +
                               CardFieldPadding +
                               StatField.Length +
                               StatFieldPadding +
                               ConsoleField.Length;

            //Make sure the whole render can fit in the console
            if (consoleHeight - 1 >= renderHeight && consoleWidth - 1 >= renderWidth)
            {
                //Generate final combined render
                string[] render = new string[renderHeight];
                int i = 0;
                foreach (string line in BaseField)
                {
                    render[i++] = line;
                }
                i += BaseFieldPadding;
                foreach (string line in CardField)
                {
                    render[i++] = line;
                }
                i += CardFieldPadding;
                foreach (string line in StatField)
                {
                    render[i++] = line;
                }
                i += StatFieldPadding;
                foreach (string line in ConsoleField)
                {
                    render[i++] = line;
                }

                buffer = RenderUtil.Center(render, (consoleHeight -1, consoleWidth - 1));
            }
            //Let the user know the screen is too small
            if (buffer is null)
            {
                string[] render =
                [
                    $@"Please increase your screen size"
                ];
                buffer = RenderUtil.Center(render, (consoleHeight - 1, consoleWidth - 1));
            }

            Console.SetCursorPosition(0, 0);
            Console.Write(buffer);
        }

        /// <summary>
        /// Generates the base graphics
        /// </summary>
        private int GenerateBaseField(int consoleWidth)
        {
            int numBases = Game.ActiveBases.Count;
            int baseGraphicVerticalLength = Game.ActiveBases[0].GetGraphic().Count;
            int baseGraphicHorizontalLength = Game.ActiveBases[0].GetGraphic()[0].Length;

            int horizontalPaddingLength = (consoleWidth / numBases) - baseGraphicHorizontalLength;
            

            BaseField = new string[baseGraphicVerticalLength];

            for (int i = 0; i < baseGraphicVerticalLength; i++)
            {
                StringBuilder lineBuilder = new();
                for (int j = 0; j < numBases; j++)
                {
                    lineBuilder.Append(Game.ActiveBases[j].GetGraphic()[i]);
                    if (j < numBases - 1)
                    {
                        lineBuilder.Append(' ', horizontalPaddingLength);
                    }
                }

                BaseField[i] = lineBuilder.ToString();
            }

            return baseGraphicHorizontalLength + horizontalPaddingLength;
        }

        /// <summary>
        /// Generates the text that lists the cards at each base for each player
        /// </summary>
        private void GenerateCardField(int baseSectionWidth)
        {
            //Gather the base lists
            List<List<string>> baseLists = [];
            foreach (BaseCard baseCard in Game.ActiveBases)
            {
                baseLists.Add(baseCard.GetDisplayList());
            }

            //Determine number of lines needed
            int maxLines = Math.Max(baseLists.Max(x => x.Count), MIN_CARD_FIELD_SIZE);
            CardField = new string[maxLines];

            for (int i = 0; i < maxLines; i++)
            {
                StringBuilder lineBuilder = new();

                foreach (List<string> baseList in baseLists)
                {

                    string currString = i < baseList.Count ? baseList[i] : string.Empty;
                    lineBuilder.Append(RenderUtil.LeftJustifyString(currString, baseSectionWidth));
                }

                CardField[i] = lineBuilder.ToString();
            }
        }

        /// <summary>
        /// Generates the field that lists the game statistics
        /// </summary>
        private void GenerateConsoleField()
        {

        }

        /// <summary>
        /// Generates the field that displays the result of user input
        /// </summary>
        private void GenerateStatField()
        {

        }


        public override string? ChangeState(UserKeyPress keyPress, ref bool stateChanged)
        {
            return keyPress switch
            {
                UserKeyPress.Escape => "QUIT",
                _ => null,
            };
        }
    }
}
