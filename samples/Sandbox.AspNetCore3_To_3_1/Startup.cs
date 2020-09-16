using Autofac;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Sandbox.Shared;

namespace Sandbox
{

    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddAutofacMultitenantRequestServices()
                .AddControllers()
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            ContainerSetup.SetupContainer(builder);
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseRouting();

            app.UseEndpoints(builder => builder.MapControllers());
        }
    }
}
