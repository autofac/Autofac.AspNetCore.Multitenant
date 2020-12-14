// Copyright (c) Autofac Project. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Autofac;

namespace Sandbox
{
    public static class ContainerSetup
    {
        public static void SetupContainer(ContainerBuilder builder)
        {
            builder.RegisterType<Dependency>()
                .As<IDependency>()
                .WithProperty("Id", "base")
                .InstancePerLifetimeScope();
        }
    }
}