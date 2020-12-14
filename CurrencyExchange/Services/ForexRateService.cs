using CurrencyExchange.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CurrencyExchange.Services
{
    // directional weighted graph
    // edge can be represented as (u,v,w) where u is start node, v is end node and w is cost
    // w is cost from u to v and 1/w is the cost from v to u
    // to find cost between a to d , multiply the cost associated with all the nodes in the path For ex. ad= ab*bc*cd


    public class ForexRateService : IForexRateService
    {
        private readonly ILogger<ForexRateService> _logger;
        private HashSet<string> visited = new HashSet<string>();

        public ForexRateService(ILogger<ForexRateService> logger)
        {

            _logger = logger;


        }


        public bool TryFindForexRate(string start, string end, ref decimal product, IDictionary<string, IList<Edge>> graph)
        {



            if (start.Equals(end))
            {

                return true;
            }

            if (!graph.ContainsKey(start) || !graph.ContainsKey(end))
            {

                throw new ArgumentException("The key is not present in the registry");
            }

            //  Since we are given end node we can just use recursion to find the product

            // add the start node to visited list . equivalent to adding node to the queue and mark the node as visited
            if (!visited.Contains(start))
            {
                visited.Add(start);
            }
            // a->[(a,b,1),(a,c,3),(a,d,4)]
            foreach (var edge in graph[start])
            {

                if (!visited.Contains(edge.To))
                {
                    product *= edge.value;
                    visited.Add(edge.To);
                    // if running node equals end node

                    if (TryFindForexRate(edge.To, end, ref product, graph))
                    {
                        if (visited.Count > 0) visited.Clear();
                        return true;
                    }
                    product /= edge.value;
                }

            }
            return false;




        }

        // Time complexity: O(V+E)
        public decimal TryFindForexRateUsingDijkstra(string start, string end, IDictionary<string, IList<Edge>> graph)
        {
            var nodes = graph.Keys.ToArray();
            int n = graph.Count;
            if (visited.Count > 0) visited.Clear();
            var prev = new decimal[n];
            Array.Fill(prev, decimal.MaxValue);
            var queue = new Queue<string>();

            //start by visiting the 'start' node and add it to the queue.
            queue.Enqueue(start);
            visited.Add(start);
            prev[0] = 1;
            while (queue.Count > 0)
            {

                string node = queue.Dequeue();

                var edges = graph[node];
                // loop through the edges connected to nodes.Edge.to has information about the neighbour

                foreach (var edge in edges)
                {

                    if (visited.Contains(edge.To)) continue;

                    var fromIdx = Array.IndexOf(nodes, edge.From);
                    var toIdx = Array.IndexOf(nodes, edge.To);
                    var newDist = prev[fromIdx] * edge.value;
                    if (newDist < prev[toIdx])
                    {
                        prev[toIdx] = newDist;
                        queue.Enqueue(edge.To);
                        visited.Add(edge.To);
                    }


                    if (edge.To == end)
                    {
                        //var cost = this.CalculateCost(prev);
                        return prev[toIdx];
                    }
                }



            }
            // cannot reach the last node
            return -1;

        }

        private decimal CalculateCost(decimal[] prev)
        {
            decimal product = 1;
            var costs = prev.Where(x => x != decimal.MaxValue);
            foreach (var cost in costs)
            {
                product *= cost;
            }
            return product;
        }
    }
}

