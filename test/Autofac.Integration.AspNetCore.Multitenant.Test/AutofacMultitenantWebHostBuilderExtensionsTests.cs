// Copyright (c) Autofac Project. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace Autofac.Integration.AspNetCore.Multitenant.Test
{
    public class AutofacMultitenantWebHostBuilderExtensionsTests
    {
        [Fact]
        public void UseAutofacMultitenantRequestServices_AddsHttpContextAccessor()
        {
            var webHostBuilder = new Mock<IWebHostBuilder>();

            var services = new ServiceCollection();
            webHostBuilder
                .Setup(x => x.ConfigureServices(It.IsAny<Action<IServiceCollection>>()))
                .Callback<Action<IServiceCollection>>(s => s(services));

            webHostBuilder.Object.UseAutofacMultitenantRequestServices();

            var serviceProvider = services.BuildServiceProvider();
            var accessor = serviceProvider.GetService<IHttpContextAccessor>();

            Assert.NotNull(accessor);
        }

        [Fact]
        public void UseAutofacMultitenantRequestServices_AddsStartupFilter()
        {
            var webHostBuilder = new Mock<IWebHostBuilder>();

            var services = new ServiceCollection();
            webHostBuilder
                .Setup(x => x.ConfigureServices(It.IsAny<Action<IServiceCollection>>()))
                .Callback<Action<IServiceCollection>>(s => s(services));

            webHostBuilder.Object.UseAutofacMultitenantRequestServices();

            var serviceProvider = services.BuildServiceProvider();
            var filter = serviceProvider.GetService<IStartupFilter>();

            Assert.IsType<MultitenantRequestServicesStartupFilter>(filter);
        }

        [Fact]
        public void UseAutofacMultitenantRequestServices_NullBuilder()
        {
            Assert.Throws<ArgumentNullException>(() => AutofacMultitenantWebHostBuilderExtensions.UseAutofacMultitenantRequestServices(null!));
        }
    }
}
