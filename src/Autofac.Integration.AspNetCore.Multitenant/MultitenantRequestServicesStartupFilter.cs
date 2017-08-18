using System;
using Autofac.Multitenant;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

namespace Autofac.Integration.AspNetCore.Multitenant
{
    internal class MultitenantRequestServicesStartupFilter : IStartupFilter
    {
        public MultitenantRequestServicesStartupFilter(Func<MultitenantContainer> multitenantContainerAccessor)
        {
            this.MultitenantContainerAccessor = multitenantContainerAccessor;
        }

        public Func<MultitenantContainer> MultitenantContainerAccessor { get; private set; }

        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            return builder =>
            {
                builder.UseMiddleware<MultitenantRequestServicesMiddleware>(this.MultitenantContainerAccessor);
                next(builder);
            };
        }
    }
}
