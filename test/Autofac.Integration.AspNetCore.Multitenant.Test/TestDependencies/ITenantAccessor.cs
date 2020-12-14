// Copyright (c) Autofac Project. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Autofac.Integration.AspNetCore.Multitenant.Test.TestDependencies
{
    public interface ITenantAccessor
    {
        string? CurrentTenant { get; }
    }
}
