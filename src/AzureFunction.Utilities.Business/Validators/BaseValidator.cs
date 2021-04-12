using System.Threading;
using System.Threading.Tasks;
using AzureFunction.Utilities.Data.Interfaces;
using FluentValidation;
using FluentValidation.Results;

namespace AzureFunction.Utilities.Business.Validators
{
    public abstract class BaseValidator<T> : AbstractValidator<T> where T : IRequest
    {
        public override async Task<ValidationResult> ValidateAsync(ValidationContext<T> context,
            CancellationToken cancellation = new CancellationToken())
        {
            return context == null || context.InstanceToValidate == null
                ? new ValidationResult(new[] {new ValidationFailure("Request", "The request has not been populated.")})
                : await base.ValidateAsync(context, cancellation);
        }        
    }
}