using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SharpDeck.Extensions.DependencyInjection;
using StreamDeck.GoXLR.Utility.Plugin.Services;

namespace StreamDeck.GoXLR.Utility.Plugin
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
#if DEBUG
            Thread.Sleep(3000);
            //I am using Entrian Attach to auto attach... but we do want to wait for the Debugger:
            while (!System.Diagnostics.Debugger.IsAttached)
            {
                Thread.Sleep(100);
            }
            //System.Diagnostics.Debugger.Launch();
#endif
            var client = new GoXlrUtilityClient();

            var serviceCollection = new ServiceCollection();

            serviceCollection.AddSingleton(client);
            //We need to assure the services are created before we start the client:
            serviceCollection.AddSingleton(new RoutingService(client));
            serviceCollection.AddSingleton(new ProfileService(client));
            serviceCollection.AddSingleton(new MicProfileService(client));
            serviceCollection.AddSingleton(new VolumeChangeService(client));

            serviceCollection.AddStreamDeck();

            var serviceProvider = serviceCollection.BuildServiceProvider();

            var services = serviceProvider.GetServices<IHostedService>();

            var tasks = services
                .Select(s => s.StartAsync(CancellationToken.None));
            
            client.Start();

            await Task.WhenAll(tasks);
        }
    }
}