using SmashUp.Frontend.Utilities;
using SmashUp.Models.Games;
using Models.Cards;

namespace SmashUp.Backend.LogicServices
{
    /// <summary>
    /// Handles targeting/selecting for the battle page
    /// </summary>
    /// <summary>
    /// Handles key presses for the battle page
    /// </summary>
    internal class BattlePageService : IBattlePageService
    {
        // MODES
        bool InCardViewMode = false;

        // TARGET INDEXES
        int _targetedXIndex = 0;
        int _targetedYIndex = -1;

        BaseCard[] _targetableBaseCards;
        PlayableCard[][] _targetableFieldCards;

        BaseCard? _targetedBaseCard;
        PlayableCard? _targetedPlayableCard;
        PlayableCard? _selectedPlayableCard;

        // SERVICES
        readonly IGameService _gameService;

        // GAME
        readonly Game _game;

        public BattlePageService(IGameService gameService)
        {
            _gameService = gameService;

            _gameService.StartTestGame();
            _game = _gameService.GetCurrentGame();

            _targetedPlayableCard = _game.CurrentTurn.ActivePlayer.Hand[_targetedXIndex];
            _targetableFieldCards = GetTargetableFieldCards(AllCardsAreTargetable);
            _targetableBaseCards = GetTargetableBaseCards(AllCardsAreTargetable);
        }


        public PlayableCard? GetSelectedCard()
        {
            return _selectedPlayableCard;
        }
        public BaseCard? GetTargetedBaseCard()
        {
            return _targetedBaseCard;
        }
        public PlayableCard? GetTargetedPlayableCard()
        {
            return _targetedPlayableCard;
        }
        public string? HandleKeyPress(UserKeyPress keyPress, ref bool stateChanged, PlayableCard[] interactableHandCards)
        {
            if (InCardViewMode)
            {
                if (SelectedFromHand())
                {
                    //Base Target Mode
                    return HandleBaseTargetMode(keyPress, ref stateChanged, interactableHandCards);
                }
                else
                {
                    //Static View Mode
                    return HandleStaticViewMode(keyPress, ref stateChanged, interactableHandCards);
                }

            }
            else
            {
                //Playable Card Target Mode (Default)
                return HandlePlayableCardTargetMode(keyPress, ref stateChanged, interactableHandCards);
            }
        }
        public bool IsCardSelectedOrTargeted(PrimitiveCard card)
        {
            return card == _targetedPlayableCard || card == _targetedBaseCard || card == _selectedPlayableCard;
        }
        public bool IsInCardViewMode()
        {
            return InCardViewMode;
        }
        public bool SelectedFromHand()
        {
            return _targetedYIndex == -1;
        }


        // KEYPRESS
        private string? HandlePlayableCardTargetMode(UserKeyPress keyPress, ref bool stateChanged, PlayableCard[] interactableHandCards)
        {
            switch (keyPress)
            {

                case UserKeyPress.Left:
                    int proposedXIndex = Math.Max(0, _targetedXIndex - 1);

                    if (SelectedFromHand())
                    {
                        _targetedXIndex = proposedXIndex;
                        _targetedPlayableCard = interactableHandCards[_targetedXIndex];
                    }
                    else
                    {
                        SetIndexToNextAvailable(proposedXIndex, _targetedYIndex, -1);
                        _targetedPlayableCard = _targetableFieldCards[_targetedXIndex][_targetedYIndex];
                    }
                    stateChanged = true;
                    break;

                case UserKeyPress.Right:
                    if (SelectedFromHand())
                    {
                        _targetedXIndex = Math.Min(interactableHandCards.Length - 1, _targetedXIndex + 1);
                        _targetedPlayableCard = interactableHandCards[_targetedXIndex];
                    }
                    else
                    {
                        proposedXIndex = Math.Min(_targetableFieldCards.Length - 1, _targetedXIndex + 1);
                        SetIndexToNextAvailable(proposedXIndex, _targetedYIndex, 1);
                        _targetedPlayableCard = _targetableFieldCards[_targetedXIndex][_targetedYIndex];
                    }
                    stateChanged = true;
                    break;

                case UserKeyPress.Up:
                    if (SelectedFromHand())
                    {
                        //Go into base field

                        //Try getting closest to the right
                        SetIndexToNextAvailable(GetClosestIndexConversion(_targetedXIndex, interactableHandCards.Length, _targetableFieldCards.Length), 99, 1);

                        //Did we get out?
                        if (SelectedFromHand())
                        {
                            //Try getting closest to the left
                            proposedXIndex = GetClosestIndexConversion(_targetedXIndex, interactableHandCards.Length - 1, _targetableFieldCards.Length - 1);
                            SetIndexToNextAvailable(proposedXIndex, 99, 1);
                        }

                        _targetedPlayableCard = _targetableFieldCards[_targetedXIndex][_targetedYIndex];
                    }
                    else
                    {
                        _targetedYIndex = Math.Max(0, _targetedYIndex - 1);
                        _targetedPlayableCard = _targetableFieldCards[_targetedXIndex][_targetedYIndex];
                    }
                    stateChanged = true;
                    break;

                case UserKeyPress.Down:
                    if (!SelectedFromHand())
                    {
                        //Is YIndex at end of list?
                        if (_targetedYIndex == _targetableFieldCards[_targetedXIndex].Length - 1)
                        {
                            //Go down to hand
                            _targetedXIndex = GetClosestIndexConversion(_targetedXIndex, _targetableFieldCards.Length - 1, interactableHandCards.Length - 1);
                            _targetedYIndex = -1;
                            _targetedPlayableCard = interactableHandCards[_targetedXIndex];
                        }
                        else
                        {
                            _targetedYIndex++;
                            _targetedPlayableCard = _targetableFieldCards[_targetedXIndex][_targetedYIndex];
                        }

                        stateChanged = true;
                    }
                    break;

                case UserKeyPress.Confirm:
                    EnterCardSelectMode();
                    stateChanged = true;
                    break;

                case UserKeyPress.Escape:
                    return "QUIT";

                default:
                    return null;
            }

            return null;
        }
        private string? HandleStaticViewMode(UserKeyPress keyPress, ref bool stateChanged, PlayableCard[] interactableHandCards)
        {
            switch (keyPress)
            {
                case UserKeyPress.Escape:
                    _targetedPlayableCard = _selectedPlayableCard;
                    _selectedPlayableCard = null;
                    InCardViewMode = false;
                    stateChanged = true;
                    break;

                default:
                    return null;
            }

            return null;
        }
        private string? HandleBaseTargetMode(UserKeyPress keyPress, ref bool stateChanged, PlayableCard[] interactableHandCards)
        {
            switch (keyPress)
            {
                case UserKeyPress.Left:
                    _targetedXIndex = Math.Max(0, _targetedXIndex - 1);
                    _targetedBaseCard = _targetableBaseCards[_targetedXIndex];
                    stateChanged = true;
                    break;

                case UserKeyPress.Right:
                    _targetedXIndex = Math.Min(_targetableBaseCards.Length - 1, _targetedXIndex + 1);
                    _targetedBaseCard = _targetableBaseCards[_targetedXIndex];
                    stateChanged = true;
                    break;

                case UserKeyPress.Confirm:
                    if (_selectedPlayableCard == null) throw new Exception("No card selected");
                    if (_targetedBaseCard == null) throw new Exception("No base selected");
                    _game.CurrentTurn.ActivePlayer.PlayCard(_selectedPlayableCard, _targetedBaseCard);
                    _targetableFieldCards = GetTargetableFieldCards(AllCardsAreTargetable);
                    _targetedXIndex = 0;
                    _targetedYIndex = -1;
                    _targetedPlayableCard = _game.CurrentTurn.ActivePlayer.Hand[_targetedXIndex];
                    _selectedPlayableCard = null;
                    InCardViewMode = false;
                    stateChanged = true;
                    break;

                case UserKeyPress.Escape:
                    if (SelectedFromHand())
                    {
                        //Return target index from base to what they were targeting in hand
                        _targetedBaseCard = null;
                        _targetedXIndex = Array.IndexOf(interactableHandCards, _selectedPlayableCard);
                    }
                    _targetedPlayableCard = _selectedPlayableCard;
                    _selectedPlayableCard = null;
                    InCardViewMode = false;
                    stateChanged = true;
                    break;

                default:
                    return null;
            }

            return null;
        }

        // MODE
        private void EnterCardSelectMode()
        {
            if (SelectedFromHand())
            {
                _selectedPlayableCard = _targetedPlayableCard;
                _targetedPlayableCard = null;
                _targetableBaseCards = GetTargetableBaseCards(AllCardsAreTargetable);
                _targetedXIndex = 0;
                _targetedBaseCard = _targetableBaseCards[_targetedXIndex];
            }
            else
            {
                _targetableFieldCards = GetTargetableFieldCards(AllCardsAreTargetable);
                _selectedPlayableCard = _targetableFieldCards[_targetedXIndex][_targetedYIndex];
            }

            InCardViewMode = true;
        }
        private static int GetClosestIndexConversion(int index, int oldIndexBase, int newIndexBase)
        {
            double proportion = (double)index / oldIndexBase;
            int mappedValue = (int)Math.Round(proportion * newIndexBase);

            return mappedValue;
        }
        private void SetIndexToNextAvailable(int proposedXIndex, int proposedYIndex, int iterNum)
        {
            //Does the proposed base have enough cards?
            if (proposedYIndex < _targetableFieldCards[proposedXIndex].Length)
            {
                _targetedXIndex = proposedXIndex;
                _targetedYIndex = proposedYIndex;
            }
            else
            {
                //Does the base have any cards?
                if (_targetableFieldCards[proposedXIndex].Length != 0)
                {
                    //Go to the next selectable card
                    _targetedXIndex = proposedXIndex;
                    _targetedYIndex = _targetableFieldCards[proposedXIndex].Length - 1;
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

        // TARGET
        private BaseCard[] GetTargetableBaseCards(Func<PrimitiveCard, bool> isTargetable)
        {
            return _game.ActiveBases.Where(x => isTargetable(x)).ToArray();
        }
        private PlayableCard[][] GetTargetableFieldCards(Func<PrimitiveCard, bool> isTargetable)
        {
            return _game.ActiveBases.Select(x => x.AttachedCards.Where(attachedCard => isTargetable(attachedCard)).ToArray()).ToArray();
        }

        // CARD FILTER FUNC
        private readonly Func<PrimitiveCard, bool> AllCardsAreTargetable = card => true;
        private static Func<PlayableCard, bool> TargetCardsOfPower(int power)
        {
            return (card) => card.CurrentPower <= power;
        }
    }
}
