using AzureFunction.Utilities.Data.Interfaces;

namespace AzureFunction.Utilities.Data.Requests
{
    public class ProxyRequest : IRequest
    {
        public string RequestType { get; set; }
        public string RequestUrl { get; set; }
        public string RequestHeaders { get; set; }
        public string RequestParameters { get; set; }
    }
}