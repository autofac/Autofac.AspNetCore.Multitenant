using System;
using Autofac.Extensions.DependencyInjection;
using Autofac.Multitenant;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Autofac.Integration.AspNetCore.Multitenant.Test
{
    public sealed class AutofacMultitenantServiceProviderExtensionsTests
    {
        [Fact]
        public void GetAutofacMultitenantRootReturnsMultitenantContainer()
        {
            var serviceProvider = new AutofacMultitenantServiceProvider(CreateMultitenantContainer());

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

        private static MultitenantContainer CreateMultitenantContainer()
            => new MultitenantContainer(new FakeTenantIdentificationStrategy(), new ContainerBuilder().Build());

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