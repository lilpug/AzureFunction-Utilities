using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AzureFunction.Utilities.Business.Services.http;
using AzureFunction.Utilities.Data;
using AzureFunction.Utilities.Data.Exceptions;
using AzureFunction.Utilities.Data.Interfaces;
using AzureFunction.Utilities.Data.Responses;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using IValidatorFactory = AzureFunction.Utilities.Data.Interfaces.IValidatorFactory;

namespace AzureFunction.Utilities.UnitTests.Business.Services.Http
{
    public class BaseHttpServiceTests
    {
        //This is mock class so that we can test our abstracted class components
        private class MockGenericProcessingService : BaseHttpService
        {
            public MockGenericProcessingService(ILogger logger, IValidatorFactory validatorFactory, IServiceValidationConverter serviceValidationConverter, IRequest request) : base(logger, validatorFactory, serviceValidationConverter, request)
            {
            }

            protected override Task<HttpServiceResult> InternalProcess(HttpServiceResult result)
            {
                return Task.Run(() => new HttpServiceResult()
                {
                    Response = true
                    //Note: we do not add a status code as this also tests the final statement in the BaseProcessService
                });
            }
        }

        private readonly Mock<ILogger> _mockLogger;
        private readonly Mock<IServiceValidationConverter> _mockServiceValidationConverter;
        private readonly Mock<IValidator> _mockValidator;
        private readonly Mock<IValidatorFactory> _mockValidatorFactory;
        
        private IRequest Request { get; set; }

        public BaseHttpServiceTests()
        {
            _mockLogger = new Mock<ILogger>();
            _mockServiceValidationConverter = new Mock<IServiceValidationConverter>();
            _mockValidator = new Mock<IValidator>();
            _mockValidatorFactory = new Mock<IValidatorFactory>();
            Request = new Mock<IRequest>().Object;
        }

        [Fact]
        public void NullLoggerConstructor()
        {
            var exception = Assert.Throws<ArgumentNullException>(() =>
            {
                var service = new MockGenericProcessingService(null, _mockValidatorFactory.Object, _mockServiceValidationConverter.Object, Request);
            });
            
            Assert.NotNull(exception);
            Assert.True((exception?.Message).IndexOf("The logger is required.", StringComparison.Ordinal) > -1);
        }
        
        [Fact]
        public void NullValidatorFactoryConstructor()
        {
            var exception = Assert.Throws<ArgumentNullException>(() =>
            {
                var service = new MockGenericProcessingService(_mockLogger.Object, null, _mockServiceValidationConverter.Object, Request);
            });
            
            Assert.NotNull(exception);
            Assert.True((exception?.Message).IndexOf("The validatorFactory is required.", StringComparison.Ordinal) > -1);
        }
        
        [Fact]
        public void NullServiceValidationConverterConstructor()
        {
            var exception = Assert.Throws<ArgumentNullException>(() =>
            {
                var service = new MockGenericProcessingService(_mockLogger.Object, _mockValidatorFactory.Object, null, Request);
            });
            
            Assert.NotNull(exception);
            Assert.True((exception?.Message).IndexOf("The serviceValidationConverter is required.", StringComparison.Ordinal) > -1);
        }

        [Fact]
        public void NullRequestConstructor()
        {
            var exception = Assert.Throws<ArgumentNullException>(() =>
            {
                var service = new MockGenericProcessingService(_mockLogger.Object, _mockValidatorFactory.Object, _mockServiceValidationConverter.Object, null);
            });
            
            Assert.NotNull(exception);
            Assert.True((exception?.Message).IndexOf("The request is required.", StringComparison.Ordinal) > -1);
        }
        
        [Fact]
        public async Task SuccessFlow()
        {
            var validationResult = new ValidationResult(new List<ValidationFailure>());
            _mockValidator.Setup(v => v.ValidateAsync(It.IsAny<IValidationContext>(), It.IsAny<CancellationToken>())).ReturnsAsync(validationResult);
            _mockValidatorFactory.Setup(v => v.GetValidator(It.IsAny<IRequest>())).Returns(_mockValidator.Object);
            
            var service = new MockGenericProcessingService(_mockLogger.Object, _mockValidatorFactory.Object, _mockServiceValidationConverter.Object, Request);
            var results = await service.ProcessAsync();
            
            _mockValidatorFactory.Verify(v => v.GetValidator(It.IsAny<IRequest>()), Times.Once);
            _mockValidator.Verify(v => v.ValidateAsync(It.IsAny<IValidationContext>(), It.IsAny<CancellationToken>()), Times.Once);
            _mockServiceValidationConverter.Verify(s => s.ConvertToDictionaryFormat(It.IsAny<ValidationResult>()), Times.Never);
            
            Assert.NotNull(results);
            Assert.Equal(true, results.Response);
            Assert.Equal(HttpStatusCode.OK, results.StatusCode);
        }

        [Fact]
        public async Task NoValidatorFlow()
        {
            _mockValidatorFactory.Setup(v => v.GetValidator(It.IsAny<IRequest>())).Returns<IValidator>(null);

            var service = new MockGenericProcessingService(_mockLogger.Object, _mockValidatorFactory.Object, _mockServiceValidationConverter.Object, Request);
            var results = await service.ProcessAsync();
            
            _mockValidatorFactory.Verify(v => v.GetValidator(It.IsAny<IRequest>()), Times.Once);
            _mockValidator.Verify(v => v.ValidateAsync(It.IsAny<IValidationContext>(), It.IsAny<CancellationToken>()), Times.Never);
            _mockServiceValidationConverter.Verify(s => s.ConvertToDictionaryFormat(It.IsAny<ValidationResult>()), Times.Never);
            
            Assert.NotNull(results);
            Assert.True(results.Response is ErrorResponse {Successful: false, Message: "There has been an error."});
            Assert.Equal(HttpStatusCode.InternalServerError, results.StatusCode);
        }
        
        [Fact]
        public async Task ValidationFlow()
        {
            const string fieldName = "testing";
            const string fieldError = "testing error";
            var convertedResults = new Dictionary<string, List<string>>()
            {
                {fieldName, new List<string>() {fieldError}}
            };
            
            var validationResult = new ValidationResult(new List<ValidationFailure>() { new ValidationFailure(fieldName, fieldError)});
            _mockValidator.Setup(v => v.ValidateAsync(It.IsAny<IValidationContext>(), It.IsAny<CancellationToken>())).ReturnsAsync(validationResult);
            _mockValidatorFactory.Setup(v => v.GetValidator(It.IsAny<IRequest>())).Returns(_mockValidator.Object);
            _mockServiceValidationConverter.Setup(s => s.ConvertToDictionaryFormat(It.IsAny<ValidationResult>())).Returns(convertedResults);
            
            var service = new MockGenericProcessingService(_mockLogger.Object, _mockValidatorFactory.Object, _mockServiceValidationConverter.Object, Request);
            var results = await service.ProcessAsync();
            
            _mockValidatorFactory.Verify(v => v.GetValidator(It.IsAny<IRequest>()), Times.Once);
            _mockValidator.Verify(v => v.ValidateAsync(It.IsAny<IValidationContext>(), It.IsAny<CancellationToken>()), Times.Once);
            _mockServiceValidationConverter.Verify(s => s.ConvertToDictionaryFormat(It.IsAny<ValidationResult>()), Times.Once);
            
            Assert.NotNull(results);
            Assert.True(results.Response is ValidationErrorResponse {Successful: false} response && response.ValidationErrors == convertedResults);
            Assert.Equal(HttpStatusCode.BadRequest, results.StatusCode);
        }

        [Fact]
        public async Task NotFoundFlow()
        {
            _mockValidatorFactory.Setup(v => v.GetValidator(It.IsAny<IRequest>())).Throws(new NotFoundException());
            
            var service = new MockGenericProcessingService(_mockLogger.Object, _mockValidatorFactory.Object, _mockServiceValidationConverter.Object, Request);
            var results = await service.ProcessAsync();
            
            _mockValidatorFactory.Verify(v => v.GetValidator(It.IsAny<IRequest>()), Times.Once);
            _mockValidator.Verify(v => v.ValidateAsync(It.IsAny<IValidationContext>(), It.IsAny<CancellationToken>()), Times.Never);
            _mockServiceValidationConverter.Verify(s => s.ConvertToDictionaryFormat(It.IsAny<ValidationResult>()), Times.Never);
            
            Assert.NotNull(results);
            Assert.True(results.Response is NotFoundResponse {Successful: false, Message: "Resource not found."});
            Assert.Equal(HttpStatusCode.NotFound, results.StatusCode);
        }
        
        [Fact]
        public async Task ExceptionFlow()
        {
            _mockValidatorFactory.Setup(v => v.GetValidator(It.IsAny<IRequest>())).Throws(new Exception("testing error journey"));
            
            var service = new MockGenericProcessingService(_mockLogger.Object, _mockValidatorFactory.Object, _mockServiceValidationConverter.Object, Request);
            var results = await service.ProcessAsync();
            
            _mockValidatorFactory.Verify(v => v.GetValidator(It.IsAny<IRequest>()), Times.Once);
            _mockValidator.Verify(v => v.ValidateAsync(It.IsAny<IValidationContext>(), It.IsAny<CancellationToken>()), Times.Never);
            _mockServiceValidationConverter.Verify(s => s.ConvertToDictionaryFormat(It.IsAny<ValidationResult>()), Times.Never);
            
            Assert.NotNull(results);
            Assert.True(results.Response is ErrorResponse {Successful: false, Message: "There has been an error."});
            Assert.Equal(HttpStatusCode.InternalServerError, results.StatusCode);
        }

    }
}