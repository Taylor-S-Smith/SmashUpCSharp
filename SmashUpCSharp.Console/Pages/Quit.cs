using SmashUp.Utilities;

namespace SmashUp.Rendering
{
    public class Quit : PrimitivePage
    {
        public override void Render(int consoleWidth, int consoleHeight) { }
        public override PrimitivePage? ChangeState(UserKeyPress keyPress, ref bool needToRender) { return null; }
    }
}
