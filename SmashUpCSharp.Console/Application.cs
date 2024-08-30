using SmashUp.The___Database__;
using SmashUp.Frontend.Pages;
using SmashUp.Frontend.Utilities;

namespace SmashUp
{
    internal class Application(IDatabaseLoader databaseLoader)
    {
        readonly IDatabaseLoader _databaseLoader = databaseLoader;

        internal void Run(Dictionary<string, Func<PrimitivePage>> availablePageDictionary)
        {
            //Initialize Data
            KeyMapUtil.SetDefaultKeyMappings();
            _databaseLoader.Load();

            //Start Game
            Presenter presenter = new(availablePageDictionary["BattlePage"].Invoke(), availablePageDictionary);

            presenter.Present();
        }
    }
}
