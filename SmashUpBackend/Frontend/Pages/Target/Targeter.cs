using System.Reflection;
using SmashUp.Frontend.Utilities;

namespace SmashUp.Frontend.Pages.Target;
internal class Targeter
{
    private List<TargetLogic> _targetLogics;

    private TargetLogic _logicInControl;

    public Guid? HandleKeyPress(UserKeyPress keyPress)
    {
        return _logicInControl.HandleKeyPress(keyPress);
    }

    public Guid? GetTargetId()
    {
        return _logicInControl.GetTargetId();
    }

    internal (int, int) GetDebugVals()
    {
        return _logicInControl.GetDebugVals();
    }

    internal bool IsInDebugMode()
    {
        return _logicInControl.IsInDebugMode();
    }

    public Targeter(List<TargetLogic> targetLogics)
    {
        if (targetLogics.Count == 0) throw new Exception("Targeter must have at least one target logic");

        _targetLogics = targetLogics;

        for(int i = 0; i < targetLogics.Count; i++)
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

        _logicInControl = targetLogics[0];
    }
}
