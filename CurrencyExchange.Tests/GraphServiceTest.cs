using CurrencyExchange.Models;
using CurrencyExchange.Services;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace CurrencyExchange.Tests
{
    public class GraphServiceTest
    {


        GraphService sut = new GraphService(NullLogger<GraphService>.Instance);

        [Fact]
        public void CreatesGraph()
        {
            var expected = new Dictionary<string, List<Edge>>
            {
                {"a",new List<Edge>{ new Edge("a","b",1), new Edge("a", "c", 2) } },
                {"b",new List<Edge>{ new Edge("b","c",2), new Edge("b", "a", 1) } },
                {"c",new List<Edge>{ new Edge("c","d",3), new Edge("c", "a", (decimal)1/2), new Edge("c", "b", (decimal)1 / 2) } },
                {"d",new List<Edge>{ new Edge("d","c",(decimal)1/3)} }
            };
            var data = new Dictionary<(string, string), decimal>
            {
                { new("a","b"),1 },{ new ("b","c"),2},{ new ("c","d"),3}
            };


            var graph = sut.CreateGraph(data);
            Assert.Equal(expected.Count, graph.Count);


        }
    }
}
