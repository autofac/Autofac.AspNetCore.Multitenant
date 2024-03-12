// Copyright (c) Autofac Project. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Autofac;

namespace Sandbox;

/// <summary>
/// Startup logic for the sandbox application.
/// </summary>
public class Startup
{
    /// <summary>
    /// Configures default services using the standard Microsoft container.
    /// </summary>
    /// <param name="services">
    /// The <see cref="IServiceCollection"/> into which registrations will be made.
    /// </param>
    public void ConfigureServices(IServiceCollection services)
    {
        services
            .AddAutofacMultitenantRequestServices()
            .AddControllers();
    }

    /// <summary>
    /// Configures default services that override standard Microsoft services. This method is called after <see cref="ConfigureServices"/> but before multitenant overrides occur.
    /// </summary>
    /// <param name="builder">
    /// The Autofac <see cref="ContainerBuilder"/> into which default registrations will occur.
    /// </param>
    public void ConfigureContainer(ContainerBuilder builder)
    {
        // Note the multitenant registrations aren't here! They're in
        // Program.cs, and they're passed into the
        // AutofacMultitenantServiceProviderFactory.
        ContainerSetup.ConfigureContainer(builder);
    }

    /// <summary>
    /// Configures the pipeline for the sandbox application.
    /// </summary>
    /// <param name="app">
    /// The <see cref="IApplicationBuilder"/> with which the pipeline is being built.
    /// </param>
    public void Configure(IApplicationBuilder app)
    {
        app.UseRouting();
        app.UseEndpoints(builder => builder.MapControllers());
    }
}
