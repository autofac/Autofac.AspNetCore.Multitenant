using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Autofac.Extensions.DependencyInjection;
using Autofac.Multitenant;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Autofac.Integration.AspNetCore.Multitenant.Test
{
    public sealed class HostedMultitenancyTests
    {
        [Fact]
        public async Task CallRootEndpoint_HasTheCorrectDependenciesAndResponseIsBase()
        {
            var client = GetApplicationClient();

            var response = await client.GetAsync("root-endpoint");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("base", await response.Content.ReadAsStringAsync());
        }

        [Theory]
        [InlineData("a", "a")]
        [InlineData("b", "b")]
        public async Task CallTenantEndpoint_HasTheCorrectDependenciesAndResponseIsTenantItself(string tenantQuery, string expectedTenantId)
        {
            var client = GetApplicationClient();

            var response = await client.GetAsync($"tenant-endpoint?tenant={tenantQuery}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(expectedTenantId, await response.Content.ReadAsStringAsync());
        }

        [Theory]
        [InlineData("tenant-does-not-exist")]
        public async Task CallTenantEndpoint_WithNonExistantTenantReturns404(string tenantQuery)
        {
            var client = GetApplicationClient();

            var response = await client.GetAsync($"tenant-endpoint?tenant={tenantQuery}");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Theory]
        [InlineData("", "base")]
        [InlineData("wrong-tenant", "base")]
        [InlineData("a", "a")]
        [InlineData("b", "b")]
        public async Task CallGenericEndpoint_HasTheCorrectDependenciesAndResponseIsTenantOrBase(string tenantQuery, string expectedTenantId)
        {
            var client = GetApplicationClient();

            var response = await client.GetAsync($"supports-with-and-without-tenant?tenant={tenantQuery}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(expectedTenantId, await response.Content.ReadAsStringAsync());
        }

        private static HttpClient GetApplicationClient()
        {
            var server = new TestServer(new WebHostBuilder()
                .UseStartup<Startup>());

            return server.CreateClient();
        }

        private sealed class Startup
        {
            private MultitenantContainer _applicationContainer;

            public IServiceProvider ConfigureServices(IServiceCollection services)
            {
                services
                    .AddAutofacMultitenantRequestServices(() => _applicationContainer)
                    .AddTransient(provider => new WhoAmIDependency("base"))
                    .AddSingleton<ITenantIdentificationStrategy, TestabletenantIdentificationStrategy>()
                    .AddSingleton<ITenantAccessor, TenantAccessorDependency>()
                    .AddRouting();

                var builder = new ContainerBuilder();
                builder.Populate(services);
                var container = builder.Build();

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

                _applicationContainer = mtc;
                return new AutofacServiceProvider(mtc);
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

            private sealed class TestabletenantIdentificationStrategy : ITenantIdentificationStrategy
            {
                private readonly IHttpContextAccessor _httpContextAccessor;

                public TestabletenantIdentificationStrategy(IHttpContextAccessor httpContextAccessor)
                {
                    _httpContextAccessor = httpContextAccessor;
                }

                public bool TryIdentifyTenant(out object tenantId)
                {
                    if (_httpContextAccessor.HttpContext?.Request.Query.TryGetValue("tenant", out var tenantValues) ?? false)
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