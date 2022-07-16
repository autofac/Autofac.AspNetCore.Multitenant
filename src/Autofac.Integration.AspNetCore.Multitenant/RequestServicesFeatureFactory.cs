// Copyright (c) Autofac Project. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;

namespace Autofac.Integration.AspNetCore.Multitenant
{
    /// <summary>
    /// The factory that is responsible for creating the <see cref="IServiceProvidersFeature"/>.
    /// </summary>
    internal static class RequestServicesFeatureFactory
    {
        // private static readonly Lazy<Func<HttpContext, IServiceScopeFactory, IServiceProvidersFeature>> FactoryFunction = new Lazy<Func<HttpContext, IServiceScopeFactory, IServiceProvidersFeature>>(GenerateLambda);

        /// <summary>
        /// The function that creates the <see cref="IServiceProvidersFeature"/> based on the framework version.
        /// </summary>
        /// <param name="context">The <see cref="HttpContext"/>.</param>
        /// <param name="scopeFactory">The <see cref="IServiceScopeFactory"/>.</param>
        /// <returns>An instance of <see cref="IServiceProvidersFeature"/>.</returns>
        public static IServiceProvidersFeature CreateFeature(HttpContext context, IServiceScopeFactory scopeFactory)
        {
            return new RequestServicesFeature(context, scopeFactory);
        }
    }
}
