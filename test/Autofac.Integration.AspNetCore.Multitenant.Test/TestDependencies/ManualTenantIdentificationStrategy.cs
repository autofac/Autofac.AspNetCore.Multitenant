// Copyright (c) Autofac Project. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Autofac.Multitenant;

namespace Autofac.Integration.AspNetCore.Multitenant.Test.TestDependencies;

public sealed class ManualTenantIdentificationStrategy : ITenantIdentificationStrategy
{
    public object? TenantId { get; set; }

    public bool TryIdentifyTenant(out object tenantId)
    {
        tenantId = null!;
        if (TenantId is null)
        {
            return false;
        }

        tenantId = TenantId;
        return true;
    }
}
