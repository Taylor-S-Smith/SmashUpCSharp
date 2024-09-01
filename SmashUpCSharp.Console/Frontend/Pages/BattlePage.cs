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
        PlayableCard[] LeftBuffer = [];
        PlayableCard[] HandCardsDisplayed = [];
        PlayableCard[] RightBuffer = [];

        //Field
        int SelectedXIndex = 0;
        int SelectedYIndex = -1;
        PrimitiveCard[][] SelectableFieldCards;

        PrimitiveCard? SelectedCard;

        // MODES
        bool InCardViewMode = false;


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
            SelectableFieldCards = Game.ActiveBases.Select(x => x.AttachedCards.Cast<PrimitiveCard>().ToArray()).ToArray();
            SelectedCard = Game.CurrentTurn.ActivePlayer.Hand[SelectedXIndex];
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
                .Select(activeBase => activeBase.GetGraphic(SelectedCard == activeBase))
                .ToArray();

            // Get Attached Cards Graphics
            string[][] attachedCardGraphics = Game.ActiveBases
                .Select(baseCard => baseCard.GetAttachedCardsGraphic(SelectedCard))
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
            string[][] GraphicsToDisplay = [];

            if (InCardViewMode)
            {
                if (SelectedCard != null)
                {
                    GraphicsToDisplay = [SelectedCard.GetGraphic(true)];
                }
            } 
            else
            {
                PlayableCard[] allCardsInHand = Game.CurrentTurn.ActivePlayer.Hand.ToArray();
                if (allCardsInHand.Length > 0)
                {
                    string[][] allGraphics = Game.CurrentTurn.ActivePlayer.Hand.Select(card => card.GetGraphic(SelectedCard == card)).ToArray();

                    int cardLength = allGraphics.Max(graphic => graphic.Max(line => line.Length));
                    int numCardsToDisplay = Math.Min(fieldLength / (cardLength + 1), allCardsInHand.Length);

                    HandCardsDisplayed = allCardsInHand[..numCardsToDisplay];
                    RightBuffer = allCardsInHand[numCardsToDisplay..];
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


        public override string? HandleKeyPress(UserKeyPress keyPress, ref bool stateChanged)
        {
            if(InCardViewMode)
            {
                switch (keyPress)
                {
                    case UserKeyPress.Escape:
                        ToggleCardViewMode();
                        stateChanged = true;
                        break;
                    default:
                        return null;
                }

            }
            else
            {
                switch (keyPress)
                {
                    case UserKeyPress.Left:
                        int proposedXIndex = Math.Max(0, SelectedXIndex - 1);

                        if (IsSelectionInHand())
                        {
                            SelectedXIndex = proposedXIndex;
                            SelectedCard = HandCardsDisplayed[SelectedXIndex];
                        }
                        else
                        {
                            SetIndexToNextAvailable(proposedXIndex, SelectedYIndex, -1);
                            SelectedCard = SelectableFieldCards[SelectedXIndex][SelectedYIndex];
                        }
                        stateChanged = true;
                        break;

                    case UserKeyPress.Right:
                        if (IsSelectionInHand())
                        {
                            SelectedXIndex = Math.Min(HandCardsDisplayed.Length - 1, SelectedXIndex + 1);
                            SelectedCard = HandCardsDisplayed[SelectedXIndex];
                        }
                        else
                        {
                            proposedXIndex = Math.Min(SelectableFieldCards.Length - 1, SelectedXIndex + 1);
                            SetIndexToNextAvailable(proposedXIndex, SelectedYIndex, 1);
                            SelectedCard = SelectableFieldCards[SelectedXIndex][SelectedYIndex];
                        }
                        stateChanged = true;
                        break;

                    case UserKeyPress.Up:
                        if (IsSelectionInHand())
                        {
                            //Go into base field

                            //Try getting closest to the right
                            SetIndexToNextAvailable(GetClosestIndexConversion(SelectedXIndex, HandCardsDisplayed.Length, SelectableFieldCards.Length), 99, 1);

                            //Did we get out?
                            if (IsSelectionInHand())
                            {
                                //Try getting closest to the left
                                proposedXIndex = GetClosestIndexConversion(SelectedXIndex, HandCardsDisplayed.Length - 1, SelectableFieldCards.Length - 1);
                                SetIndexToNextAvailable(proposedXIndex, 99, 1);
                            }

                            SelectedCard = SelectableFieldCards[SelectedXIndex][SelectedYIndex];
                        }
                        else
                        {
                            SelectedYIndex = Math.Max(0, SelectedYIndex - 1);
                            SelectedCard = SelectableFieldCards[SelectedXIndex][SelectedYIndex];
                        }
                        stateChanged = true;
                        break;

                    case UserKeyPress.Down:
                        if (!IsSelectionInHand())
                        {
                            //Is YIndex at end of list?
                            if (SelectedYIndex == SelectableFieldCards[SelectedXIndex].Length - 1)
                            {
                                //Go down to hand
                                SelectedXIndex = GetClosestIndexConversion(SelectedXIndex, SelectableFieldCards.Length - 1, HandCardsDisplayed.Length - 1);
                                SelectedYIndex = -1;
                                SelectedCard = HandCardsDisplayed[SelectedXIndex];
                            }
                            else
                            {
                                SelectedYIndex++;
                                SelectedCard = SelectableFieldCards[SelectedXIndex][SelectedYIndex];
                            }

                            stateChanged = true;
                        }
                        break;

                    case UserKeyPress.Confirm:
                        ToggleCardViewMode();
                        stateChanged = true;
                        break;
                    case UserKeyPress.Escape:
                        return "QUIT";
                    default:
                        return null;
                }
            }

            return null;
        }

        private void SetIndexToNextAvailable(int proposedXIndex, int proposedYIndex, int iterNum)
        {
            //Does the proposed base have enough cards?
            if (proposedYIndex < SelectableFieldCards[proposedXIndex].Length)
            {
                SelectedXIndex = proposedXIndex;
                SelectedYIndex = proposedYIndex;
            }
            else
            {
                //Does the base have any cards?
                if (SelectableFieldCards[proposedXIndex].Length != 0)
                {
                    //Go to the next selectable card
                    SelectedXIndex = proposedXIndex;
                    SelectedYIndex = SelectableFieldCards[proposedXIndex].Length - 1;
                }
                else
                {
                    //Do we still have bases to our side?
                    if (proposedXIndex > 0)
                    {
                        //Try again one base to the side
                        SetIndexToNextAvailable(proposedXIndex + iterNum, proposedYIndex, iterNum);
                    }
                }
            }
        }

        private int GetClosestIndexConversion(int index, int oldIndexBase, int newIndexBase)
        {
            //if (index == 0) return 0;
            //if (index == oldIndexBase) return newIndexBase;

            // Compute the proportion in base 10
            double proportion = (double)index / oldIndexBase;

            // Convert the proportion to the new base
            int mappedValue = (int)Math.Round(proportion * newIndexBase);

            return mappedValue;
        }

        private bool IsSelectionInHand()
        {
            return SelectedYIndex == -1;
        }
        private void ToggleCardViewMode()
        {
            InCardViewMode = !InCardViewMode;            
        }
    }
}
