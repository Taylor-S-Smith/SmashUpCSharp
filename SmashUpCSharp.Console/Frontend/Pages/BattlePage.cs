using SmashUp.Frontend.Utilities;
using SmashUp.Backend.Services;
using SmashUp.Models.Games;
using Models.Player;
using Models.Cards;
using System.Text;

namespace SmashUp.Frontend.Pages
{
    internal class BattlePage : PrimitivePage
    {
        // STATIC VARIABLES
        readonly int CARD_FIELD_SIZE = 15;

        // SERVICES
        readonly IBaseService _baseService;
        readonly IFactionService _factionService;
        readonly IPlayableCardService _playableCardService;
        readonly IPlayerService _playerService;

        // GAME OBJECTS
        private Battle Game { get; set; }

        // USER INPUT AREA

        //Hand
        List<PlayableCard> LeftBuffer = [];
        List<PlayableCard> CardsDisplayed = [];
        List<PlayableCard> RightBuffer = [];

        //Field
        int SelectedIndex = 0;
        PrimitiveCard[][] SelectableFieldCards;

        PlayableCard SelectedHandCard;
        PrimitiveCard SelectedFieldCard;

        PrimitiveCard ActiveSelectedCard;


        public BattlePage(IBaseService baseService, IFactionService factionService, IPlayableCardService playableCardService, IPlayerService playerService)
        {
            //JUST TO TEST

            _baseService = baseService;
            _factionService = factionService;
            _playableCardService = playableCardService;
            _playerService = playerService;

            Random testRandom = new();

            List<PlayableCard> testCards =
            [
                _playableCardService.Get(testRandom.Next(93)),
                _playableCardService.Get(testRandom.Next(93)),
                _playableCardService.Get(testRandom.Next(93))
            ];

            List<Faction> Factions1 = [_factionService.Get(testRandom.Next(8)), _factionService.Get(testRandom.Next(8))];
            List<Faction> Factions2 = [_factionService.Get(testRandom.Next(8)), _factionService.Get(testRandom.Next(8))];

            var deckCards = _playableCardService.Get(Factions1);
            _playerService.Create(new HumanPlayer("Taylor", Factions1, deckCards));
            deckCards = _playableCardService.Get(Factions2);
            _playerService.Create(new HumanPlayer("Andrew", Factions2, deckCards));

            var players = _playerService.GetAll();
            var bases = _baseService.Get(players.SelectMany(x => x.Factions).ToList());

            Game = new(players, bases);

            Game.ActiveBases[0].AttachCard(testCards[0]);
            Game.ActiveBases[1].AttachCard(testCards[1]);
            Game.ActiveBases[2].AttachCard(testCards[2]);


            //Initialize Selections
            //SelectableFieldCards = Game.ActiveBases.Select(x => x.GetAttachedCardsGraphic)
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

        /// <summary>
        /// Generates the base graphics
        /// </summary>
        private string[] GenerateBaseField(int consoleWidth)
        {
            int numBases = Game.ActiveBases.Count;

            // Get Active Bases Graphics
            string[][] activeBasesGraphics = Game.ActiveBases
                .Select(activeBase => activeBase.GetGraphic(ActiveSelectedCard == activeBase))
                .ToArray();

            // Get Attached Cards Graphics
            string[][] attachedCardGraphics = Game.ActiveBases
                .Select(baseCard => baseCard.GetAttachedCardsGraphic())
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
            List<PlayableCard> allCardsInHand = Game.CurrentTurn.ActivePlayer.Hand;

            string[] inputField = [""];
            if (allCardsInHand.Count > 0)
            {
                string[][] graphics = Game.CurrentTurn.ActivePlayer.Hand.Select(card => card.GetGraphic(ActiveSelectedCard == card)).ToArray();
                int cardHeight = graphics.Max(graphic => graphic.Length);
                int cardLength = graphics.Max(graphic => graphic.Max(line => line.Length));

                inputField = new string[cardHeight];

                int numCardsToDisplay = Math.Min(fieldLength/(cardLength+1), allCardsInHand.Count);

                CardsDisplayed = allCardsInHand[..numCardsToDisplay];
                RightBuffer = allCardsInHand[numCardsToDisplay..];

                for (int i = 0; i < cardHeight; i++)
                {
                    StringBuilder lineBuilder = new();

                    for (int j = 0; j < numCardsToDisplay; j++)
                    {
                        var cardToDisplay = CardsDisplayed[j];
                        
                        lineBuilder.Append(graphics[j][i]);
                        if (j < numCardsToDisplay - 1) lineBuilder.Append(' ');
                    }

                    inputField[i] = lineBuilder.ToString();
                }
            }
            
            return inputField;
        }


        public override string? ChangeState(UserKeyPress keyPress, ref bool stateChanged)
        {
            switch (keyPress)
            {
                case UserKeyPress.Left:
                    SelectedIndex = Math.Max(0, SelectedIndex - 1);
                    stateChanged = true;
                    break;
                case UserKeyPress.Right:
                    SelectedIndex = Math.Min(CardsDisplayed.Count - 1, SelectedIndex + 1);
                    stateChanged = true;
                    break;
                case UserKeyPress.Escape:
                    return "QUIT";
                default:
                    return null;
            }

            return null;
        }
    }
}
