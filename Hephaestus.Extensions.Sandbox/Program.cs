using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Hephaestus.Extensions.Sandbox
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var builder = Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<ExampleService>();
                });

            builder
                .UseConsoleLifetime()
                .Build()
                .Run();

        }
    }
}


