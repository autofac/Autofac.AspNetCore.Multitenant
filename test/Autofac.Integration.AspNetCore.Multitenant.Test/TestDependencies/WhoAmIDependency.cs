namespace Autofac.Integration.AspNetCore.Multitenant.Test.TestDependencies
{
    public sealed class WhoAmIDependency
    {
        public string Id { get; }

        public WhoAmIDependency(string id)
        {
            Id = id;
        }
    }
}
