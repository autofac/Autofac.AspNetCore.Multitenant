// Copyright (c) Autofac Project. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microsoft.AspNetCore.Hosting;

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
    /// <returns>The existing <see cref="IWebHostBuilder"/> instance.</returns>
    /// <exception cref="System.ArgumentNullException">
    /// Thrown if <paramref name="builder" /> is <see langword="null" />.
    /// </exception>
    public static IWebHostBuilder UseAutofacMultitenantRequestServices(this IWebHostBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        return builder.ConfigureServices(services =>
        {
            services.AddAutofacMultitenantRequestServices();
        });
    }
}
