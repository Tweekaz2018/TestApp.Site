using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using TestApp.Domain;
using TestApp.Integration_Tests.Helpers;
using TestApp.Repo;
using TestApp.Services.Interfaces;
using TestApp.Site;
using Xunit;


namespace TestApp.Integration_Tests
{
    public class Admin_Tests
        : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly CustomWebApplicationFactory<TestApp.Site.Startup> _factory;
        private readonly HttpClient _hc;

        public Admin_Tests(CustomWebApplicationFactory<TestApp.Site.Startup> factory)
        {
            _factory = factory;
            _hc = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddAuthentication("Test")
                        .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                            "Test", options => { });
                    services.AddTransient<ISaveFile, TestISaveFile>();
                });
            }).CreateClient(new Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
        }

        [Theory]
        [InlineData("/admin/index")]
        [InlineData("/admin/getitemspage")]
        [InlineData("/admin/getitemaddmodal")]
        [InlineData("/admin/getuserspage")]
        [InlineData("/admin/getorderspage")]
        [InlineData("/admin/getordersdeliverymethodspage")]
        [InlineData("/admin/getorderspaymethodspage")]
        [InlineData("/admin/adddeliverymethodmodal")]
        [InlineData("/admin/addpaymethodmodal")]
        [InlineData("/admin/getitemscategories")]
        [InlineData("/admin/addcategorymodal")]
        public async Task Access_Admin_Pages_Tests(string url)
        {
            var responce = await _hc.GetAsync(url);

            Assert.Equal(HttpStatusCode.OK, responce.StatusCode);
        }

        [Fact]
        public async Task AddItem_Test()
        {
            var file = File.OpenRead(@"Helpers/test-image.jpg");
            var streamContent = new StreamContent(file);
            var form = new MultipartFormDataContent();
            form.Add(streamContent, "image", "image.jpg");
            form.Add(new StringContent("Title"), "title");
            form.Add(new StringContent("descr"), "description");
            form.Add(new StringContent("123"), "price");
            form.Add(new StringContent("1"), "categoryId");
            var scopeFactory = _factory.Services.GetService<IServiceScopeFactory>();
            var scope = scopeFactory.CreateScope();
            Context db = scope.ServiceProvider.GetService<Context>();
            int itemsCount = db.Set<Item>().Count();

            var responce = await _hc.PostAsync("/admin/AddItem", form);
            int itemsAfterCount = db.Set<Item>().Count();
            scope.Dispose();

            Assert.Equal(HttpStatusCode.Redirect, responce.StatusCode);
            Assert.False(itemsCount == itemsAfterCount);
        }
        [Fact]
        public async Task AddItem_Wrong_File_Type_Test()
        {
            var file = File.OpenRead(@"Helpers/test-image.jpg");
            var streamContent = new StreamContent(file);
            var form = new MultipartFormDataContent();
            form.Add(streamContent, "image", "image.dll");
            form.Add(new StringContent("Title"), "title");
            form.Add(new StringContent("descr"), "description");
            form.Add(new StringContent("123"), "price");
            form.Add(new StringContent("1"), "categoryId");
            var scopeFactory = _factory.Services.GetService<IServiceScopeFactory>();
            var scope = scopeFactory.CreateScope();
            Context db = scope.ServiceProvider.GetService<Context>();
            int itemsCount = db.Set<Item>().Count();

            var responce = await _hc.PostAsync("/admin/AddItem", form);
            int itemsAfterCount = db.Set<Item>().Count();
            scope.Dispose();

            Assert.Equal(HttpStatusCode.Redirect, responce.StatusCode);
            Assert.True(itemsCount == itemsAfterCount);
        }

        [Fact]
        public async Task DeleteItem_Test()
        {
            var scopeFactory = _factory.Services.GetService<IServiceScopeFactory>();
            var scope = scopeFactory.CreateScope();
            Context db = scope.ServiceProvider.GetService<Context>();
            int itemsCount = db.Set<Item>().Count();

            var responce = await _hc.GetAsync("/admin/deleteitem/1");
            int itemsAfterCount = db.Set<Item>().Count();
            scope.Dispose();

            Assert.Equal(HttpStatusCode.Redirect, responce.StatusCode);
            Assert.False(itemsCount == itemsAfterCount);
        }

        [Fact]
        public async Task AddDeliveryMethod_Test()
        {
            var scopeFactory = _factory.Services.GetService<IServiceScopeFactory>();
            var scope = scopeFactory.CreateScope();
            Context db = scope.ServiceProvider.GetService<Context>();
            int deliveryMethodsCount = db.Set<OrderDeliveryMethod>().Count();

            var responce = await _hc.PostAsync("/admin/adddeliverymethod", new FormUrlEncodedContent(new[] {
                new KeyValuePair<string,string>("deliveryMethodName", "deliveryMethodName")
            }));

            scopeFactory = _factory.Services.GetService<IServiceScopeFactory>();
            scope = scopeFactory.CreateScope();
            db = scope.ServiceProvider.GetService<Context>();
            int deliveryMethodsAfterCount = db.Set<OrderDeliveryMethod>().Count();

            Assert.Equal(HttpStatusCode.Redirect, responce.StatusCode);
            Assert.False(deliveryMethodsAfterCount == deliveryMethodsCount);
        }

        [Fact]
        public async Task UpdateDeliveryMethod_Test()
        {
            int id = 1;
            var scopeFactory = _factory.Services.GetService<IServiceScopeFactory>();
            var scope = scopeFactory.CreateScope();
            Context db = scope.ServiceProvider.GetService<Context>();
            var deliveryMethod = await db.Set<OrderDeliveryMethod>().FindAsync(id);
            string name = deliveryMethod.name;

            var responce = await _hc.PostAsync("/admin/updatedeliverymethod", new FormUrlEncodedContent(new[] {
                new KeyValuePair<string,string>("name", "deliveryMethodName"),
                new KeyValuePair<string,string>("id", deliveryMethod.Id.ToString())
            }));

            scopeFactory = _factory.Services.GetService<IServiceScopeFactory>();
            scope = scopeFactory.CreateScope();
            db = scope.ServiceProvider.GetService<Context>();
            var deliveryMethodsAfterUpdate = await db.Set<OrderDeliveryMethod>().FindAsync(id);

            Assert.Equal(HttpStatusCode.OK, responce.StatusCode);
            Assert.False(name == deliveryMethodsAfterUpdate.name);
        }

        [Fact]
        public async Task DeleteDeliveryMethod_Test()
        {
            var scopeFactory = _factory.Services.GetService<IServiceScopeFactory>();
            var scope = scopeFactory.CreateScope();
            Context db = scope.ServiceProvider.GetService<Context>();
            int orderDeliveryMethodCount = db.Set<OrderDeliveryMethod>().Count();

            var responce = await _hc.GetAsync("/admin/deletedeliverymethod/1");
            int orderDeliveryMethodAfterCount = db.Set<OrderDeliveryMethod>().Count();
            scope.Dispose();

            Assert.Equal(HttpStatusCode.OK, responce.StatusCode);
            Assert.False(orderDeliveryMethodCount == orderDeliveryMethodAfterCount);
        }

        [Fact]
        public async Task AddOrderPayMethod_Test()
        {
            var scopeFactory = _factory.Services.GetService<IServiceScopeFactory>();
            var scope = scopeFactory.CreateScope();
            Context db = scope.ServiceProvider.GetService<Context>();
            int orderPayMethodsCount = db.Set<OrderPayMethod>().Count();

            var responce = await _hc.PostAsync("/admin/addpaymethod", new FormUrlEncodedContent(new[] {
                new KeyValuePair<string,string>("payMethodName", "payMethodName")
            }));

            scopeFactory = _factory.Services.GetService<IServiceScopeFactory>();
            scope = scopeFactory.CreateScope();
            db = scope.ServiceProvider.GetService<Context>();
            int orderPayMethodsAfterCount = db.Set<OrderPayMethod>().Count();

            Assert.Equal(HttpStatusCode.Redirect, responce.StatusCode);
            Assert.False(orderPayMethodsCount == orderPayMethodsAfterCount);
        }

        [Fact]
        public async Task UpdatePayMethod_Test()
        {
            int id = 1;
            var scopeFactory = _factory.Services.GetService<IServiceScopeFactory>();
            var scope = scopeFactory.CreateScope();
            Context db = scope.ServiceProvider.GetService<Context>();
            var payMethod = await db.Set<OrderPayMethod>().FindAsync(id);
            string name = payMethod.name;

            var responce = await _hc.PostAsync("/admin/updatepaymethod", new FormUrlEncodedContent(new[] {
                new KeyValuePair<string,string>("name", "name"),
                new KeyValuePair<string,string>("id", payMethod.Id.ToString())
            }));

            scopeFactory = _factory.Services.GetService<IServiceScopeFactory>();
            scope = scopeFactory.CreateScope();
            db = scope.ServiceProvider.GetService<Context>();
            var payMethodAfterUpdate = await db.Set<OrderDeliveryMethod>().FindAsync(id);

            Assert.Equal(HttpStatusCode.OK, responce.StatusCode);
            Assert.False(name == payMethodAfterUpdate.name);
        }

        [Fact]
        public async Task DeletePayMethod_Test()
        {
            var scopeFactory = _factory.Services.GetService<IServiceScopeFactory>();
            var scope = scopeFactory.CreateScope();
            Context db = scope.ServiceProvider.GetService<Context>();
            int orderPayMethodCount = db.Set<OrderPayMethod>().Count();

            var responce = await _hc.GetAsync("/admin/deletepaymethod/1");
            int orderPayMethodAfterCount = db.Set<OrderPayMethod>().Count();
            scope.Dispose();

            Assert.Equal(HttpStatusCode.OK, responce.StatusCode);
            Assert.False(orderPayMethodCount == orderPayMethodAfterCount);
        }

        [Fact]
        public async Task AddCategory_Test()
        {
            var scopeFactory = _factory.Services.GetService<IServiceScopeFactory>();
            var scope = scopeFactory.CreateScope();
            Context db = scope.ServiceProvider.GetService<Context>();
            int catsCount = db.Set<Category>().Count();

            var responce = await _hc.PostAsync("/admin/addcategory", new FormUrlEncodedContent(new[] {
                new KeyValuePair<string,string>("categoryName", "categoryName")
            }));

            scopeFactory = _factory.Services.GetService<IServiceScopeFactory>();
            scope = scopeFactory.CreateScope();
            db = scope.ServiceProvider.GetService<Context>();
            int catsAfterCount = db.Set<Category>().Count();

            Assert.Equal(HttpStatusCode.Redirect, responce.StatusCode);
            Assert.False(catsCount == catsAfterCount);
        }

        [Fact]
        public async Task UpdateCategory_Test()
        {
            int id = 1;
            var scopeFactory = _factory.Services.GetService<IServiceScopeFactory>();
            var scope = scopeFactory.CreateScope();
            Context db = scope.ServiceProvider.GetService<Context>();
            var category = await db.Set<Category>().FindAsync(id);
            string name = category.name;

            var responce = await _hc.PostAsync("/admin/updatecategory", new FormUrlEncodedContent(new[] {
                new KeyValuePair<string,string>("name", "name"),
                new KeyValuePair<string,string>("id", category.Id.ToString())
            }));

            scopeFactory = _factory.Services.GetService<IServiceScopeFactory>();
            scope = scopeFactory.CreateScope();
            db = scope.ServiceProvider.GetService<Context>();
            var categoryAfterUpdate = await db.Set<Category>().FindAsync(id);

            Assert.Equal(HttpStatusCode.OK, responce.StatusCode);
            Assert.False(name == categoryAfterUpdate.name);
        }

        [Fact]
        public async Task DeleteCategory_Test()
        {
            var scopeFactory = _factory.Services.GetService<IServiceScopeFactory>();
            var scope = scopeFactory.CreateScope();
            Context db = scope.ServiceProvider.GetService<Context>();
            int categoryCount = db.Set<Category>().Count();

            var responce = await _hc.GetAsync("/admin/deletecategory/1");
            int categoryAfterCount = db.Set<Category>().Count();
            scope.Dispose();

            Assert.Equal(HttpStatusCode.OK, responce.StatusCode);
            Assert.False(categoryCount == categoryAfterCount);
        }
    }
}
