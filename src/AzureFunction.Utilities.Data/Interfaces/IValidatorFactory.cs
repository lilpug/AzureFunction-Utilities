using FluentValidation;

namespace AzureFunction.Utilities.Data.Interfaces
{
    public interface IValidatorFactory
    {
        IValidator GetValidator(IRequest request);
    }
}