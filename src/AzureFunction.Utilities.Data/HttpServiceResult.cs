using System.Net;

namespace AzureFunction.Utilities.Data
{
    public class HttpServiceResult
    {
        public object Response { get; set; }
        public HttpStatusCode? StatusCode { get; set; }
    }
}