using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using ProjectTracker.Web;
using Xunit;

namespace ProjectTracker.Tests
{
    public class CultureMiddlewareTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public CultureMiddlewareTests(WebApplicationFactory<Program> factory)
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
