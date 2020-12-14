using System;
using Xunit;
using CurrencyExchange.Services;
using System.Collections.Generic;
using CurrencyExchange.Models;
using Moq;
using System.Threading;
using Microsoft.Extensions.Logging.Abstractions;

namespace CurrencyExchange.Tests
{
    public class ForexRateServiceTests
    {

        decimal product = 1;

        public IDictionary<string, IList<Edge>> graphData = new Dictionary<string, IList<Edge>>()
        {
            { "a",new List<Edge>{ new Edge("a","b",2), new Edge("a","c",1), new Edge("a", "d",(decimal)5/4) } },
            { "b",new List<Edge>{ new Edge("b","a",1/2), new Edge("b","e",5) } },
            { "c",new List<Edge>{ new Edge("c","d",3), new Edge("c","a",1/2) } },
            { "d",new List<Edge>{ new Edge("d","c",(decimal)1/3), new Edge("d","a",(decimal)4/5), new Edge("d", "a",4 )} },
            { "e",new List<Edge>{ new Edge("e","b",(decimal)1/5), new Edge("e","d",(decimal)1/4) } }
        };


        [Fact]
        public void TryFindForexRateUsingRecursion_Success_Returns_Rate()
        {


            var sut = new ForexRateService(NullLogger<ForexRateService>.Instance);
            var result = sut.TryFindForexRateUsingRecursion("a", "e", ref product, graphData);
            Assert.True(result);
            Assert.Equal((decimal)10, product);
        }

        [Fact]
        public void TryFindForexRateUsingDijkstra_Success_Returns_Rate()
        {

            var sut = new ForexRateService(NullLogger<ForexRateService>.Instance);
            var product = sut.TryFindForexRateUsingDijkstra("a", "e", graphData);
            Assert.Equal((decimal)10, product);
        }

        [Fact]
        public void TryFindForexRate_IfNoPathFound_ReturnsNegativeOne()
        {

            var sut = new ForexRateService(NullLogger<ForexRateService>.Instance);
            var product = sut.TryFindForexRateUsingDijkstra("a", "f", graphData);
            Assert.Equal(-1, product);
        }


    }
}
