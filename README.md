# Autofac.AspNetCore.Multitenant

ASP.NET Core support for multitenant DI via [Autofac.Multitenant](https://github.com/autofac/Autofac.Multitenant).

[![Build status](https://github.com/autofac/Autofac.AspNetCore.Multitenant/actions/workflows/main.yml/badge.svg)](https://github.com/autofac/Autofac.AspNetCore.Multitenant/actions/workflows/main.yml) [![codecov](https://codecov.io/gh/Autofac/Autofac.AspNetCore.Multitenant/branch/develop/graph/badge.svg)](https://codecov.io/gh/Autofac/Autofac.AspNetCore.Multitenant) [![NuGet](https://img.shields.io/nuget/v/Autofac.AspNetCore.Multitenant.svg)](https://nuget.org/packages/Autofac.AspNetCore.Multitenant)

Please file issues and pull requests for this package in this repository rather than in the Autofac core repo.

- [Documentation - .NET Core Integration](https://autofac.readthedocs.io/en/latest/integration/netcore.html)
- [Documentation - ASP.NET Core Integration](https://autofac.readthedocs.io/en/latest/integration/aspnetcore.html)
- [Documentation - Multitenant Applications](https://autofac.readthedocs.io/en/latest/advanced/multitenant.html)
- [NuGet](https://www.nuget.org/packages/Autofac.AspNetCore.Multitenant)
- [Contributing](https://autofac.readthedocs.io/en/latest/contributors.html)
- [Open in Visual Studio Code](https://open.vscode.dev/autofac/Autofac.AspNetCore.Multitenant)

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

## Get Help

**Need help with Autofac?** We have [a documentation site](https://autofac.readthedocs.io/) as well as [API documentation](https://autofac.org/apidoc/). We're ready to answer your questions on [Stack Overflow](https://stackoverflow.com/questions/tagged/autofac) or check out the [discussion forum](https://groups.google.com/forum/#forum/autofac).
