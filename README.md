# Autofac.AspNetCore.Multitenant

ASP.NET Core support for multitenant DI via [Autofac.Multitenant](https://github.com/autofac/Autofac.Multitenant).

[![Build status](https://ci.appveyor.com/api/projects/status/9120t73i97ywdoav?svg=true)](https://ci.appveyor.com/project/Autofac/autofac-aspnetcore-multitenant)

Please file issues and pull requests for this package in this repository rather than in the Autofac core repo.

## Why Use This?

ASP.NET Core default `RequestServicesContainerMiddleware` is where the per-request lifetime scope usually gets generated. However, [its constructor is where it wants the `IServiceScopeFactory`](https://github.com/aspnet/Hosting/blob/b6a3fee08869cd9ac9d266b15b4eb7de205199ed/src/Microsoft.AspNetCore.Hosting/Internal/RequestServicesContainerMiddleware.cs#L17) that will be used later during the request to create the request lifetime scope.

Unfortunately, that means the `IServiceScopeFactory` is created/resolved at the point when the request comes in, long before an `HttpContext` is set in any `IHttpContextAccessor`. The result is the scope factory ends up coming from the default tenant scope, before a tenant can be identified, and per-request services will later all come from the default tenant. Multitenancy fails.

This package provides a different request services middleware that ensures the `IHttpContextAccessor.HttpContext` is set and defers creation of the request lifetime scope until as late as possible so anything needed for tenant identification can be established.

## Reference

- [Documentation for Autofac.Multitenant](http://autofac.readthedocs.io/en/latest/advanced/multitenant.html)
- [Documentation for ASP.NET Core Integration](http://autofac.readthedocs.io/en/latest/integration/aspnetcore.html)
- [Original explanation of why multitenancy failed in ASP.NET Core](https://stackoverflow.com/questions/38940241/autofac-multitenant-in-an-aspnet-core-application-does-not-seem-to-resolve-tenan/38960122#38960122)
- [NuGet](https://www.nuget.org/packages/Autofac.AspNetCore.Multitenant)
- [Contributing](http://autofac.readthedocs.io/en/latest/contributors.html)
