using System;
using System.Net.Http;

namespace AzureFunction.Utilities.Business.Helpers
{
    public static class HttpHelpers
    {
        public static HttpMethod GetMethod(string type)
        {
            return type?.ToLower() switch
            {
                "get" => HttpMethod.Get,
                "post" => HttpMethod.Post,
                "put" => HttpMethod.Put,
                "delete" => HttpMethod.Delete,
                "head" => HttpMethod.Head,
                "options" => HttpMethod.Options,
                "trace" => HttpMethod.Trace,
                "patch" => HttpMethod.Patch,
                _ => throw new ArgumentException("The type is not valid.")
            };
        }
    }
}