using System;

namespace Autofac.Integration.AspNetCore.Multitenant.Test.TestDependencies
{
    public class ScopedDependency : IScopedDependency
    {
        public Guid Id { get; }

        public ScopedDependency()
        {
            this.Id = Guid.NewGuid();
        }
    }
}
