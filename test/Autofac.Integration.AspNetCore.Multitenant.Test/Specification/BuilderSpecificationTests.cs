// Copyright (c) Autofac Project. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Autofac.Multitenant;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Specification;

namespace Autofac.Integration.AspNetCore.Multitenant.Test.Specification;

public class BuilderSpecificationTests : DependencyInjectionSpecificationTests
{
    public override bool SupportsIServiceProviderIsService => true;

    protected override IServiceProvider CreateServiceProvider(IServiceCollection serviceCollection)
        => CreateServiceProviderFromCollection(serviceCollection);

    private static IServiceProvider CreateServiceProviderFromCollection(IServiceCollection services)
    {
        var factory = new AutofacMultitenantServiceProviderFactory(CreateContainer);
        var builder = factory.CreateBuilder(services);
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
