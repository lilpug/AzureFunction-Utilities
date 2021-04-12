using System;
using AzureFunction.Utilities.Business.Helpers;
using AzureFunction.Utilities.Data.Requests;
using FluentValidation;

namespace AzureFunction.Utilities.Business.Validators
{
    public class ProxyRequestValidator : BaseValidator<ProxyRequest>
    {
        public ProxyRequestValidator()
        {
            RuleFor(x => x.RequestType)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("RequestType is a required field.")
                .Custom(ValidRequestType);

            RuleFor(x => x.RequestUrl)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("RequestUrl is a required field.");
        }
        
        private void ValidRequestType(string requestType, ValidationContext<ProxyRequest> context)
        {
            try
            {
                HttpHelpers.GetMethod(requestType);
            }
            catch (ArgumentException)
            {
                context.AddFailure("RequestType is invalid.");
            }
        }
    }
}