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
        var startPage = new StartPage();
        while(true)
        {
            var result = startPage.Run();

            if (result == StartPageResult.Exit)
            {
                return;
            }
            else if (result == StartPageResult.StartGame)
            {
                Battle battle = new(new ConsoleAppBattleUI(), new GlobalEventManager(), new Random());
                battle.StartBattle();
            }
            else if (result == StartPageResult.RogueLike)
            {
                Console.WriteLine("Coming Soon!");
                Console.ReadLine();
            }
            else if (result == StartPageResult.ShowCollection)
            {
                Console.WriteLine("Coming Soon!");
                Console.ReadLine();
            }
            else if (result == StartPageResult.Options)
            {
                Console.WriteLine("Coming Soon!");
                Console.ReadLine();
            }
        }      

    }
}


