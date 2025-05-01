
/// <summary>
/// This is to keep code that I may need in the near future
/// </summary>
internal class ARCHIVE
{
    /// <summary>
    /// This has the UI handling when switching between UI layers
    /// </summary>
    private T? HandlePlayableCardTargetMode(UserKeyPress keyPress, ref bool needToRender, List<PlayableCard> interactableHandCards)
    {
        switch (keyPress)
        {

            case UserKeyPress.Left:
                int proposedXIndex = Math.Max(0, _cursorXIndex - 1);

                if (CursorInHand())
                {
                    _cursorXIndex = proposedXIndex;
                    _targetedPlayableCard = interactableHandCards[_cursorXIndex];
                }
                else if (!CurserInInputArea())
                {
                    SetIndexToNextAvailable(proposedXIndex, _cursorYIndex, -1);
                    _targetedPlayableCard = _targets.PlayableCards[_cursorXIndex][_cursorYIndex];
                }
                needToRender = true;
                break;

            case UserKeyPress.Right:
                if (CursorInHand())
                {
                    _cursorXIndex = Math.Min(interactableHandCards.Count - 1, _cursorXIndex + 1);
                    _targetedPlayableCard = interactableHandCards[_cursorXIndex];
                }
                else if (!CurserInInputArea())
                {
                    proposedXIndex = Math.Min(_targets.PlayableCards.Count - 1, _cursorXIndex + 1);
                    SetIndexToNextAvailable(proposedXIndex, _cursorYIndex, 1);
                    _targetedPlayableCard = _targets.PlayableCards[_cursorXIndex][_cursorYIndex];
                }
                needToRender = true;
                break;

            case UserKeyPress.Up:
                if (CursorOnEndTurnButton())
                {
                    //Go into hand
                    _cursorYIndex = -1;

                    if (_table.ActivePlayer.Player.Hand.Count > 0)
                    {
                        _targetedPlayableCard = interactableHandCards[_cursorXIndex];
                    }
                    else
                    {
                        // Check if there are any targetable cards
                        if (_targets.PlayableCards.Sum(x => x.Count) == 0)
                        {
                            break;
                        }
                        //Go into base field

                        //Try getting closest to the right
                        SetIndexToNextAvailable(GetClosestIndexConversion(_cursorXIndex, interactableHandCards.Count, _targets.PlayableCards.Count), 99, 1);

                        //Did we get out?
                        if (CursorInHand())
                        {
                            //Try getting closest to the left
                            proposedXIndex = GetClosestIndexConversion(_cursorXIndex, interactableHandCards.Count - 1, _targets.PlayableCards.Count - 1);
                            SetIndexToNextAvailable(proposedXIndex, 99, 1);
                        }

                        _targetedPlayableCard = _targets.PlayableCards[_cursorXIndex][_cursorYIndex];
                    }

                    needToRender = true;
                }
                else if (CursorInHand())
                {
                    // Check if there are any targetable cards
                    if (_targets.PlayableCards.Sum(x => x.Count) == 0)
                    {
                        break;
                    }
                    //Go into base field

                    //Try getting closest to the right
                    SetIndexToNextAvailable(GetClosestIndexConversion(_cursorXIndex, interactableHandCards.Count, _targets.PlayableCards.Count), 99, 1);

                    //Did we get out?
                    if (CursorInHand())
                    {
                        //Try getting closest to the left
                        proposedXIndex = GetClosestIndexConversion(_cursorXIndex, interactableHandCards.Count - 1, _targets.PlayableCards.Count - 1);
                        SetIndexToNextAvailable(proposedXIndex, 99, -1);
                    }

                    _targetedPlayableCard = _targets.PlayableCards[_cursorXIndex][_cursorYIndex];
                }
                else
                {
                    _cursorYIndex = Math.Max(0, _cursorYIndex - 1);
                    _targetedPlayableCard = _targets.PlayableCards[_cursorXIndex][_cursorYIndex];
                }
                needToRender = true;
                break;

            case UserKeyPress.Down:
                if (!CurserInInputArea())
                {
                    //Is YIndex at end of list?
                    if (_cursorYIndex == _targets.PlayableCards[_cursorXIndex].Count - 1)
                    {
                        //Go down to hand
                        if (interactableHandCards.Count > 0)
                        {
                            _cursorXIndex = GetClosestIndexConversion(_cursorXIndex, _targets.PlayableCards.Count - 1, interactableHandCards.Count - 1);
                            _cursorYIndex = -1;
                            _targetedPlayableCard = interactableHandCards[_cursorXIndex];
                        }
                        else
                        {
                            TargetEndTurnButton(ref needToRender);
                        }
                    }
                    else
                    {
                        _cursorYIndex++;
                        _targetedPlayableCard = _targets.PlayableCards[_cursorXIndex][_cursorYIndex];
                    }

                    needToRender = true;
                }
                else if (CursorInHand())
                {
                    TargetEndTurnButton(ref needToRender);
                }

                break;

            case UserKeyPress.Confirm:
                if (CursorOnEndTurnButton())
                {
                    return null;
                }
                EnterCardSelectMode();
                needToRender = true;
                break;

            case UserKeyPress.Escape:
                if (DEBUG_MODE == true)
                {
                    Console.WriteLine("Enter Command: ");
                    var input = Console.ReadLine();
                    if (input == "add plays")
                    {
                        _table.ActivePlayer.Player.MinionPlays = 100;
                        _table.ActivePlayer.Player.ActionPlays = 100;
                    }

                    ResetToDefault(ref needToRender);
                }
                break;

            default:
                return null;
        }

        return null;
    }
    
    /// <summary>
    /// When switching UI layers, this is what converts the indexes to make it more seamless
    /// </summary>
    private static int GetClosestIndexConversion(int index, int oldIndexBase, int newIndexBase)
    {
        if (oldIndexBase == 0)
        {
            return 0;
        }
        double proportion = (double)index / oldIndexBase;
        int mappedValue = (int)Math.Round(proportion * newIndexBase);

        return mappedValue;
    }
}

