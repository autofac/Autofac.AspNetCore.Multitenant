// Copyright (c) Autofac Project. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Autofac.Multitenant;

namespace Autofac.Integration.AspNetCore.Multitenant.Test.TestDependencies;

public sealed class TenantAccessorDependency : ITenantAccessor
{
    public TenantAccessorDependency(ITenantIdentificationStrategy tenantIdentificationStrategy)
    {
        if (tenantIdentificationStrategy == null)
        {
            throw new ArgumentNullException(nameof(tenantIdentificationStrategy));
        }

        if (tenantIdentificationStrategy.TryIdentifyTenant(out var tenantId) &&
            tenantId is string currentTenant)
        {
            CurrentTenant = currentTenant;
        }
    }

    public string? CurrentTenant { get; }
}
