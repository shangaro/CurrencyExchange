using CurrencyExchange;
using CurrencyExchange.Services;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace CurrencyExchange.Tests
{
    public class DataServiceTests : IDisposable
    {
        HttpClient httpClient;


        [Fact]
        public async void GetForexData_ReturnsNull_WhenHttpFails()
        {
            httpClient = new HttpClient(new ExceptionHandler());
            var sut = new DataService(httpClient, NullLogger<DataService>.Instance);
            //await Assert.ThrowsAsync<HttpRequestException>(() => sut.GetForexRateFromBankOfCanada("2020-12-10"));
            var result = await sut.GetForexRateFromBankOfCanada("");
            Assert.Null(result);
        }


        [Fact]
        public async void GetForexData_ReturnsEmpty_WhenForexRateNotFound()
        {
            var expected = new Dictionary<(string, string), decimal>();
            httpClient = new HttpClient(new SuccessHandler());
            var sut = new DataService(httpClient, NullLogger<DataService>.Instance);
            var result = await sut.GetForexRateFromBankOfCanada("2020-12-10");
            Assert.Equal(expected, result);
        }


        public void Dispose()
        {
            httpClient.Dispose();
        }


        private class ExceptionHandler : HttpClientHandler
        {
            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                throw new HttpRequestException();
            }
        }

        private class SuccessHandler : HttpClientHandler
        {
            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {

                var result = "no forex rate found";
                var contentString = JsonConvert.SerializeObject(new { result });

                var response = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(contentString, Encoding.UTF8, "application/json")
                };

                return Task.FromResult(response);
            }
        }
    }
}
