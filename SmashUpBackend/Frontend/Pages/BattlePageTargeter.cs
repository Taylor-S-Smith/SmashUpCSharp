using SmashUp.Backend.GameObjects;
using SmashUp.Frontend.Utilities;
using SmashUp.Backend.Models;

namespace SmashUp.Backend.LogicServices
{
    /// <summary>
    /// Handles logic and state for the BattlePage including:
    /// - targeting/selecting
    /// - key presses
    /// - interacting with backend
    /// </summary>
    internal class BattlePageTargeter
    {
        private readonly Table _table;
        private readonly IBackendBattleAPI _battleAPI;

        // MODES
        bool InCardViewMode = false;

        // TARGET INDEXES
        int _targetedXIndex = 0;
        int _targetedYIndex = -1;

        BaseCard[] _targetableBaseCards;
        //First array is bases, second is cards
        PlayableCard[][] _targetableFieldCards;
       
        BaseCard? _targetedBaseCard;
        PlayableCard? _targetedPlayableCard;
        PlayableCard? _selectedPlayableCard;

        public BattlePageTargeter(Table table, IBackendBattleAPI battleAPI)
        {
            _table = table;
            _targetedPlayableCard = GetCurrentPlayerHand().FirstOrDefault();
            _targetableFieldCards = GetTargetableFieldCards(AllCardsAreTargetable);
            _targetableBaseCards = GetTargetableBaseCards(AllCardsAreTargetable);
            _battleAPI = battleAPI;

            if(_table.ActivePlayer.Player.Hand.Count == 0)
            {
                _targetedYIndex = -2;
            }
        }


        public PlayableCard? GetSelectedCard()
        {
            return _selectedPlayableCard;
        }
        public BaseCard? GetTargetedBaseCard()
        {
            return _targetedBaseCard;
        }
        public Guid? GetTargetedPlayableCardId()
        {
            return _targetedPlayableCard?.Id;
        }
        public bool? HandleKeyPress(UserKeyPress keyPress, ref bool _needToRender, List<PlayableCard> interactableHandCards)
        {
            if (InCardViewMode)
            {
                if (CursorInHand())
                {
                    //Base Target Mode
                    return HandleBaseTargetMode(keyPress, ref _needToRender, interactableHandCards);
                }
                else
                {
                    //Static View Mode
                    return HandleStaticViewMode(keyPress, ref _needToRender, interactableHandCards);
                }

            }
            else
            {
                //Playable Card Target Mode (Default)
                return HandlePlayableCardTargetMode(keyPress, ref _needToRender, interactableHandCards);
            }
        }
        public bool IsCardSelectedOrTargeted(object card)
        {
            return card == _targetedPlayableCard || card == _targetedBaseCard || card == _selectedPlayableCard;
        }
        public bool IsInCardViewMode()
        {
            return InCardViewMode;
        }
        public bool CursorInHand()
        {
            return _targetedYIndex == -1;
        }
        private bool CursorOnEndTurnButton()
        {
            return _targetedYIndex == -2;
        }
        public bool CurserInInputArea()
        {
            return _targetedYIndex < 0;
        }



        // KEYPRESS
        private bool? HandlePlayableCardTargetMode(UserKeyPress keyPress, ref bool needToRender, List<PlayableCard> interactableHandCards)
        {
            switch (keyPress)
            {

                case UserKeyPress.Left:
                    int proposedXIndex = Math.Max(0, _targetedXIndex - 1);

                    if (CursorInHand())
                    {
                        _targetedXIndex = proposedXIndex;
                        _targetedPlayableCard = interactableHandCards[_targetedXIndex];
                    }
                    else if(!CurserInInputArea())
                    {
                        SetIndexToNextAvailable(proposedXIndex, _targetedYIndex, -1);
                        _targetedPlayableCard = _targetableFieldCards[_targetedXIndex][_targetedYIndex];
                    }
                    needToRender = true;
                    break;

                case UserKeyPress.Right:
                    if (CursorInHand())
                    {
                        _targetedXIndex = Math.Min(interactableHandCards.Count - 1, _targetedXIndex + 1);
                        _targetedPlayableCard = interactableHandCards[_targetedXIndex];
                    }
                    else if (!CurserInInputArea())
                    {
                        proposedXIndex = Math.Min(_targetableFieldCards.Length - 1, _targetedXIndex + 1);
                        SetIndexToNextAvailable(proposedXIndex, _targetedYIndex, 1);
                        _targetedPlayableCard = _targetableFieldCards[_targetedXIndex][_targetedYIndex];
                    }
                    needToRender = true;
                    break;

                case UserKeyPress.Up:
                    if (CursorOnEndTurnButton())
                    {
                        //Go into hand
                        _targetedYIndex = -1;

                        if (_table.ActivePlayer.Player.Hand.Count > 0)
                        {
                            _targetedPlayableCard = interactableHandCards[_targetedXIndex];
                        } else
                        {
                            // Check if there are any targetable cards
                            if (_targetableFieldCards.Sum(x => x.Length) == 0)
                            {
                                break;
                            }
                            //Go into base field

                            //Try getting closest to the right
                            SetIndexToNextAvailable(GetClosestIndexConversion(_targetedXIndex, interactableHandCards.Count, _targetableFieldCards.Length), 99, 1);

                            //Did we get out?
                            if (CursorInHand())
                            {
                                //Try getting closest to the left
                                proposedXIndex = GetClosestIndexConversion(_targetedXIndex, interactableHandCards.Count - 1, _targetableFieldCards.Length - 1);
                                SetIndexToNextAvailable(proposedXIndex, 99, 1);
                            }

                            _targetedPlayableCard = _targetableFieldCards[_targetedXIndex][_targetedYIndex];
                        }

                        needToRender = true;
                    }
                    else if (CursorInHand())
                    {
                        // Check if there are any targetable cards
                        if(_targetableFieldCards.Sum(x => x.Length) == 0)
                        {
                            break;
                        }
                        //Go into base field

                        //Try getting closest to the right
                        SetIndexToNextAvailable(GetClosestIndexConversion(_targetedXIndex, interactableHandCards.Count, _targetableFieldCards.Length), 99, 1);

                        //Did we get out?
                        if (CursorInHand())
                        {
                            //Try getting closest to the left
                            proposedXIndex = GetClosestIndexConversion(_targetedXIndex, interactableHandCards.Count - 1, _targetableFieldCards.Length - 1);
                            SetIndexToNextAvailable(proposedXIndex, 99, -1);
                        }

                        _targetedPlayableCard = _targetableFieldCards[_targetedXIndex][_targetedYIndex];
                    } 
                    else 
                    {
                        _targetedYIndex = Math.Max(0, _targetedYIndex - 1);
                        _targetedPlayableCard = _targetableFieldCards[_targetedXIndex][_targetedYIndex];
                    }
                    needToRender = true;
                    break;

                case UserKeyPress.Down:
                    if (!CurserInInputArea())
                    {
                        //Is YIndex at end of list?
                        if (_targetedYIndex == _targetableFieldCards[_targetedXIndex].Length - 1)
                        {
                            //Go down to hand
                            if(interactableHandCards.Count > 0)
                            {
                                _targetedXIndex = GetClosestIndexConversion(_targetedXIndex, _targetableFieldCards.Length - 1, interactableHandCards.Count - 1);
                                _targetedYIndex = -1;
                                _targetedPlayableCard = interactableHandCards[_targetedXIndex];
                            } else
                            {
                                TargetEndTurnButton(ref needToRender);
                            }
                        }
                        else
                        {
                            _targetedYIndex++;
                            _targetedPlayableCard = _targetableFieldCards[_targetedXIndex][_targetedYIndex];
                        }

                        needToRender = true;
                    }
                    else if(CursorInHand())
                    {
                        TargetEndTurnButton(ref needToRender);
                    }

                    break;

                case UserKeyPress.Confirm:
                    if (CursorOnEndTurnButton())
                    {
                        return true;
                    }
                    EnterCardSelectMode();
                    needToRender = true;
                    break;

                default:
                    return null;
            }

            return null;
        }

        private void TargetEndTurnButton(ref bool needToRender)
        {
            _targetedPlayableCard = null;
            _targetedYIndex = -2;
            needToRender = true;
        }

        private bool? HandleStaticViewMode(UserKeyPress keyPress, ref bool _needToRender, List<PlayableCard> interactableHandCards)
        {
            switch (keyPress)
            {
                case UserKeyPress.Escape:
                    _targetedPlayableCard = _selectedPlayableCard;
                    _selectedPlayableCard = null;
                    InCardViewMode = false;
                    _needToRender = true;
                    break;

                default:
                    return null;
            }

            return null;
        }
        private bool? HandleBaseTargetMode(UserKeyPress keyPress, ref bool _needToRender, List<PlayableCard> interactableHandCards)
        {
            switch (keyPress)
            {
                case UserKeyPress.Left:
                    _targetedXIndex = Math.Max(0, _targetedXIndex - 1);
                    _targetedBaseCard = _targetableBaseCards[_targetedXIndex];
                    _needToRender = true;
                    break;

                case UserKeyPress.Right:
                    _targetedXIndex = Math.Min(_targetableBaseCards.Length - 1, _targetedXIndex + 1);
                    _targetedBaseCard = _targetableBaseCards[_targetedXIndex];
                    _needToRender = true;
                    break;

                case UserKeyPress.Confirm:
                    if (_selectedPlayableCard == null) throw new Exception("No card selected");
                    if (_targetedBaseCard == null) throw new Exception("No base selected");
                    _battleAPI.PlayCard(_table.ActivePlayer.Player, _selectedPlayableCard, _targetedBaseCard);
                    _targetableFieldCards = GetTargetableFieldCards(AllCardsAreTargetable);
                    _targetedXIndex = 0;
                    IReadOnlyList<PlayableCard> PlayerHand = GetCurrentPlayerHand();
                    if(PlayerHand.Count > 0)
                    {
                        _targetedYIndex = -1;
                        _targetedPlayableCard =  PlayerHand[_targetedXIndex];
                    }
                    else
                    {
                        _targetedYIndex = -2;
                        _targetedPlayableCard = null;
                    }
                    _targetedBaseCard = null;
                    _selectedPlayableCard = null;
                    InCardViewMode = false;
                    _needToRender = true;
                    break;

                case UserKeyPress.Escape:
                    if (CursorInHand())
                    {
                        //Return target index from base to what they were targeting in hand
                        _targetedBaseCard = null;
                        _targetedXIndex = interactableHandCards.FindIndex(x => x == _selectedPlayableCard);
                    }
                    _targetedPlayableCard = _selectedPlayableCard;
                    _selectedPlayableCard = null;
                    InCardViewMode = false;
                    _needToRender = true;
                    break;

                default:
                    return null;
            }

            return null;
        }

        // MODE
        private void EnterCardSelectMode()
        {
            if (CursorInHand())
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
            if(oldIndexBase == 0)
            {
                return 0;
            }
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
                    if (proposedXIndex + iterNum < _targetableFieldCards.Length && proposedXIndex + iterNum > -1)
                    {
                        //Try again one base to the side
                        SetIndexToNextAvailable(proposedXIndex + iterNum, proposedYIndex, iterNum);
                    }
                }
            }
        }

        // TARGET
        private BaseCard[] GetTargetableBaseCards(Func<BaseCard, bool> isTargetable)
        {
            return _table.GetActiveBases().Where(x => isTargetable(x)).ToArray();
        }
        private PlayableCard[][] GetTargetableFieldCards(Func<PlayableCard, bool> isTargetable)
        {
            return _table.GetBaseSlots().Select(x => x.Territories.SelectMany(t => t.Cards).Where(attachedCard => isTargetable(attachedCard)).ToArray()).ToArray();
        }

        // CARD FILTER FUNC
        private readonly Func<object, bool> AllCardsAreTargetable = card => true;
        private static Func<PlayableCard, bool> TargetCardsOfPower(int power)
        {
            return (card) => card.CurrentPower <= power;
        }

        internal IReadOnlyList<PlayableCard> GetCurrentPlayerHand()
        {
            return _table.ActivePlayer.Player.Hand;
        }

        internal ActivePlayer GetActivePlayer()
        {
            return _table.ActivePlayer;
        }
        internal List<BaseSlot> GetBaseSlots()
        {
            return _table.GetBaseSlots();
        }

        internal bool EndTurnSelected()
        {
            return _targetedYIndex == -2;
        }

        internal (int, int, PlayableCard[][]) GetDebugVals()
        {
            return (_targetedXIndex, _targetedYIndex, _targetableFieldCards);
        }
    }
}
