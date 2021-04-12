using System;
using System.Net.Http;
using AzureFunction.Utilities.Business.Helpers;
using Xunit;

namespace AzureFunction.Utilities.UnitTests.Business.Helpers
{
    public class HttpHelpersTests
    {
        [Fact]
        public void GetMethodInvalid()
        {
            const string type = "test";
            var exception = Assert.Throws<ArgumentException>(() =>
            {
                HttpHelpers.GetMethod(type);
            });
            
            Assert.NotNull(exception);
            Assert.True((exception?.Message).IndexOf("The type is not valid.", StringComparison.Ordinal) > -1);
        }
        
        [Fact]
        public void GetMethodGet()
        {
            const string type = "GET";
            var result = HttpHelpers.GetMethod(type);
            
            Assert.NotNull(result);
            Assert.Equal(HttpMethod.Get, result);
        }
        
        [Fact]
        public void GetMethodPost()
        {
            const string type = "POST";
            var result = HttpHelpers.GetMethod(type);
            
            Assert.NotNull(result);
            Assert.Equal(HttpMethod.Post, result);
        }

        [Fact]
        public void GetMethodPut()
        {
            const string type = "PUT";
            var result = HttpHelpers.GetMethod(type);

            Assert.NotNull(result);
            Assert.Equal(HttpMethod.Put, result);
        }
        
        [Fact]
        public void GetMethodDelete()
        {
            const string type = "DELETE";
            var result = HttpHelpers.GetMethod(type);

            Assert.NotNull(result);
            Assert.Equal(HttpMethod.Delete, result);
        }
        
        [Fact]
        public void GetMethodHead()
        {
            const string type = "HEAD";
            var result = HttpHelpers.GetMethod(type);

            Assert.NotNull(result);
            Assert.Equal(HttpMethod.Head, result);
        }
        
        [Fact]
        public void GetMethodOptions()
        {
            const string type = "Options";
            var result = HttpHelpers.GetMethod(type);

            Assert.NotNull(result);
            Assert.Equal(HttpMethod.Options, result);
        }
        
        [Fact]
        public void GetMethodTrace()
        {
            const string type = "Trace";
            var result = HttpHelpers.GetMethod(type);

            Assert.NotNull(result);
            Assert.Equal(HttpMethod.Trace, result);
        }
        
        [Fact]
        public void GetMethodPatch()
        {
            const string type = "Patch";
            var result = HttpHelpers.GetMethod(type);

            Assert.NotNull(result);
            Assert.Equal(HttpMethod.Patch, result);
        }
    }
}