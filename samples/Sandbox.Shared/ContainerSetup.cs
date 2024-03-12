// Copyright (c) Autofac Project. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Autofac;
using Autofac.Multitenant;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Sandbox;

/// <summary>
/// Common methods used to set up the default and multitenant container registrations in the sandbox projects.
/// </summary>
public static class ContainerSetup
{
    /// <summary>
    /// Configures the container with default dependencies. These can be
    /// overridden by multitenant registrations.
    /// </summary>
    /// <param name="builder">
    /// The <see cref="ContainerBuilder"/> with which registrations are being
    /// added.
    /// </param>
    public static void ConfigureContainer(ContainerBuilder builder)
    {
        builder.RegisterType<Dependency>()
            .As<IDependency>()
            .WithProperty("Id", "base")
            .InstancePerLifetimeScope();
    }

    /// <summary>
    /// Configures tenant-specific overrides for the multitenant container.
    /// </summary>
    /// <param name="container">
    /// The container supplying base dependency registrations that will be
    /// overridden by tenant-specific registrations.
    /// </param>
    /// <returns>
    /// A multitenant container with two tenants configured.
    /// </returns>
    public static MultitenantContainer ConfigureMultitenantContainer(IContainer container)
    {
        var strategy = new QueryStringTenantIdentificationStrategy(
            container.Resolve<IHttpContextAccessor>(),
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
