namespace Autofac.Integration.AspNetCore.Multitenant.Test.TestDependencies
{
    public sealed class WhoAmIDependency
    {
        public WhoAmIDependency(string id)
        {
            Id = id;
        }

        public string Id { get; }
    }
}
