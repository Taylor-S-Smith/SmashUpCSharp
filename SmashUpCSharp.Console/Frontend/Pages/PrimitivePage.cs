using SmashUp.Frontend.Utilities;

namespace SmashUp.Frontend.Pages
{
    public abstract class PrimitivePage
    {
        public abstract void Render(int consoleWidth, int consoleHeight);
        public abstract string? HandleKeyPress(UserKeyPress keyPress, ref bool stateChanged);
        public virtual Dictionary<ConsoleKey, UserKeyPress> KeyMappings { get; set; } = GameplayKeyMappings;
    }
}
