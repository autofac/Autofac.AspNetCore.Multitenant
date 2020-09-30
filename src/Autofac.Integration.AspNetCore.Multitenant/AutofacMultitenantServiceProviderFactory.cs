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
using System.Collections.Generic;
using System.Reflection;
using Autofac;
using Autofac.Builder;
using Autofac.Extensions.DependencyInjection;
using Autofac.Integration.AspNetCore.Multitenant;
using Autofac.Integration.AspNetCore.Multitenant.Properties;
using Autofac.Multitenant;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.Hosting
{
    /// <summary>
    /// A factory for creating a <see cref="ContainerBuilder"/> and an <see cref="IServiceProvider" /> for usage with a <see cref="MultitenantContainer" /> in ASP.NET Core.
    /// </summary>
    public class AutofacMultitenantServiceProviderFactory : IServiceProviderFactory<ContainerBuilder>
    {
        private readonly Action<ContainerBuilder> _configurationAction;
        private readonly Func<IContainer, MultitenantContainer> _multitenantContainerAccessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="AutofacMultitenantServiceProviderFactory"/> class.
        /// </summary>
        /// <param name="multitenantContainerAccessor">A function that will access the multitenant container from which request lifetimes should be generated.</param>
        /// <param name="configurationAction">Action on a <see cref="ContainerBuilder"/> that adds component registrations to the conatiner.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// Thrown if <paramref name="multitenantContainerAccessor" /> is <see langword="null" />.
        public AutofacMultitenantServiceProviderFactory(Func<IContainer, MultitenantContainer> multitenantContainerAccessor, Action<ContainerBuilder> configurationAction = null)
        {
            this._multitenantContainerAccessor = multitenantContainerAccessor ?? throw new ArgumentNullException(nameof(multitenantContainerAccessor));
            this._configurationAction = configurationAction ?? (builder => { });
        }

        /// <summary>
        /// Creates a container builder from an <see cref="IServiceCollection" />.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <returns>A container builder that can be used to create an <see cref="IServiceProvider" />.</returns>
        public ContainerBuilder CreateBuilder(IServiceCollection services)
        {
            var builder = new ContainerBuilder();

            builder.Populate(services);

            this._configurationAction(builder);

            return builder;
        }

        /// <summary>
        /// Creates an <see cref="IServiceProvider" /> from the container builder.
        /// </summary>
        /// <param name="containerBuilder">The container builder.</param>
        /// <returns>An <see cref="IServiceProvider" />.</returns>
        public IServiceProvider CreateServiceProvider(ContainerBuilder containerBuilder)
        {
            if (containerBuilder == null) throw new ArgumentNullException(nameof(containerBuilder));

            MultitenantContainer multitenantContainer = null;

            containerBuilder.Register(_ => multitenantContainer)
              .AsSelf()
              .ExternallyOwned();

            multitenantContainer = this._multitenantContainerAccessor(containerBuilder.Build());

            if (multitenantContainer == null) throw new InvalidOperationException(Resources.NoMultitenantContainerAvailable);

            return new AutofacServiceProvider(multitenantContainer);
        }
    }
}
