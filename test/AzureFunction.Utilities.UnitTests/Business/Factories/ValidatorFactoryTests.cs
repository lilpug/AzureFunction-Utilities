using AzureFunction.Utilities.Business.Factories;
using AzureFunction.Utilities.Business.Validators;
using AzureFunction.Utilities.Data.Interfaces;
using AzureFunction.Utilities.Data.Requests;
using Moq;
using Xunit;

namespace AzureFunction.Utilities.UnitTests.Business.Factories
{
    public class ValidatorFactoryTests
    {
        private readonly IValidatorFactory _validatorFactory;

        public ValidatorFactoryTests()
        {
            _validatorFactory = new ValidatorFactory();
        }

        [Fact]
        public void EmptyValidator()
        {
            var request = new Mock<IRequest>().Object;
            var result = _validatorFactory.GetValidator(request);
            Assert.True(result == null);
        }

        [Fact]
        public void ProxyRequest()
        {
            IRequest request = new ProxyRequest();
            var result = _validatorFactory.GetValidator(request);
            Assert.True(result is ProxyRequestValidator);
        }
    }
}