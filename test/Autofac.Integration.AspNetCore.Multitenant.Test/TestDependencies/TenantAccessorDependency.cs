using Autofac.Multitenant;

namespace Autofac.Integration.AspNetCore.Multitenant.Test.TestDependencies
{
    public sealed class TenantAccessorDependency : ITenantAccessor
    {
        public string CurrentTenant { get; }

        public TenantAccessorDependency(ITenantIdentificationStrategy tenantIdentificationStrategy)
        {
            if (tenantIdentificationStrategy.TryIdentifyTenant(out var tenantId) &&
                tenantId is string currentTenant)
            {
                CurrentTenant = currentTenant;
            }
        }
    }
}
