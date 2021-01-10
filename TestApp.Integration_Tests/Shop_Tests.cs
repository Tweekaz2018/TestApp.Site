using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TestApp.Domain;
using TestApp.Integration_Tests.Helpers;
using TestApp.Repo;
using TestApp.Site;
using Xunit;


namespace TestApp.Integration_Tests
{
    public class Shop_Tests
        : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly CustomWebApplicationFactory<TestApp.Site.Startup> _factory;
        private readonly HttpClient _hc;

        public Shop_Tests(CustomWebApplicationFactory<TestApp.Site.Startup> factory)
        {
            _factory = factory;
            _hc = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddAuthentication("Test")
                        .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                            "Test", options => { });
                });
            }).CreateClient(new Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
        }

        [Fact]
        public async Task GetIndexPage_Test()
        {
            var responce = await _hc.GetAsync("/shop/");

            Assert.Equal(HttpStatusCode.OK, responce.StatusCode);
        }

        [Fact]
        public async Task GetItemsGetails_Page_Wrong_Id_Test()
        {
            var responce = await _hc.GetAsync("/shop/itemdetails/1000");
            var html = await responce.Content.ReadAsStringAsync();

            Assert.Contains("an error", html);
            Assert.Equal(HttpStatusCode.OK, responce.StatusCode);
        }

        [Fact]
        public async Task GetItemsGetails_Test()
        {
            var responce = await _hc.GetAsync("/shop/itemdetails/1");

            Assert.Equal(HttpStatusCode.OK, responce.StatusCode);
        }
    }
}
