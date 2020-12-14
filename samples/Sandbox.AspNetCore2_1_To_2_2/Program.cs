// Copyright (c) Autofac Project. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Threading.Tasks;
using Autofac;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Sandbox
{
    public static class Program
    {
        public static Task Main(string[] args)
        {
            return CreateWebHostBuilder(args).Build().RunAsync();
        }

        private static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseAutofacMultitenantRequestServices()
                .ConfigureServices(sp =>
                {
                    sp.AddSingleton<IServiceProviderFactory<ContainerBuilder>>(
                        new AutofacMultitenantServiceProviderFactory(MultitenantContainerSetup
                            .ConfigureMultitenantContainer));
                });
    }
}