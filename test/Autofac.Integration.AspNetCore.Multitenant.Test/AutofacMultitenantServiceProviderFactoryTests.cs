using System;
using Autofac.Integration.AspNetCore.Multitenant.Properties;
using Autofac.Multitenant;
using Microsoft.AspNetCore.Hosting;
using Xunit;

namespace Autofac.Integration.AspNetCore.Multitenant.Test
{
    public class AutofacMultitenantServiceProviderFactoryTests
    {
        [Fact]
        public void
            NewAutofacMultitenantServiceProviderFactory_MultitenantContainerAccessorRetursNull_ThrowsInvalidOperationExecption()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => new AutofacMultitenantServiceProviderFactory(null!));

            Assert.Contains(nameof(MultitenantContainer), ex.Message, StringComparison.InvariantCultureIgnoreCase);
        }

        [Fact]
        public void CreateServiceProvider_MultitenantContainerAccessorRetursNull_ThrowsInvalidOperationExecption()
        {
            var factory = new AutofacMultitenantServiceProviderFactory(_ => null!);

            var ex = Assert.Throws<InvalidOperationException>(() =>
                factory.CreateServiceProvider(new ContainerBuilder()));

            Assert.Equal(Resources.NoMultitenantContainerAvailable, ex.Message);
        }
    }
}
