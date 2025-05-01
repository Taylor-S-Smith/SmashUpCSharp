using SmashUp.Frontend.Utilities;

namespace SmashUp.Frontend.Pages.Target;

/// <summary>
/// Handles logic and state for the BattlePage including:
/// - targeting/selecting
/// - key presses
/// </summary>
internal abstract class TargetLogic
{
    //These allow us to dynamically transfer control to other targeters
    public Action TransferControlUp = delegate { };
    public Action TransferControlDown = delegate { };

    protected Guid? _targetedOption;

    // CURSOR INDEXES
    protected int _cursorXIndex = 0;
    protected int _cursorYIndex = 0;

    public Guid? HandleKeyPress(UserKeyPress keyPress)
    {
        Guid? result = ExtendHandleKeyPress(keyPress);

        // Put all common keypresses here. I may reimplement a DEBUG mode, but right now I really don't want any
        // Dependencies on backend objects.
        /*switch (keyPress)
        {
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
                }
                break;
        }*/

        return result;
    }
    protected virtual Guid? ExtendHandleKeyPress(UserKeyPress keyPress)
    {
        return Guid.NewGuid();
    }
    public Guid? GetTargetId() => _targetedOption;
}
