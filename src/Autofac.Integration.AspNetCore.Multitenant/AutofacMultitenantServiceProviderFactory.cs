﻿// Copyright (c) Autofac Project. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Autofac;
using Autofac.Extensions.DependencyInjection;
using Autofac.Integration.AspNetCore.Multitenant;
using Autofac.Integration.AspNetCore.Multitenant.Properties;
using Autofac.Multitenant;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.Hosting;

/// <summary>
/// A factory for creating a <see cref="ContainerBuilder"/> and an <see cref="IServiceProvider" /> for usage with a <see cref="MultitenantContainer" /> in ASP.NET Core.
/// </summary>
public class AutofacMultitenantServiceProviderFactory : IServiceProviderFactory<ContainerBuilder>
{
    private readonly Action<ContainerBuilder> _configurationAction;
    private readonly Func<IContainer, MultitenantContainer> _multitenantContainerAccessor;

    /// <summary>
    /// Initializes a new instance of the <see cref="AutofacMultitenantServiceProviderFactory"/> class.
    /// </summary>
    /// <param name="multitenantContainerAccessor">A function that will access the multitenant container from which request lifetimes should be generated.</param>
    /// <param name="configurationAction">Action on a <see cref="ContainerBuilder"/> that adds component registrations to the container.</param>
    /// <exception cref="System.ArgumentNullException">Throws when the multitenant container accessor is null.</exception>
    /// Thrown if <paramref name="multitenantContainerAccessor" /> is <see langword="null" />.
    public AutofacMultitenantServiceProviderFactory(Func<IContainer, MultitenantContainer>? multitenantContainerAccessor, Action<ContainerBuilder>? configurationAction = null)
    {
        _multitenantContainerAccessor = multitenantContainerAccessor ?? throw new ArgumentNullException(nameof(multitenantContainerAccessor));
        _configurationAction = configurationAction ?? (builder => { });
    }

    /// <summary>
    /// Creates a container builder from an <see cref="IServiceCollection" />.
    /// </summary>
    /// <param name="services">The collection of services.</param>
    /// <returns>A container builder that can be used to create an <see cref="IServiceProvider" />.</returns>
    public ContainerBuilder CreateBuilder(IServiceCollection services)
    {
        var builder = new ContainerBuilder();

        builder.Populate(services);

        _configurationAction(builder);

        return builder;
    }

    /// <summary>
    /// Creates an <see cref="IServiceProvider" /> from the container builder.
    /// </summary>
    /// <param name="containerBuilder">The container builder.</param>
    /// <returns>An <see cref="IServiceProvider" />.</returns>
    public IServiceProvider CreateServiceProvider(ContainerBuilder containerBuilder)
    {
        ArgumentNullException.ThrowIfNull(containerBuilder);

        MultitenantContainer multitenantContainer = null!;

        containerBuilder.Register(_ => multitenantContainer)
          .AsSelf()
          .ExternallyOwned();

        containerBuilder.Register(componentContext =>
            {
                var scope = componentContext.Resolve<MultitenantContainer>().GetCurrentTenantScope();
                var autofacChildLifetimeScopeServiceProviderFactory =
                    new AutofacChildLifetimeScopeServiceProviderFactory(scope);
                var adapter =
                    autofacChildLifetimeScopeServiceProviderFactory.CreateBuilder(new ServiceCollection());
                var serviceProvider =
                    autofacChildLifetimeScopeServiceProviderFactory.CreateServiceProvider(adapter);
                var factory = serviceProvider.GetRequiredService<IServiceScopeFactory>();
                return new MultitenantServiceScopeFactoryAdapter(factory);
            })
            .InstancePerTenant();

        multitenantContainer = _multitenantContainerAccessor(containerBuilder.Build());

        if (multitenantContainer == null)
        {
            throw new InvalidOperationException(Resources.NoMultitenantContainerAvailable);
        }

        return new AutofacServiceProvider(multitenantContainer);
    }
}
