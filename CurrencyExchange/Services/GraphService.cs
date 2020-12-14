using CurrencyExchange.Models;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

namespace CurrencyExchange.Services
{
    public class GraphService : IGraphService
    {
        private readonly ILogger<GraphService> _logger;

        public GraphService(ILogger<GraphService> logger)
        {
            _logger = logger;
        }
        public IDictionary<string, IList<Edge>> CreateGraph(IDictionary<(string, string), decimal> data)
        {
            var equations = data.Keys.ToList();
            var values = data.Values.ToList();
            IDictionary<string, IList<Edge>> graph = new Dictionary<string, IList<Edge>>();
            int i = 0;
            foreach (var equation in equations)
            {
                var cost = values[i++];

                // check if the keys are present in dictionary
                //ToDo refactor
                AddEdges(equation.Item1, equation.Item2, cost, graph);
                AddEdges(equation.Item2, equation.Item1, 1 / cost, graph);

            }
            return graph;
        }

        public void AddEdges(string start, string end, decimal cost, IDictionary<string, IList<Edge>> graph)
        {
            if (!graph.ContainsKey(start))
            {
                graph.Add(start, new List<Edge>());

            }
            graph[start].Add(new Edge(start, end, cost));

        }


    }
}
