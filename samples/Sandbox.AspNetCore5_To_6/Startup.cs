// Copyright (c) Autofac Project. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Autofac;

namespace Sandbox;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services
            .AddAutofacMultitenantRequestServices()
            .AddControllers();
    }

    public void ConfigureContainer(ContainerBuilder builder)
    {
        ContainerSetup.SetupContainer(builder);
    }

    public void Configure(IApplicationBuilder app)
    {
        app.UseRouting();

        app.UseEndpoints(builder => builder.MapControllers());
    }
}
