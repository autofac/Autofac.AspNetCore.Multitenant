// Copyright (c) Autofac Project. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Sandbox;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
            .UseServiceProviderFactory(
                new AutofacMultitenantServiceProviderFactory(
                    MultitenantContainerSetup.ConfigureMultitenantContainer))
            .ConfigureWebHostDefaults(webHostBuilder => webHostBuilder.UseStartup<Startup>())
            .Build();

        await host.RunAsync();
    }
}
