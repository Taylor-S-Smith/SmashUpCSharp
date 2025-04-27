using SmashUp.Frontend.Utilities;
namespace SmashUp.Frontend.Pages.Target;

internal class SelectOption : TargetLogic
{
    protected readonly List<Guid> _options;

    public SelectOption(List<Guid> options)
    {
        _options = options;
        _targetedOption = options.FirstOrDefault();
    }

    protected override Guid? ExtendHandleKeyPress(UserKeyPress keyPress)
    {
        switch (keyPress)
        {
            case UserKeyPress.Left:
                _cursorXIndex = Math.Max(0, _cursorXIndex - 1);
                _targetedOption = _options[_cursorXIndex];
                break;

            case UserKeyPress.Right:
                _cursorXIndex = Math.Min(_options.Count - 1, _cursorXIndex + 1);
                _targetedOption = _options[_cursorXIndex];
                break;

            case UserKeyPress.Down:
                TransferControlDown.Invoke();
                break;

            case UserKeyPress.Up:
                TransferControlUp.Invoke();
                break;

            case UserKeyPress.Confirm:
                return _targetedOption;
        }

        return null;
    }
}
