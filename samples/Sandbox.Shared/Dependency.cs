// Copyright (c) Autofac Project. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.Extensions.Logging;

namespace Sandbox;

/// <summary>
/// Simple dependency used to illustrate multitenant overrides.
/// </summary>
public sealed class Dependency : IDependency, IDisposable
{
    private static readonly Action<ILogger, string, Exception?> LogDisposal = LoggerMessage.Define<string>(LogLevel.Information, new EventId(0), "Disposing dependency '{Id}'.");

    private readonly ILogger<Dependency> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="Dependency"/> class.
    /// </summary>
    /// <param name="logger">
    /// The logger to use for diagnostic messages.
    /// </param>
    public Dependency(ILogger<Dependency> logger)
    {
        _logger = logger;
        Id = "";
    }

    /// <summary>
    /// Gets or sets a unique identifier for the component.
    /// </summary>
    /// <value>
    /// A <see cref="string"/> that can be used to illustrate which dependency
    /// is being resolved by a given tenant.
    /// </value>
    public string Id { get; set; }

    /// <summary>
    /// Disposes the component. This will be called when the owning lifetime
    /// scope is disposed.
    /// </summary>
    public void Dispose()
    {
        LogDisposal(_logger, Id, null);
    }
}
