using System.Collections.Generic;
using FluentValidation.Results;

namespace AzureFunction.Utilities.Data.Interfaces
{
    public interface IServiceValidationConverter
    {
        Dictionary<string, List<string>> ConvertToDictionaryFormat(ValidationResult results);
    }
}