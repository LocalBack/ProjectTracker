using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using WebMarker = ProjectTracker.Web.WebAssemblyMarker;

namespace ProjectTracker.Tests
{
    public class CultureMiddlewareTests : IClassFixture<CustomWebApplicationFactory<WebMarker>>
    {
        private readonly CustomWebApplicationFactory<WebMarker> _factory;

        public CultureMiddlewareTests(CustomWebApplicationFactory<WebMarker> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task AcceptLanguageHeader_SetsUiCulture()
        {
            var client = _factory.CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Get, "/");
            request.Headers.Add("Accept-Language", "en-US");
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var html = await response.Content.ReadAsStringAsync();
            Assert.Contains("<html lang=\"en-US\"", html);
        }
    }
}
