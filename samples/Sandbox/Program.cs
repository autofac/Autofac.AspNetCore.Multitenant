using Microsoft.AspNetCore.Hosting;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Sandbox
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = Host.CreateDefaultBuilder(args)
                .UseServiceProviderFactory(new AutofacMultitenantServiceProviderFactory(Startup.ConfigureMultitenantContainer))
                .ConfigureWebHostDefaults(webHostBuilder => webHostBuilder.UseStartup<Startup>())
                .Build();


            await host.RunAsync();
        }
    }
}
