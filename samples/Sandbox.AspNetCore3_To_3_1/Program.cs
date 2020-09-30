using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Sandbox.Shared;

namespace Sandbox
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            var host = Host.CreateDefaultBuilder(args)
                .UseServiceProviderFactory(
                    new AutofacMultitenantServiceProviderFactory(
                        MultitenantContainerSetup.ConfigureMultitenantContainer))
                .ConfigureWebHostDefaults(webHostBuilder => webHostBuilder.UseStartup<Startup>())
                .Build();

            await host.RunAsync();
        }
    }
}