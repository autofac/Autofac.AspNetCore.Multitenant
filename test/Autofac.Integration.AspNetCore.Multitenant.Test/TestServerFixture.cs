using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http;
using Autofac.Multitenant;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace Autofac.Integration.AspNetCore.Multitenant.Test
{
    public class TestServerFixture
    {
        public HttpClient GetApplicationClient()
        {
            var webHostBuilder = new WebHostBuilder()
                .UseStartup<Startup>()
                .ConfigureServices(sp =>
                    sp.AddSingleton<IServiceProviderFactory<ContainerBuilder>>(
                        new AutofacMultitenantServiceProviderFactory(Startup.CreateMultitenantContainer)));

            var testServer = new TestServer(webHostBuilder);

            return testServer.CreateClient();
        }

        private sealed class Startup
        {
            public void ConfigureServices(IServiceCollection services)
            {
                services
                    .AddAutofacMultitenantRequestServices()
                    .AddTransient(provider => new WhoAmIDependency("base"))
                    .AddSingleton<ITenantAccessor, TenantAccessorDependency>()
                    .AddSingleton<ITenantIdentificationStrategy, TestableTenantIdentificationStrategy>()
                    .AddRouting();
            }

            [SuppressMessage("IDE0060", "IDE0060", Justification = "Method is required so container will be built.")]
            public void ConfigureContainer(ContainerBuilder builder)
            {
                // You must have ConfigureContainer here, even if it's
                // not used, or the Autofac container won't be built.
            }

            public static MultitenantContainer CreateMultitenantContainer(IContainer container)
            {
                var strategy = container.Resolve<ITenantIdentificationStrategy>();

                var mtc = new MultitenantContainer(strategy, container);

                mtc.ConfigureTenant(
                    "a",
                    cb => cb
                        .RegisterType<WhoAmIDependency>()
                        .WithParameter("id", "a")
                        .InstancePerLifetimeScope());
                mtc.ConfigureTenant(
                    "b",
                    cb => cb
                        .RegisterType<WhoAmIDependency>()
                        .WithParameter("id", "b")
                        .InstancePerLifetimeScope());

                return mtc;
            }

            public void Configure(IApplicationBuilder builder)
            {
                builder.UseRouter(routeBuilder =>
                {
                    routeBuilder.MapGet("root-endpoint", async context =>
                    {
                        var whoAmI = context.RequestServices.GetRequiredService<WhoAmIDependency>();
                        var tenantAccessor = context.RequestServices.GetRequiredService<ITenantAccessor>();
                        var strategy = context.RequestServices.GetRequiredService<ITenantIdentificationStrategy>();

                        Assert.Equal("base", whoAmI.Id);
                        Assert.Null(tenantAccessor.CurrentTenant);
                        Assert.False(strategy.TryIdentifyTenant(out var tenantId));
                        Assert.Null(tenantId);

                        await context.Response.WriteAsync(whoAmI.Id);
                    });

                    routeBuilder.MapGet("tenant-endpoint", async context =>
                    {
                        var whoAmI = context.RequestServices.GetRequiredService<WhoAmIDependency>();
                        var tenantAccessor = context.RequestServices.GetRequiredService<ITenantAccessor>();
                        var strategy = context.RequestServices.GetRequiredService<ITenantIdentificationStrategy>();

                        if (whoAmI.Id == "base")
                        {
                            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                            return;
                        }

                        Assert.Equal(tenantAccessor.CurrentTenant, whoAmI.Id);
                        Assert.NotEqual("base", tenantAccessor.CurrentTenant);
                        Assert.True(strategy.TryIdentifyTenant(out var tenantId));
                        Assert.NotEqual("base", tenantId);
                        Assert.Equal(tenantId, tenantAccessor.CurrentTenant);
                        Assert.Equal(tenantId, whoAmI.Id);

                        await context.Response.WriteAsync(tenantAccessor.CurrentTenant);
                    });

                    routeBuilder.MapGet("supports-with-and-without-tenant", async context =>
                    {
                        var whoAmI = context.RequestServices.GetRequiredService<WhoAmIDependency>();
                        var tenantAccessor = context.RequestServices.GetRequiredService<ITenantAccessor>();
                        var strategy = context.RequestServices.GetRequiredService<ITenantIdentificationStrategy>();

                        Assert.True(strategy.TryIdentifyTenant(out var tenantId));
                        Assert.Equal(tenantId, tenantAccessor.CurrentTenant);

                        await context.Response.WriteAsync(whoAmI.Id);
                    });
                });
            }

            private interface ITenantAccessor
            {
                string CurrentTenant { get; }
            }

            private sealed class TenantAccessorDependency : ITenantAccessor
            {
                public string CurrentTenant { get; }

                public TenantAccessorDependency(ITenantIdentificationStrategy tenantIdentificationStrategy)
                {
                    if (tenantIdentificationStrategy.TryIdentifyTenant(out var tenantId) &&
                        tenantId is string currentTenant)
                    {
                        CurrentTenant = currentTenant;
                    }
                }
            }

            private sealed class WhoAmIDependency
            {
                public string Id { get; }

                public WhoAmIDependency(string id)
                {
                    Id = id;
                }
            }

            private sealed class TestableTenantIdentificationStrategy : ITenantIdentificationStrategy
            {
                private readonly IHttpContextAccessor _httpContextAccessor;

                public TestableTenantIdentificationStrategy(IHttpContextAccessor httpContextAccessor)
                {
                    _httpContextAccessor = httpContextAccessor;
                }

                public bool TryIdentifyTenant(out object tenantId)
                {
                    if (_httpContextAccessor.HttpContext?.Request.Query.TryGetValue("tenant", out var tenantValues) ??
                        false)
                    {
                        tenantId = tenantValues[0];
                        return true;
                    }

                    tenantId = null;
                    return false;
                }
            }
        }
    }
}
