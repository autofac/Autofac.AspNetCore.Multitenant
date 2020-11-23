using System;
using Microsoft.Extensions.Logging;

namespace Sandbox.Shared
{
    public class Dependency : IDependency, IDisposable
    {
        private readonly ILogger<Dependency> _logger;

        public Dependency(ILogger<Dependency> logger)
        {
            _logger = logger;
        }

        public string Id { get; set; }

        public void Dispose()
        {
            _logger.LogInformation("Disposing dependency '{id}'.", Id);
        }
    }
}
