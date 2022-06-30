// Copyright (c) Autofac Project. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.Extensions.DependencyInjection;

namespace Autofac.Integration.AspNetCore.Multitenant
{
    /// <summary>
    /// An adapter that wraps the instance of <see cref="IServiceScopeFactory"/> for a specific tenant.
    /// </summary>
    public class MultitenantServiceScopeFactoryAdapter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MultitenantServiceScopeFactoryAdapter"/> class.
        /// </summary>
        /// <param name="factory">The <see cref="IServiceScopeFactory"/> for a specific tenant.</param>
        public MultitenantServiceScopeFactoryAdapter(IServiceScopeFactory factory)
        {
            Factory = factory;
        }

        /// <summary>
        /// Gets the <see cref="IServiceScopeFactory"/> for a specific tenant.
        /// </summary>
        public IServiceScopeFactory Factory { get; }
    }
}
