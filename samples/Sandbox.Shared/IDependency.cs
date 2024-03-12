// Copyright (c) Autofac Project. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Sandbox;

/// <summary>
/// Simple dependency interface to provide a way to differentiate between components.
/// </summary>
public interface IDependency
{
    /// <summary>
    /// Gets a unique identifier for the component.
    /// </summary>
    /// <value>
    /// A <see cref="string"/> that can be used to illustrate which dependency
    /// is being resolved by a given tenant.
    /// </value>
    string Id { get; }
}
