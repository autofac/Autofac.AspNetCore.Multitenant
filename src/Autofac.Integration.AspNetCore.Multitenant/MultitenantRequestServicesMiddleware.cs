using System;
using System.Threading.Tasks;
using Autofac.Multitenant;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;

namespace Autofac.Integration.AspNetCore.Multitenant
{
    /// <summary>
    /// Middleware that forces the request lifetime scope to be created from the root lifetime scope
    /// lazily so any contextual information (e.g., tenant identification) can be used if needed.
    /// </summary>
    internal class MultitenantRequestServicesMiddleware
    {
        private readonly IHttpContextAccessor _contextAccessor;

        private readonly Func<MultitenantContainer> _multitenantContainerAccessor;

        private readonly RequestDelegate _next;

        public MultitenantRequestServicesMiddleware(RequestDelegate next, Func<MultitenantContainer> multitenantContainerAccessor, IHttpContextAccessor contextAccessor)
        {
            this._next = next;
            this._multitenantContainerAccessor = multitenantContainerAccessor;
            this._contextAccessor = contextAccessor;
        }

        public async Task Invoke(HttpContext context)
        {
            if (this._contextAccessor.HttpContext == null)
            {
                this._contextAccessor.HttpContext = context;
            }

            var existingFeature = context.Features.Get<IServiceProvidersFeature>();
            using (var feature = new RequestServicesFeature(this._multitenantContainerAccessor().Resolve<IServiceScopeFactory>()))
            {
                try
                {
                    context.Features.Set<IServiceProvidersFeature>(feature);
                    await this._next.Invoke(context);
                }
                finally
                {
                    context.Features.Set(existingFeature);
                }
            }
        }
    }
}
