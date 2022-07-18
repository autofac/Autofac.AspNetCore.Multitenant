// Copyright (c) Autofac Project. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Autofac.Extensions.DependencyInjection;
using Autofac.Multitenant;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;

namespace Autofac.Integration.AspNetCore.Multitenant.Test;

public class MultitenantRequestServicesMiddlewareTests
{
    [Fact]
    public async Task Invoke_DoesNotOverrideExistingHttpContextOnAccessor()
    {
        var accessor = Mock.Of<IHttpContextAccessor>();
        accessor.HttpContext = new DefaultHttpContext();
        var next = new RequestDelegate(ctx => Task.FromResult(0));
        var context = CreateContext();

        var mtc = CreateServiceProvider().GetRequiredService<MultitenantContainer>();

        var mw = new MultitenantRequestServicesMiddleware(next, accessor, mtc);
        await mw.Invoke(context);
        Assert.NotSame(context, accessor.HttpContext);
    }

    [Fact]
    public async Task Invoke_ReplacesRequestServices()
    {
        var accessor = Mock.Of<IHttpContextAccessor>();
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
        var mtc = CreateServiceProvider().GetRequiredService<MultitenantContainer>();
        var mw = new MultitenantRequestServicesMiddleware(next, accessor, mtc);

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
        var next = new RequestDelegate(ctx => Task.FromResult(0));
        var context = CreateContext();
        var mtc = CreateServiceProvider().GetRequiredService<MultitenantContainer>();
        var mw = new MultitenantRequestServicesMiddleware(next, accessor, mtc);

        await mw.Invoke(context);

        Assert.Same(context, accessor.HttpContext);
    }

    private static IServiceProvider CreateServiceProvider()
    {
        var factory = new AutofacMultitenantServiceProviderFactory(CreateContainer);
        var containerBuilder = new ContainerBuilder();
        var services = new ServiceCollection();
        containerBuilder.Populate(services);
        factory.CreateBuilder(services);
        return factory.CreateServiceProvider(containerBuilder);
    }

    private static MultitenantContainer CreateContainer(IContainer container)
    {
        var mtc = new MultitenantContainer(Mock.Of<ITenantIdentificationStrategy>(), container);
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
