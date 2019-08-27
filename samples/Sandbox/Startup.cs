using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Autofac.Multitenant;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Sandbox
{

    public class Startup
    {
        public void Configure(IApplicationBuilder app)
        {
            app.UseRouting();

            app.UseEndpoints(builder => builder.MapControllers());
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddAutofacMultitenantRequestServices()
                .AddControllers()
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterType<Dependency>()
                .As<IDependency>()
                .WithProperty("Id", "base")
                .InstancePerLifetimeScope();
        }

        public static MultitenantContainer ConfigureMultitenantContainer(IContainer container)
        {
            var strategy = new QueryStringTenantIdentificationStrategy(container.Resolve<IHttpContextAccessor>(),
                container.Resolve<ILogger<QueryStringTenantIdentificationStrategy>>());

            var multitenantContainer = new MultitenantContainer(strategy, container);

            multitenantContainer.ConfigureTenant(
                "a",
                cb => cb
                    .RegisterType<Dependency>()
                    .As<IDependency>()
                    .WithProperty("Id", "a")
                    .InstancePerLifetimeScope());
            multitenantContainer.ConfigureTenant(
                "b",
                cb => cb
                    .RegisterType<Dependency>()
                    .As<IDependency>()
                    .WithProperty("Id", "b")
                    .InstancePerLifetimeScope());

            return multitenantContainer;
        }
    }
}
