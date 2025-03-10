using Microsoft.Extensions.DependencyInjection;

namespace SmashUpBackend;

internal class Program
{

    static void Main()
    {
        var services = new ServiceCollection();

        services.AddSingleton<Application>();

        ServiceProvider serviceProvider = services.BuildServiceProvider();


        var application = serviceProvider.GetRequiredService<Application>();

        application.Run();
    }
}
