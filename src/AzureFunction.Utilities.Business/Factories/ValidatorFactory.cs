using AzureFunction.Utilities.Business.Validators;
using AzureFunction.Utilities.Data.Interfaces;
using AzureFunction.Utilities.Data.Requests;
using FluentValidation;
using IValidatorFactory = AzureFunction.Utilities.Data.Interfaces.IValidatorFactory;

namespace AzureFunction.Utilities.Business.Factories
{
    public class ValidatorFactory : IValidatorFactory
    {
        public IValidator GetValidator(IRequest request)
        {
            return request switch
            {
                ProxyRequest _ => new ProxyRequestValidator(),                
                _ => null
            };
        }
    }
}