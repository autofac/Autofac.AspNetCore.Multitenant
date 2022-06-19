using System;
using Autofac.Multitenant;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Autofac.Integration.AspNetCore.Multitenant.Test.Specification
{
    public class AssumedBehaviorTests
    {
        [Fact]
        public void ServiceScopeFactoryAccessor_ServiceScopeFactoryCreatedOncePerTenant()
        {
            var serviceProvider = CreateServiceProvider();
            var mtc = serviceProvider.GetRequiredService<MultitenantContainer>();
            var serviceScopeFactoryAccessor = mtc.Resolve<ServiceScopeFactoryAccessor>();

            var firstServiceScopeFactory = serviceScopeFactoryAccessor.Invoke(mtc.GetCurrentTenantScope());
            var secondServiceScopeFactory = serviceScopeFactoryAccessor.Invoke(mtc.GetCurrentTenantScope());

            Assert.Same(firstServiceScopeFactory, secondServiceScopeFactory);
        }

        private static IServiceProvider CreateServiceProvider()
        {
            var factory = new AutofacMultitenantServiceProviderFactory(CreateContainer);
            var builder = factory.CreateBuilder(new ServiceCollection());
            return factory.CreateServiceProvider(builder);
        }

        private static MultitenantContainer CreateContainer(IContainer container)
        {
            var mtc = new MultitenantContainer(new StaticTenantIdentificationStrategy(), container);
            return mtc;
        }

        private class StaticTenantIdentificationStrategy : ITenantIdentificationStrategy
        {
            public bool TryIdentifyTenant(out object tenantId)
            {
                tenantId = "a";
                return true;
            }
        }
    }
}