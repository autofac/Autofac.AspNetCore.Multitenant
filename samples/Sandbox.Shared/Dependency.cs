// Copyright (c) Autofac Project. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.Extensions.Logging;

namespace Sandbox;

public class Dependency : IDependency, IDisposable
{
    private readonly ILogger<Dependency> _logger;
    private bool _disposedValue;

    public Dependency(ILogger<Dependency> logger)
    {
        _logger = logger;
    }

    public string Id { get; set; }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _logger.LogInformation("Disposing dependency '{Id}'.", Id);
            }

            _disposedValue = true;
        }
    }
}
