using System;
using Autofac.Extensions.DependencyInjection;
using Autofac.Multitenant;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Autofac.Integration.AspNetCore.Multitenant.Test
{
    public sealed class AutofacMultitenantServiceProviderExtensionsTests
    {
        [Fact]
        public void GetAutofacMultitenantRootReturnsMultitenantContainer()
        {
            var factory = new AutofacMultitenantServiceProviderFactory(CreateMultitenantContainer);
            factory.CreateBuilder(new ServiceCollection());
            var serviceProvider = factory.CreateServiceProvider(new ContainerBuilder());

            Assert.NotNull(serviceProvider.GetAutofacMultitenantRoot());
        }

        [Fact]
        public void GetAutofacRootServiceProviderNotAutofacServiceProviderThrows()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.Populate(new ServiceCollection());
            var container = containerBuilder.Build();

            var serviceProvider = container.Resolve<IServiceProvider>();

            Assert.Throws<InvalidOperationException>(() =>
                serviceProvider.GetAutofacMultitenantRoot());
        }

        private static MultitenantContainer CreateMultitenantContainer(IContainer container)
            => new MultitenantContainer(new FakeTenantIdentificationStrategy(), container);

        private class FakeTenantIdentificationStrategy : ITenantIdentificationStrategy
        {
            public bool TryIdentifyTenant(out object tenantId)
            {
                tenantId = null;
                return false;
            }
        }
    }
}
