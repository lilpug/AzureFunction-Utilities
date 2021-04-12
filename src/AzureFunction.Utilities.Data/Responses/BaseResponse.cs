using AzureFunction.Utilities.Data.Interfaces;

namespace AzureFunction.Utilities.Data.Responses
{
    public abstract class BaseResponse : IResponse
    {
        public bool Successful { get; set; }
    }
}