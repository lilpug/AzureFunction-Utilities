using System.Net.Http;
using AzureFunction.Utilities.Business.Services.http;
using AzureFunction.Utilities.Data.Enums;
using AzureFunction.Utilities.Data.Interfaces;
using Microsoft.Extensions.Logging;

namespace AzureFunction.Utilities.Business.Factories
{
    public class ServiceFactory : IServiceFactory
    {   
        private readonly ILogger _logger;
        private readonly IValidatorFactory _validatorFactory;
        private readonly IServiceValidationConverter _serviceValidationConverter;
        private readonly HttpClient _httpClient;

        public ServiceFactory(ILogger<ServiceFactory> logger, IValidatorFactory validatorFactory, IServiceValidationConverter serviceValidationConverter, HttpClient httpClient)
        {
            _logger = logger;
            _validatorFactory = validatorFactory;
            _serviceValidationConverter = serviceValidationConverter;
            _httpClient = httpClient;
        }

        public IService GetService(ServiceType? service, IRequest request)
        {
            return service switch
            {
                ServiceType.ProxyRequest => new ProxyRequestHttpService(_logger, _validatorFactory, _serviceValidationConverter, _httpClient, request),
                _ => null
            };
        }
    }
}