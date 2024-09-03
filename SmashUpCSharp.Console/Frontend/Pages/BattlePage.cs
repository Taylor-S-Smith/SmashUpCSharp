using SmashUp.Backend.LogicServices;
using SmashUp.Frontend.Utilities;
using SmashUp.Models.Games;
using Models.Cards;
using System.Text;

namespace SmashUp.Frontend.Pages
{
    /// <summary>
    /// Handles all rendering the Battle page
    /// </summary>
    internal class BattlePage : PrimitivePage
    {
        // STATIC VARIABLES
        readonly int CARD_FIELD_SIZE = 15;

        // SERVICES
        readonly IBattlePageService _battleService;
        readonly IGameService _gameService;

        // HAND
        PlayableCard[] _leftBuffer = [];
        PlayableCard[] _handCardsDisplayed = [];
        PlayableCard[] _rightBuffer = [];

        // GAME
        readonly Game _game;

        public BattlePage(IBattlePageService battleService, IGameService gameService)
        {
            _battleService = battleService;
            _gameService = gameService;

            _game = _gameService.GetCurrentGame();
        }

        public override void Render(int consoleWidth, int consoleHeight)
        {
            int baseFieldPadding = 1;
            int statFieldPadding = 1;
            int otherRenderGraphicsLength = 1;

            //Initalize vars and generate fields
            StringBuilder? buffer = null;
            var baseField = GenerateBaseField(consoleWidth);
            var statField = GenerateStatField();
            var consoleField = GenerateInputField(new[] {baseField.Max(line => line.Length), statField.Max(line => line.Length)}.Max());

            //Ensure the current console size will fit the header
            int renderWidth = new[] {
                baseField.Max(line => line.Length),
                statField.Max(line => line.Length),
                consoleField.Max(line => line.Length)
            }.Max();

            int renderHeight = baseField.Length +
                               baseFieldPadding +
                               statField.Length +
                               statFieldPadding +
                               consoleField.Length +
                               otherRenderGraphicsLength;

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
                i += baseFieldPadding;

                //Separation Graphic
                StringBuilder lineBuilder = new();
                lineBuilder.Append('-', renderWidth);
                render[i++] = lineBuilder.ToString();

                foreach (string line in statField)
                {
                    render[i++] = line;
                }
                i += statFieldPadding;
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
        public override string? HandleKeyPress(UserKeyPress keyPress, ref bool stateChanged)
        {
            return _battleService.HandleKeyPress(keyPress, ref stateChanged, _handCardsDisplayed);
        }

        /// <summary>
        /// Generates the base graphics
        /// </summary>
        private string[] GenerateBaseField(int consoleWidth)
        {
            int numBases = _game.ActiveBases.Count;

            // Get Active Bases Graphics
            string[][] activeBasesGraphics = _game.ActiveBases
                .Select(activeBase => activeBase.GetGraphic(IsCardHighlighted(activeBase)))
                .ToArray();

            // Get Attached Cards Graphics
            string[][] attachedCardGraphics = _game.ActiveBases
                .Select(baseCard => baseCard.GetAttachedCardsGraphic(_battleService.GetTargetedPlayableCard()))
                .ToArray();

            if (activeBasesGraphics.Length != attachedCardGraphics.Length) throw new Exception($"Recieved {activeBasesGraphics.Length} active bases but only {attachedCardGraphics.Length} attached card graphics. They should be equal");

            // Calculate array size
            int baseGraphicHeight = activeBasesGraphics.Max(baseGraphic => baseGraphic.Length);
            int paddingHeight = 1;
            int numCardFieldLines = Math.Max(attachedCardGraphics.Max(x => x.Length), CARD_FIELD_SIZE);
            var baseField = new string[baseGraphicHeight + paddingHeight +  numCardFieldLines];

            // Calculate padding
            int baseGraphicWidth = activeBasesGraphics.Max(baseGraphic => baseGraphic.Max(line => line.Length));
            int horizontalPaddingLength = (consoleWidth-1 - (baseGraphicWidth * numBases)) / (numBases+1);

            // Generate combined graphics
            for (int i = 0; i < baseField.Length; i++)
            {
                StringBuilder lineBuilder = new();

                for (int j = 0; j < numBases; j++)
                {
                    if (i < activeBasesGraphics[j].Length)
                    {
                        lineBuilder.Append(activeBasesGraphics[j][i]);

                        if (j < numBases - 1 && horizontalPaddingLength > 0)
                            lineBuilder.Append(' ', horizontalPaddingLength);
                    }
                    else if(i < activeBasesGraphics[j].Length + paddingHeight)
                    {
                        lineBuilder.Append(String.Empty);
                    }
                    else
                    {
                        int attachedCardIndex = i - (baseGraphicHeight + paddingHeight);
                        int currBaseArea = activeBasesGraphics[j].Max(x => x.Length) + horizontalPaddingLength;

                        string currString = attachedCardIndex < attachedCardGraphics[j].Length
                            ? attachedCardGraphics[j][attachedCardIndex]
                            : string.Empty;

                        lineBuilder.Append(RenderUtil.LeftJustifyString(currString, currBaseArea));
                    }
                }

                baseField[i] = lineBuilder.ToString();
            }

            return baseField;
        }

        /// <summary>
        /// Generates the field that lists the game statistics
        /// </summary>
        private string[] GenerateStatField()
        {
            //These are all the elements of the stat field
            List<string> statFieldElements =
            [
                $"{_game.CurrentTurn.ActivePlayer.Name}'s Turn",
                $"Minion Plays: {_game.CurrentTurn.MinionPlays}",
                $"Action Plays: {_game.CurrentTurn.ActionPlays}",
                $"VP: {_game.CurrentTurn.ActivePlayer.VictoryPoints}"
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
            string[][] GraphicsToDisplay = [];

            if (_battleService.IsInCardViewMode())
            {
                PlayableCard? selectedCard = _battleService.GetSelectedCard();

                // Card View Mode
                if (selectedCard != null)
                {
                    if(_battleService.SelectedFromHand())
                    {
                        GraphicsToDisplay = [selectedCard.GetPlayGraphic()];
                    }
                    else
                    {
                        GraphicsToDisplay = [selectedCard.GetGraphic(IsCardHighlighted(selectedCard))];
                    }
                }
            } 
            else
            {
                // Hand Display
                PlayableCard[] allCardsInHand = _game.CurrentTurn.ActivePlayer.Hand.ToArray();
                if (allCardsInHand.Length > 0)
                {
                    string[][] allGraphics = _game.CurrentTurn.ActivePlayer.Hand.Select(card => card.GetGraphic(IsCardHighlighted(card))).ToArray();

                    int cardLength = allGraphics.Max(graphic => graphic.Max(line => line.Length));
                    int numCardsToDisplay = Math.Min(fieldLength / (cardLength + 1), allCardsInHand.Length);

                    _handCardsDisplayed = allCardsInHand[..numCardsToDisplay];
                    _rightBuffer = allCardsInHand[numCardsToDisplay..];
                    GraphicsToDisplay = allGraphics[..numCardsToDisplay];
                }
            }

            int cardHeight = GraphicsToDisplay.Max(graphic => graphic.Length);
            string[] inputField = new string[cardHeight];

            for (int i = 0; i < cardHeight; i++)
            {
                StringBuilder lineBuilder = new();

                for (int j = 0; j < GraphicsToDisplay.Length; j++)
                {
                    lineBuilder.Append(GraphicsToDisplay[j][i]);
                    if (j < GraphicsToDisplay.Length - 1) lineBuilder.Append(' ');
                }

                inputField[i] = lineBuilder.ToString();
            }

            return inputField;
        }

        private bool IsCardHighlighted(PrimitiveCard card)
        {
            return _battleService.IsCardSelectedOrTargeted(card);
        }
    }
}
