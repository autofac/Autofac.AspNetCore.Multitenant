// This software is part of the Autofac IoC container
// Copyright © 2017 Autofac Contributors
// https://autofac.org
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
using Autofac.Integration.AspNetCore.Multitenant;
using Autofac.Multitenant;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.Hosting
{
    /// <summary>
    /// Extension methods for the <see cref="IWebHostBuilder"/> interface.
    /// </summary>
    public static class AutofacMultitenantWebHostBuilderExtensions
    {
        /// <summary>
        /// Adds the multitenant Autofac request services middleware, which ensures request lifetimes spawn from the container
        /// rather than a pre-resolved tenant lifetime scope. This allows tenant identification to occur at the time of request
        /// scope generation.
        /// </summary>
        /// <param name="builder">The <see cref="IWebHostBuilder"/> instance being configured.</param>
        /// <param name="multitenantContainerAccessor">A function that will access the multitenant container from which request lifetimes should be generated.</param>
        /// <returns>The existing <see cref="IWebHostBuilder"/> instance.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown if <paramref name="builder" /> or <paramref name="multitenantContainerAccessor" /> is <see langword="null" />.
        /// </exception>
        public static IWebHostBuilder UseAutofacMultitenantRequestServices(this IWebHostBuilder builder, Func<MultitenantContainer> multitenantContainerAccessor)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (multitenantContainerAccessor == null)
            {
                throw new ArgumentNullException(nameof(multitenantContainerAccessor));
            }

            var descriptor = new ServiceDescriptor(typeof(IStartupFilter), sp => new MultitenantRequestServicesStartupFilter(multitenantContainerAccessor), ServiceLifetime.Transient);
            return builder.ConfigureServices(services =>
            {
                services.Insert(0, descriptor);
                services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            });
        }
    }
}
