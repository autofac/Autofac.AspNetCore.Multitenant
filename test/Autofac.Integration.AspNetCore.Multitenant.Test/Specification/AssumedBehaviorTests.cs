// Copyright (c) Autofac Project. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Autofac.Multitenant;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Autofac.Integration.AspNetCore.Multitenant.Test.Specification;

public class AssumedBehaviorTests
{
    [Fact]
    public void MultitenantServiceScopeFactoryAdapter_ResolvedOncePerTenant()
    {
        var serviceProvider = CreateServiceProvider();
        var mtc = serviceProvider.GetRequiredService<MultitenantContainer>();

        var firstServiceScopeFactory = mtc.Resolve<MultitenantServiceScopeFactoryAdapter>();
        var secondServiceScopeFactory = mtc.Resolve<MultitenantServiceScopeFactoryAdapter>();

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

    private sealed class StaticTenantIdentificationStrategy : ITenantIdentificationStrategy
    {
        public bool TryIdentifyTenant(out object tenantId)
        {
            tenantId = "a";
            return true;
        }
    }
}
