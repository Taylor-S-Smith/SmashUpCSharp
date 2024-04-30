using SmashUp.Rendering;
using SmashUp.Utilities;

namespace SmashUpCSharp.Program
{
    internal class Program
    {
        
        static void Main(string[] args)
        {
            //Initialize
            KeyMapUtil.SetDefaultKeyMappings();

            //Start Game
            Presenter presenter = new(new StartPage());
            presenter.Present();
        }        
    }
}
