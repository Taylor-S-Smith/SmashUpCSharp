using SmashUp.Utilities;

namespace SmashUp.Rendering
{
    public abstract class PrimitivePage
    {
        public abstract void Render(int consoleWidth, int consoleHeight);
        public abstract PrimitivePage? ChangeState(UserKeyPress keyPress, ref bool needToRender);
    }
}
