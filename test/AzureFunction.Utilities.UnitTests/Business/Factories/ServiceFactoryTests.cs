using System.Net.Http;
using AzureFunction.Utilities.Business.Factories;
using AzureFunction.Utilities.Business.Services.http;
using AzureFunction.Utilities.Data.Enums;
using AzureFunction.Utilities.Data.Interfaces;
using AzureFunction.Utilities.Data.Requests;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace AzureFunction.Utilities.UnitTests.Business.Factories
{
    public class ServiceFactoryTests
    {
        private readonly IServiceFactory _processServiceFactory;
        
        public ServiceFactoryTests()
        {
            var mockLogger = new Mock<ILogger<ServiceFactory>>();
            var mockServiceValidationConverter = new Mock<IServiceValidationConverter>();
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();  
            _processServiceFactory = new ServiceFactory(mockLogger.Object, new ValidatorFactory(), mockServiceValidationConverter.Object, new HttpClient(mockHttpMessageHandler.Object));
        }

        [Fact]
        public void EmptyService()
        {
            var result = _processServiceFactory.GetService(null, null);
            Assert.True(result == null);
        }

        [Fact]
        public void RequestProxyService()
        {
            var result = _processServiceFactory.GetService(ServiceType.ProxyRequest, new ProxyRequest());
            Assert.True(result is ProxyRequestHttpService);
        }
    }
}