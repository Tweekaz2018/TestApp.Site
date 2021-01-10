using Xunit;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using TestApp.Site;
using TestApp.Integration_Tests.Helpers;

namespace TestApp.Integration_Tests
{
    public class Basic_Access_Tests
        : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly CustomWebApplicationFactory<TestApp.Site.Startup> _factory;
        private readonly HttpClient _hc;

        public Basic_Access_Tests(CustomWebApplicationFactory<TestApp.Site.Startup> factory)
        {
            _factory = factory;
            _hc = factory.CreateClient(new Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
        }

        [Theory]
        [InlineData("/")]
        [InlineData("/Login/Login")]
        [InlineData("/Login/Register")]
        [InlineData("/Shop")]
        [InlineData("/Shop/ItemDetails/1")]
        public async Task CheckAccess_Non_Autorize_pages_Test(string url)
        {
            var response = await _hc.GetAsync(url);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Theory]
        [InlineData("/Admin")]
        [InlineData("/Order/MakeOrder")]
        [InlineData("/Order/GetOrderDetails")]
        [InlineData("/Cart")]
        public async Task CheckAccess_Pages_need_to_be_secured_Test(string url)
        {
            var response = await _hc.GetAsync(url);

            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        }
    }
}
