using SmashUp.Frontend.Utilities;

namespace SmashUp.Frontend.Pages.Target;

internal class Select2DOption : TargetLogic
{
    //Each list represents a different base
    protected List<List<Guid>> _options;

    public Select2DOption(List<List<Guid>> options)
    {
        _options = options;
        _targetedOption = options.FirstOrDefault()?.FirstOrDefault();
    }

    protected override Guid? ExtendHandleKeyPress(UserKeyPress keyPress)
    {
        switch (keyPress)
        {
            case UserKeyPress.Left:
                int proposedXIndex = Math.Max(0, _cursorXIndex - 1);


                SetIndexToNextAvailable(proposedXIndex, _cursorYIndex, -1);
                _targetedOption = _options[_cursorXIndex][_cursorYIndex];
                break;

            case UserKeyPress.Right:

                proposedXIndex = Math.Min(_options.Count - 1, _cursorXIndex + 1);
                SetIndexToNextAvailable(proposedXIndex, _cursorYIndex, 1);
                _targetedOption = _options[_cursorXIndex][_cursorYIndex];
                break;

            case UserKeyPress.Up:
                if(_cursorYIndex == 0)
                {
                    TransferControlUp();
                } 
                else
                {
                    _cursorYIndex = _cursorYIndex - 1;
                    _targetedOption = _options[_cursorXIndex][_cursorYIndex];
                }
                
                break;

            case UserKeyPress.Down:
                //Only move if cursor is not at end of list
                if (_cursorYIndex != _options[_cursorXIndex].Count - 1)
                {
                    _cursorYIndex++;
                    _targetedOption = _options[_cursorXIndex][_cursorYIndex];
                } 
                else
                {
                    TransferControlDown();
                }
                break;

            case UserKeyPress.Confirm:
                return _options[_cursorXIndex][_cursorYIndex];
        }

        return null;
    }

    private void SetIndexToNextAvailable(int proposedXIndex, int proposedYIndex, int iterNum)
    {
        //Does the proposed base have enough cards?
        if (proposedYIndex < _options[proposedXIndex].Count)
        {
            _cursorXIndex = proposedXIndex;
            _cursorYIndex = proposedYIndex;
        }
        else
        {
            //Does the base have any cards?
            if (_options[proposedXIndex].Count != 0)
            {
                //Go to the next selectable card
                _cursorXIndex = proposedXIndex;
                _cursorYIndex = _options[proposedXIndex].Count - 1;
            }
            else
            {
                //Do we still have bases to our side?
                if (proposedXIndex + iterNum < _options.Count && proposedXIndex + iterNum > -1)
                {
                    //Try again one base to the side
                    SetIndexToNextAvailable(proposedXIndex + iterNum, proposedYIndex, iterNum);
                }
            }
        }
    }
}
