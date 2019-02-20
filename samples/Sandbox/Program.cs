using System;
using System.Linq;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

namespace Sandbox
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = WebHost.CreateDefaultBuilder(args)

                // This enables the request lifetime scope to be properly spawned from
                // the container rather than be a child of the default tenant scope.
                .UseAutofacMultitenantRequestServices(() => Startup.ApplicationContainer)
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }
}
