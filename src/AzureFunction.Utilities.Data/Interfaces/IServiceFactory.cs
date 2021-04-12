using AzureFunction.Utilities.Data.Enums;

namespace AzureFunction.Utilities.Data.Interfaces
{
    public interface IServiceFactory
    {
        IService GetService(ServiceType? service, IRequest request);
    }
}