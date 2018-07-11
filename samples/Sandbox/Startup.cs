using System;
using System.Linq;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Autofac.Multitenant;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Sandbox
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        public static MultitenantContainer ApplicationContainer { get; set; }

        public IConfiguration Configuration { get; }

        public void Configure(IApplicationBuilder app)
        {
            app.UseMvc();
        }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            var builder = new ContainerBuilder();
            builder.Populate(services);
            builder.RegisterType<Dependency>()
                .As<IDependency>()
                .WithProperty("Id", "base")
                .InstancePerLifetimeScope();
            var container = builder.Build();
            var strategy = new QueryStringTenantIdentificationStrategy(container.Resolve<IHttpContextAccessor>(), container.Resolve<ILogger<QueryStringTenantIdentificationStrategy>>());
            var mtc = new MultitenantContainer(strategy, container);
            mtc.ConfigureTenant(
                "a",
                cb => cb
                        .RegisterType<Dependency>()
                        .As<IDependency>()
                        .WithProperty("Id", "a")
                        .InstancePerLifetimeScope());
            mtc.ConfigureTenant(
                "b",
                cb => cb
                        .RegisterType<Dependency>()
                        .As<IDependency>()
                        .WithProperty("Id", "b")
                        .InstancePerLifetimeScope());
            Startup.ApplicationContainer = mtc;
            return new AutofacServiceProvider(mtc);
        }
    }
}
