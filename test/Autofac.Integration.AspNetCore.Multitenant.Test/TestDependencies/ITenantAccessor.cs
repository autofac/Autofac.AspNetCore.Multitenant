namespace Autofac.Integration.AspNetCore.Multitenant.Test.TestDependencies
{
    public interface ITenantAccessor
    {
        string CurrentTenant { get; }
    }
}