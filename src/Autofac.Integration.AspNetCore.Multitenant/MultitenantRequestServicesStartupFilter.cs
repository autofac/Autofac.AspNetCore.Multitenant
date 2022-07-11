// Copyright (c) Autofac Project. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

namespace Autofac.Integration.AspNetCore.Multitenant;

/// <summary>
/// ASP.NET startup filter that adds the <see cref="MultitenantRequestServicesMiddleware"/>
/// to the request pipeline. Ensure this runs before all other startup filters
/// or the multitenant request services may not happen until after the standard
/// request services middleware has already run.
/// </summary>
/// <seealso cref="IStartupFilter" />
internal class MultitenantRequestServicesStartupFilter : IStartupFilter
{
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
            builder.UseMiddleware<MultitenantRequestServicesMiddleware>();
            next(builder);
        };
    }
}
