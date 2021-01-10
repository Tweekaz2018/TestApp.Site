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
    public class Order_Tests : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly CustomWebApplicationFactory<TestApp.Site.Startup> _factory;
        private HttpClient _hc;

        public Order_Tests(CustomWebApplicationFactory<TestApp.Site.Startup> factory)
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
        public async Task MakeOrder_Test()
        {
            var responce = await _hc.GetAsync("/order/makeorder");

            Assert.Equal(HttpStatusCode.OK, responce.StatusCode);
        }

        [Fact]
        public async Task MakeOrder_Post_Test()
        {
            var scopeFactory = _factory.Services.GetService<IServiceScopeFactory>();
            var scope = scopeFactory.CreateScope();
            Context db = scope.ServiceProvider.GetService<Context>();
            var cartItemsCount = db.Set<CartItem>().Count();
            var orderItemsCount = db.Set<OrderItem>().Count();

            var responce = await _hc.PostAsync("/order/makeorder", new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("orderDeliveryMethod", "1"),
                new KeyValuePair<string, string>("orderPayMethod", "1" )
            }));

            db = scope.ServiceProvider.GetService<Context>();
            var cartItemsCountAfterUpdate = db.Set<CartItem>().Count();
            var orderItemsCountAfterUpdate = db.Set<OrderItem>().Count();
            scope.Dispose();

            Assert.Equal(HttpStatusCode.OK, responce.StatusCode);
            Assert.False(cartItemsCount == cartItemsCountAfterUpdate);
            Assert.False(orderItemsCount == orderItemsCountAfterUpdate);
        }

        [Fact]
        public async Task GetOrderDetails_With_Wrong_Role_Test()
        {
            _hc = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddAuthentication("Test")
                        .AddScheme<AuthenticationSchemeOptions, TestAuthHandlerWithUserRole>(
                            "Test", options => { });
                });
            }).CreateClient(new Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

            var responce = await _hc.GetAsync("/order/getorderdetails/1");

            Assert.Equal(HttpStatusCode.Redirect, responce.StatusCode);
        }
        [Fact]
        public async Task GetOrderDetails_Test()
        {
            var responce = await _hc.GetAsync("/order/getorderdetails/1");

            Assert.Equal(HttpStatusCode.OK, responce.StatusCode);
        }
    }
}
