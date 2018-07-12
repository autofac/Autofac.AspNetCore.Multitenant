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
    /// Middleware that forces the request lifetime scope to be created from the multitenant container
    /// directly to avoid inadvertent incorrect tenant identification.
    /// </summary>
    internal class MultitenantRequestServicesMiddleware
    {
        private readonly IHttpContextAccessor _contextAccessor;

        private readonly Func<MultitenantContainer> _multitenantContainerAccessor;

        private readonly RequestDelegate _next;

        /// <summary>
        /// Initializes a new instance of the <see cref="MultitenantRequestServicesMiddleware"/> class.
        /// </summary>
        /// <param name="next">The next step in the request pipeline.</param>
        /// <param name="multitenantContainerAccessor">A function that will access the multitenant container from which request lifetimes should be generated.</param>
        /// <param name="contextAccessor">The <see cref="IHttpContextAccessor"/> to set up with the current request context.</param>
        public MultitenantRequestServicesMiddleware(RequestDelegate next, Func<MultitenantContainer> multitenantContainerAccessor, IHttpContextAccessor contextAccessor)
        {
            this._next = next;
            this._multitenantContainerAccessor = multitenantContainerAccessor;
            this._contextAccessor = contextAccessor;
        }

        /// <summary>
        /// Invokes the middleware using the specified context.
        /// </summary>
        /// <param name="context">
        /// The request context to process through the middleware.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> to await for completion of the operation.
        /// </returns>
        public async Task Invoke(HttpContext context)
        {
            // If there isn't already an HttpContext set on the context
            // accessor for this async/thread operation, set it. This allows
            // tenant identification to use it.
            if (this._contextAccessor.HttpContext == null)
            {
                this._contextAccessor.HttpContext = context;
            }

            var container = this._multitenantContainerAccessor();
            if (container == null)
            {
                throw new InvalidOperationException(Properties.Resources.NoMultitenantContainerAvailable);
            }

            IServiceProvidersFeature existingFeature = null;
            try
            {
                var autofacFeature = RequestServicesFeatureFactory.CreateFeature(context, container.Resolve<IServiceScopeFactory>());
                var disp = autofacFeature as IDisposable;
                if (disp != null)
                {
                    context.Response.RegisterForDispose(disp);
                }

                existingFeature = context.Features.Get<IServiceProvidersFeature>();
                context.Features.Set(autofacFeature);
                await this._next.Invoke(context);
            }
            finally
            {
                // In ASP.NET Core 1.x the existing feature will disposed as part of
                // a using statement; in ASP.NET Core 2.x it is registered directly
                // with the response for disposal. In either case, we don't have to
                // do that. We do put back any existing feature, though, since
                // at this point there may have been some default tenant or base
                // container level stuff resolved and after this middleware it needs
                // to be what it was before.
                context.Features.Set(existingFeature);
            }
        }
    }
}
