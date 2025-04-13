using SmashUp.Backend.GameObjects;
using SmashUp.Backend.LogicServices;
using SmashUp.Backend.Models;
using SmashUp.Frontend.Utilities;
using System.Text;

namespace SmashUp.Frontend.Pages
{
    /// <summary>
    /// Handles all rendering the Battle page
    /// </summary>
    internal class BattlePage(BattlePageTargeter service) : ValuePage<bool>
    {
        private readonly BattlePageTargeter _service = service;

        // STATIC VARIABLES
        readonly int CARD_FIELD_SIZE = 15;

        // HAND
        List<PlayableCard> _handCardsDisplayed = [];

        protected override StringBuilder GenerateRender(int consoleWidth, int consoleHeight)
        {
            int baseFieldPadding = 1;
            int statFieldPadding = 1;
            int otherRenderGraphicsLength = 1;

            //Initalize vars and generate fields
            StringBuilder? renderBuffer = null;
            var debugField = GenerateDebugField();
            var baseField = GenerateBaseField(consoleWidth);
            var statField = GenerateStatField();
            var consoleField = GenerateInputField(new[] { baseField.Max(line => line.Length), statField.Max(line => line.Length) }.Max());
            var endTurnField = GenerateEndTurnField(baseField.Max(line => line.Length));

            //Ensure the current console size will fit the header
            int renderWidth = new[] {
                debugField.Max(line => line.Length),
                baseField.Max(line => line.Length),
                statField.Max(line => line.Length),
                consoleField.Length > 0 ? consoleField.Max(line => line.Length) : 0,
                endTurnField.Max(line => line.Length)
            }.Max();

            int renderHeight = debugField.Length +
                               baseField.Length +
                               baseFieldPadding +
                               statField.Length +
                               statFieldPadding +
                               consoleField.Length +
                               endTurnField.Length +
                               otherRenderGraphicsLength;

            //Make sure the whole render can fit in the console
            if (consoleHeight - 1 >= renderHeight && consoleWidth - 1 >= renderWidth)
            {
                //Generate final combined render
                string[] render = new string[renderHeight];
                int i = 0;
                foreach (string line in debugField)
                {
                    render[i++] = line;
                }
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
                foreach (string line in endTurnField)
                {
                    render[i++] = line;
                }

                renderBuffer = RenderUtil.Center(render, (consoleHeight - 1, consoleWidth - 1));
            }
            //Let the user know the screen is too small
            if (renderBuffer is null)
            {
                string[] render =
                [
                    $@"Please increase your screen size"
                ];
                renderBuffer = RenderUtil.Center(render, (consoleHeight - 1, consoleWidth - 1));
            }

            return renderBuffer;
        }

        /// <summary>
        /// Debug stats
        /// </summary>
        /// <returns></returns>
        private string[] GenerateDebugField()
        {
            (var x_index, var y_index, var targetableCards) = _service.GetDebugVals();

            StringBuilder targetableCardsString = new();
            for(int i = 0; i < targetableCards.Length; i++)
            {
                var baseField = targetableCards[i];
                targetableCardsString.Append("[");
                for (int j = 0; j < baseField.Length; j++)
                {
                    var card = baseField[j];
                    targetableCardsString.Append($"{card.Owner?.Name}'s {card.Name}, ");
                }

                targetableCardsString.Append("]");
            }

            return [$"(X: {x_index}, Y: {y_index}); TargetableFieldCards = {targetableCardsString.ToString()}"];
        }

        /// <summary>
        /// Generates the base graphics
        /// </summary>
        private string[] GenerateBaseField(int consoleWidth)
        {
            List<BaseSlot> slots = _service.GetBaseSlots();
            List<BaseCard> activeBases = slots.Select(x => x.BaseCard).ToList();

            // Get Active Bases Graphics
            string[][] activeBasesGraphics = activeBases
                .Select(GenerateBaseGraphic)
                .ToArray();

            // Get Attached Cards Graphics
            string[][] attachedCardGraphics = slots
                .Select(GetBaseAttachmentGraphics)
                .ToArray();

            if (activeBasesGraphics.Length != attachedCardGraphics.Length) throw new Exception($"Recieved {activeBasesGraphics.Length} active bases but only {attachedCardGraphics.Length} attached card graphics. They should be equal");

            // Calculate array size
            int baseGraphicHeight = activeBasesGraphics.Max(baseGraphic => baseGraphic.Length);
            int paddingHeight = 1;
            int numCardFieldLines = Math.Max(attachedCardGraphics.Max(x => x.Length), CARD_FIELD_SIZE);
            var baseField = new string[baseGraphicHeight + paddingHeight + numCardFieldLines];

            // Calculate padding
            int baseGraphicWidth = activeBasesGraphics.Max(baseGraphic => baseGraphic.Max(line => line.Length));
            int horizontalPaddingLength = (consoleWidth - 1 - (baseGraphicWidth * activeBases.Count)) / (activeBases.Count + 1);

            // Generate combined graphics
            for (int i = 0; i < baseField.Length; i++)
            {
                StringBuilder lineBuilder = new();

                for (int j = 0; j < activeBases.Count; j++)
                {
                    if (i < activeBasesGraphics[j].Length)
                    {
                        lineBuilder.Append(activeBasesGraphics[j][i]);

                        if (j < activeBases.Count - 1 && horizontalPaddingLength > 0)
                            lineBuilder.Append(' ', horizontalPaddingLength);
                    }
                    else if (i < activeBasesGraphics[j].Length + paddingHeight)
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

        private string[] GetBaseAttachmentGraphics(BaseSlot baseSlot)
        {
            return CardGraphicUtil.GetAttachedCardsGraphic(baseSlot, _service.GetTargetedPlayableCardId());
        }

        private string[] GenerateBaseGraphic(BaseCard baseCard)
        {
            bool isHighlighted = IsCardHighlighted(baseCard);
            return CardGraphicUtil.GenerateBaseCardGraphic(baseCard.Graphic, baseCard.Name, baseCard.CurrentPower, baseCard.CurrentBreakpoint, isHighlighted);
        }

        /// <summary>
        /// Generates the field that lists the game statistics
        /// </summary>
        private string[] GenerateStatField()
        {
            ActivePlayer activePlayer = _service.GetActivePlayer();
            //These are all the elements of the stat field
            List<string> statFieldElements =
            [
                $"{activePlayer.Player.Name}'s Turn",
                $"Minion Plays: {activePlayer.Player.MinionPlays}",
                $"Action Plays: {activePlayer.Player.ActionPlays}",
                $"VP: {activePlayer.Player.VictoryPoints}"
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

            if (_service.IsInCardViewMode())
            {
                PlayableCard? selectedCard = _service.GetSelectedCard();

                // Card View Mode
                if (selectedCard != null)
                {
                    if (_service.CursorInHand())
                    {
                        GraphicsToDisplay = [GetPlayGraphic(selectedCard)];
                    }
                    else
                    {
                        GraphicsToDisplay = [GeneratePlayableCardGraphic(selectedCard)];
                    }
                }
            }
            else
            {
                // Hand Display
                var hand = _service.GetCurrentPlayerHand().ToList();
                if (hand.Count > 0)
                {
                    string[][] allGraphics = hand.Select(GeneratePlayableCardGraphic).ToArray();

                    int cardLength = allGraphics.Max(graphic => graphic.Max(line => line.Length));
                    int numCardsToDisplay = Math.Min(fieldLength / (cardLength + 1), hand.Count);

                    _handCardsDisplayed = hand[..numCardsToDisplay];
                    GraphicsToDisplay = allGraphics[..numCardsToDisplay];
                }
            }

            int cardHeight = GraphicsToDisplay.Length > 0 ? GraphicsToDisplay.Max(graphic => graphic.Length) : 0;
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
        private string[] GeneratePlayableCardGraphic(PlayableCard card)
        {
            bool isHighlighted = IsCardHighlighted(card);
            return CardGraphicUtil.GeneratePlayableCardGraphic(card.Graphic, isHighlighted);
        }
        private string[] GetPlayGraphic(PlayableCard card)
        {
            string playText = $"Choose a base to play {card.Name} on";

            string[] graphic = GeneratePlayableCardGraphic(card);

            graphic[graphic.Length / 2] += $" {playText}";

            return graphic;
        }

        /// <summary>
        /// Generates the end turn button
        /// </summary>
        /// <returns></returns>
        private string[] GenerateEndTurnField(int lineWidth)
        {
            // Add End Button
            string button;
            if (_service.EndTurnSelected())
            {
                button = ">END TURN<";
            }
            else
            {
                button = "END TURN";
            }
            return [RenderUtil.CenterString(button, lineWidth)];
        }

        private bool IsCardHighlighted(object card)
        {
            return _service.IsCardSelectedOrTargeted(card);
        }

        public override bool? HandleKeyPress(UserKeyPress keyPress)
        {
            return _service.HandleKeyPress(keyPress, ref _needToRender, _handCardsDisplayed);
        }
    }
}
