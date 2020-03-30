using System;

namespace Autofac.Integration.AspNetCore.Multitenant.Test.TestDependencies
{
    public interface IScopedDependency
    {
        Guid Id { get; }
    }
}
