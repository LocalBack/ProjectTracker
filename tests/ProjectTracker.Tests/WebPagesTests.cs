using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using WebMarker = ProjectTracker.Web.WebAssemblyMarker;

namespace ProjectTracker.Tests;

public class WebPagesTests : IClassFixture<CustomWebApplicationFactory<WebMarker>>
{
    private readonly HttpClient _client;

    public WebPagesTests(CustomWebApplicationFactory<WebMarker> factory)
    {
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }

    [Fact]
    public async Task HomePage_ReturnsSuccess()
    {
        var response = await _client.GetAsync("/");
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task PrivacyPage_RedirectsToLogin()
    {
        var response = await _client.GetAsync("/Home/Privacy");
        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.Contains("/Account/Login", response.Headers.Location?.OriginalString);
    }

    [Fact]
    public async Task LoginPage_ReturnsSuccess()
    {
        var response = await _client.GetAsync("/Account/Login");
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task WorkLog_Index_RedirectsToLogin()
    {
        var response = await _client.GetAsync("/WorkLog");
        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.Contains("/Account/Login", response.Headers.Location?.OriginalString);
    }

    [Fact]
    public async Task NonExistingPage_RedirectsToLogin()
    {
        var response = await _client.GetAsync("/does-not-exist");
        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.Contains("/Account/Login", response.Headers.Location?.OriginalString);
    }
}
