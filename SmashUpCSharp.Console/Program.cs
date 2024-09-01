using Microsoft.Extensions.DependencyInjection;
using SmashUp;
using SmashUp.Backend.Repositories;
using SmashUp.Backend.Services;
using SmashUp.Frontend.Pages;
using SmashUp.Frontend.Pages.GameSetUp;
using SmashUp.The___Database__;

namespace SmashUpCSharp.Program
{
    internal class Program
    {
        
        static void Main()
        {
            var services = new ServiceCollection();

            //Repo
            services.AddSingleton<IBaseCardRepository, BaseCardRepository>();
            services.AddSingleton<IFactionRepository, FactionRepository>();
            services.AddSingleton<IPlayableCardRepository, PlayableCardRepository>();
            services.AddSingleton<IPlayerRepository, PlayerRepository>();

            //Service
            services.AddSingleton<IBaseService, BaseService>();
            services.AddSingleton<IFactionService, FactionService>();
            services.AddSingleton<IPlayableCardService, PlayableCardService>();
            services.AddSingleton<IPlayerService, PlayerService>();

            //Pages
            services.AddSingleton<StartPage>();
            services.AddSingleton<PlayerNumPage>();
            services.AddSingleton<PlayerNamePage>();
            services.AddSingleton<DeckSelectionPage>();
            services.AddSingleton<BattlePage>();

            //Other
            services.AddSingleton<IDatabase, Database>();
            services.AddSingleton<Application>();

            ServiceProvider serviceProvider = services.BuildServiceProvider();

            Dictionary<string, Func<PrimitivePage>> availablePageDictionary = new()
            {
                { "StartPage",  serviceProvider.GetRequiredService<StartPage>},
                { "PlayerNumPage",  serviceProvider.GetRequiredService<PlayerNumPage>},
                { "PlayerNamePage",  serviceProvider.GetRequiredService<PlayerNamePage>},
                { "DeckSelectionPage",  serviceProvider.GetRequiredService<DeckSelectionPage>},
                { "BattlePage",  serviceProvider.GetRequiredService<BattlePage>}
            };

            var application = serviceProvider.GetRequiredService<Application>();

            application.Run(availablePageDictionary);
        }        
    }
}
