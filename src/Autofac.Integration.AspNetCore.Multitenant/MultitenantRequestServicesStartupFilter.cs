using System;
using Autofac.Multitenant;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

namespace Autofac.Integration.AspNetCore.Multitenant
{
    /// <summary>
    /// ASP.NET startup filter that adds the <see cref="MultitenantRequestServicesMiddleware"/>
    /// to the request pipeline. Ensure this runs before all other startup filters
    /// or the multitenant request services may not happen until after the standard
    /// request services middleware has already run.
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Hosting.IStartupFilter" />
    internal class MultitenantRequestServicesStartupFilter : IStartupFilter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MultitenantRequestServicesStartupFilter"/> class.
        /// </summary>
        /// <param name="multitenantContainerAccessor">A function that will access the multitenant container from which request lifetimes should be generated.</param>
        public MultitenantRequestServicesStartupFilter(Func<MultitenantContainer> multitenantContainerAccessor)
        {
            this.MultitenantContainerAccessor = multitenantContainerAccessor;
        }

        /// <summary>
        /// Gets the multitenant container accessor.
        /// </summary>
        /// <value>
        /// A function that will access the multitenant container from which request lifetimes should be generated.
        /// </value>
        public Func<MultitenantContainer> MultitenantContainerAccessor { get; private set; }

        /// <summary>
        /// Adds the multitenant request services middleware to the app pipeline.
        /// </summary>
        /// <param name="next">
        /// The next middleware registration method that should execute.
        /// </param>
        /// <returns>
        /// The <see cref="Action{T}"/> for continued configuration or execution.
        /// </returns>
        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            return builder =>
            {
                builder.UseMiddleware<MultitenantRequestServicesMiddleware>(this.MultitenantContainerAccessor);
                next(builder);
            };
        }
    }
}
