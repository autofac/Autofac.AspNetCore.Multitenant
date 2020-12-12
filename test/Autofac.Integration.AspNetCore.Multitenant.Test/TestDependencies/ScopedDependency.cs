using System;

namespace Autofac.Integration.AspNetCore.Multitenant.Test.TestDependencies
{
    public class ScopedDependency : IScopedDependency
    {
        public ScopedDependency()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; }
    }
}
