using Autofac.Multitenant;
using Microsoft.AspNetCore.Http;

namespace Autofac.Integration.AspNetCore.Multitenant.Test.TestDependencies
{
    public sealed class TestableTenantIdentificationStrategy : ITenantIdentificationStrategy
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TestableTenantIdentificationStrategy(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public bool TryIdentifyTenant(out object tenantId)
        {
            tenantId = null!;

            if (this._httpContextAccessor.HttpContext is null)
                return false;

            if (!_httpContextAccessor.HttpContext.Request.Query.TryGetValue("tenant", out var tenantValues))
                return false;

            tenantId = tenantValues[0];
            return true;
        }
    }
}
