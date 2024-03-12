// Copyright (c) Autofac Project. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Sandbox;

/// <summary>
/// Entry pont for the ASP.NET sandbox application.
/// </summary>
public static class Program
{
    /// <summary>
    /// Primary method for execution of the sandbox application.
    /// </summary>
    /// <param name="args">
    /// The set of command line arguments provided to the application.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> to await completion of the host execution.
    /// </returns>
    public static async Task Main(string[] args)
    {
        // Note the AutofacMultitenantServiceProviderFactory takes the method
        // that configures the tenant-specific overrides. That doesn't show up
        // in your Startup class.
        var host = Host.CreateDefaultBuilder(args)
            .UseServiceProviderFactory(new AutofacMultitenantServiceProviderFactory(ContainerSetup.ConfigureMultitenantContainer))
            .ConfigureWebHostDefaults(webHostBuilder => webHostBuilder.UseStartup<Startup>())
            .Build();

        await host.RunAsync();
    }
}
