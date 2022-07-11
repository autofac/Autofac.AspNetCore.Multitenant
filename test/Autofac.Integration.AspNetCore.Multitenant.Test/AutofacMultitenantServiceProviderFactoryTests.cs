// Copyright (c) Autofac Project. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Autofac.Integration.AspNetCore.Multitenant.Properties;
using Autofac.Integration.AspNetCore.Multitenant.Test.TestDependencies;
using Autofac.Multitenant;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Autofac.Integration.AspNetCore.Multitenant.Test;

public class AutofacMultitenantServiceProviderFactoryTests
{
    [Fact]
    public void Ctor_NullContainerAccessor()
    {
        var ex = Assert.Throws<ArgumentNullException>(() => new AutofacMultitenantServiceProviderFactory(null!));

        Assert.Contains(nameof(MultitenantContainer), ex.Message, StringComparison.InvariantCultureIgnoreCase);
    }

    [Fact]
    public void CreateServiceProvider_ContainerAccessorReturnsNull()
    {
        var factory = new AutofacMultitenantServiceProviderFactory(_ => null!);

        var ex = Assert.Throws<InvalidOperationException>(() =>
            factory.CreateServiceProvider(new ContainerBuilder()));

        Assert.Equal(Resources.NoMultitenantContainerAvailable, ex.Message);
    }

    [Fact]
    public void CreateServiceProvider_RegistersFactoryAdapterPerTenant()
    {
        var strategy = new ManualTenantIdentificationStrategy();
        MultitenantContainer Accessor(IContainer container)
        {
            var mtc = new MultitenantContainer(strategy, container);
            mtc.ConfigureTenant("configured", b => { });
            return mtc;
        }

        var factory = new AutofacMultitenantServiceProviderFactory(Accessor);
        var builder = factory.CreateBuilder(new ServiceCollection());
        var sp = factory.CreateServiceProvider(builder);

        // One instance for all unidentifiable tenants.
        var noTenant1 = sp.GetRequiredService<MultitenantServiceScopeFactoryAdapter>();
        var noTenant2 = sp.GetRequiredService<MultitenantServiceScopeFactoryAdapter>();
        Assert.Same(noTenant1, noTenant2);

        // One instance for a tenant that wasn't explicitly configured.
        strategy.TenantId = "not-configured";
        var notConfigured1 = sp.GetRequiredService<MultitenantServiceScopeFactoryAdapter>();
        var notConfigured2 = sp.GetRequiredService<MultitenantServiceScopeFactoryAdapter>();
        Assert.NotSame(noTenant1, notConfigured1);
        Assert.Same(notConfigured1, notConfigured2);

        // One instance for a tenant that was explicitly configured
        strategy.TenantId = "configured";
        var configured1 = sp.GetRequiredService<MultitenantServiceScopeFactoryAdapter>();
        var configured2 = sp.GetRequiredService<MultitenantServiceScopeFactoryAdapter>();
        Assert.NotSame(noTenant1, configured1);
        Assert.NotSame(notConfigured1, configured1);
        Assert.Same(configured1, configured2);
    }
}
