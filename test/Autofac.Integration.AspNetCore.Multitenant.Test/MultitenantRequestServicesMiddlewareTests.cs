using System;
using System.Threading.Tasks;
using Autofac.Extensions.DependencyInjection;
using Autofac.Multitenant;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace Autofac.Integration.AspNetCore.Multitenant.Test
{
    public class MultitenantRequestServicesMiddlewareTests
    {
        [Fact]
        public async Task Invoke_DoesNotOverrideExistingHttpContextOnAccessor()
        {
            var accessor = Mock.Of<IHttpContextAccessor>();
            accessor.HttpContext = new DefaultHttpContext();
            var mtc = CreateContainer();
            var next = new RequestDelegate(ctx => Task.FromResult(0));
            var context = CreateContext();

            var mw = new MultitenantRequestServicesMiddleware(next, () => mtc, accessor);
            await mw.Invoke(context);
            Assert.NotSame(context, accessor.HttpContext);
        }

        [Fact]
        public async Task Invoke_NoMultitenantContainer()
        {
            var accessor = Mock.Of<IHttpContextAccessor>();
            var next = new RequestDelegate(ctx => Task.FromResult(0));
            var context = CreateContext();

            var mw = new MultitenantRequestServicesMiddleware(next, () => null, accessor);
            await Assert.ThrowsAsync<InvalidOperationException>(() => mw.Invoke(context));
        }

        [Fact]
        public async Task Invoke_ReplacesRequestServices()
        {
            var accessor = Mock.Of<IHttpContextAccessor>();
            var mtc = CreateContainer();
            var originalFeature = Mock.Of<IServiceProvidersFeature>();
            var next = new RequestDelegate(ctx =>
            {
                // When the next delegate is invoked, it should get
                // an updated request services feature.
                var currentFeature = ctx.Features.Get<IServiceProvidersFeature>();
                Assert.NotSame(originalFeature, currentFeature);
                return Task.FromResult(0);
            });
            var context = CreateContext();
            context.Features.Set<IServiceProvidersFeature>(originalFeature);

            var mw = new MultitenantRequestServicesMiddleware(next, () => mtc, accessor);
            await mw.Invoke(context);

            // The original request services feature should have been replaced
            // after the middleware finished.
            var revertedFeature = context.Features.Get<IServiceProvidersFeature>();
            Assert.Same(originalFeature, revertedFeature);
        }

        [Fact]
        public async Task Invoke_SetsHttpContextOnAccessor()
        {
            var accessor = Mock.Of<IHttpContextAccessor>();
            var mtc = CreateContainer();
            var next = new RequestDelegate(ctx => Task.FromResult(0));
            var context = CreateContext();

            var mw = new MultitenantRequestServicesMiddleware(next, () => mtc, accessor);
            await mw.Invoke(context);
            Assert.Same(context, accessor.HttpContext);
        }

        private static MultitenantContainer CreateContainer()
        {
            var builder = new ContainerBuilder();
            builder.Populate(new ServiceCollection());
            var mtc = new MultitenantContainer(Mock.Of<ITenantIdentificationStrategy>(), builder.Build());
            return mtc;
        }

        private static DefaultHttpContext CreateContext()
        {
            var context = new DefaultHttpContext();
            context.Features.Set<IHttpResponseFeature>(new TestHttpResponseFeature());
            return context;
        }

        private class TestHttpResponseFeature : HttpResponseFeature
        {
            public override void OnCompleted(Func<object, Task> callback, object state)
            {
                // ASP.NET Core 1.1.x throws in the base/default feature.
            }

            public override void OnStarting(Func<object, Task> callback, object state)
            {
                // ASP.NET Core 1.1.x throws in the base/default feature.
            }
        }
    }
}
