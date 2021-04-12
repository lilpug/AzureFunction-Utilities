using System.Linq;
using System.Threading.Tasks;
using AzureFunction.Utilities.Business.Validators;
using AzureFunction.Utilities.Data.Interfaces;
using Xunit;

namespace AzureFunction.Utilities.UnitTests.Business.Validators
{
    public class BaseValidatorTests
    {
        private class MockRequest : IRequest
        {
        }

        private class MockValidator : BaseValidator<MockRequest>
        {
        }
        
        [Fact]
        public async Task NullRequest()
        {
            var validator = new MockValidator();
            var validatorResults = await validator.ValidateAsync((MockRequest) null);
            Assert.True(!validatorResults.IsValid);
            Assert.True(validatorResults?.Errors?.Count == 1);
            Assert.True(validatorResults.Errors.FirstOrDefault(e => e.PropertyName == "Request" && e.ErrorMessage == "The request has not been populated.") != null);
            Assert.True(validatorResults?.Errors[0].PropertyName == "Request");
        }
    }

}