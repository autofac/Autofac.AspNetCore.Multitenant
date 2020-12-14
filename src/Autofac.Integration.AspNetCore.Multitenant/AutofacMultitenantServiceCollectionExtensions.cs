// Copyright (c) Autofac Project. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using Autofac;
using Autofac.Integration.AspNetCore.Multitenant;
using Autofac.Multitenant;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.Hosting
{
    /// <summary>
    /// Extension methods for the <see cref="IServiceCollection"/> interface.
    /// </summary>
    public static class AutofacMultitenantServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the multitenant Autofac request services middleware, which ensures request lifetimes spawn from the container
        /// rather than a pre-resolved tenant lifetime scope. This allows tenant identification to occur at the time of request
        /// scope generation.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> instance being configured.</param>
        /// <returns>The existing <see cref="IServiceCollection"/> instance.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown if <paramref name="services" /> is <see langword="null" />.
        /// </exception>
        public static IServiceCollection AddAutofacMultitenantRequestServices(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.Insert(0, ServiceDescriptor.Transient<IStartupFilter>(provider => new MultitenantRequestServicesStartupFilter()));
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            return services;
        }
    }
}