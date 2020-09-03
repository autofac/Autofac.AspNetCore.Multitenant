using System;
using Autofac.Integration.AspNetCore.Multitenant.Properties;
using Autofac.Multitenant;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace Autofac.Integration.AspNetCore.Multitenant.Test
{
    public class AutofacMultitenantServiceCollectionExtensionsTests
    {
        [Fact]
        public void AddAutofacMultitenantRequestServices_AddsHttpContextAccessor()
        {
            var services = new ServiceCollection();

            services.AddAutofacMultitenantRequestServices();

            var serviceProvider = services.BuildServiceProvider();
            var accessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();

            Assert.NotNull(accessor);
        }

        [Fact]
        public void AddAutofacMultitenantRequestServices_AddsStartupFilter()
        {
            var services = new ServiceCollection();

            services.AddAutofacMultitenantRequestServices();

            var serviceProvider = services.BuildServiceProvider();
            var filter = serviceProvider.GetRequiredService<IStartupFilter>();

            Assert.IsType<MultitenantRequestServicesStartupFilter>(filter);
        }

        [Fact]
        public void AddAutofacMultitenantRequestServices_NullBuilder()
        {
            var mtc = new MultitenantContainer(Mock.Of<ITenantIdentificationStrategy>(), new ContainerBuilder().Build());
            Assert.Throws<ArgumentNullException>(() => AutofacMultitenantServiceCollectionExtensions.AddAutofacMultitenantRequestServices(null));
        }
    }
}