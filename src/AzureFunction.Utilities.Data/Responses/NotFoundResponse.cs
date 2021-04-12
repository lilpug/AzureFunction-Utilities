using AzureFunction.Utilities.Data.Interfaces;

namespace AzureFunction.Utilities.Data.Responses
{
    public class NotFoundResponse : BaseResponse
    {
        public string Message { get; set; }
    }
}