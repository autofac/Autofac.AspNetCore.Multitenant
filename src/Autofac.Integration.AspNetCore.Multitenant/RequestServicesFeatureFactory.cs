// Copyright (c) Autofac Project. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Linq;
using System.Linq.Expressions;
using Autofac.Integration.AspNetCore.Multitenant.Properties;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;
#if NETSTANDARD2_0
using Microsoft.AspNetCore.Hosting.Internal;
#endif

namespace Autofac.Integration.AspNetCore.Multitenant
{
    /// <summary>
    /// The factory that is responsible for creating the <see cref="IServiceProvidersFeature"/>.
    /// </summary>
    internal static class RequestServicesFeatureFactory
    {
        private static readonly Lazy<Func<HttpContext, IServiceScopeFactory, IServiceProvidersFeature>> FactoryFunction = new Lazy<Func<HttpContext, IServiceScopeFactory, IServiceProvidersFeature>>(GenerateLambda);

        /// <summary>
        /// The function that creates the <see cref="IServiceProvidersFeature"/> based on the framework version.
        /// </summary>
        /// <param name="context">The <see cref="HttpContext"/>.</param>
        /// <param name="scopeFactory">The <see cref="IServiceScopeFactory"/>.</param>
        /// <returns>An instance of <see cref="IServiceProvidersFeature"/>.</returns>
        public static IServiceProvidersFeature CreateFeature(HttpContext context, IServiceScopeFactory scopeFactory)
        {
            return FactoryFunction.Value.Invoke(context, scopeFactory);
        }

        private static Func<HttpContext, IServiceScopeFactory, IServiceProvidersFeature> GenerateLambda()
        {
            var constructor = typeof(RequestServicesFeature).GetConstructors().OrderByDescending(ci => ci.GetParameters().Length).First();
            var parameterCount = constructor.GetParameters().Length;
            var contextParameter = Expression.Parameter(typeof(HttpContext), "context");
            var factoryParameter = Expression.Parameter(typeof(IServiceScopeFactory), "factory");
            switch (parameterCount)
            {
                case 1:
                    // ASP.NET Core through 2.1 has RequestServicesFeature ctor only with scope factory.
                    var subExpression = Expression.Lambda<Func<IServiceScopeFactory, IServiceProvidersFeature>>(
                        Expression.Convert(
                            Expression.New(
                                constructor,
                                factoryParameter),
                            typeof(IServiceProvidersFeature)),
                        factoryParameter)
                        .Compile();
                    return (ctx, factory) => subExpression(factory);
                case 2:
                    // ASP.NET Core 2.1 changed RequestServicesFeature ctor to take context and scope factory.
                    return Expression.Lambda<Func<HttpContext, IServiceScopeFactory, IServiceProvidersFeature>>(
                        Expression.Convert(
                            Expression.New(
                                constructor,
                                contextParameter,
                                factoryParameter),
                            typeof(IServiceProvidersFeature)),
                        contextParameter,
                        factoryParameter)
                        .Compile();
                default:
                    throw new NotSupportedException(Resources.NoSupportedRequestServicesConstructorFound);
            }
        }
    }
}
