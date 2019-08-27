// This software is part of the Autofac IoC container
// Copyright © 2019 Autofac Contributors
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
using Autofac.Extensions.DependencyInjection;
using Autofac.Multitenant;

namespace Autofac.Integration.AspNetCore.Multitenant
{
    /// <summary>
    /// Autofac implementation of the ASP.NET Core <see cref="IServiceProvider"/> for a <see cref="MultitenantContainer" />.
    /// </summary>
    /// <seealso cref="IServiceProvider" />
    public sealed class AutofacMultitenantServiceProvider : AutofacServiceProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AutofacMultitenantServiceProvider"/> class.
        /// </summary>
        /// <param name="lifetimeScope">The <see cref="ILifetimeScope"/> in form of a <see cref="MultitenantContainer"/>.</param>
        public AutofacMultitenantServiceProvider(ILifetimeScope lifetimeScope)
            : base(lifetimeScope)
        {
        }
    }
}
