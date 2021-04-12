using System;
using System.Collections.Specialized;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using AzureFunction.Utilities.Business.Helpers;
using AzureFunction.Utilities.Data;
using AzureFunction.Utilities.Data.Interfaces;
using AzureFunction.Utilities.Data.Requests;
using Microsoft.Extensions.Logging;
using IValidatorFactory = AzureFunction.Utilities.Data.Interfaces.IValidatorFactory;

namespace AzureFunction.Utilities.Business.Services.http
{
    public class ProxyRequestHttpService : BaseHttpService, IService
    {
        private readonly HttpClient _client;
        private readonly ProxyRequest _request;

        public ProxyRequestHttpService(ILogger logger,
            IValidatorFactory validatorFactory,
            IServiceValidationConverter serviceValidationConverter,
            HttpClient httpClient,
            IRequest request) : base(logger, validatorFactory, serviceValidationConverter, request)
        {
            _client = httpClient ?? throw new ArgumentNullException($"The {nameof(httpClient)} is required.");
            _request = request as ProxyRequest;
        }

        protected override async Task<HttpServiceResult> InternalProcess(HttpServiceResult result)
        { 
            NameValueCollection headers = new NameValueCollection();
            if (!string.IsNullOrWhiteSpace(_request.RequestHeaders))
            {
                var decodedHeaders = HttpUtility.UrlDecode(_request.RequestHeaders);
                headers = HttpUtility.ParseQueryString(decodedHeaders);
            }

            string url = _request.RequestUrl;
            string parameterJson = null;
            if (!string.IsNullOrWhiteSpace(_request.RequestParameters))
            {
                //Decodes the values
                var decodedParameters = HttpUtility.UrlDecode(_request.RequestParameters);

                //Plugs the parameters in either via the get url or via json if its not a get request
                if (_request.RequestType.ToLower() == "get")
                {
                    var parameters = HttpUtility.ParseQueryString(decodedParameters);
                    var builder = new UriBuilder(_request.RequestUrl);
                    builder.Port = -1;
                    builder.Query = parameters?.ToString() ?? string.Empty;
                    url = builder.ToString();
                }
                else
                {
                    parameterJson = decodedParameters;
                }
            }
            
            //Creates a request object
            var request = new HttpRequestMessage() {
                RequestUri = new Uri(url),
                Method = HttpHelpers.GetMethod(_request.RequestType)
            };

            //If we have json parameters then we add them to the body of the message
            if (!string.IsNullOrWhiteSpace(parameterJson))
            {
                request.Content = new StringContent(parameterJson, Encoding.UTF8, "application/json");
            }
            
            //Loops through our supplied headers and adds them to the request object
            foreach (var key in headers.AllKeys)
            {
                request.Headers.Add(key, headers[key]);
            }
            
            //Returns the http response
            var proxyResponse = await _client.SendAsync(request);
            
            //Sets up our response to return
            if (proxyResponse?.Content != null)
            {
                result.Response = await proxyResponse?.Content?.ReadAsStringAsync();
            }
            result.StatusCode = proxyResponse?.StatusCode;
            
            //Returns our response
            return result;
        }
    }
}