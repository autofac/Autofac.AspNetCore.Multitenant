using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace Sandbox
{
    public class Dependency : IDependency, IDisposable
    {
        private readonly ILogger<Dependency> _logger;

        public Dependency(ILogger<Dependency> logger)
        {
            this._logger = logger;
        }

        public string Id { get; set; }

        public void Dispose()
        {
            this._logger.LogInformation("Disposing dependency '{id}'.", this.Id);
        }
    }
}
