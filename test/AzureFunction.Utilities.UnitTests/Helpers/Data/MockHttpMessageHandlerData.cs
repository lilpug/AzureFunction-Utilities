using System.Net;
using System.Net.Http;

namespace AzureFunction.Utilities.UnitTests.Helpers.Data
{
    public class MockHttpMessageHandlerData
    {
        public HttpStatusCode StatusCode { get; set; }
        public HttpContent Content { get; set; }
    }
}