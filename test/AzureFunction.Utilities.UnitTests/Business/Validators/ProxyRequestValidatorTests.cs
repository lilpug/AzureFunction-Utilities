using System.Linq;
using System.Threading.Tasks;
using AzureFunction.Utilities.Business.Validators;
using AzureFunction.Utilities.Data.Requests;
using Moq;
using Xunit;

namespace AzureFunction.Utilities.UnitTests.Business.Validators
{
    public class ProxyRequestValidatorTests
    {
        private ProxyRequest Request { get; set; }
        public ProxyRequestValidatorTests()
        {
            Request = new ProxyRequest
            {
                RequestType = "GET",
                RequestUrl = "https://google.com/",
                RequestHeaders = null,
                RequestParameters = null
            };
        }
        
        private ProxyRequestValidator CreateInstance()
        {
            return new ProxyRequestValidator();
        }
        
        [Fact]
        public async Task EmptyRequest()
        {
            var mockRequest = new Mock<ProxyRequest>();
            var validator = CreateInstance();
            var validatorResults = await validator.ValidateAsync(mockRequest.Object);

            Assert.True(!validatorResults.IsValid);
            Assert.True(validatorResults?.Errors?.Count == 2);
            Assert.True(validatorResults.Errors.FirstOrDefault(e => e.PropertyName == "RequestType" && e.ErrorMessage == "RequestType is a required field.") != null);
            Assert.True(validatorResults.Errors.FirstOrDefault(e => e.PropertyName == "RequestUrl" && e.ErrorMessage == "RequestUrl is a required field.") != null);
        }

        [Fact]
        public async Task EmptyRequestTypeRequest()
        {
            Request.RequestType = null;
            var validator = CreateInstance();
            var validatorResults = await validator.ValidateAsync(Request);

            Assert.True(!validatorResults.IsValid);
            Assert.True(validatorResults?.Errors?.Count == 1);
            Assert.True(validatorResults.Errors.FirstOrDefault(e => e.PropertyName == "RequestType" && e.ErrorMessage == "RequestType is a required field.") != null);
        }
        
        [Fact]
        public async Task EmptyRequestUrlRequest()
        {
            Request.RequestUrl = null;
            var validator = CreateInstance();
            var validatorResults = await validator.ValidateAsync(Request);

            Assert.True(!validatorResults.IsValid);
            Assert.True(validatorResults?.Errors?.Count == 1);
            Assert.True(validatorResults.Errors.FirstOrDefault(e => e.PropertyName == "RequestUrl" && e.ErrorMessage == "RequestUrl is a required field.") != null);
        }
        
        [Fact]
        public async Task InvalidTypeRequest()
        {
            Request.RequestType = "test";
            var validator = CreateInstance();
            var validatorResults = await validator.ValidateAsync(Request);

            Assert.True(!validatorResults.IsValid);
            Assert.True(validatorResults?.Errors?.Count == 1);
            Assert.True(validatorResults.Errors.FirstOrDefault(e => e.PropertyName == "RequestType" && e.ErrorMessage == "RequestType is invalid.") != null);
        }
        
        [Fact]
        public async Task SuccessfulRequest()
        {
            var validator = CreateInstance();
            var validatorResults = await validator.ValidateAsync(Request);

            Assert.True(validatorResults.IsValid);
            Assert.True(validatorResults?.Errors?.Count == 0);
        }

    }
}