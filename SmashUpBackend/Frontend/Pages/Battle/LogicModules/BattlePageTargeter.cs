using SmashUp.Frontend.Utilities;

namespace SmashUp.Frontend.Pages.Battle.LogicModules;

/// <summary>
/// Handles logic and state for the BattlePage including:
/// - targeting/selecting
/// - key presses
/// </summary>
internal abstract class BattlePageTargeter
{
    protected Guid? _targetedOption;

    readonly public bool DEBUG_MODE = true;

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
        return null;
    }
    public Guid? GetTargetId() => _targetedOption;


    // CARD FILTER FUNCS
    protected readonly Func<object, bool> AllCardsAreTargetable = card => true;

    // Just to serve Battle Page
    public (int, int) GetDebugVals()
    {
        return (_cursorXIndex, _cursorYIndex);
    }
    public bool IsInDebugMode()
    {
        return DEBUG_MODE;
    }
}
