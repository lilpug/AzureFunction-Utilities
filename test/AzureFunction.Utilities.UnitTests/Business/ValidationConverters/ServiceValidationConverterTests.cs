using System.Collections.Generic;
using AzureFunction.Utilities.Business.ValidationConverters;
using AzureFunction.Utilities.Data.Interfaces;
using FluentValidation.Results;
using Xunit;

namespace AzureFunction.Utilities.UnitTests.Business.ValidationConverters
{
    public class ServiceValidationConverterTests
    {
        private IServiceValidationConverter CreateInstance()
        {
            return new ServiceValidationConverter();
        }
        
        [Fact]
        public void EmptyConvert()
        {
            var validationResult = new ValidationResult(new List<ValidationFailure>());
            var instance = CreateInstance();
            var results = instance.ConvertToDictionaryFormat(null);
            
            Assert.Null(results);
        }

        [Fact]
        public void PopulatedConvert()
        {
            const string fieldName = "testing";
            const string fieldError = "testing error";
            var validationResult = new ValidationResult(new List<ValidationFailure>() {new ValidationFailure(fieldName, fieldError)});
            
            var instance = CreateInstance();
            var results = instance.ConvertToDictionaryFormat(validationResult);
            
            Assert.NotNull(results);
            Assert.True(results?[fieldName]?[0] == fieldError);
        }
    }
}