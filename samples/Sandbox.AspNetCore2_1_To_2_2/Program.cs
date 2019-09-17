using System.Threading.Tasks;
using Autofac;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Sandbox.Shared;

namespace Sandbox.AspNetCore2_1_To_2_2
{
    public class Program
    {
        public static Task Main(string[] args)
        {
            return CreateWebHostBuilder(args).Build().RunAsync();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseAutofacMultitenantRequestServices()
                .ConfigureServices(sp =>
                {
                    sp.AddSingleton<IServiceProviderFactory<ContainerBuilder>>(
                        new AutofacMultitenantServiceProviderFactory(MultitenantContainerSetup
                            .ConfigureMultitenantContainer));
                });
    }
}