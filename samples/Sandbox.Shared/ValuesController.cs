// Copyright (c) Autofac Project. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Mvc;

namespace Sandbox;

/// <summary>
/// API controller allowing a dependency to be resolved to illustrate multitenant dependency support.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ValuesController : ControllerBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ValuesController"/> class.
    /// </summary>
    /// <param name="dependency">
    /// The <see cref="IDependency"/> that will be returned in the GET method of the controller.
    /// </param>
    public ValuesController(IDependency dependency)
    {
        Dependency = dependency;
    }

    /// <summary>
    /// Gets the tenant-specific resolved dependency.
    /// </summary>
    /// <value>
    /// An <see cref="IDependency"/> that will have been resolved from a
    /// tenant-specific lifetime scope based on the <c>tenant</c> query string
    /// parameter.
    /// </value>
    public IDependency Dependency { get; }

    /// <summary>
    /// Gets the ID of the resolved dependency.
    /// </summary>
    /// <returns>
    /// The <see cref="IDependency.Id"/> for the dependency that was resolved
    /// for the tenant. Change the <c>tenant</c> query string parameter to get
    /// the value for different tenants.
    /// </returns>
    [HttpGet]
    public string Get()
    {
        return Dependency.Id;
    }
}
