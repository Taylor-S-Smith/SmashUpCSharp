using SmashUp.Backend.GameObjects;
using SmashUp.Frontend.Utilities;
using SmashUp.Backend.Services;
using SmashUp.Frontend.Pages;
using SmashUp.Frontend.Presenters;

namespace SmashUp;

internal class Application()
{
    internal void Run()
    {
        // Initialize Data
        KeyMapUtil.SetDefaultKeyMappings();

        //Run Start Page
        var result = new StartPage().Run();

        if(result == StartPageResult.Exit)
        {
            return;
        }
        else if (result == StartPageResult.StartGame)
        {
            Battle battle = new(new ConsoleApp2PlayerTestUI(), new GlobalEventManager(), new Random());
            battle.StartBattle();
        }
        else if (result == StartPageResult.ShowCollection)
        {
            Console.WriteLine("Showing Collection...");
        }
        else if (result == StartPageResult.Options)
        {
            Console.WriteLine("Showing Options...");
        }

    }
}


