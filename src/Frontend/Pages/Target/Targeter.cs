using SmashUp.Frontend.Utilities;

namespace SmashUp.Frontend.Pages.Target;
internal class Targeter
{
    private List<TargetLogic> _targetLogics;

    private TargetLogic _logicInControl;

    public Targeter(List<TargetLogic> targetLogics, TargetLogic? startingLogic = null)
    {
        _targetLogics = targetLogics;

        if (targetLogics.Count == 0)
        {
            _logicInControl = new DefaultLogic();
        }
        else
        {
            for (int i = 0; i < targetLogics.Count; i++)
            {
                if (i + 1 < targetLogics.Count)
                {
                    var logicBelow = _targetLogics[i + 1];
                    _targetLogics[i].TransferControlDown += () => _logicInControl = logicBelow;
                }
                if (i - 1 > -1)
                {
                    var logicAbove = _targetLogics[i - 1];
                    _targetLogics[i].TransferControlUp += () => _logicInControl = logicAbove;
                }
            }

            _logicInControl = startingLogic ?? targetLogics[^1];
        }
    }

    public Guid? HandleKeyPress(UserKeyPress keyPress)
    {
        return _logicInControl.HandleKeyPress(keyPress);
    }

    public Guid? GetTargetId()
    {
        return _logicInControl.GetTargetId();
    }

    private class DefaultLogic : TargetLogic
    {

    }
}
