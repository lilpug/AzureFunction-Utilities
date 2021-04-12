using System.Collections.Generic;
using System.Linq;
using AzureFunction.Utilities.Data.Interfaces;
using FluentValidation.Results;

namespace AzureFunction.Utilities.Business.ValidationConverters
{
    public class ServiceValidationConverter: IServiceValidationConverter
    {
        public Dictionary<string, List<string>> ConvertToDictionaryFormat(ValidationResult results)
        {
            if (results?.Errors?.Count > 0)
            {
                var validationResultsConversion = 
                    (from field in results.Errors.Select(e => e.PropertyName).Distinct()
                        let errors = (from error in results.Errors.Where(e => e.PropertyName == field) select error.ErrorMessage).ToList()
                        select new KeyValuePair<string, List<string>>(field, errors))?.ToList();
                if (validationResultsConversion.Any())
                {
                    return validationResultsConversion.ToDictionary(t => t.Key, t => t.Value);
                }
            }
            return null;
        }
    }

}