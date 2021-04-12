using System.Net;
using System.Threading.Tasks;
using AzureFunction.Utilities.Data.Enums;
using AzureFunction.Utilities.Data.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace AzureFunction.Utilities.Triggers.http
{
    
    public class ProxyRequest
    {
        private readonly IServiceFactory _serviceFactory;
        public ProxyRequest(IServiceFactory serviceFactory)
        {
            _serviceFactory = serviceFactory;
        }

        [FunctionName("ProxyRequest")]
        public async Task<IActionResult> RunAsync([HttpTrigger(AuthorizationLevel.Function, "get", Route = null)]
            HttpRequest req, ILogger log)
        {
            var request = new Data.Requests.ProxyRequest()
            {
                RequestType = req.Query["RequestType"],
                RequestUrl = req.Query["RequestUrl"], 
                RequestHeaders = req.Query["RequestHeaders"],
                RequestParameters = req.Query["RequestParameters"]
            };

            var service = _serviceFactory.GetService(ServiceType.ProxyRequest, request);
            var result = await service.ProcessAsync();
            return new ObjectResult(result?.Response) {StatusCode = (int) (result?.StatusCode ?? HttpStatusCode.InternalServerError)};
        }
    }
   
}