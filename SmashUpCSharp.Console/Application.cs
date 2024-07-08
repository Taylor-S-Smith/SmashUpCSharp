using SmashUp.The___Database__;
using SmashUp.Rendering;
using SmashUp.Utilities;

namespace SmashUp
{
    internal class Application(StartPage startPage, IDatabaseLoader databaseLoader)
    {
        readonly IDatabaseLoader _databaseLoader = databaseLoader;
        readonly StartPage _startPage = startPage;

        internal void Run()
        {
            //Initialize Data
            KeyMapUtil.SetDefaultKeyMappings();
            _databaseLoader.Load();

            //Start Game
            Presenter presenter = new(_startPage);
            presenter.Present();
        }
    }
}
