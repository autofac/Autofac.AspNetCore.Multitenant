# Autofac.AspNetCore.Multitenant

ASP.NET Core support for multitenant DI via [Autofac.Multitenant](https://github.com/autofac/Autofac.Multitenant).

[![Build status](https://ci.appveyor.com/api/projects/status/u6epu5sc9f9sgav3?svg=true)](https://ci.appveyor.com/project/Autofac/autofac-aspnetcore-multitenant) [![codecov](https://codecov.io/gh/Autofac/Autofac.AspNetCore.Multitenant/branch/develop/graph/badge.svg)](https://codecov.io/gh/Autofac/Autofac.AspNetCore.Multitenant)

Please file issues and pull requests for this package in this repository rather than in the Autofac core repo.

## Why Use This?

ASP.NET Core default `RequestServicesContainerMiddleware` is where the per-request lifetime scope usually gets generated. However, [its constructor is where it wants the `IServiceScopeFactory`](https://github.com/aspnet/Hosting/blob/b6a3fee08869cd9ac9d266b15b4eb7de205199ed/src/Microsoft.AspNetCore.Hosting/Internal/RequestServicesContainerMiddleware.cs#L17) that will be used later during the request to create the request lifetime scope.

Unfortunately, that means the `IServiceScopeFactory` is created/resolved at the point when the request comes in, long before an `HttpContext` is set in any `IHttpContextAccessor`. The result is the scope factory ends up coming from the default tenant scope, before a tenant can be identified, and per-request services will later all come from the default tenant. Multitenancy fails.

This package provides a different request services middleware that ensures the `IHttpContextAccessor.HttpContext` is set and defers creation of the request lifetime scope until as late as possible so anything needed for tenant identification can be established.

## Quick Start

When creating your application host, use the multitenant service provider factory. Your `Startup` will register common dependencies, but you'll need to provide a static method to initialize the tenant-specific overrides.

```c#
var host = Host.CreateDefaultBuilder(args)
    .UseServiceProviderFactory(new AutofacMultitenantServiceProviderFactory(MultitenantContainerSetup.ConfigureMultitenantContainer))
    .ConfigureWebHostDefaults(webHostBuilder => webHostBuilder.UseStartup<Startup>())
    .Build();
```

In your `Startup` class, make sure to use the multitenant request services middleware and register your common dependencies.

```c#
public class Startup
{
  public void ConfigureServices(IServiceCollection services)
  {
    // Add the multitenant request services handler.
    services
      .AddAutofacMultitenantRequestServices()
      .AddControllers();
  }

  public void ConfigureContainer(ContainerBuilder builder)
  {
    // Register tenant-shared dependencies and defaults.
    builder.RegisterType<CommonDependency>()
      .As<IDependency>()
      .InstancePerLifetimeScope();
  }

  public void Configure(IApplicationBuilder app)
  {
      app.UseRouting();
      app.UseEndpoints(builder => builder.MapControllers());
  }
}
```

Provide a method to override things for tenant-specific dependencies. This is what gets passed to the multitenant service provider factory.

```c#
public static class MultitenantContainerSetup
{
  public static MultitenantContainer ConfigureMultitenantContainer(IContainer container)
  {
    // Define how you're going to identify tenants.
    var strategy = new QueryStringTenantIdentificationStrategy(
        container.Resolve<IHttpContextAccessor>(),
        container.Resolve<ILogger<QueryStringTenantIdentificationStrategy>>());

    // Create the multitenant container.
    var multitenantContainer = new MultitenantContainer(strategy, container);

    // Register tenant overrides.
    multitenantContainer.ConfigureTenant(
        "some-tenant",
        cb => cb
          .RegisterType<OverrideDependency>()
          .As<IDependency>()
          .WithProperty("Id", "some-tenant")
          .InstancePerLifetimeScope());

    // Return the built container for use in the app.
    return multitenantContainer;
  }
}
```

## Reference

- [Documentation for Autofac.Multitenant](https://autofac.readthedocs.io/en/latest/advanced/multitenant.html)
- [Documentation for ASP.NET Core Integration](https://autofac.readthedocs.io/en/latest/integration/aspnetcore.html)
- [Original explanation of why multitenancy failed in ASP.NET Core](https://stackoverflow.com/questions/38940241/autofac-multitenant-in-an-aspnet-core-application-does-not-seem-to-resolve-tenan/38960122#38960122)
- [NuGet](https://www.nuget.org/packages/Autofac.AspNetCore.Multitenant)
- [Contributing](https://autofac.readthedocs.io/en/latest/contributors.html)
- [Open in Visual Studio Code](https://open.vscode.dev/autofac/Autofac.AspNetCore.Multitenant)
