using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AzureFunction.Utilities.UnitTests.Helpers.Data;
using Moq;
using Moq.Protected;

namespace AzureFunction.Utilities.UnitTests.Helpers
{
    public static class MockHttpMessageHandler
    {
        public static Mock<HttpMessageHandler> MockSendAsync(HttpStatusCode statusCode, HttpContent content)
        {
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = statusCode,
                    Content = content
                });
            return mockHttpMessageHandler;
        }

        public static Mock<HttpMessageHandler> MockSendAsyncSequence(IEnumerable<MockHttpMessageHandlerData> mockData)
        {
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            var theSequence = mockHttpMessageHandler.Protected().SetupSequence<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>());
            mockData.Aggregate(theSequence, (current, entry) => current.ReturnsAsync(new HttpResponseMessage() {StatusCode = entry.StatusCode, Content = entry.Content}));
            return mockHttpMessageHandler;
        }
        
        public static void VerifySendAsync(Mock<HttpMessageHandler> mockHttpMessageHandler, Times calledHowManyTimes, Expression<Func<HttpRequestMessage, bool>> lambdaChecking)
        {
            mockHttpMessageHandler.Protected().Verify("SendAsync", calledHowManyTimes, ItExpr.Is(lambdaChecking), ItExpr.IsAny<CancellationToken>());
        }
    }
}