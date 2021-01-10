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
    public class Cart_Tests : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly CustomWebApplicationFactory<TestApp.Site.Startup> _factory;
        private readonly HttpClient _hc;
        private Context db;

        public Cart_Tests(CustomWebApplicationFactory<TestApp.Site.Startup> factory)
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
            var sp = factory.Server.Services;

            using (var scope = sp.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                db = scopedServices.GetRequiredService<Context>();
            }
        }

        [Fact]
        public async Task Cart_Index_Test()
        {
            var response = await _hc.GetAsync("/cart/");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Add_Items_To_Cart_Modal_Test()
        {
            int itemId = 1;

            var response = await _hc.GetAsync("/cart/AddItemToCart/" + itemId);
            var parser = new HtmlParser();
            var doc = await parser.ParseDocumentAsync(await response.Content.ReadAsStringAsync());
            var itemIdFromSite = doc.GetElementById("ItemId") as IHtmlInputElement;

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(itemId.ToString(), itemIdFromSite.DefaultValue);
        }

        [Fact]
        public async Task Add_Items_To_Cart_Post_test()
        {
            int itemId = 3;
            int quantity = 4;

            var response = await _hc.PostAsync("/cart/AddItemToCart/", new FormUrlEncodedContent(new[] {
                new KeyValuePair<string, string>("itemId", itemId.ToString()),
                new KeyValuePair<string, string>("quantity", quantity.ToString())
            }));

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
