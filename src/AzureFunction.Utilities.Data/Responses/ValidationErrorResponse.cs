using System.Collections.Generic;
using AzureFunction.Utilities.Data.Interfaces;

namespace AzureFunction.Utilities.Data.Responses
{
    public class ValidationErrorResponse : BaseResponse, IResponse
    {
        public Dictionary<string, List<string>> ValidationErrors { get; set; }
    }
}