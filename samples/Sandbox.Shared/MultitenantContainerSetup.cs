using System;
using Autofac;
using Autofac.Multitenant;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Sandbox.Shared
{
    public static class MultitenantContainerSetup
    {
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