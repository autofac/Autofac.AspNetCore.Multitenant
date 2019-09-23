using Autofac;

namespace Sandbox.Shared
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