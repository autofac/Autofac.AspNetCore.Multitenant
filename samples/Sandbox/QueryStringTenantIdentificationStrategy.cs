using System;
using System.Linq;
using Autofac.Multitenant;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Sandbox
{
    public class QueryStringTenantIdentificationStrategy : ITenantIdentificationStrategy
    {
        public QueryStringTenantIdentificationStrategy(IHttpContextAccessor accessor)
        {
            this.Accessor = accessor;
        }

        public IHttpContextAccessor Accessor { get; private set; }

        public bool TryIdentifyTenant(out object tenantId)
        {
            var context = this.Accessor.HttpContext;
            if (context == null)
            {
                tenantId = null;
                return false;
            }

            StringValues tenantValues;
            if (context.Request.Query.TryGetValue("tenant", out tenantValues))
            {
                tenantId = tenantValues[0];
                return true;
            }

            tenantId = null;
            return false;
        }
    }
}
