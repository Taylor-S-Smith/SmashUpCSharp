using Microsoft.Extensions.DependencyInjection;
using Repositories;
using Services;
using SmashUp;
using SmashUp.Pages.GameSetUp;
using SmashUp.Rendering;
using SmashUp.The___Database__;

namespace SmashUpCSharp.Program
{
    internal class Program
    {
        
        static void Main(string[] args)
        {
            //Dependency Injection
            var services = new ServiceCollection();

            services.AddSingleton<IBaseCardRepository, BaseCardRepository>();
            services.AddSingleton<IFactionRepository, FactionRepository>();
            services.AddSingleton<IPlayableCardRepository, PlayableCardRepository>();
            services.AddSingleton<IPlayerRepository, PlayerRepository>();

            services.AddSingleton<IBaseService, BaseService>();
            services.AddSingleton<IFactionService, FactionService>();
            services.AddSingleton<IPlayableCardService, PlayableCardService>();
            services.AddSingleton<IPlayerService, PlayerService>();

            services.AddSingleton<StartPage>();
            services.AddSingleton<DeckSelectionPage>();
            services.AddSingleton<PlayerNamePage>();
            services.AddSingleton<PlayerNumPage>();
            services.AddSingleton<BattlePage>();

            services.AddSingleton<IDatabaseLoader, DatabaseLoader>();
            services.AddSingleton<Application>();

            ServiceProvider serviceProvider = services.BuildServiceProvider();

            var application = serviceProvider.GetRequiredService<Application>();

            application.Run();
        }        
    }
}
