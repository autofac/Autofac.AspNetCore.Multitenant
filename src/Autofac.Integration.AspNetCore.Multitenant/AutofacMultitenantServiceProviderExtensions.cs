// This software is part of the Autofac IoC container
// Copyright © 2019 Autofac Contributors
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
using Autofac.Extensions.DependencyInjection;
using Autofac.Integration.AspNetCore.Multitenant.Properties;
using Autofac.Multitenant;

namespace Autofac.Integration.AspNetCore.Multitenant
{
    /// <summary>
    /// Extension methods for use with the <see cref="AutofacServiceProvider"/>.
    /// </summary>
    public static class AutofacMultitenantServiceProviderExtensions
    {
        /// <summary>
        /// Tries to cast the instance of <see cref="MultitenantContainer"/> from <see cref="AutofacServiceProvider"/> when possible.
        /// </summary>
        /// <param name="serviceProvider">The instance of <see cref="IServiceProvider"/>.</param>
        /// <returns>Returns the instance of <see cref="MultitenantContainer"/> exposed by <see cref="AutofacServiceProvider"/> when it can be casted down from <see cref="ILifetimeScope"/>.</returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown if <paramref name="serviceProvider" /> can't be casted to <see cref="MultitenantContainer"/>.
        /// </exception>
        [Obsolete("This function will be removed in a future release. Please use IServiceProvider.GetRequiredService<MultitenantContainer>() instead.")]
        public static MultitenantContainer GetAutofacMultitenantRoot(this IServiceProvider serviceProvider)
        {
            var root = serviceProvider.GetAutofacRoot();

            if (!root.IsRegistered<MultitenantContainer>())
                throw new InvalidOperationException(Resources.NoMultitenantContainerAvailable);

            return root.Resolve<MultitenantContainer>();
        }
    }
}