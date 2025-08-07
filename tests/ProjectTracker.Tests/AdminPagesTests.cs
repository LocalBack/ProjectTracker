using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using AdminMarker = ProjectTracker.Admin.AdminAssemblyMarker;

namespace ProjectTracker.Tests;

public class AdminPagesTests : IClassFixture<CustomWebApplicationFactory<AdminMarker>>
{
    private readonly HttpClient _client;

    public AdminPagesTests(CustomWebApplicationFactory<AdminMarker> factory)
    {
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }

    [Fact]
    public async Task Index_RedirectsToLogin()
    {
        var response = await _client.GetAsync("/");
        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.Contains("/Identity/Account/Login", response.Headers.Location?.OriginalString);
    }

    [Fact]
    public async Task Dashboard_RedirectsToLogin()
    {
        var response = await _client.GetAsync("/Dashboard");
        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.Contains("/Identity/Account/Login", response.Headers.Location?.OriginalString);
    }

    [Fact]
    public async Task Projects_RedirectsToLogin()
    {
        var response = await _client.GetAsync("/Projects");
        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.Contains("/Identity/Account/Login", response.Headers.Location?.OriginalString);
    }

    [Fact]
    public async Task Users_RedirectsToLogin()
    {
        var response = await _client.GetAsync("/Users");
        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.Contains("/Identity/Account/Login", response.Headers.Location?.OriginalString);
    }

    [Fact]
    public async Task Tasks_RedirectsToLogin()
    {
        var response = await _client.GetAsync("/Tasks");
        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.Contains("/Identity/Account/Login", response.Headers.Location?.OriginalString);
    }
}
