// Copyright (c) Autofac Project. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Net;
using Autofac.Integration.AspNetCore.Multitenant.Test.TestDependencies;

namespace Autofac.Integration.AspNetCore.Multitenant.Test;

public sealed class HostedMultitenancyTests : IClassFixture<TestServerFixture>
{
    private readonly TestServerFixture _testServerFixture;

    public HostedMultitenancyTests(TestServerFixture testServerFixture)
    {
        _testServerFixture = testServerFixture;
    }

    [Fact]
    public async Task CallRootEndpoint_HasTheCorrectDependenciesAndResponseIsBase()
    {
        var client = _testServerFixture.GetApplicationClient();

        var response = await client.GetAsync("root-endpoint");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("base", await response.Content.ReadAsStringAsync());
    }

    [Theory]
    [InlineData("")]
    [InlineData("a")]
    [InlineData("b")]
    public async Task CallScopedEndpoint_TwoCallsFuncResolveCalledTwice_DependencyIdNotEqual(string tenantQuery)
    {
        var client = _testServerFixture.GetApplicationClient();

        var resultA = await client.GetAsync($"scoped-endpoint?tenant={tenantQuery}");
        var resultB = await client.GetAsync($"scoped-endpoint?tenant={tenantQuery}");

        Assert.Equal(HttpStatusCode.OK, resultA.StatusCode);
        Assert.Equal(HttpStatusCode.OK, resultB.StatusCode);
        Assert.NotEqual(await resultA.Content.ReadAsStringAsync(), await resultB.Content.ReadAsStringAsync());
    }

    [Theory]
    [InlineData("a", "a")]
    [InlineData("b", "b")]
    public async Task CallTenantEndpoint_HasTheCorrectDependenciesAndResponseIsTenantItself(string tenantQuery, string expectedTenantId)
    {
        var client = _testServerFixture.GetApplicationClient();

        var response = await client.GetAsync($"tenant-endpoint?tenant={tenantQuery}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(expectedTenantId, await response.Content.ReadAsStringAsync());
    }

    [Theory]
    [InlineData("tenant-does-not-exist")]
    public async Task CallTenantEndpoint_WithNonExistentTenantReturns404(string tenantQuery)
    {
        var client = _testServerFixture.GetApplicationClient();

        var response = await client.GetAsync($"tenant-endpoint?tenant={tenantQuery}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Theory]
    [InlineData("", "base")]
    [InlineData("wrong-tenant", "base")]
    [InlineData("a", "a")]
    [InlineData("b", "b")]
    public async Task CallGenericEndpoint_HasTheCorrectDependenciesAndResponseIsTenantOrBase(string tenantQuery, string expectedTenantId)
    {
        var client = _testServerFixture.GetApplicationClient();

        var response = await client.GetAsync($"supports-with-and-without-tenant?tenant={tenantQuery}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(expectedTenantId, await response.Content.ReadAsStringAsync());
    }
}
