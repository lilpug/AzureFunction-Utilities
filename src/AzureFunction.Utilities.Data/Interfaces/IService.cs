using System.Threading.Tasks;

namespace AzureFunction.Utilities.Data.Interfaces
{
    public interface IService
    {
        public Task<HttpServiceResult> ProcessAsync();
    }
}