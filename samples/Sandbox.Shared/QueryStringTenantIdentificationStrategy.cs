// Copyright (c) Autofac Project. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Autofac.Multitenant;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;

namespace Sandbox;

/// <summary>
/// Tenant identification strategy that uses a query string parameter to determine the current tenant.
/// </summary>
public class QueryStringTenantIdentificationStrategy : ITenantIdentificationStrategy
{
    private static readonly Action<ILogger, object?, Exception?> LogTenantIdentified = LoggerMessage.Define<object?>(LogLevel.Information, new EventId(0), "Identified tenant: {Tenant}");
    private static readonly Action<ILogger, Exception?> LogNoTenantIdentity = LoggerMessage.Define(LogLevel.Warning, new EventId(1), "Unable to identify tenant from query string. Falling back to default.");
    private readonly ILogger<QueryStringTenantIdentificationStrategy> _logger;
    private readonly IHttpContextAccessor _accessor;

    /// <summary>
    /// Initializes a new instance of the <see cref="QueryStringTenantIdentificationStrategy"/> class.
    /// </summary>
    /// <param name="accessor">
    /// An <see cref="IHttpContextAccessor"/> that can provide the current request context.
    /// </param>
    /// <param name="logger">
    /// The logger for diagnostic messages.
    /// </param>
    public QueryStringTenantIdentificationStrategy(IHttpContextAccessor accessor, ILogger<QueryStringTenantIdentificationStrategy> logger)
    {
        _accessor = accessor;
        _logger = logger;
    }

    /// <summary>
    /// Attempts to identify the tenant from the current request query string.
    /// </summary>
    /// <param name="tenantId">
    /// The current tenant identifier.
    /// </param>
    /// <returns>
    /// <see langword="true" /> if the tenant could be identified; <see langword="false" />
    /// if not.
    /// </returns>
    public bool TryIdentifyTenant(out object? tenantId)
    {
        var context = _accessor.HttpContext;
        if (context == null)
        {
            // No current HttpContext. This happens during app startup
            // and isn't really an error, but is something to be aware of.
            tenantId = null;
            return false;
        }

        // Caching the value both speeds up tenant identification for
        // later and ensures we only see one log message indicating
        // relative success or failure for tenant ID.
        if (context.Items.TryGetValue("_tenantId", out tenantId))
        {
            // We've already identified the tenant at some point
            // so just return the cached value (even if the cached value
            // indicates we couldn't identify the tenant for this context).
            return tenantId != null;
        }

        if (context.Request.Query.TryGetValue("tenant", out StringValues tenantValues))
        {
            tenantId = tenantValues[0];
            context.Items["_tenantId"] = tenantId;
            LogTenantIdentified(_logger, tenantId, null);
            return true;
        }

        LogNoTenantIdentity(_logger, null);
        tenantId = null;
        context.Items["_tenantId"] = null;
        return false;
    }
}
