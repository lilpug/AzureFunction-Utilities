using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using AzureFunction.Utilities.Business.Factories;
using AzureFunction.Utilities.Business.Services.http;
using AzureFunction.Utilities.Business.ValidationConverters;
using AzureFunction.Utilities.Data.Interfaces;
using AzureFunction.Utilities.Data.Requests;
using AzureFunction.Utilities.UnitTests.Helpers;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace AzureFunction.Utilities.UnitTests.Business.Services.Http
{
    public class ProxyRequestHttpServiceTests
    {
        private readonly Mock<ILogger> _mockLogger;
        private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
        private ProxyRequest Request { get; set; }
        
        public ProxyRequestHttpServiceTests()
        {
            _mockLogger = new Mock<ILogger>();
            _mockHttpMessageHandler = new Mock<HttpMessageHandler>();  
            Request = new ProxyRequest()
            {
                RequestType = "GET",
                RequestUrl = "https://google.com/",
                RequestHeaders = null,
                RequestParameters = null
            };
        }

        private IService CreateInstance(HttpClient httpClient)
        {
            return new ProxyRequestHttpService(_mockLogger.Object, new ValidatorFactory(), new ServiceValidationConverter(), httpClient, Request);
        }
        
        [Fact]
        public void NullHttpClientConstructor()
        {
            var exception = Assert.Throws<ArgumentNullException>(() =>
            {
                var service = new ProxyRequestHttpService(_mockLogger.Object, new ValidatorFactory(), new ServiceValidationConverter(), null, Request);
            });
            
            Assert.NotNull(exception);
            Assert.True((exception?.Message).IndexOf("The httpClient is required.", StringComparison.Ordinal) > -1);
        }

        [Fact]
        public void NullRequestConstructor()
        {
            var exception = Assert.Throws<ArgumentNullException>(() =>
            {
                var service = new ProxyRequestHttpService(_mockLogger.Object, new ValidatorFactory(), new ServiceValidationConverter(), new HttpClient(_mockHttpMessageHandler.Object), null);
            });
            
            Assert.NotNull(exception);
            Assert.True((exception?.Message).IndexOf("The request is required.", StringComparison.Ordinal) > -1);
        }
        
        [Fact]
        public void InvalidTypeSupplied()
        {
            var exception = Assert.Throws<ArgumentNullException>(() =>
            {
                var service = new ProxyRequestHttpService(_mockLogger.Object, new ValidatorFactory(), new ServiceValidationConverter(), new HttpClient(_mockHttpMessageHandler.Object), null);
            });
            
            Assert.NotNull(exception);
            Assert.True((exception?.Message).IndexOf("The request is required.", StringComparison.Ordinal) > -1);
        }
        
        [Fact]
        public async Task GetWithoutHeadersAndParameters()
        {
            const string url = "https://google.com/";
            const string type = "GET";
            
            //Sets the request values
            Request.RequestUrl = url;
            Request.RequestType = type;
            
            //Valid response
            var mockHttpMessageHandler = MockHttpMessageHandler.MockSendAsync(HttpStatusCode.OK, new StringContent(""));
            var httpClient = new HttpClient(mockHttpMessageHandler.Object);
            
            var service = CreateInstance(httpClient);
            var results = await service.ProcessAsync();
            
            MockHttpMessageHandler.VerifySendAsync(mockHttpMessageHandler, Times.Once(), 
                req =>
                    req.Method.ToString() == type &&
                    req.RequestUri == new Uri(url));
            
            Assert.NotNull(results);
            Assert.Equal(HttpStatusCode.OK, results.StatusCode);
        }
        
        [Fact]
        public async Task GetWithHeadersAndParameters()
        {
            const string url = "https://google.com/";
            const string type = "GET";
            KeyValuePair<string, string> headerOne = new KeyValuePair<string, string>("testOne", "testOneValue");
            KeyValuePair<string, string> headerTwo = new KeyValuePair<string, string>("testTwo", "testTwoValue");
            string headers = $"{headerOne.Key}={headerOne.Value}&{headerTwo.Key}={headerTwo.Value}";
            
            KeyValuePair<string, string> parameterOne = new KeyValuePair<string, string>("testOne", "testOneValue");
            KeyValuePair<string, string> parameterTwo = new KeyValuePair<string, string>("testTwo", "testTwoValue");
            string parameters = $"{parameterOne.Key}={parameterOne.Value}&{parameterTwo.Key}={parameterTwo.Value}";
            
            //Sets the request values
            Request.RequestHeaders = HttpUtility.UrlEncode(headers);
            Request.RequestParameters = HttpUtility.UrlEncode(parameters);
            Request.RequestUrl = url;
            Request.RequestType = type;
            
            //Invalid response
            /*var mockHttpMessageHandler = MockHttpMessageHandler.MockSendAsync(HttpStatusCode.BadRequest, new StringContent(""));
            var httpClient = new HttpClient(mockHttpMessageHandler.Object);
            MockHttpMessageHandler.VerifySendAsync(mockHttpMessageHandler, Times.Once(), 
                req => 
                    req.Content.ReadAsStringAsync().Result == requestJson &&
                    req.Method.ToString() == type && 
                    req.RequestUri == new Uri($"{url}"));*/
            
            //Valid response
            var mockHttpMessageHandler = MockHttpMessageHandler.MockSendAsync(HttpStatusCode.OK, new StringContent(""));
            var httpClient = new HttpClient(mockHttpMessageHandler.Object);
            
            var service = CreateInstance(httpClient);
            var results = await service.ProcessAsync();
            
            MockHttpMessageHandler.VerifySendAsync(mockHttpMessageHandler, Times.Once(), 
                req =>
                    req.Headers.Contains(headerOne.Key) &&
                    req.Headers.Contains(headerTwo.Key) &&
                    req.Method.ToString() == type &&
                    req.RequestUri == new Uri($"{url}?{parameters}"));
            
            Assert.NotNull(results);
            Assert.Equal(HttpStatusCode.OK, results.StatusCode);
        }

        [Fact]
        public async Task PostWithoutHeadersAndParameters()
        {
            const string url = "https://google.com/";
            const string type = "POST";
           
            //Sets the request values
            Request.RequestUrl = url;
            Request.RequestType = type;
            
            //Valid response
            var mockHttpMessageHandler = MockHttpMessageHandler.MockSendAsync(HttpStatusCode.OK, new StringContent(""));
            var httpClient = new HttpClient(mockHttpMessageHandler.Object);
            
            var service = CreateInstance(httpClient);
            var results = await service.ProcessAsync();
            
            MockHttpMessageHandler.VerifySendAsync(mockHttpMessageHandler, Times.Once(), 
                req =>
                    req.Method.ToString() == type &&
                    req.RequestUri == new Uri(url));
            
            Assert.NotNull(results);
            Assert.Equal(HttpStatusCode.OK, results.StatusCode);
        }
        
        [Fact]
        public async Task PostWithHeadersAndParameters()
        {
            const string url = "https://google.com/";
            const string type = "POST";
            KeyValuePair<string, string> headerOne = new KeyValuePair<string, string>("testOne", "testOneValue");
            KeyValuePair<string, string> headerTwo = new KeyValuePair<string, string>("testTwo", "testTwoValue");
            string headers = $"{headerOne.Key}={headerOne.Value}&{headerTwo.Key}={headerTwo.Value}";
            
            KeyValuePair<string, string> parameterOne = new KeyValuePair<string, string>("testOne", "testOneValue");
            KeyValuePair<string, string> parameterTwo = new KeyValuePair<string, string>("testTwo", "testTwoValue");
            string parameters = $"{{ '{parameterOne.Key}': '{parameterOne.Value}', '{parameterTwo.Key}': '{parameterTwo.Value}' }}";
            
            //Sets the request values
            Request.RequestHeaders = HttpUtility.UrlEncode(headers);
            Request.RequestParameters = HttpUtility.UrlEncode(parameters);
            Request.RequestUrl = url;
            Request.RequestType = type;
            
            //Valid response
            var mockHttpMessageHandler = MockHttpMessageHandler.MockSendAsync(HttpStatusCode.OK, new StringContent(""));
            var httpClient = new HttpClient(mockHttpMessageHandler.Object);
            
            var service = CreateInstance(httpClient);
            var results = await service.ProcessAsync();
            
            MockHttpMessageHandler.VerifySendAsync(mockHttpMessageHandler, Times.Once(), 
                req =>
                    req.Content.ReadAsStringAsync().Result == parameters &&
                    req.Headers.Contains(headerOne.Key) &&
                    req.Headers.Contains(headerTwo.Key) &&
                    req.Method.ToString() == type &&
                    req.RequestUri == new Uri(url));
            
            Assert.NotNull(results);
            Assert.Equal(HttpStatusCode.OK, results.StatusCode);
        }
        
    }
}