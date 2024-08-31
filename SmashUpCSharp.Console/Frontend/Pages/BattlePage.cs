using SmashUp.Models.Games;
using Models.Cards;
using System.Text;
using SmashUp.Backend.Services;
using SmashUp.Frontend.Utilities;
using Models.Player;
using System.Linq.Expressions;
using System.Numerics;

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
        private readonly int BaseFieldPadding = 1;
        private readonly int CardFieldPadding = 1;
        private readonly int StatFieldPadding = 1;

        private int BaseAreaWidth;

        //GAME OBJECTS
        private Battle Game { get; set; }

        public BattlePage(IBaseService baseService, IFactionService factionService, IPlayableCardService playableCardService, IPlayerService playerService)
        {
            //JUST TO TEST

            _baseService = baseService;
            _factionService = factionService;
            _playableCardService = playableCardService;
            _playerService = playerService;

            List<PlayableCard> testCards = [_playableCardService.Get(0), _playableCardService.Get(1), _playableCardService.Get(2)];

            Faction faction0 = _factionService.Get(0);
            Faction faction1 = _factionService.Get(1);
            Faction faction2 = _factionService.Get(2);
            Faction faction3 = _factionService.Get(3);

            _playerService.Create(new HumanPlayer()
            {
                Name = "Taylor",
                Factions = [faction0, faction1],
                Hand = testCards
            });
            _playerService.Create(new HumanPlayer()
            {
                Name = "Andrew",
                Factions = [faction2, faction3],
                Hand = testCards
            });

            var players = _playerService.GetAll();
            var bases = _baseService.GetBaseCards(players.SelectMany(x => x.Factions).ToList());

            Game = new(players, bases);

            Game.ActiveBases[0].AttachCard(testCards[0]);
            Game.ActiveBases[1].AttachCard(testCards[1]);
            Game.ActiveBases[2].AttachCard(testCards[2]);
        }

        public override void Render(int consoleWidth, int consoleHeight)
        {
            int OtherRenderGraphicsLength = 1;

            //Initalize vars and generate fields
            StringBuilder? buffer = null;
            var baseField = GenerateBaseField(consoleWidth);
            var cardField = GenerateCardField();
            var statField = GenerateStatField();
            var consoleField = GenerateInputField(new[] {baseField.Max(line => line.Length), cardField.Max(line => line.Length), statField.Max(line => line.Length)}.Max());

            //Ensure the current console size will fit the header
            int renderWidth = new[] {
                baseField.Max(line => line.Length),
                cardField.Max(line => line.Length),
                statField.Max(line => line.Length),
                consoleField.Max(line => line.Length)
            }.Max();

            int renderHeight = baseField.Length +
                               BaseFieldPadding +
                               cardField.Length +
                               CardFieldPadding +
                               statField.Length +
                               StatFieldPadding +
                               consoleField.Length +
                               OtherRenderGraphicsLength;

            //Make sure the whole render can fit in the console
            if (consoleHeight - 1 >= renderHeight && consoleWidth - 1 >= renderWidth)
            {
                //Generate final combined render
                string[] render = new string[renderHeight];
                int i = 0;
                foreach (string line in baseField)
                {
                    render[i++] = line;
                }
                i += BaseFieldPadding;
                foreach (string line in cardField)
                {
                    render[i++] = line;
                }
                i += CardFieldPadding;

                //Separation Graphic
                StringBuilder lineBuilder = new();
                lineBuilder.Append('-', renderWidth);
                render[i++] = lineBuilder.ToString();

                foreach (string line in statField)
                {
                    render[i++] = line;
                }
                i += StatFieldPadding;
                foreach (string line in consoleField)
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
        private string[] GenerateBaseField(int consoleWidth)
        {
            int numBases = Game.ActiveBases.Count;
            int baseGraphicVerticalLength = Game.ActiveBases[0].GetGraphic().Count;
            int baseGraphicHorizontalLength = Game.ActiveBases[0].GetGraphic()[0].Length;

            int horizontalPaddingLength = (consoleWidth-1 - (baseGraphicHorizontalLength * numBases)) / (numBases+1);
            BaseAreaWidth = baseGraphicHorizontalLength + horizontalPaddingLength;

            var baseField = new string[baseGraphicVerticalLength];

            for (int i = 0; i < baseGraphicVerticalLength; i++)
            {
                StringBuilder lineBuilder = new();

                for (int j = 0; j < numBases; j++)
                {
                    lineBuilder.Append(Game.ActiveBases[j].GetGraphic()[i]);

                    if (j < numBases - 1 && horizontalPaddingLength > 0) 
                        lineBuilder.Append(' ', horizontalPaddingLength);
                }

                baseField[i] = lineBuilder.ToString();
            }
            return baseField;
        }

        /// <summary>
        /// Generates the text that lists the cards at each base for each player
        /// </summary>
        private string[] GenerateCardField()
        {
            //Gather the base lists
            List<List<string>> baseLists = [];
            foreach (BaseCard baseCard in Game.ActiveBases)
            {
                baseLists.Add(baseCard.GetDisplayList());
            }

            //Determine number of lines needed
            int maxLines = Math.Max(baseLists.Max(x => x.Count), MIN_CARD_FIELD_SIZE);
            var cardField = new string[maxLines];

            for (int i = 0; i < maxLines; i++)
            {
                StringBuilder lineBuilder = new();
                foreach (List<string> baseList in baseLists)
                {
                    string currString = i < baseList.Count ? baseList[i] : string.Empty;
                    lineBuilder.Append(RenderUtil.LeftJustifyString(currString, BaseAreaWidth));
                }

                cardField[i] = lineBuilder.ToString();
            }

            return cardField;
        }

        /// <summary>
        /// Generates the field that lists the game statistics
        /// </summary>
        private string[] GenerateStatField()
        {
            //These are all the elements of the stat field
            List<string> statFieldElements =
            [
                $"{Game.CurrentTurn.ActivePlayer.Name}'s Turn",
                $"Minion Plays: {Game.CurrentTurn.MinionPlays}",
                $"Action Plays: {Game.CurrentTurn.ActionPlays}",
                $"VP: {Game.CurrentTurn.ActivePlayer.VictoryPoints}"
            ];

            //Join elements and padding
            string elementSeparator = $" | ";
            string paddedStatField = string.Join(elementSeparator, statFieldElements);            

            //Return as array
            return [paddedStatField];
        }

        /// <summary>
        /// Generates the field that displays the result of user input. Usually will be their hand, but is also how they view their decks
        /// </summary>
        private string[] GenerateInputField(int fieldLength)
        {
            List<PlayableCard> allCards = Game.CurrentTurn.ActivePlayer.Hand;

            string[] inputField = [""];
            if (allCards.Count > 0)
            {
                int cardHeight = allCards.Max(card => card.GetGraphic().Count);
                int cardLength = allCards.Max(card => card.GetGraphic().Max(graphic => graphic.Length));

                inputField = new string[cardHeight];

                int numCardsToRender = Math.Min(fieldLength/cardLength, allCards.Count);

                List<PlayableCard> cardsToRender = allCards[..numCardsToRender];

                for (int i = 0; i < cardHeight; i++)
                {
                    StringBuilder lineBuilder = new();

                    foreach (PlayableCard card in cardsToRender)
                    {
                        lineBuilder.Append(card.GetGraphic()[i]);
                    }

                    inputField[i] = lineBuilder.ToString();
                }
            }
            
            return inputField;
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
