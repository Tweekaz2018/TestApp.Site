using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TestApp.Site;
using Xunit;
using TestApp.Integration_Tests.Helpers;

namespace TestApp.Integration_Tests
{
    public class Login_Tests : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly CustomWebApplicationFactory<TestApp.Site.Startup> _factory;
        private readonly HttpClient _hc;
        public Login_Tests(CustomWebApplicationFactory<TestApp.Site.Startup> factory)
        {
            _factory = factory;
            _hc = factory.CreateClient(new Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false,
                HandleCookies = true
            });

        }

        [Fact]
        public async Task Log_in_with_True_Data_Site_Test()
        {
            var loginPage = await _hc.GetAsync(@"/Login/Login/");
            var parser = new HtmlParser();
            var doc = await parser.ParseDocumentAsync(await loginPage.Content.ReadAsStringAsync());
            var requestVerficationToken = doc.GetElementsByName("__RequestVerificationToken").First() as IHtmlInputElement;

            var result = await _hc.PostAsync(@"/Login/Login/", new FormUrlEncodedContent(new[]{
                new KeyValuePair<string, string>("login", "Admin"),
                new KeyValuePair<string, string>("password", "123qwe123qwe"),
                new KeyValuePair<string, string>("__RequestVerificationToken", requestVerficationToken.DefaultValue)
            })); ;

            Assert.Equal(HttpStatusCode.Redirect, result.StatusCode);
        }

        [Fact]
        public async Task Log_in_with_False_Data_Site_Test()
        {
            var loginPage = await _hc.GetAsync(@"/Login/Login/");
            var parser = new HtmlParser();
            var doc = await parser.ParseDocumentAsync(await loginPage.Content.ReadAsStringAsync());
            var requestVerficationToken = doc.GetElementsByName("__RequestVerificationToken").First() as IHtmlInputElement;

            var result = await _hc.PostAsync(@"/Login/Login/", new FormUrlEncodedContent(new[]{
                new KeyValuePair<string, string>("login", "Admin"),
                new KeyValuePair<string, string>("password", "asdasdasd"),
                new KeyValuePair<string, string>("__RequestVerificationToken", requestVerficationToken.DefaultValue)
            }));

            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        }

        [Fact]
        public async Task Registration_InValid_Data_Test()
        {
            var loginPage = await _hc.GetAsync(@"/Login/Register/");
            var parser = new HtmlParser();
            var doc = await parser.ParseDocumentAsync(await loginPage.Content.ReadAsStringAsync());
            var requestVerficationToken = doc.GetElementsByName("__RequestVerificationToken").First() as IHtmlInputElement;

            var result = await _hc.PostAsync(@"/Login/Register/", new FormUrlEncodedContent(new[]{
                new KeyValuePair<string, string>("login", "Admin"),
                new KeyValuePair<string, string>("password", "asdasdasd"),
                new KeyValuePair<string, string>("phone", "123123123"),
                new KeyValuePair<string, string>("__RequestVerificationToken", requestVerficationToken.DefaultValue)
            }));

            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        }
        [Fact]
        public async Task Registration_Valid_Data_Test()
        {
            var loginPage = await _hc.GetAsync(@"/Login/Register/");
            var parser = new HtmlParser();
            var doc = await parser.ParseDocumentAsync(await loginPage.Content.ReadAsStringAsync());
            var requestVerficationToken = doc.GetElementsByName("__RequestVerificationToken").First() as IHtmlInputElement;

            var result = await _hc.PostAsync(@"/Login/Register/", new FormUrlEncodedContent(new[]{
                new KeyValuePair<string, string>("login", "qweqweqwe"),
                new KeyValuePair<string, string>("password", "asdasdasd"),
                new KeyValuePair<string, string>("phone", "123123123"),
                new KeyValuePair<string, string>("__RequestVerificationToken", requestVerficationToken.DefaultValue)
            }));

            Assert.Equal(HttpStatusCode.Redirect, result.StatusCode);
        }
    }
}
