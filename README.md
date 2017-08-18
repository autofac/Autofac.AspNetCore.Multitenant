# Autofac.AspNetCore.Multitenant

ASP.NET Core support for multitenant DI via [Autofac.Multitenant](https://github.com/autofac/Autofac.Multitenant).

The primary thing here is the ability to ensure a request lifetime scope is created after the `HttpContext` has been established and a tenant can be identified.

[![Build status](https://ci.appveyor.com/api/projects/status/9120t73i97ywdoav?svg=true)](https://ci.appveyor.com/project/Autofac/autofac-aspnetcore-multitenant)

Please file issues and pull requests for this package in this repository rather than in the Autofac core repo.

- [Documentation for Autofac.Multitenant](http://autofac.readthedocs.io/en/latest/advanced/multitenant.html)
- [Documentation for ASP.NET Core Integration](http://autofac.readthedocs.io/en/latest/integration/aspnetcore.html)
- [NuGet](https://www.nuget.org/packages/Autofac.AspNetCore.Multitenant)
- [Contributing](http://autofac.readthedocs.io/en/latest/contributors.html)
