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
    public class Cart_Tests
        : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly CustomWebApplicationFactory<TestApp.Site.Startup> _factory;
        private readonly HttpClient _hc;

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
            var scopeFactory = _factory.Services.GetService<IServiceScopeFactory>();
            var scope = scopeFactory.CreateScope();
               Context db = scope.ServiceProvider.GetService<Context>();
            int cartItemsCount = db.Set<CartItem>().Count();

            var response = await _hc.PostAsync("/cart/AddItemToCart/", new FormUrlEncodedContent(new[] {
                new KeyValuePair<string, string>("itemId", itemId.ToString()),
                new KeyValuePair<string, string>("quantity", quantity.ToString())
            }));
            int cartItemsAfterUpdate = db.Set<CartItem>().Count();
            scope.Dispose();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.False(cartItemsCount == cartItemsAfterUpdate);
        }

        [Theory]
        [InlineData("/cart/SupplementItemInCart")]
        [InlineData("/cart/DevideItemIncart")]
        public async Task Testing_Operations_On_Cart_Items_Test(string url)
        {
            int itemId = 2;
            int cartId = 1;
            var scopeFactory = _factory.Services.GetService<IServiceScopeFactory>();
            var scope = scopeFactory.CreateScope();
            Context db = scope.ServiceProvider.GetService<Context>();
            var cartItem = db.Set<CartItem>().Where(x => x.ItemId == itemId && x.CartId == cartId).First();
            scope.Dispose();

            var response = await _hc.PostAsync(url, new FormUrlEncodedContent(new[] {
                new KeyValuePair<string, string>("itemId", itemId.ToString()),
                new KeyValuePair<string, string>("cartId", cartId.ToString())
            }));
            scope = scopeFactory.CreateScope();
            db = scope.ServiceProvider.GetService<Context>();
            var cartItem1 = db.Set<CartItem>().Where(x => x.ItemId == itemId && x.CartId == cartId).First();
            scope.Dispose();
            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
            Assert.False(cartItem.quantity == cartItem1.quantity);
        }

        [Fact]
        public async Task ClearCart_Test()
        {
            int cartId = 1;
            var scopeFactory = _factory.Services.GetService<IServiceScopeFactory>();
            var scope = scopeFactory.CreateScope();
            Context db = scope.ServiceProvider.GetService<Context>();
            int cartItemsCount = db.Set<CartItem>().Count();
            scope.Dispose();

            var response = await _hc.PostAsync("/cart/ClearCart/", new FormUrlEncodedContent(new[] {
                new KeyValuePair<string, string>("cartId", cartId.ToString())
            }));

            scope = scopeFactory.CreateScope();
            db = scope.ServiceProvider.GetService<Context>();
            int cartItemsAfterUpdate = db.Set<CartItem>().Count();
            scope.Dispose();

            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
            Assert.False(cartItemsCount == cartItemsAfterUpdate);
        }
        [Fact]
        public async Task Remove_Item_From_Cart_Test()
        {
            int cartId = 1;
            int itemId = 1;
            var scopeFactory = _factory.Services.GetService<IServiceScopeFactory>();
            var scope = scopeFactory.CreateScope();
            Context db = scope.ServiceProvider.GetService<Context>();
            int cartItemsCount = db.Set<CartItem>().Count();
            scope.Dispose();

            var response = await _hc.PostAsync("/cart/RemoveItemFromCart/", new FormUrlEncodedContent(new[] {
                new KeyValuePair<string, string>("cartId", cartId.ToString()),
                new KeyValuePair<string, string>("itemId", itemId.ToString())
            }));

            scope = scopeFactory.CreateScope();
            db = scope.ServiceProvider.GetService<Context>();
            int cartItemsAfterUpdate = db.Set<CartItem>().Count();
            scope.Dispose();

            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
            Assert.False(cartItemsCount == cartItemsAfterUpdate);
        }
    }
}
