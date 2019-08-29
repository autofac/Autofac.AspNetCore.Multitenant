// This software is part of the Autofac IoC container
// Copyright © 2017 Autofac Contributors
// http://autofac.org
//
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.

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
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.Insert(0, ServiceDescriptor.Transient<IStartupFilter>(provider => new MultitenantRequestServicesStartupFilter()));
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            return services;
        }
    }
}