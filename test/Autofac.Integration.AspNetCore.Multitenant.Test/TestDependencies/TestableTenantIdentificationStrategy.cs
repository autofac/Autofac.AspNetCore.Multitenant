// Copyright (c) Autofac Project. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Autofac.Multitenant;
using Microsoft.AspNetCore.Http;

namespace Autofac.Integration.AspNetCore.Multitenant.Test.TestDependencies;

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
        var context = _httpContextAccessor.HttpContext;
        if (context is null || !context.Request.Query.TryGetValue("tenant", out var tenantValues))
        {
            return false;
        }

        tenantId = tenantValues[0];
        return true;
    }
}
