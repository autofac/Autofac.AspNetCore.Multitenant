// Copyright (c) Autofac Project. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Diagnostics.CodeAnalysis;
using Autofac;

namespace Sandbox;

/// <summary>
/// Startup logic for the sandbox application.
/// </summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by ASP.NET Core framework.")]
[SuppressMessage("Performance", "CA1852:Seal internal types", Justification = "Used by ASP.NET Core framework.")]
internal class Startup
{
    /// <summary>
    /// Configures default services using the standard Microsoft container.
    /// </summary>
    /// <param name="services">
    /// The <see cref="IServiceCollection"/> into which registrations will be made.
    /// </param>
    [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "ASP.NET Core expects instance methods.")]
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
    [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "ASP.NET Core expects instance methods.")]
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
    [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "ASP.NET Core expects instance methods.")]
    public void Configure(IApplicationBuilder app)
    {
        app.UseRouting();
        app.UseEndpoints(builder => builder.MapControllers());
    }
}
