using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AzureFunction.Utilities.Data;
using AzureFunction.Utilities.Data.Exceptions;
using AzureFunction.Utilities.Data.Interfaces;
using AzureFunction.Utilities.Data.Responses;
using FluentValidation;
using Microsoft.Extensions.Logging;
using IValidatorFactory = AzureFunction.Utilities.Data.Interfaces.IValidatorFactory;

namespace AzureFunction.Utilities.Business.Services.http
{
    public abstract class BaseHttpService
    {
        protected readonly ILogger _logger;
        private readonly IValidatorFactory _validatorFactory;
        private readonly IServiceValidationConverter _serviceValidationConverter;
        private readonly IRequest _request;

        protected Dictionary<string, List<string>> ValidationResults { get; set; }
        
        protected BaseHttpService(ILogger logger, IValidatorFactory validatorFactory, IServiceValidationConverter serviceValidationConverter, IRequest request)
        {
            _logger = logger ?? throw new ArgumentNullException($"The {nameof(logger)} is required.");
            _validatorFactory = validatorFactory ?? throw new ArgumentNullException($"The {nameof(validatorFactory)} is required.");
            _serviceValidationConverter = serviceValidationConverter ?? throw new ArgumentNullException($"The {nameof(serviceValidationConverter)} is required.");
            _request = request ?? throw new ArgumentNullException($"The {nameof(request)} is required.");
            ValidationResults = new Dictionary<string, List<string>>();
        }
        
        protected async Task<Dictionary<string, List<string>>> ServiceValidation()
        {
            //Attempts to get the validator from the factory using the request type
            var tempValidator = _validatorFactory.GetValidator(_request);

            //Checks the validator is set
            if (tempValidator == null) throw new Exception("The validator could not be found.");
            
            //Creates the validation context ready for the validator
            IValidationContext context = new ValidationContext<object>(_request);
                
            //Runs our validation
            var results = await tempValidator.ValidateAsync(context);   
                
            //Checks if it was invalid
            return !results.IsValid ? _serviceValidationConverter.ConvertToDictionaryFormat(results) : null;
        }

        public async Task<HttpServiceResult> ProcessAsync()
        {
            var result = new HttpServiceResult();
            
            try
            {
                //Runs our validation and checks if we have any validation errors
                ValidationResults = await ServiceValidation();
                if (ValidationResults != null && ValidationResults.Count > 0)
                {
                    throw new ValidationException("There has been a validation error.");
                }

                //Runs the custom service logic
                result = await InternalProcess(result);
            }
            catch (ValidationException)
            {
                _logger.LogWarning("A validation failure has occurred.", ValidationResults);

                result.Response = new ValidationErrorResponse() {Successful = false, ValidationErrors = ValidationResults};
                result.StatusCode = HttpStatusCode.BadRequest;
            }
            catch (NotFoundException)
            {
                result.Response = new NotFoundResponse() { Successful = false, Message = "Resource not found."};
                result.StatusCode = HttpStatusCode.NotFound;
            }
            catch(Exception exception)
            {
                _logger.LogError(exception, exception?.Message);
                
                result.Response = new ErrorResponse() { Successful = false, Message = "There has been an error." };
                result.StatusCode = HttpStatusCode.InternalServerError;
            }
            finally
            {   
                if(result?.StatusCode == null)
                {
                    result ??= new HttpServiceResult();
                    result.StatusCode = HttpStatusCode.OK;
                }
            }

            return result;
        }
        
        //This is where the main processing logic sits after being wrapped in the try catches for the correct response code outputs
        protected abstract Task<HttpServiceResult> InternalProcess(HttpServiceResult result);
    }
}
